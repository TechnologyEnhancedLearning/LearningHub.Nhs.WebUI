CREATE TABLE [resources].[EmbeddedResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[EmbedCode] [nvarchar](255) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ResourceVersion_EmbeddedResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[EmbeddedResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_EmbeddedResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[EmbeddedResourceVersion] CHECK CONSTRAINT [FK_EmbeddedResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_EmbeddedResourceVersion]
    ON [resources].[EmbeddedResourceVersion]([ResourceVersionId] ASC);
GO