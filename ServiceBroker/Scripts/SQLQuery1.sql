USE [master]
GO

IF  EXISTS (SELECT name FROM sys.databases WHERE name = N'ServiceBrokerDB')
DROP DATABASE [ServiceBrokerDB]
GO

CREATE DATABASE ServiceBrokerDB
GO

ALTER DATABASE ServiceBrokerDB
      SET ENABLE_BROKER;
GO

USE ServiceBrokerDB;
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


--************************************************************************
--*  A stored procedure used for internal activation on the target queue
--************************************************************************
CREATE PROCEDURE ProcessRequestMessages
AS
	DECLARE @ch UNIQUEIDENTIFIER
	DECLARE @messagetypename NVARCHAR(256)
	DECLARE	@messagebody XML
	DECLARE @responsemessage XML;
	
	WHILE (1=1)
	BEGIN
		BEGIN TRY
			BEGIN TRANSACTION

			WAITFOR (
				RECEIVE TOP(1)
					@ch = conversation_handle,
					@messagetypename = message_type_name,
					@messagebody = CAST(message_body AS XML)
				FROM TargetQueue
			), TIMEOUT 60000

			IF (@@ROWCOUNT = 0)
			BEGIN
				ROLLBACK TRANSACTION
				BREAK
			END
			IF (@messagetypename = 'http://traceone.com/InternalActivation/RequestMessage')
			BEGIN
				-- Store the received request message in a table
				INSERT INTO ProcessedMessages (ID, MessageBody, ServiceName) VALUES (NEWID(), @messagebody, 'TargetService')

				---- Construct the response message
				--SET @responsemessage = '<HelloWorldResponse>' + @messagebody.value('/HelloWorldRequest[1]', 'NVARCHAR(MAX)') + '</HelloWorldResponse>';

				---- Send the response message back to the initiating service
				--SEND ON CONVERSATION @ch MESSAGE TYPE [http://traceone.com/InternalActivation/ResponseMessage] (@responsemessage);

				---- End the conversation on the target's side
				--END CONVERSATION @ch;
			END

			IF (@messagetypename = 'http://schemas.microsoft.com/SQL/ServiceBroker/EndDialog')
			BEGIN
				-- End the conversation
				END CONVERSATION @ch;
			END

			COMMIT TRANSACTION
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION
		END CATCH
	END
GO



--********************************************************
--*  Create the queues "InitiatorQueue" and "TargetQueue"
--********************************************************
CREATE QUEUE InitiatorQueue
WITH STATUS = ON
GO

CREATE QUEUE TargetQueue
WITH ACTIVATION
(
	STATUS = ON,
	PROCEDURE_NAME = [ProcessRequestMessages],
	MAX_QUEUE_READERS = 1,
	EXECUTE AS SELF
)
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
