CREATE TABLE [hub].[Notification]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(300) NOT NULL, 
    [Message] TEXT NOT NULL, 
    [StartDate] DATETIMEOFFSET NULL, 
    [EndDate] DATETIMEOFFSET NULL,
	[UserDismissable] BIT NOT NULL DEFAULT 1,
	[NotificationTypeId] [int] NULL,
	[NotificationPriorityId] [int] NULL,
	[IsUserSpecific] [bit] NOT NULL DEFAULT 0,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NULL,
	CONSTRAINT [PK_notification] PRIMARY KEY CLUSTERED 
	(
	[Id] ASC
	)
)
GO
ALTER TABLE [hub].[Notification] ADD CONSTRAINT [FK_Notification_userCreated] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO
ALTER TABLE [hub].[Notification] ADD CONSTRAINT [FK_Notification_userAmended] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO
ALTER TABLE [hub].[Notification] ADD CONSTRAINT [FK_Notification_NotificationTypeId] FOREIGN KEY([NotificationTypeId])
REFERENCES [hub].[NotificationType] ([Id])
GO
ALTER TABLE [hub].[Notification] ADD CONSTRAINT [FK_Notification_NotificationPriorityId] FOREIGN KEY([NotificationPriorityId])
REFERENCES [hub].[NotificationPriority] ([Id])
GO
