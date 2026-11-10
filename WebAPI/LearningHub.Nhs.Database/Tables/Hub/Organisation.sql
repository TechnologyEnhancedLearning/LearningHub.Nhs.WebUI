CREATE TABLE [hub].[Organisation]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [OrganisationName] NVARCHAR(255) NOT NULL,
    [ODSCode] NVARCHAR(50) NULL,
    [PostCode] NVARCHAR(20) NULL,

    [OrganisationTypeId] INT NOT NULL,
    [ParentId] INT NULL,
    [RegionId] INT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_Organisation_CreateDate] DEFAULT (SYSDATETIMEOFFSET()),

    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_Organisation_OrganisationType] FOREIGN KEY ([OrganisationTypeId]) REFERENCES [hub].[OrganisationType] ([Id]),

    CONSTRAINT [FK_Organisation_Parent] FOREIGN KEY ([ParentId]) REFERENCES [hub].[Organisation] ([Id]),

    CONSTRAINT [FK_Organisation_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id]),

    CONSTRAINT [FK_Organisation_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id]),

    CONSTRAINT [FK_Organisation_RemoveUser] FOREIGN KEY ([RemoveUserId]) REFERENCES [hub].[User] ([Id])
);
GO