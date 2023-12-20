CREATE TABLE [migrations].[MigrationInputRecord] (
    [Id]                           INT                IDENTITY (1, 1) NOT NULL,
    [MigrationId]                  INT                NOT NULL,
    [MigrationInputRecordStatusId] INT                NOT NULL,
    [Data]                         NVARCHAR (MAX)     NOT NULL,
    [RecordReference]              NVARCHAR (255)     NULL,
    [RecordTitle]                  NVARCHAR (255)     NULL,
    [ScormEsrLinkUrl]              NVARCHAR (4000)    NULL,
    [ValidationErrors]             NVARCHAR (MAX)     NULL,
    [ValidationWarnings]           NVARCHAR (MAX)     NULL,
    [ExceptionDetails]             NVARCHAR (MAX)     NULL,
    [ResourceVersionId]            INT                NULL,
    [Deleted]                      BIT                NOT NULL,
    [CreateUserId]                 INT                NOT NULL,
    [CreateDate]                   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]                  INT                NOT NULL,
    [AmendDate]                    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_MigrationInputRecord] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MigrationInputRecord_Migration] FOREIGN KEY ([MigrationId]) REFERENCES [migrations].[Migration] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MigrationInputRecord_MigrationInputRecordStatus] FOREIGN KEY ([MigrationInputRecordStatusId]) REFERENCES [migrations].[MigrationInputRecordStatus] ([Id]),
    CONSTRAINT [FK_MigrationInputRecord_ResourceVersionId] FOREIGN KEY ([ResourceVersionId]) REFERENCES [resources].[ResourceVersion] ([Id])
);








GO
CREATE NONCLUSTERED INDEX [IX_MigrationInputRecord_ResourceVersionId]
    ON [migrations].[MigrationInputRecord]([ResourceVersionId] ASC)
    INCLUDE([MigrationId]);


GO
CREATE NONCLUSTERED INDEX [IX_MigrationInputRecord_MigrationId_MigrationInputRecordStatusId]
    ON [migrations].[MigrationInputRecord]([MigrationId] ASC, [MigrationInputRecordStatusId] ASC);

