CREATE TABLE [hierarchy].[CatalogueNodeVersionCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CatalogueNodeVersionId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_CatalogueNodeVersionCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionCategory]  WITH CHECK ADD  CONSTRAINT [FK_catalogueNodeVersionCategory_catalogueNodeVersion] FOREIGN KEY([CatalogueNodeVersionId])
REFERENCES [hierarchy].[CatalogueNodeVersion] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionCategory] CHECK CONSTRAINT [FK_catalogueNodeVersionCategory_catalogueNodeVersion]
GO