CREATE TABLE [activity].[NodeActivity] (
    [Id]                     INT                IDENTITY (1, 1) NOT NULL,
    [UserId]                 INT                NOT NULL,
    [NodeId]                 INT                NOT NULL,
    [CatalogueNodeVersionId] INT                NOT NULL,
    [ActivityStatusId]       INT                NOT NULL,
    [Deleted]                BIT                NOT NULL,
    [CreateUserID]           INT                NOT NULL,
    [CreateDate]             DATETIMEOFFSET (7) NOT NULL,
    [AmendUserID]            INT                NOT NULL,
    [AmendDate]              DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Activity_NodeActivity] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NodeActivity_ActivityStatus] FOREIGN KEY ([ActivityStatusId]) REFERENCES [activity].[ActivityStatus] ([Id]),
    CONSTRAINT [FK_NodeActivity_AmendUser] FOREIGN KEY ([AmendUserID]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_NodeActivity_CreateUser] FOREIGN KEY ([CreateUserID]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_NodeActivity_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id])
);

