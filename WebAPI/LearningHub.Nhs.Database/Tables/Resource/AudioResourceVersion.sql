CREATE TABLE [resources].[AudioResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[AudioFileId] [int] NOT NULL,
	[TranscriptFileId] [int] NULL,
	[DurationInMilliseconds] [int] NULL,
	[ResourceAzureMediaAssetId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_AudioResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[AudioResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_AudioResource_File] FOREIGN KEY([AudioFileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[AudioResourceVersion] CHECK CONSTRAINT [FK_AudioResource_File]
GO

ALTER TABLE [resources].[AudioResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_AudioResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[AudioResourceVersion] CHECK CONSTRAINT [FK_AudioResourceVersion_ResourceVersion]
GO

ALTER TABLE [resources].[AudioResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_AudioResource_TranscriptFile] FOREIGN KEY([TranscriptFileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[AudioResourceVersion] CHECK CONSTRAINT [FK_AudioResource_TranscriptFile]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AudioResourceVersion]
    ON [resources].[AudioResourceVersion]([ResourceVersionId] ASC);
GO

ALTER TABLE [resources].[AudioResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_AudioResourceVersion_ResourceAzureMediaAsset] FOREIGN KEY([ResourceAzureMediaAssetId])
REFERENCES [resources].[ResourceAzureMediaAsset] ([Id])
GO

ALTER TABLE [resources].[AudioResourceVersion] CHECK CONSTRAINT [FK_AudioResourceVersion_ResourceAzureMediaAsset]
GO