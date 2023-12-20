CREATE TABLE [adf].[Staging_Elfh_UserProfile](
	[JobRunId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[userName] [nvarchar](50) NOT NULL,
	[emailAddress] [nvarchar](100) NOT NULL,
	[firstName] [nvarchar](50) NOT NULL,
	[lastName] [nvarchar](50) NOT NULL,
	[active] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[createdDate] [datetimeoffset](7) NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL
) ON [PRIMARY]
GO