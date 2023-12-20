CREATE TABLE [resources].[ResourceVersionRating] (
    [Id]                INT                IDENTITY (1, 1) NOT NULL,
    [ResourceVersionId] INT                NOT NULL,
    [UserId]            INT                NOT NULL,
    [Rating]            INT                NOT NULL,
    [JobRoleId]         INT                NOT NULL,
    [LocationId]        INT                NOT NULL,
    [Deleted]           BIT                NOT NULL,
    [CreateUserId]      INT                NOT NULL,
    [CreateDate]        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]       INT                NOT NULL,
    [AmendDate]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Rating] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Rating_ResourceVersion] FOREIGN KEY ([ResourceVersionId]) REFERENCES [resources].[ResourceVersion] ([Id]),
    CONSTRAINT [FK_Rating_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id])
);

