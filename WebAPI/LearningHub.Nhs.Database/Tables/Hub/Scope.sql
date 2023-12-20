CREATE TABLE [hub].[Scope](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScopeTypeId] [int] NOT NULL,
	[CatalogueNodeId] [int] NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Scope] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[Scope]  WITH CHECK ADD  CONSTRAINT [FK_Scope_CatalogueNode] FOREIGN KEY([CatalogueNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hub].[Scope] CHECK CONSTRAINT [FK_Scope_CatalogueNode]
GO

ALTER TABLE [hub].[Scope]  WITH CHECK ADD  CONSTRAINT [FK_Scope_ScopeType] FOREIGN KEY([ScopeTypeId])
REFERENCES [hub].[ScopeType] ([Id])
GO

ALTER TABLE [hub].[Scope] CHECK CONSTRAINT [FK_Scope_ScopeType]
GO