CREATE TABLE [resources].[WebLinkResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[WebLinkURL] [nvarchar](1024) NOT NULL,
	[DisplayText] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_WebLinkResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[WebLinkResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_WebLinkResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[WebLinkResourceVersion] CHECK CONSTRAINT [FK_WebLinkResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WebLinkResourceVersion]
    ON [resources].[WebLinkResourceVersion]([ResourceVersionId] ASC);
GO