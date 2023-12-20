CREATE TABLE [hierarchy].[NodeResourceLookup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodeResourceLookup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))
GO

ALTER TABLE [hierarchy].[NodeResourceLookup]  WITH CHECK ADD  CONSTRAINT [FK_NodeResourceLookup_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO


ALTER TABLE [hierarchy].[NodeResourceLookup]  WITH CHECK ADD  CONSTRAINT [FK_NodeResourceLookup_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO