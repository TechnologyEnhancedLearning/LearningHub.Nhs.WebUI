CREATE TABLE [hierarchy].[NodePathNode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodePathId] [int] NOT NULL,
	[NodeId] [int] NOT NULL,
	[Depth] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePathNode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[NodePathNode]  WITH CHECK ADD  CONSTRAINT [FK_NodePathNode_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathNode] CHECK CONSTRAINT [FK_NodePathNode_Node]
GO

ALTER TABLE [hierarchy].[NodePathNode]  WITH CHECK ADD  CONSTRAINT [FK_NodePathNode_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathNode] CHECK CONSTRAINT [FK_NodePathNode_NodePath]
GO

CREATE INDEX IX_NodePathNode_Node ON [hierarchy].[NodePathNode](NodeId)
WITH (FILLFACTOR = 95);
GO