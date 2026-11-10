CREATE TABLE [hub].[User]
(
    [Id] INT NOT NULL,
    [FirstName] NVARCHAR(50) NULL,
    [LastName] NVARCHAR(50) NULL,
    [EmailAddress] NVARCHAR(100) NULL,
    [RecoveryEmailAddress] NVARCHAR(100) NULL,
    [LegacyUserName] NVARCHAR(50) NOT NULL,
    [ProfessionalBodyId] INT NULL,
    [ProfessionalRegistrationNumber] NVARCHAR(50) NULL,
    [Active] BIT NULL,
    [PasswordHash] NVARCHAR(255) NULL,
    [MustChangePassword] BIT NULL,
    [PasswordLifeCounter] INT NULL,
    [SecurityLifeCounter] INT NULL,
    [RemoteLoginKey] NVARCHAR(50) NULL,
    [RemoteLoginGuid] UNIQUEIDENTIFIER NULL,
    [RemoteLoginStart] DATETIMEOFFSET(7) NULL,
    [RestrictToSSO] BIT NULL,
    [RequestUserLogout] BIT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL,
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,
    [RemovalMethodId] INT NULL,

    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_User_ProfessionalBody] FOREIGN KEY ([ProfessionalBodyId]) REFERENCES [hub].[ProfessionalBody] ([Id])
);
GO
