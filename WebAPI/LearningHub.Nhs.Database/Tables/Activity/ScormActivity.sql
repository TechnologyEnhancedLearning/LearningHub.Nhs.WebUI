CREATE TABLE [activity].[ScormActivity]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [ResourceActivityId] INT NOT NULL, 
    [CmiCoreLesson_location] NVARCHAR(255) NULL, 
    [CmiCoreLesson_status] INT NULL, 
    [CmiCoreScoreRaw] DECIMAL(16, 2) NULL, 
    [CmiCoreScoreMin] DECIMAL(16, 2) NULL, 
    [CmiCoreScoreMax] DECIMAL(16, 2) NULL, 
    [CmiCoreExit] NVARCHAR(255) NULL, 
    [CmiCoreSession_time] NVARCHAR(255) NULL, 
    [CmiSuspend_data] NVARCHAR(MAX) NULL, 
    [DurationSeconds] INT NOT NULL,
	[Deleted] bit NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Activity_ScormActivity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

CREATE INDEX IX_ScormActivity_ResourceActivity ON [activity].[ScormActivity](ResourceActivityId)
WITH (FILLFACTOR = 95);
GO