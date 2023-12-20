CREATE TABLE [activity].[MediaResourceActivity] (
    [Id]                 INT                IDENTITY (1, 1) NOT NULL,
    [ResourceActivityId] INT                NOT NULL,
    [ActivityStart]      DATETIMEOFFSET (7) NOT NULL,
    [SecondsPlayed]      INT                NULL,
    [PercentComplete]    DECIMAL (7, 3)     NULL,
    [CreateDate]         DATETIMEOFFSET (7) NOT NULL,
    [CreateUserID]       INT                NOT NULL,
    [AmendUserID]        INT                NOT NULL,
    [AmendDate]          DATETIMEOFFSET (7) NOT NULL,
    [Deleted]            BIT                NOT NULL,
    CONSTRAINT [PK_MediaResourceActivity] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MediaResourceActivity_ResourceActivity] FOREIGN KEY ([ResourceActivityId]) REFERENCES [activity].[ResourceActivity] ([Id]),
    CONSTRAINT [FK_MediaResourceActivityLocation_AmendUser] FOREIGN KEY ([AmendUserID]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_MediaResourceActivityLocation_CreateUser] FOREIGN KEY ([CreateUserID]) REFERENCES [hub].[User] ([Id])
);

GO

CREATE INDEX IX_MediaResourceActivity_ResourceActivity ON [activity].MediaResourceActivity(ResourceActivityId)
WITH (FILLFACTOR = 95);
GO
