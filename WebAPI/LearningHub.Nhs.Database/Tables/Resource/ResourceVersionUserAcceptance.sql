CREATE TABLE [resources].[ResourceVersionUserAcceptance] (
    [Id]                INT                IDENTITY (1, 1) NOT NULL,
    [ResourceVersionId] INT                NOT NULL,
    [UserId]            INT                NOT NULL,
    [Deleted]           BIT                NOT NULL,
    [CreateUserId]      INT                NOT NULL,
    [CreateDate]        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]       INT                NOT NULL,
    [AmendDate]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_ResourceVersionUserAcceptance] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResourceVersionUserAcceptance_ResourceVersion] FOREIGN KEY ([ResourceVersionId]) REFERENCES [resources].[ResourceVersion] ([Id]),
    CONSTRAINT [FK_ResourceVersionUserAcceptance_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_ResourceVersionUserAcceptance_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_ResourceVersionUserAcceptance_AmendUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id])
);

