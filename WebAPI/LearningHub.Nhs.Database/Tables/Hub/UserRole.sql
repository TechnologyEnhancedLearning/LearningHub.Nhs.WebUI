CREATE TABLE [hub].[UserRole]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    [ScopeOrganisationId] INT NULL,
    [ScopeCatalogueId] INT NULL,
    [ScopeCategoryId] INT NULL,
    [ScopeSelfAssessmentId] INT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_UserRole_CreateDate] DEFAULT (SYSDATETIMEOFFSET()),
    [CreateUserId] INT NULL,

    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,

    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRole_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_UserRole_Role] FOREIGN KEY ([RoleId]) REFERENCES [hub].[Role] ([Id]),
    CONSTRAINT [FK_UserRole_Organisation] FOREIGN KEY ([ScopeOrganisationId]) REFERENCES [hub].[Organisation] ([Id]),
    CONSTRAINT [FK_UserRole_Catalogue] FOREIGN KEY ([ScopeCatalogueId]) REFERENCES [hierarchy].[CatalogueNodeVersion] ([Id]),
    CONSTRAINT [FK_UserRole_Category] FOREIGN KEY ([ScopeCategoryId]) REFERENCES [hierarchy].[CatalogueNodeVersionCategory] ([Id]),
);