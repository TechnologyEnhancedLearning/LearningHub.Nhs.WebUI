CREATE TABLE [activity].[MediaResourcePlayedSegment] (
    [Id]               INT                IDENTITY (1, 1) NOT NULL,
    [ResourceId]       INT                NOT NULL,
    [UserId]           INT                NOT NULL,
    [MajorVersion]     INT                NOT NULL,
    [SegmentStartTime] INT                NOT NULL,
    [SegmentEndTime]   INT                NOT NULL,
    [SegmentDuration]  INT                NOT NULL,
    [Deleted]          BIT                NOT NULL,
    [CreateUserId]     INT                NOT NULL,
    [CreateDate]       DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]      INT                NOT NULL,
    [AmendDate]        DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_MediaResourcePlayedSegment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MediaResourcePlayedSegment_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [resources].[Resource] ([Id]),
    CONSTRAINT [FK_MediaResourcePlayedSegment_User] FOREIGN KEY ([UserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_MediaResourcePlayedSegment_User1] FOREIGN KEY ([AmendUserId]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_MediaResourcePlayedSegment_User2] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id])
);
GO

CREATE INDEX IX_MediaResourcePlayedSegment_User ON [activity].MediaResourcePlayedSegment(UserId)
WITH (FILLFACTOR = 95);
GO
