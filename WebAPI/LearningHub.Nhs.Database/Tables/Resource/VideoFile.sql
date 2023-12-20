CREATE TABLE [resources].[VideoFile]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [FileId] [int] NOT NULL,
    [Status] [int] NOT NULL,
    [ProcessingErrorMessage] [nvarchar](1024) NULL,
    [CaptionsFileId] [int] NULL,
    [TranscriptFileId] [int] NULL,
    [AzureAssetOutputFilePath] [nvarchar](1024) NULL,
    [LocatorUri] [nvarchar](1024) NULL,
    [EncodeJobName] [nvarchar](1024) NULL,
    [DurationInMilliseconds] [int] NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_VideoFile] PRIMARY KEY CLUSTERED
        ([Id] ASC),
)
GO

ALTER TABLE [resources].[VideoFile] WITH CHECK ADD CONSTRAINT [FK_VideoFile_FileId] FOREIGN KEY([FileId])
    REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[VideoFile] CHECK CONSTRAINT [FK_VideoFile_FileId]
GO

ALTER TABLE [resources].[VideoFile] WITH CHECK ADD CONSTRAINT [FK_VideoFile_CaptionsFileId] FOREIGN KEY([CaptionsFileId])
    REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[VideoFile] CHECK CONSTRAINT [FK_VideoFile_CaptionsFileId]
GO

ALTER TABLE [resources].[VideoFile] WITH CHECK ADD CONSTRAINT [FK_VideoFile_TranscriptFileId] FOREIGN KEY([TranscriptFileId])
    REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[VideoFile] CHECK CONSTRAINT [FK_VideoFile_TranscriptFileId]
GO 
