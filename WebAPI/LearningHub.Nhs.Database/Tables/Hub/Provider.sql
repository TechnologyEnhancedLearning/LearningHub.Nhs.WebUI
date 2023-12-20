CREATE TABLE [hub].[Provider] (
    [Id]           INT                NOT NULL,
    [Name]         VARCHAR (255)      NOT NULL,
    [Description]  VARCHAR (255)      NULL,
    [Logo]         VARCHAR (100)      NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Provider] PRIMARY KEY CLUSTERED ([Id] ASC)
);

