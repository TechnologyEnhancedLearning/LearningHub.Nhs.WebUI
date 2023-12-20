CREATE TABLE [adf].[Staging_Elfh_UserGroup](
	[JobRunId] [int] NOT NULL,
	[userGroupId] [int] NOT NULL,
	[userGroupName] [nvarchar](100) NOT NULL,
	[userGroupDescription] [nvarchar](255) NULL,
	[deleted] [bit] NOT NULL,
	[amendDate] [datetimeoffset](7) NULL
)
GO