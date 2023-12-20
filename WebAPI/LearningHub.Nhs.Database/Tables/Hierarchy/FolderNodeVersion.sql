CREATE TABLE [hierarchy].[FolderNodeVersion] (
    [Id]            INT                IDENTITY (1, 1) NOT NULL,
    [NodeVersionId] INT                NOT NULL,
    [Name]          NVARCHAR (255)     NOT NULL,
    [Description]   NVARCHAR (4000)    NOT NULL,
    [Deleted]       BIT                NOT NULL,
    [CreateUserId]  INT                NOT NULL,
    [CreateDate]    DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]   INT                NOT NULL,
    [AmendDate]     DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Hierarchy_FolderNodeVersion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FolderNodeVersion_NodeVersion] FOREIGN KEY ([NodeVersionId]) REFERENCES [hierarchy].[NodeVersion] ([Id])
);
GO

CREATE INDEX IX_FolderNodeVersion_NodeVersionId ON [hierarchy].[FolderNodeVersion](NodeVersionId)
WITH (FILLFACTOR = 95);
GO