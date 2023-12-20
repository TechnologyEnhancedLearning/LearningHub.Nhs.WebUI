CREATE TABLE [hierarchy].[CatalogueNodeVersionProvider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CatalogueNodeVersionId] [int] NOT NULL,
	[ProviderId] [int] NOT NULL,
	[RemovalDate] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_catalogueNodeVersionProvider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionProvider]  WITH CHECK ADD  CONSTRAINT [FK_catalogueNodeVersionProvider_catalogueNodeVersion] FOREIGN KEY([CatalogueNodeVersionId])
REFERENCES [hierarchy].[CatalogueNodeVersion] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionProvider] CHECK CONSTRAINT [FK_catalogueNodeVersionProvider_catalogueNodeVersion]
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionProvider]  WITH CHECK ADD  CONSTRAINT [FK_catalogueNodeVersionProvider_provider] FOREIGN KEY([ProviderId])
REFERENCES [hub].[Provider] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionProvider] CHECK CONSTRAINT [FK_catalogueNodeVersionProvider_provider]
GO