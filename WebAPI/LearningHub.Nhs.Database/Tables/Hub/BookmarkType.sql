CREATE TABLE [hub].[BookmarkType] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (100)      NOT NULL,
    [Description]  VARCHAR (255)      NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_BookmarkType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

