CREATE TABLE [content].[ImageAsset] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [PageSectionDetailId] INT                NOT NULL,
    [ImageFileId]         INT                NULL,
    [AltTag]              NVARCHAR (125)     NULL,
    [Description]         NVARCHAR (MAX)     NULL,
    [Deleted]             BIT                NOT NULL,
    [CreateUserId]        INT                NOT NULL,
    [CreateDate]          DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]         INT                NOT NULL,
    [AmendDate]           DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Resources_ImageAsset] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ImageAsset_File] FOREIGN KEY ([ImageFileId]) REFERENCES [resources].[File] ([Id]),
    CONSTRAINT [FK_PageSectionDetail_ImageAsset] FOREIGN KEY ([PageSectionDetailId]) REFERENCES [content].[PageSectionDetail] ([Id])
);

