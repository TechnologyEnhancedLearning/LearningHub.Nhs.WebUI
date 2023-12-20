CREATE TABLE [activity].[ScormActivityInteractionCorrectResponse] (
    [Id]                         INT                IDENTITY (1, 1) NOT NULL,
    [ScormActivityInteractionId] INT                NOT NULL,
    [Index]                      INT                CONSTRAINT [DF_ScormActivityInteractionCorrectResponse_Index] DEFAULT ((0)) NOT NULL,
    [Pattern]                    NVARCHAR (255)     NULL,
    [Deleted]                    BIT                NOT NULL,
    [CreateUserID]               INT                NOT NULL,
    [CreateDate]                 DATETIMEOFFSET (7) NOT NULL,
    [AmendUserID]                INT                NOT NULL,
    [AmendDate]                  DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Activity_ScormActivityInteractionCorrectResponse] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ScormActivityInteractionCorrectResponse_ScormActivityInteraction] FOREIGN KEY ([ScormActivityInteractionId]) REFERENCES [activity].[ScormActivityInteraction] ([Id])
);



