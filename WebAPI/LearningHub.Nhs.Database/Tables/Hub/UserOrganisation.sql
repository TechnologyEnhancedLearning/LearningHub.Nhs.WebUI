CREATE TABLE [hub].[UserOrganisation]
(
    [Id] INT IDENTITY(1,1) NOT NULL,

    [UserId] INT NOT NULL,
    [OrganisationId] INT NOT NULL,
    [JobRoleTypeId] INT NOT NULL,
    [JobRole] NVARCHAR(100) NULL,

    [StartDate] DATETIMEOFFSET(7) NULL,
    [EndDate] DATETIMEOFFSET(7) NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_UserOrganisation_CreateDate] DEFAULT (SYSDATETIMEOFFSET()),
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_UserOrganisation] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_UserOrganisation_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),

    CONSTRAINT [FK_UserOrganisation_Organisation] FOREIGN KEY ([OrganisationId]) REFERENCES [hub].[Organisation] ([Id]),

    CONSTRAINT [FK_UserOrganisation_JobRoleType] FOREIGN KEY ([JobRoleTypeId]) REFERENCES [hub].[JobRoleType] ([Id]),

    CONSTRAINT [FK_UserOrganisation_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id]),

    CONSTRAINT [FK_UserOrganisation_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id]),

    CONSTRAINT [FK_UserOrganisation_RemoveUser] FOREIGN KEY ([RemoveUserId]) REFERENCES [hub].[User] ([Id])
);
GO