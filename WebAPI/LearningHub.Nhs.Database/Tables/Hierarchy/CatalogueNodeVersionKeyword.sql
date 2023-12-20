CREATE TABLE [hierarchy].[CatalogueNodeVersionKeyword]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CatalogueNodeVersionId] [int] NOT NULL,
	[Keyword] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_CatalogueNodeVersionKeyword] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionKeyword]  WITH CHECK ADD  CONSTRAINT [FK_CatalogueNodeVersionKeyword_CatalogueNodeVersion] FOREIGN KEY([CatalogueNodeVersionId])
REFERENCES [hierarchy].[CatalogueNodeVersion] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionKeyword] CHECK CONSTRAINT [FK_CatalogueNodeVersionKeyword_CatalogueNodeVersion]
GO