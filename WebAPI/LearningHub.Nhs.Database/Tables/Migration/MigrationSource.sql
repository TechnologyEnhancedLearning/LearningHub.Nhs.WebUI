CREATE TABLE [migrations].[MigrationSource] (
    [Id]           INT                NOT NULL,
    [Description]  NVARCHAR (256)     NULL,
    [HostName]  NVARCHAR (256)        NULL,
    [ResourcePath]  NVARCHAR (256)    NULL,
    [ResourceIdentifierPosition] INT DEFAULT 0 NOT NULL,
    [ResourceRegEx]  NVARCHAR (256)   NULL,
    [DefaultEsrLinkTypeId] INT        NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Migrations_MigrationSource] PRIMARY KEY CLUSTERED ([Id] ASC)
);
