CREATE TABLE [hub].[EmailChangeValidationToken] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [UserId]       INT                NOT NULL,
    [Email]        NVARCHAR (128)     NOT NULL,
    [StatusId]     INT                NOT NULL,
    [HashedToken]  NVARCHAR (128)     NOT NULL,
    [Salt]         NVARCHAR (128)     NOT NULL,
    [Lookup]       NVARCHAR (128)     NOT NULL,
    [Expiry]       DATETIMEOFFSET (7) NOT NULL,
    [TenantId]     INT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    [Deleted]      BIT                NOT NULL,
    CONSTRAINT [PK_UserDetailsValidationToken] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmailChangeValidationToken_EmailChangeValidationTokenStatus] FOREIGN KEY ([StatusId]) REFERENCES [hub].[EmailChangeValidationTokenStatus] ([Id]),
    CONSTRAINT [FK_EmailChangeValidationToken_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id])
);
GO
