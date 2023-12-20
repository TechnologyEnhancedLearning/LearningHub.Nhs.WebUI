CREATE TABLE [content].[VideoAsset] (
    [Id]                     INT                IDENTITY (1, 1) NOT NULL,
    [PageSectionDetailId]    INT                NOT NULL,
    [VideoFileId]            INT                NULL,
    [TranscriptFileId]       INT                NULL,
    [ClosedCaptionsFileId]   INT                NULL,
    [ThumbnailImageFileId]   INT                NULL,
    [DurationInMilliseconds] INT                NULL,
    [AzureMediaAssetId]      INT                NULL,
    [Deleted]                BIT                NOT NULL,
    [CreateUserId]           INT                NOT NULL,
    [CreateDate]             DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]            INT                NOT NULL,
    [AmendDate]              DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Resources_VideoAssett] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PageSectionDetail_VideoAssett] FOREIGN KEY ([PageSectionDetailId]) REFERENCES [content].[PageSectionDetail] ([Id]),
    CONSTRAINT [FK_VideoAssett_File] FOREIGN KEY ([ClosedCaptionsFileId]) REFERENCES [resources].[File] ([Id]),
    CONSTRAINT [FK_VideoAssett_ResourceAzureMediaAsset] FOREIGN KEY ([AzureMediaAssetId]) REFERENCES [resources].[ResourceAzureMediaAsset] ([Id]),
    CONSTRAINT [FK_VideoAssett_ThumbnailFile] FOREIGN KEY ([ThumbnailImageFileId]) REFERENCES [resources].[File] ([Id]),
    CONSTRAINT [FK_VideoAssett_TransScriptFile] FOREIGN KEY ([TranscriptFileId]) REFERENCES [resources].[File] ([Id]),
    CONSTRAINT [FK_VideoAssett_VideoFile] FOREIGN KEY ([VideoFileId]) REFERENCES [resources].[File] ([Id])
);

