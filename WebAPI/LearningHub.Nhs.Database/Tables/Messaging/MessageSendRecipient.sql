CREATE TABLE [messaging].[MessageSendRecipient]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[MessageSendId] INT NOT NULL,
	[UserId] INT NULL,
	[UserGroupId] INT NULL,
	[EmailAddress] NVARCHAR(100) NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
	CONSTRAINT [PK_MessageSendRecipient] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [messaging].[MessageSendRecipient] WITH CHECK ADD CONSTRAINT [FK_MessageSendRecipient_User] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [messaging].[MessageSendRecipient] CHECK CONSTRAINT [FK_MessageSendRecipient_User]
GO

ALTER TABLE [messaging].[MessageSendRecipient] WITH CHECK ADD CONSTRAINT [FK_MessageSendRecipient_UserGroup] FOREIGN KEY([UserGroupId])
REFERENCES [hub].[UserGroup] ([Id])
GO

ALTER TABLE [messaging].[MessageSendRecipient] CHECK CONSTRAINT [FK_MessageSendRecipient_UserGroup]
GO

ALTER TABLE [messaging].[MessageSendRecipient] WITH CHECK ADD CONSTRAINT [FK_MessageSendRecipient_MessageSend] FOREIGN KEY([MessageSendId])
REFERENCES [messaging].[MessageSend] ([Id])
GO

ALTER TABLE [messaging].[MessageSendRecipient] CHECK CONSTRAINT [FK_MessageSendRecipient_MessageSend]
GO