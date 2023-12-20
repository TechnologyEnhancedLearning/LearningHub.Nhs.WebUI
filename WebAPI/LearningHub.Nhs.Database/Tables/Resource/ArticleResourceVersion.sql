CREATE TABLE [resources].[ArticleResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ArticleResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ArticleResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ArticleResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ArticleResourceVersion] CHECK CONSTRAINT [FK_ArticleResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ArticleResourceVersion]
    ON [resources].[ArticleResourceVersion]([ResourceVersionId] ASC);
GO
