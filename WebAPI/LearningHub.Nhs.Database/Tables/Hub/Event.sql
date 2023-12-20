CREATE TABLE [hub].[Event](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventLogId] [int] NOT NULL,
	[EventTypeId] [int] NOT NULL,
	[HierarchyEditId] [int] NULL,
	[NodeId] [int] NULL,
	[ResourceVersionId] [int] NULL,
	[Details] [nvarchar](1024) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hub_Event] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[Event]  WITH CHECK ADD  CONSTRAINT [FK_HubEvent_EventLog] FOREIGN KEY([EventLogId])
REFERENCES [hub].[EventLog] ([Id])
GO

ALTER TABLE [hub].[Event]  WITH CHECK ADD  CONSTRAINT [FK_HubEvent_EventType] FOREIGN KEY([EventTypeId])
REFERENCES [hub].[EventType] ([Id])
GO

ALTER TABLE [hub].[Event]  WITH CHECK ADD  CONSTRAINT [FK_HubEvent_HierarchyEdit] FOREIGN KEY([HierarchyEditId])
REFERENCES [hierarchy].[HierarchyEdit] ([Id])
GO

ALTER TABLE [hub].[Event]  WITH CHECK ADD  CONSTRAINT [FK_HubEvent_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hub].[Event]  WITH CHECK ADD  CONSTRAINT [FK_HubEvent_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

