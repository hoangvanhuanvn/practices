USE master;
GO
--Create a database for this learning session, it will help you to do 
--clean up easily, you can create SSBS objects in any existing database
--also but you need to drop all objects individually if you want to do
--clean up of these objects than dropping a single database
IF EXISTS(SELECT COUNT(1) FROM sys.databases WHERE name = 'SSBSLearning')
	DROP DATABASE SSBSLearning
GO
CREATE DATABASE SSBSLearning
GO

--By default a database will have service broker enabled, which you can verify 
--with is_broker_enabled column of the below resultset
SELECT name, service_broker_guid, is_broker_enabled, is_honor_broker_priority_on 
FROM sys.databases WHERE name = 'SSBSLearning'

--If your database is not enabled for Service Broker becuase you have 
--changed the default setting in Model database, even then you can enable
--service broker for a database with this statement
ALTER DATABASE SSBSLearning
      SET ENABLE_BROKER;
      --WITH ROLLBACK IMMEDIATE
GO
----To disable service broker for a database
--ALTER DATABASE SSBSLearning
--      SET DISABLE_BROKER;
--GO


USE SSBSLearning;
GO
--Create message types which will allow valid xml messages to be sent
--and received, SSBS validates whether a message is well formed XML 
--or not by loading it into XML parser 
CREATE MESSAGE TYPE
       [//SSBSLearning/ProductStockStatusCheckRequest]
       VALIDATION = WELL_FORMED_XML;
CREATE MESSAGE TYPE
       [//SSBSLearning/ProductStockStatusCheckResponse]
       VALIDATION = WELL_FORMED_XML;
GO

--Create a contract which will be used by Service to validate
--what message types are allowed for Initiator and for Target.
--As because communication starts from Initiator hence 
--SENT BY INITIATOR or SENT BY ANY is mandatory
CREATE CONTRACT [//SSBSLearning/ProductStockStatusCheckContract]
      ([//SSBSLearning/ProductStockStatusCheckRequest]
       SENT BY INITIATOR,
       [//SSBSLearning/ProductStockStatusCheckResponse]
       SENT BY TARGET
      );
GO

--Create a queue which is an internal physical table to hold 
--the messages passed to the service, by default it will be 
--created in default file group, if you want to create it in 
--another file group you need to specify the ON clause with 
--this statement. You can use SELECT statement to query this 
--queue or special table but you can not use other DML statement
--like INSERT, UPDATE and DELETE. You need to use SEND and RECEIVE
--commands to send messages to queue and receive from it
CREATE QUEUE dbo.SSBSLearningTargetQueue;
GO

--Create a service, which is a logical endpoint which sits on top 
--of a queue on which either message is sent or received. With 
--Service creation you all specify the contract which will be
--used to validate message sent on that service
CREATE SERVICE
       [//SSBSLearning/ProductStockStatusCheck/TargetService]
       ON QUEUE dbo.SSBSLearningTargetQueue
       ([//SSBSLearning/ProductStockStatusCheckContract]);
GO

--A Target can also send messages back to Initiator and hence
--you can create a queue for Initiator also
CREATE QUEUE dbo.SSBSLearningInitiatorQueue;
GO

--Likewsie you would need to create a service which will sit 
--on top of Initiator queue and used by Target to send messages
--back to Initiator
CREATE SERVICE
       [//SSBSLearning/ProductStockStatusCheck/InitiatorService]
       ON QUEUE dbo.SSBSLearningInitiatorQueue;
GO







--To send message, first you need to initiate a dialog with 
--BEGIN DIALOG command and specify the Initiator and Target
--services which will be talking in this dialog conversation
DECLARE @SSBSInitiatorDialogHandle UNIQUEIDENTIFIER;
DECLARE @RequestMessage XML;
BEGIN TRANSACTION;
	BEGIN DIALOG @SSBSInitiatorDialogHandle
		 FROM SERVICE
		  [//SSBSLearning/ProductStockStatusCheck/InitiatorService]
		 TO SERVICE
		  N'//SSBSLearning/ProductStockStatusCheck/TargetService'
		 ON CONTRACT
		  [//SSBSLearning/ProductStockStatusCheckContract]
		 WITH ENCRYPTION = OFF;
	SELECT @RequestMessage =
		   N'<Request>
				<ProductID>316</ProductID>
				<LocationID>10</LocationID>
			</Request>';
	--To send message you use SEND command and specify the dialog
	--handle which you got above after initiating a dialog		
	SEND ON CONVERSATION @SSBSInitiatorDialogHandle
		 MESSAGE TYPE 
		 [//SSBSLearning/ProductStockStatusCheckRequest]
		 (@RequestMessage);
	SELECT @RequestMessage AS RequestMessageSent;
COMMIT TRANSACTION;
GO


SELECT CAST(message_body as XML) ,* FROM SSBSLearningTargetQueue
GO



--Creating event notification queue
--which will hold notification messages
--raised by SSBSLearningTargetQueue and will be
--consumed by External Activator
CREATE QUEUE dbo.SSBSLearningNotificationQueue 
GO
--Creating event notification service
--which will be used to send notification messages
CREATE SERVICE SSBSLearningNotificationService
ON QUEUE dbo.SSBSLearningNotificationQueue
(
    [http://schemas.microsoft.com/SQL/Notifications/PostEventNotification]
)
GO

--Create an event notification for QUEUE_ACTIVATION event on 
--the queue SSBSLearningTargetQueue to notify service SSBSLearningNotificationService 
CREATE EVENT NOTIFICATION SSBSLearningENForSSBSLearningTargetQueue
ON QUEUE SSBSLearningTargetQueue
FOR QUEUE_ACTIVATION
TO SERVICE 'SSBSLearningNotificationService', 'current database'
GO