CREATE TABLE [hub].[UserExternalSystem]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [ExternalSystemId] INT NOT NULL,
    [Active] BIT NOT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_UserExternalSystem_CreateDate] DEFAULT (SYSDATETIMEOFFSET()),
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_UserExternalSystem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserExternalSystem_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_UserExternalSystem_ExternalSystem] FOREIGN KEY ([ExternalSystemId]) REFERENCES [external].[ExternalSystem] ([Id]),
    CONSTRAINT [FK_UserExternalSystem_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id])
);
GO