CREATE TABLE [adf].[Staging_Elfh_UserUserGroup](
	[JobRunId] [int] NOT NULL,
	[userUserGroupId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[userGroupId] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendDate] [datetimeoffset](7) NULL
)
GO