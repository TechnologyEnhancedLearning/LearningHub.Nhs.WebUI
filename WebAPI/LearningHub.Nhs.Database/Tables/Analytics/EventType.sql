CREATE TABLE [analytics].[EventType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](128) NULL,
	[Description] [nvarchar](512) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_EventType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) 
GO

ALTER TABLE [analytics].[EventType]  WITH CHECK ADD  CONSTRAINT [FK_EventType_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[EventType] CHECK CONSTRAINT [FK_EventType_AmendUser]
GO

ALTER TABLE [analytics].[EventType]  WITH CHECK ADD  CONSTRAINT [FK_EventType_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[EventType] CHECK CONSTRAINT [FK_EventType_CreateUser]
GO
