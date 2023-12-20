CREATE TABLE [activity].[ScormActivityInteraction] (
    [Id]               INT                IDENTITY (1, 1) NOT NULL,
    [ScormActivityId]  INT                NOT NULL,
    [InteractionId]    NVARCHAR (255)     NULL,
    [SequenceNumber]   INT                CONSTRAINT [DF_ScormActivityInteraction_SequenceNumber] DEFAULT ((0)) NOT NULL,
    [Type]             NVARCHAR (255)     NULL,
    [Weighting]        DECIMAL (16, 2)    NULL,
    [Student_response] NVARCHAR (255)     NULL,
    [Result]           NVARCHAR (255)     NULL,
    [Latency]          NVARCHAR (255)     NULL,
    [Deleted]          BIT                NOT NULL,
    [CreateUserID]     INT                NOT NULL,
    [CreateDate]       DATETIMEOFFSET (7) NOT NULL,
    [AmendUserID]      INT                NOT NULL,
    [AmendDate]        DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Activity_ScormActivityInteraction] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ScormActivityInteraction_ScormActivity] FOREIGN KEY ([ScormActivityId]) REFERENCES [activity].[ScormActivity] ([Id]),
    CONSTRAINT UC_ScormActivityInteraction UNIQUE (ScormActivityId,SequenceNumber)
);






GO