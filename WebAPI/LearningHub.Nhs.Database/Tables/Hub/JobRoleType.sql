CREATE TABLE [hub].[JobRoleType]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [JobRoleType] NVARCHAR(50) NOT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL,
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_JobRoleType] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_JobRoleType_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User]([Id]),
    CONSTRAINT [FK_JobRoleType_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User]([Id]),
    CONSTRAINT [FK_JobRoleType_RemoveUser] FOREIGN KEY ([RemoveUserId]) REFERENCES [hub].[User]([Id])
);
GO