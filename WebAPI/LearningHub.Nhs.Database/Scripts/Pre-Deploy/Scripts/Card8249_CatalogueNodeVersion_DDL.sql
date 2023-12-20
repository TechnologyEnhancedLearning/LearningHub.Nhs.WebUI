/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Killian Davies - 27 Jan 2021
	Card 8249 - Add RestrictedAccess column to CatalogueNodeVersion table.
--------------------------------------------------------------------------------------
*/
CREATE TABLE [hierarchy].[CatalogueNodeVersion_temp](
	[Id] [int] NOT NULL,
	[NodeVersionId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Url] [nvarchar](1000) NOT NULL,
	[BadgeUrl] [nvarchar](128) NULL,
	[BannerUrl] [nvarchar](128) NULL,
	[Description] [nvarchar](max) NOT NULL,
	[OwnerName] [nvarchar](250) NOT NULL,
	[OwnerEmailAddress] [nvarchar](250) NOT NULL,
	[Notes] [nvarchar](max) NOT NULL,
	[Order] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL
) 
GO

INSERT INTO [hierarchy].[CatalogueNodeVersion_temp]
           ([Id],[NodeVersionId],[Name],[Url],[BadgeUrl],[BannerUrl],[Description],[OwnerName],[OwnerEmailAddress],
		   [Notes],[Order],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
SELECT [Id],[NodeVersionId],[Name],[Url],[BadgeUrl],[BannerUrl],[Description],[OwnerName],[OwnerEmailAddress],
	   [Notes],[Order],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]
FROM [hierarchy].[CatalogueNodeVersion]

ALTER TABLE [hierarchy].[CatalogueNodeVersion] DROP CONSTRAINT [FK_CatalogueNodeVersion_NodeVersion]
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionKeyword] DROP CONSTRAINT [FK_CatalogueNodeVersionKeyword_CatalogueNodeVersion]
GO

DROP TABLE [hierarchy].[CatalogueNodeVersion]
GO

CREATE TABLE [hierarchy].[CatalogueNodeVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeVersionId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Url] [nvarchar](1000) NOT NULL,
	[BadgeUrl] [nvarchar](128) NULL,
	[BannerUrl] [nvarchar](128) NULL,
	[Description] [nvarchar](max) NOT NULL,
	[OwnerName] [nvarchar](250) NOT NULL,
	[OwnerEmailAddress] [nvarchar](250) NOT NULL,
	[Notes] [nvarchar](max) NOT NULL,
	[Order] [int] NOT NULL,
	[RestrictedAccess] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_CatalogueNodeVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

SET IDENTITY_INSERT [hierarchy].[CatalogueNodeVersion] ON

INSERT INTO [hierarchy].[CatalogueNodeVersion]
           ([Id],[NodeVersionId],[Name],[Url],[BadgeUrl],[BannerUrl],[Description],[OwnerName],[OwnerEmailAddress],
		   [Notes],[Order],[RestrictedAccess],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
SELECT [Id],[NodeVersionId],[Name],[Url],[BadgeUrl],[BannerUrl],[Description],[OwnerName],[OwnerEmailAddress],
		   [Notes],[Order],0,[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]
FROM [hierarchy].[CatalogueNodeVersion_temp]

SET IDENTITY_INSERT [hierarchy].[CatalogueNodeVersion] OFF

ALTER TABLE [hierarchy].[CatalogueNodeVersion]  WITH CHECK ADD  CONSTRAINT [FK_CatalogueNodeVersion_NodeVersion] FOREIGN KEY([NodeVersionId])
REFERENCES [hierarchy].[NodeVersion] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersion] CHECK CONSTRAINT [FK_CatalogueNodeVersion_NodeVersion]
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionKeyword]  WITH CHECK ADD  CONSTRAINT [FK_CatalogueNodeVersionKeyword_CatalogueNodeVersion] FOREIGN KEY([CatalogueNodeVersionId])
REFERENCES [hierarchy].[CatalogueNodeVersion] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueNodeVersionKeyword] CHECK CONSTRAINT [FK_CatalogueNodeVersionKeyword_CatalogueNodeVersion]
GO

DROP TABLE  [hierarchy].[CatalogueNodeVersion_temp]
GO
