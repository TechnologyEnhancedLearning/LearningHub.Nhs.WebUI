CREATE TABLE [hub].[UserPasswordValidationTokens]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [HashedToken] NVARCHAR(128) NOT NULL,
    [Salt] NVARCHAR(128) NOT NULL,
    [Lookup] NVARCHAR(128) NOT NULL,
    [Expiry] DATETIMEOFFSET(7) NOT NULL,
    [TenantId] INT NOT NULL,
    [UserId] INT NOT NULL,

    [CreateUserId] INT NOT NULL,
    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_UserPasswordValidationTokens_CreateDate] DEFAULT (SYSDATETIMEOFFSET()),

    CONSTRAINT [PK_UserPasswordValidationTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserPasswordValidationTokens_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [hub].[Tenant] ([Id]),
    CONSTRAINT [FK_UserPasswordValidationTokens_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_UserPasswordValidationTokens_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id])
);