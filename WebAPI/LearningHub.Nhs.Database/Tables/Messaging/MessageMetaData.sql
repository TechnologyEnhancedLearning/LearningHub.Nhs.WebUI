CREATE TABLE [messaging].[MessageMetaData]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[NotificationPriority] INT NULL,
	[NotificationStartDate] DATETIMEOFFSET(7) NULL,
	[NotificationEndDate] DATETIMEOFFSET(7) NULL,
	[NotificationType] INT NULL,
	[MessageId] INT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
	CONSTRAINT [PK_MessageMetaData] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [messaging].[MessageMetaData] WITH CHECK ADD CONSTRAINT [FK_MessageMetaData_Message] FOREIGN KEY([MessageId])
REFERENCES [messaging].[Message] ([Id])
GO

ALTER TABLE [messaging].[MessageMetaData] CHECK CONSTRAINT [FK_MessageMetaData_Message]
GO