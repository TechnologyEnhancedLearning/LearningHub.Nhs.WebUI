CREATE TABLE [migrations].[MigrationInputRecordStatus] (
    [Id]           INT                NOT NULL,
    [Description]  NVARCHAR (50)      NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Migrations_MigrationInputRecordStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

