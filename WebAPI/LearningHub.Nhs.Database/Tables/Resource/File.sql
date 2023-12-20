CREATE TABLE [resources].[File] (
    [Id]                INT                IDENTITY (1, 1) NOT NULL,
    [FileTypeId]        INT                NOT NULL,
    [FileChunkDetailId] INT                NULL,
    [FileName]          NVARCHAR (255)     NOT NULL,
    [FilePath]          NVARCHAR (1024)    NOT NULL,
    [FileSizeKb]        INT                NULL,
    [Deleted]           BIT                NOT NULL,
    [CreateUserId]      INT                NOT NULL,
    [CreateDate]        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]       INT                NOT NULL,
    [AmendDate]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Resources_File] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_File_FileType] FOREIGN KEY ([FileTypeId]) REFERENCES [resources].[FileType] ([Id])
);


GO


GO


GO
