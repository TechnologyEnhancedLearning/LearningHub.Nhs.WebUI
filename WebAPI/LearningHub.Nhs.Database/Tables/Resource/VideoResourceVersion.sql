CREATE TABLE [resources].[VideoResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[VideoFileId] [int] NOT NULL,
	[TranscriptFileId] [int] NULL,
	[ClosedCaptionsFileId] [int] NULL,
	[DurationInMilliseconds] [int] NULL,
	[ResourceAzureMediaAssetId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_VideoResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[VideoResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_VideoResource_ClosedCaptionsFile] FOREIGN KEY([ClosedCaptionsFileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[VideoResourceVersion] CHECK CONSTRAINT [FK_VideoResource_ClosedCaptionsFile]
GO

ALTER TABLE [resources].[VideoResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_VideoResource_File] FOREIGN KEY([VideoFileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[VideoResourceVersion] CHECK CONSTRAINT [FK_VideoResource_File]
GO

ALTER TABLE [resources].[VideoResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_VideoResource_TranscriptFile] FOREIGN KEY([TranscriptFileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[VideoResourceVersion] CHECK CONSTRAINT [FK_VideoResource_TranscriptFile]
GO

ALTER TABLE [resources].[VideoResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_VideoResource_VideoResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[VideoResourceVersion] CHECK CONSTRAINT [FK_VideoResource_VideoResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_VideoResourceVersion]
    ON [resources].[VideoResourceVersion]([ResourceVersionId] ASC);
GO

ALTER TABLE [resources].[VideoResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_VideoResourceVersion_ResourceAzureMediaAsset] FOREIGN KEY([ResourceAzureMediaAssetId])
REFERENCES [resources].[ResourceAzureMediaAsset] ([Id])
GO

ALTER TABLE [resources].[VideoResourceVersion] CHECK CONSTRAINT [FK_VideoResourceVersion_ResourceAzureMediaAsset]
GO
