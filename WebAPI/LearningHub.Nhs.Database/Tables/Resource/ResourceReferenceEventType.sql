﻿CREATE TABLE [resources].[ResourceReferenceEventType] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (32)      NULL,
    [Description]  NVARCHAR (200)     NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_ResourceReferenceEventType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

