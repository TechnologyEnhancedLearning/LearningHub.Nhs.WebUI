CREATE TABLE [messaging].[MessageSend]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[MessageId] INT NOT NULL,
	[MessageTypeId] INT NOT NULL,
	[Status] [int] NOT NULL DEFAULT 0,
	[SendAttempts] [int] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
	CONSTRAINT [PK_MessageSend] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [messaging].[MessageSend] WITH CHECK ADD CONSTRAINT [FK_MessageSend_MessageType] FOREIGN KEY([MessageTypeId])
REFERENCES [messaging].[MessageType] ([Id])
GO

ALTER TABLE [messaging].[MessageSend] CHECK CONSTRAINT [FK_MessageSend_MessageType]
GO

ALTER TABLE [messaging].[MessageSend] WITH CHECK ADD CONSTRAINT [FK_MessageSend_Message] FOREIGN KEY([MessageId])
REFERENCES [messaging].[Message] ([Id])
GO

ALTER TABLE [messaging].[MessageSend] CHECK CONSTRAINT [FK_MessageSend_Message]
GO