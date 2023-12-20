CREATE TABLE [hierarchy].[NodePath](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[NodePath] [nvarchar](256) NOT NULL,
	[CatalogueNodeId] [int] NULL,
	[CourseNodeId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePath] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[NodePath]  WITH CHECK ADD  CONSTRAINT [FK_NodePath_CatalogueNode] FOREIGN KEY([CatalogueNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodePath] CHECK CONSTRAINT [FK_NodePath_CatalogueNode]
GO

ALTER TABLE [hierarchy].[NodePath]  WITH CHECK ADD  CONSTRAINT [FK_NodePath_CourseNode] FOREIGN KEY([CourseNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodePath] CHECK CONSTRAINT [FK_NodePath_CourseNode]
GO

ALTER TABLE [hierarchy].[NodePath]  WITH CHECK ADD  CONSTRAINT [FK_NodePath_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodePath] CHECK CONSTRAINT [FK_NodePath_Node]
GO

CREATE INDEX IX_NodePath_Node ON [hierarchy].[NodePath](NodeId) WITH FILLFACTOR=95
GO