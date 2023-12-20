CREATE TABLE [resources].[ArticleResourceVersionFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ArticleResourceVersionId] [int] NOT NULL,
	[FileId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ArticleResourceFile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ArticleResourceVersionFile]  WITH CHECK ADD  CONSTRAINT [FK_ArticleResourceVersionFile_ArticleResourceVersion] FOREIGN KEY([ArticleResourceVersionId])
REFERENCES [resources].[ArticleResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ArticleResourceVersionFile] CHECK CONSTRAINT [FK_ArticleResourceVersionFile_ArticleResourceVersion]
GO

ALTER TABLE [resources].[ArticleResourceVersionFile]  WITH CHECK ADD  CONSTRAINT [FK_ArticleResourceVersionFile_File] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[ArticleResourceVersionFile] CHECK CONSTRAINT [FK_ArticleResourceVersionFile_File]
GO