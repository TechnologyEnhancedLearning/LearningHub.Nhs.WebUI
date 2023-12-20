CREATE TABLE [activity].[ResourceActivity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[LaunchResourceActivityId] [int] NULL,
	[ResourceId] [int] NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[MajorVersion] [int] NOT NULL,
	[MinorVersion] [int] NOT NULL,
	[NodePathId] [int] NOT NULL,
	[ActivityStatusId] [int] NOT NULL,
	[ActivityStart] [datetimeoffset](7) NULL,
	[ActivityEnd] [datetimeoffset](7) NULL,
	[DurationSeconds] [int] NOT NULL,
	[Score] [decimal](16, 2) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Activity_ResourceActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [activity].[ResourceActivity]  WITH CHECK ADD  CONSTRAINT [FK_ResourceActivity_ActivityStatus] FOREIGN KEY([ActivityStatusId])
REFERENCES [activity].[ActivityStatus] ([Id])
GO

ALTER TABLE [activity].[ResourceActivity] CHECK CONSTRAINT [FK_ResourceActivity_ActivityStatus]
GO

ALTER TABLE [activity].[ResourceActivity]  WITH CHECK ADD  CONSTRAINT [FK_ResourceActivity_AmendUser] FOREIGN KEY([AmendUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[ResourceActivity] CHECK CONSTRAINT [FK_ResourceActivity_AmendUser]
GO

ALTER TABLE [activity].[ResourceActivity]  WITH CHECK ADD  CONSTRAINT [FK_ResourceActivity_CreateUser] FOREIGN KEY([CreateUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[ResourceActivity] CHECK CONSTRAINT [FK_ResourceActivity_CreateUser]
GO

ALTER TABLE [activity].[ResourceActivity]  WITH CHECK ADD  CONSTRAINT [FK_ResourceActivity_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [activity].[ResourceActivity] CHECK CONSTRAINT [FK_ResourceActivity_NodePath]
GO

ALTER TABLE [activity].[ResourceActivity]  WITH CHECK ADD  CONSTRAINT [FK_ResourceActivity_ResourceActivity] FOREIGN KEY([LaunchResourceActivityId])
REFERENCES [activity].[ResourceActivity] ([Id])
GO

ALTER TABLE [activity].[ResourceActivity] CHECK CONSTRAINT [FK_ResourceActivity_ResourceActivity]
GO

ALTER TABLE [activity].[ResourceActivity]  WITH CHECK ADD  CONSTRAINT [FK_ResourceActivity_User] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[ResourceActivity] CHECK CONSTRAINT [FK_ResourceActivity_User]
GO

CREATE INDEX IX_ResourceActivity_User ON [activity].[ResourceActivity](UserId)
WITH (FILLFACTOR = 95);
GO

CREATE INDEX IX_ResourceActivity_ResourceVersion ON [activity].[ResourceActivity](ResourceVersionId)
WITH (FILLFACTOR = 95);
GO

CREATE INDEX IX_ResourceActivity_LaunchResourceActivity ON [activity].[ResourceActivity](LaunchResourceActivityId)
WITH (FILLFACTOR = 95);
GO

CREATE INDEX IX_ResourceActivity_User_Delete ON [activity].[ResourceActivity]([UserId],[Deleted])
WITH (FILLFACTOR = 95);
GO