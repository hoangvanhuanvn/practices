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
[http://traceone.com/InternalActivation/RequestMessage]
VALIDATION = NONE
GO

--*********************************************
--*  Create the message type "ResponseMessage"
--*********************************************
CREATE MESSAGE TYPE
[http://traceone.com/InternalActivation/ResponseMessage]
VALIDATION = NONE
GO

--************************************************
--*  Changing the validation of the message types
--************************************************
ALTER MESSAGE TYPE [http://traceone.com/InternalActivation/RequestMessage]
VALIDATION = WELL_FORMED_XML
GO

ALTER MESSAGE TYPE [http://traceone.com/InternalActivation/ResponseMessage]
VALIDATION = WELL_FORMED_XML
GO

--************************************************
--*  Create the contract "HelloWorldContract"
--************************************************
CREATE CONTRACT [http://traceone.com/InternalActivation/HelloWorldContract]
(
	[http://traceone.com/InternalActivation/RequestMessage] SENT BY INITIATOR,
	[http://traceone.com/InternalActivation/ResponseMessage] SENT BY TARGET
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
	[http://traceone.com/InternalActivation/HelloWorldContract]
)
GO

CREATE SERVICE TargetService
ON QUEUE TargetQueue
(
	[http://traceone.com/InternalActivation/HelloWorldContract]
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
				ON CONTRACT [http://traceone.com/InternalActivation/HelloWorldContract]
				WITH ENCRYPTION = OFF;

			SET @msg = '<HelloWorldRequest>' + @body + '</HelloWorldRequest>';

			SEND ON CONVERSATION @ch MESSAGE TYPE [http://traceone.com/InternalActivation/RequestMessage] (@msg);
		COMMIT;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
GO

--- Create an event notification for QUEUE_ACTIVATION event
CREATE EVENT NOTIFICATION NotificationOnTargetQueue
ON QUEUE TargetQueue
FOR QUEUE_ACTIVATION
TO SERVICE 'TargetService', 'current database'
GO



---- TEST MESSAGES
exec SendRequestMessages 'HuanHV'
GO

SELECT CAST(message_body as XML) ,* FROM TargetQueue
GO

