CREATE TABLE [content].[PageSectionStatus] (
    [Id]           INT                NOT NULL,
    [Description]  NVARCHAR (32)      NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Page_VersionStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

