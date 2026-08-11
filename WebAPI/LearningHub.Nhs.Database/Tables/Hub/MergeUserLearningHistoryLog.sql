CREATE TABLE [hub].[MergeUserLearningHistoryLog]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FromUserId] INT NOT NULL,
    [ToUserId] INT NOT NULL,

    [CreateDate] DATETIMEOFFSET(7) NOT NULL CONSTRAINT [DF_MergeUserLearningHistoryLog_CreateDate] DEFAULT (SYSDATETIMEOFFSET()),
    [CreateUserId] INT NOT NULL,

    CONSTRAINT [PK_MergeUserLearningHistoryLog] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MergeUserLearningHistoryLog_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [hub].[User] ([Id])
);