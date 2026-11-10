CREATE TABLE [hub].[ProfessionalBody]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [ProfessionalBody] NVARCHAR(100) NOT NULL,
    [OrderByNumber] INT NOT NULL,
    [PlaceholderText] NVARCHAR(50) NOT NULL,
    [HelpText] NVARCHAR(250) NOT NULL,
    [RegexPattern] NVARCHAR(100) NULL,
    [RegisterUrl] NVARCHAR(250) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_ProfessionalBody_IsActive] DEFAULT (1),

    [CreateDate] DATETIMEOFFSET(7) NOT NULL,
    [CreateUserId] INT NULL,
    [AmendDate] DATETIMEOFFSET(7) NULL,
    [AmendUserId] INT NULL,
    [RemoveDate] DATETIMEOFFSET(7) NULL,
    [RemoveUserId] INT NULL,

    CONSTRAINT [PK_ProfessionalBody] PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT [FK_ProfessionalBody_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User]([Id]),
    CONSTRAINT [FK_ProfessionalBody_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User]([Id]),
    CONSTRAINT [FK_ProfessionalBody_RemoveUser] FOREIGN KEY ([RemoveUserId]) REFERENCES [hub].[User]([Id])
);
GO