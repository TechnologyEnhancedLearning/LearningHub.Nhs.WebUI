CREATE TABLE [resources].[ResourceReference](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceId] [int] NOT NULL,
	[NodePathId] [int] NULL,
	[OriginalResourceReferenceId] [int] NULL,
	[IsActive] [bit] NOT NULL DEFAULT 1, -- Default set so that existing records are active and column can be added during release
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceReference] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceReference]  WITH CHECK ADD  CONSTRAINT [FK_ResourceReference_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [resources].[ResourceReference] CHECK CONSTRAINT [FK_ResourceReference_NodePath]
GO

ALTER TABLE [resources].[ResourceReference]  WITH CHECK ADD  CONSTRAINT [FK_ResourceReference_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO

ALTER TABLE [resources].[ResourceReference] CHECK CONSTRAINT [FK_ResourceReference_Resource]
GO

ALTER TABLE [resources].[ResourceReference]  WITH CHECK ADD  CONSTRAINT [FK_OriginalResourceReference_ResourceReference] FOREIGN KEY([OriginalResourceReferenceId])
REFERENCES [resources].[ResourceReference] ([Id])
GO
GO

CREATE NONCLUSTERED INDEX [IX_ResourceReference]
	ON [resources].[ResourceReference]([Deleted] ASC) INCLUDE([Id], [ResourceId], [NodePathId]);
GO

CREATE INDEX IX_ResourceReference_Resource ON [resources].ResourceReference(ResourceId) WITH FILLFACTOR=95
GO