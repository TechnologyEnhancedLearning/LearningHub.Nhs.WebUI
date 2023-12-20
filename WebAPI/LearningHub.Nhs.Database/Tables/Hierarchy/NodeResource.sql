CREATE TABLE [hierarchy].[NodeResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[DisplayOrder] [int] NULL,
	[VersionStatusId] [int] NOT NULL,
	[PublicationId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[VersionStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[VersionEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodeResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
	PERIOD FOR SYSTEM_TIME ([VersionStartTime], [VersionEndTime])
)
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeResourceHistory] )
)
GO

ALTER TABLE [hierarchy].[NodeResource]  WITH CHECK ADD  CONSTRAINT [FK_NodeResource_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodeResource] CHECK CONSTRAINT [FK_NodeResource_Node]
GO

ALTER TABLE [hierarchy].[NodeResource]  WITH CHECK ADD  CONSTRAINT [FK_NodeResource_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO

ALTER TABLE [hierarchy].[NodeResource] CHECK CONSTRAINT [FK_NodeResource_Resource]
GO

ALTER TABLE hierarchy.NodeResource ADD CONSTRAINT DF_NodeResource_DisplayOrder DEFAULT 0 FOR DisplayOrder;
GO

CREATE INDEX IX_NodeResource_Resource ON [hierarchy].[NodeResource](ResourceId) WITH FILLFACTOR=95
GO

CREATE NONCLUSTERED INDEX NCI_NodeResource
		ON [hierarchy].[NodeResource] ([NodeId],[Deleted])
		INCLUDE ([ResourceId],[DisplayOrder])

GO