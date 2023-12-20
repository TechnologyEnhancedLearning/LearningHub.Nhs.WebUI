CREATE TABLE [resources].[ImageResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[ImageFileId] [int] NULL,
	[AltTag] [nvarchar](125) NULL,
	[Description] [nvarchar](max) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ImageResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ImageResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ImageResourceVersion_File] FOREIGN KEY([ImageFileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[ImageResourceVersion] CHECK CONSTRAINT [FK_ImageResourceVersion_File]
GO

ALTER TABLE [resources].[ImageResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ImageResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ImageResourceVersion] CHECK CONSTRAINT [FK_ImageResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ImageResourceVersion]
    ON [resources].[ImageResourceVersion]([ResourceVersionId] ASC);
GO