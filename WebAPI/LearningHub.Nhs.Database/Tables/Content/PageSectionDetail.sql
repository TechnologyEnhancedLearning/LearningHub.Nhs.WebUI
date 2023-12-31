﻿CREATE TABLE [content].[PageSectionDetail] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [PageSectionId]       INT                NOT NULL,
    [PageSectionStatusId] INT                CONSTRAINT [DF_PageSectionDetail_PageSectionStatusId] DEFAULT ((1)) NOT NULL,
    [SectionLayoutTypeId] INT                CONSTRAINT [DF_PageSectionDetail_SectionLayoutTypeId] DEFAULT ((1)) NOT NULL,
    [SectionTitle]        NVARCHAR(128)      NULL,
    [SectionTitleElement] NVARCHAR(128)      NULL,
    [TopMargin]           BIT                NOT NULL DEFAULT 0,
    [BottomMargin]        BIT                NOT NULL DEFAULT 0,
    [HasBorder]           BIT                NOT NULL DEFAULT 0,
    [BackgroundColour]    NVARCHAR (20)      NULL,
    [TextColour]          NVARCHAR (20)      NULL,
    [HyperLinkColour]     NVARCHAR (20)      NULL,
    [TextBackgroundColour] NVARCHAR(20)      NULL ,
    [Description]         NVARCHAR (4000)    NULL,
    [DeletePending]       BIT                NULL,
    [DraftPosition]       INT                NULL,
    [DraftHidden]         BIT                NULL,
    [Deleted]             BIT                NOT NULL,
    [CreateUserId]        INT                NOT NULL,
    [CreateDate]          DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]         INT                NOT NULL,
    [AmendDate]           DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Pages_PageIdentifier] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PageSectionDetail_PageSection] FOREIGN KEY ([PageSectionId]) REFERENCES [content].[PageSection] ([Id]),
    CONSTRAINT [FK_PageSectionDetail_PageSectionStatus] FOREIGN KEY ([PageSectionStatusId]) REFERENCES [content].[PageSectionStatus] ([Id]),
    CONSTRAINT [FK_PageSectionDetail_SectionLayoutType] FOREIGN KEY ([SectionLayoutTypeId]) REFERENCES [content].[SectionLayoutType] ([Id])
);
