CREATE TABLE [activity].[ScormActivityObjective] (
    [Id]              INT                IDENTITY (1, 1) NOT NULL,
    [ScormActivityId] INT                NOT NULL,
    [ObjectiveId]     NVARCHAR (255)     NULL,
    [SequenceNumber]  INT                CONSTRAINT [DF_ScormActivityObjective_SequenceNumber] DEFAULT ((0)) NOT NULL,
    [Score_raw]       NVARCHAR (255)     NULL,
    [Score_max]       NVARCHAR (255)     NULL,
    [Score_min]       NVARCHAR (255)     NULL,
    [Status]          NVARCHAR (255)     NULL,
    [Deleted]         BIT                NOT NULL,
    [CreateUserID]    INT                NOT NULL,
    [CreateDate]      DATETIMEOFFSET (7) NOT NULL,
    [AmendUserID]     INT                NOT NULL,
    [AmendDate]       DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Activity_ScormActivityObjective] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ScormActivityObjective_ScormActivity] FOREIGN KEY ([ScormActivityId]) REFERENCES [activity].[ScormActivity] ([Id]),
    CONSTRAINT UC_ScormActivityObjective UNIQUE (ScormActivityId,SequenceNumber)
);






GO