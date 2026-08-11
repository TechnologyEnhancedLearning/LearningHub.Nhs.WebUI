CREATE TABLE [hub].[OrganisationType]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [OrganisationType] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    [EligibilityLevelId] INT NOT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL,
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_OrganisationType] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_OrganisationType_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User]([Id]),
    CONSTRAINT [FK_OrganisationType_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User]([Id]),
    CONSTRAINT [FK_OrganisationType_RemoveUser] FOREIGN KEY ([RemoveUserId]) REFERENCES [hub].[User]([Id])
);
GO