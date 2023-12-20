CREATE TABLE [content].[SectionTemplateType] (
    [Id]           INT                NOT NULL,
    [Type]         NVARCHAR (128)     NOT NULL,
    [Description]  NVARCHAR (512)     NOT NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Section_TemplateType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

