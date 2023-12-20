CREATE TABLE [activity].[AssessmentResourceActivity] (
    [Id]                 INT                IDENTITY (1, 1) NOT NULL,
    [ResourceActivityId] INT                NOT NULL,
    [Score]              DECIMAL (7, 3)     NULL,
    [CreateDate]         DATETIMEOFFSET (7) NOT NULL,
    [CreateUserID]       INT                NOT NULL,
    [AmendUserID]        INT                NOT NULL,
    [AmendDate]          DATETIMEOFFSET (7) NOT NULL,
    [Deleted]            BIT                NOT NULL,
    [Reason]             NVARCHAR (240)     NULL,
    CONSTRAINT [PK_AssessmentResourceActivity] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AssessmentResourceActivity_ResourceActivity] FOREIGN KEY ([ResourceActivityId]) REFERENCES [activity].[ResourceActivity] ([Id]),
    CONSTRAINT [FK_AssessmentResourceActivityLocation_AmendUser] FOREIGN KEY ([AmendUserID]) REFERENCES [hub].[User] ([Id]),
    CONSTRAINT [FK_AssessmentResourceActivityLocation_CreateUser] FOREIGN KEY ([CreateUserID]) REFERENCES [hub].[User] ([Id])
);

GO

CREATE INDEX IX_AssessmentResourceActivity_ResourceActivity ON [activity].AssessmentResourceActivity(ResourceActivityId)
WITH (FILLFACTOR = 95);
GO
