CREATE TABLE [adf].[Staging_Elfh_User](
	[JobRunId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[userName] [nvarchar](50) NOT NULL,
	[deleted] [bit] NOT NULL,
	[createdDate] [datetimeoffset](7) NULL,
	[amendDate] [datetimeoffset](7) NULL
)
GO