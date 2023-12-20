CREATE TABLE [migrations].[Migration] (
    [Id]                          INT                IDENTITY (1, 1) NOT NULL,
    [MigrationSourceId]           INT                NOT NULL,
    [MigrationStatusId]           INT                NOT NULL,
    [DestinationNodeId]           INT                NULL,
    [AzureMigrationContainerName] NVARCHAR (256)     NOT NULL,
    [MetadataFileName]            NVARCHAR (1024)    NULL,
    [MetadataFilePath]            NVARCHAR (1024)    NULL,
    [DefaultEsrLinkTypeId]        INT                NULL,
    [Deleted]                     BIT                NOT NULL,
    [CreateUserId]                INT                NOT NULL,
    [CreateDate]                  DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]                 INT                NOT NULL,
    [AmendDate]                   DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Migration] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Migration_MigrationSource] FOREIGN KEY ([MigrationSourceId]) REFERENCES [migrations].[MigrationSource] ([Id]),
    CONSTRAINT [FK_Migration_MigrationStatus] FOREIGN KEY ([MigrationStatusId]) REFERENCES [migrations].[MigrationStatus] ([Id]),
    CONSTRAINT [FK_Migration_Node] FOREIGN KEY ([DestinationNodeId]) REFERENCES [hierarchy].[Node] ([Id])
);



