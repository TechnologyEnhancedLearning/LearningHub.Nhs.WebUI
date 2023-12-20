CREATE TABLE [hub].[UserNotification]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[NotificationId] [int] NOT NULL,	
	[Dismissed] [bit] NOT NULL Default(0),
	[ReadOnDate] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL Default(0),
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_UserNotification] PRIMARY KEY CLUSTERED 
	(
	[Id] ASC
	)
)
GO

ALTER TABLE [hub].[UserNotification] ADD CONSTRAINT [FK_UserNotification_userCreated] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hub].[UserNotification] ADD CONSTRAINT [FK_UserNotification_userAmended] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hub].[UserNotification] ADD CONSTRAINT [FK_UserNotification_notification] FOREIGN KEY([NotificationId])
REFERENCES [hub].[Notification] ([Id])
GO

ALTER TABLE [hub].[UserNotification] ADD CONSTRAINT [FK_UserNotification_user] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

CREATE INDEX IX_UserNotification_User ON [hub].[UserNotification](UserId)
WITH (FILLFACTOR = 95);
