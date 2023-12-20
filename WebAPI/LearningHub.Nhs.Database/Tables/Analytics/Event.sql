CREATE TABLE [analytics].[Event](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[EventTypeId] [int] NOT NULL,
	[UserId] [int] NULL,
	[JsonData] [nvarchar](max) NULL,
	[GroupId] UNIQUEIDENTIFIER NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [analytics].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[Event] CHECK CONSTRAINT [FK_Event_AmendUser]
GO

ALTER TABLE [analytics].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[Event] CHECK CONSTRAINT [FK_Event_CreateUser]
GO

ALTER TABLE [analytics].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_EventType] FOREIGN KEY([EventTypeId])
REFERENCES [analytics].[EventType] ([Id])
GO

ALTER TABLE [analytics].[Event] CHECK CONSTRAINT [FK_Event_EventType]
GO

ALTER TABLE [analytics].[Event]  WITH CHECK ADD  CONSTRAINT [FK_Event_User] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[Event] CHECK CONSTRAINT [FK_Event_User]
GO