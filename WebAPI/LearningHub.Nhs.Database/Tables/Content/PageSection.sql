CREATE TABLE [content].[PageSection] (
    [Id]                    INT                IDENTITY (1, 1) NOT NULL,
    [PageId]                INT                NOT NULL,
    [SectionTemplateTypeId] INT                NOT NULL,
    [Position]              INT                NOT NULL,
    [IsHidden]              BIT                NOT NULL,
    [Deleted]               BIT                NOT NULL,
    [CreateUserId]          INT                NOT NULL,
    [CreateDate]            DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]           INT                NOT NULL,
    [AmendDate]             DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_PageSectionGroup_Group] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PageSection_SectionTemplateType] FOREIGN KEY ([SectionTemplateTypeId]) REFERENCES [content].[SectionTemplateType] ([Id]),
    CONSTRAINT [FK_PageSectionGroup_Page] FOREIGN KEY ([PageId]) REFERENCES [content].[Page] ([Id])
);



