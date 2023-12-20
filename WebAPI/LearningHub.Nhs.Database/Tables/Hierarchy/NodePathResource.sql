CREATE TABLE [hierarchy].[NodePathResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodePathId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePathResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[NodePathResource]  WITH CHECK ADD  CONSTRAINT [FK_NodePathResource_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathResource] CHECK CONSTRAINT [FK_NodePathResource_NodePath]
GO

ALTER TABLE [hierarchy].[NodePathResource]  WITH CHECK ADD  CONSTRAINT [FK_NodePathResource_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathResource] CHECK CONSTRAINT [FK_NodePathResource_Resource]
GO