CREATE TABLE [resources].[HtmlResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[FileId] [int] NULL,
	[ContentFilePath] [nvarchar](1024) NULL,
	[PopupWidth] [int] NULL,
	[PopupHeight] [int] NULL,
	[EsrLinkTypeId] [int] NULL DEFAULT 1,  
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_HtmlResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[HtmlResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_HtmlResourceVersion_File] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[HtmlResourceVersion] CHECK CONSTRAINT [FK_HtmlResourceVersion_File]
GO

ALTER TABLE [resources].[HtmlResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_HtmlResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[HtmlResourceVersion] CHECK CONSTRAINT [FK_HtmlResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_HtmlResourceVersion]
    ON [resources].[HtmlResourceVersion]([ResourceVersionId] ASC);
GO