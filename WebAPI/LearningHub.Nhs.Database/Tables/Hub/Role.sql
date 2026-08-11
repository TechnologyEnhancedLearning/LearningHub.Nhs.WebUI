CREATE TABLE [hub].[Role]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Code] NVARCHAR(50) NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [ScopeType] NVARCHAR(250) NULL,
    [Description] NVARCHAR(500) NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL,
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id]),
    --CONSTRAINT [UQ_Role_Code] UNIQUE ([Code]),
    CONSTRAINT [FK_Role_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_Role_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_Role_RemoveUser] FOREIGN KEY ([RemoveUserId]) REFERENCES [hub].[User] ([Id])
);
GO