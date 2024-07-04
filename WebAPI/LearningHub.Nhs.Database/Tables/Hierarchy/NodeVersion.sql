CREATE TABLE [hierarchy].[NodeVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[VersionStatusId] [int] NOT NULL,
	[PublicationId] [int] NULL,
	[MajorVersion] [int] NULL,
	[MinorVersion] [int] NULL,
	[PrimaryCatalogueNodeId] [int] NOT NULL DEFAULT 1,
	[Deleted] [bit] NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_Version] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
), 
)
GO

ALTER TABLE [hierarchy].[NodeVersion]  WITH CHECK ADD  CONSTRAINT [FK_NodeVersion_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodeVersion] CHECK CONSTRAINT [FK_NodeVersion_Node]
GO

ALTER TABLE [hierarchy].[NodeVersion]  WITH CHECK ADD  CONSTRAINT [FK_NodeVersion_PrimaryCatalogueNode] FOREIGN KEY([PrimaryCatalogueNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[NodeVersion] CHECK CONSTRAINT [FK_NodeVersion_PrimaryCatalogueNode]
GO