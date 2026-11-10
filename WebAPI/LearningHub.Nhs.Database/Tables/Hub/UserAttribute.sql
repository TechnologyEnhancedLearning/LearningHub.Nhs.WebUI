CREATE TABLE [hub].[UserAttribute]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [AttributeId] INT NOT NULL,
    [TextValue] NVARCHAR(255) NULL,
    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_userAttribute_CreateDate] DEFAULT SYSDATETIMEOFFSET(),
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,



    CONSTRAINT [PK_userAttribute] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_userAttribute_AttributeId] FOREIGN KEY ([AttributeId]) REFERENCES [hub].[attribute] ([Id]),
    CONSTRAINT [FK_userAttribute_UserId] FOREIGN KEY ([UserId]) REFERENCES [hub].[user] ([Id])
);