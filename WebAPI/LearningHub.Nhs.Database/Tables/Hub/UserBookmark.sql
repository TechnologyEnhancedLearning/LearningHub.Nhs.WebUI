CREATE TABLE [hub].[UserBookmark] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [ParentId]            INT                NULL,
    [UserId]              INT                NULL,
    [BookmarkTypeId]      INT                NOT NULL,
    [ResourceReferenceId] INT                NULL,
    [NodeId]              INT                NULL,
    [Title]               VARCHAR (256)      NOT NULL,
    [Link]                VARCHAR (256)      NULL,
    [Position]            INT                NOT NULL,
    [Deleted]             BIT                NOT NULL,
    [CreateUserId]       INT                NOT NULL,
    [CreateDate]          DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]         INT                NOT NULL,
    [AmendDate]           DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_UserBookmark] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserBookmark_BookmarkType] FOREIGN KEY ([BookmarkTypeId]) REFERENCES [hub].[BookmarkType] ([Id]),
    CONSTRAINT [FK_UserBookmark_ResourceReference] FOREIGN KEY ([ResourceReferenceId]) REFERENCES [resources].[ResourceReference] ([Id]),
    CONSTRAINT [FK_UserBookmark_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_UserBookmark_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_UserBookmark_AmendedUser] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_UserBookmark_UserBookmark] FOREIGN KEY ([ParentId]) REFERENCES [hub].[UserBookmark] ([Id])
);

