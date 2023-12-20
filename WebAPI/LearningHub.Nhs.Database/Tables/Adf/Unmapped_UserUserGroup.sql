CREATE TABLE [adf].[Unmapped_UserUserGroup](
	[JobRunId] [int] NOT NULL,
	[userUserGroupId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[userGroupId] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendDate] [datetimeoffset](7) NULL,
	[UserMissingInd] [bit] NOT NULL,
	[UserGroupMissingInd] [bit] NOT NULL
)
GO
