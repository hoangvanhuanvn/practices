USE [master]
GO

IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'ServiceBrokerExternalActivation')
DROP DATABASE [ServiceBrokerExternalActivation]
GO

CREATE DATABASE ServiceBrokerExternalActivation
GO

ALTER DATABASE ServiceBrokerExternalActivation
      SET ENABLE_BROKER;
GO

USE ServiceBrokerExternalActivation;
GO

--*********************************************
--*  Create the message type "RequestMessage"
--*********************************************
CREATE MESSAGE TYPE
[http://traceone.com/ExternalActivation/RequestMessage]
VALIDATION = WELL_FORMED_XML
GO

--*********************************************
--*  Create the message type "ResponseMessage"
--*********************************************
CREATE MESSAGE TYPE
[http://traceone.com/ExternalActivation/ResponseMessage]
VALIDATION = WELL_FORMED_XML
GO

--************************************************
--*  Create the contract "HelloWorldContract"
--************************************************
CREATE CONTRACT [http://traceone.com/ExternalActivation/HelloWorldContract]
(
	[http://traceone.com/ExternalActivation/RequestMessage] SENT BY INITIATOR,
	[http://traceone.com/ExternalActivation/ResponseMessage] SENT BY TARGET
)
GO

--**************************************************
--*  Create a table to store the processed messages
--**************************************************
CREATE TABLE ProcessedMessages
(
	ID UNIQUEIDENTIFIER NOT NULL,
	MessageBody XML NOT NULL,
	ServiceName NVARCHAR(MAX) NOT NULL
)
GO

--********************************************************
--*  Create the queues "InitiatorQueue" and "TargetQueue"
--********************************************************
CREATE QUEUE InitiatorQueue
GO

CREATE QUEUE TargetQueue
GO

--************************************************************
--*  Create the queues "InitiatorService" and "TargetService"
--************************************************************
CREATE SERVICE InitiatorService
ON QUEUE InitiatorQueue 
(
	[http://traceone.com/ExternalActivation/HelloWorldContract]
)
GO

CREATE SERVICE TargetService
ON QUEUE TargetQueue
(
	[http://traceone.com/ExternalActivation/HelloWorldContract]
)
GO

--********************************************************************
--*  Sending a message from the InitiatorService to the TargetService
--********************************************************************
CREATE PROCEDURE SendRequestMessages
	@body nvarchar(max)
AS
	BEGIN TRY
		BEGIN TRANSACTION;
			DECLARE @ch UNIQUEIDENTIFIER
			DECLARE @msg NVARCHAR(MAX);

			BEGIN DIALOG CONVERSATION @ch
				FROM SERVICE [InitiatorService]
				TO SERVICE 'TargetService'
				ON CONTRACT [http://traceone.com/ExternalActivation/HelloWorldContract]
				WITH ENCRYPTION = OFF;

			SET @msg = '<HelloWorldRequest>' + @body + '</HelloWorldRequest>';

			SEND ON CONVERSATION @ch MESSAGE TYPE [http://traceone.com/ExternalActivation/RequestMessage] (@msg);
		COMMIT;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
GO

--- Create an event notification for QUEUE_ACTIVATION event
-- Deactivate internal activation on the queue if necessary
ALTER QUEUE TargetQueue
WITH ACTIVATION (DROP)
GO
-- Create the event-notification queue
CREATE QUEUE ExternalActivatorQueue
GO
-- Create the event-notification service
CREATE SERVICE ExternalActivatorService
ON QUEUE ExternalActivatorQueue
(
[http://schemas.microsoft.com/SQL/Notifications/PostEventNotification]
)
GO
-- Subscribe to the QUEUE_ACTIVATION event on the queue TargetQueue
CREATE EVENT NOTIFICATION EventNotificationTargetQueue
ON QUEUE TargetQueue
FOR QUEUE_ACTIVATION
TO SERVICE 'ExternalActivatorService', 'current database'
GO



---- TEST MESSAGES
exec SendRequestMessages 'HuanHV 2'
GO

SELECT CAST(message_body as XML) as message_body_xml,* FROM TargetQueue
GO

SELECT CAST(message_body as XML) as message_body_xml,* FROM ExternalActivatorQueue
GO

SELECT * FROM sys.event_notifications

select * from sys.dm_broker_queue_monitors m with (nolock)
join sys.service_queues q with (nolock) on m.queue_id = q.object_id


--USE master
--GO
 
---- create a sql-login for the same named service account from windows
--CREATE LOGIN [NT AUTHORITY\NETWORK SERVICE] FROM WINDOWS
--GO

--USE [ServiceBrokerExternalActivation]
--GO

---- allow CONNECT to the notification database
--GRANT CONNECT TO [NT AUTHORITY\NETWORK SERVICE]
--GO
 
---- allow RECEIVE from the notification service queue
--GRANT RECEIVE ON TargetQueue TO [NT AUTHORITY\NETWORK SERVICE]
--GO
 
---- allow VIEW DEFINITION right on the notification service
--GRANT VIEW DEFINITION ON SERVICE::TargetService TO [NT AUTHORITY\NETWORK SERVICE]
--GO
 
---- allow REFRENCES right on the notification queue schema
--GRANT REFERENCES ON SCHEMA::dbo TO [NT AUTHORITY\NETWORK SERVICE]
--GO