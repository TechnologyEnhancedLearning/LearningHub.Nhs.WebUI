CREATE TABLE [content].[Page] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (128)     NOT NULL,
    [Url]          NVARCHAR (512)     NOT NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Pages_Page] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Page_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

