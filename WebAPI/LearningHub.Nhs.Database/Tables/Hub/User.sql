CREATE TABLE [hub].[User](
	[Id] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[VersionStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[VersionEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
	PERIOD FOR SYSTEM_TIME ([VersionStartTime], [VersionEndTime])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hub].[UserHistory] )
)
GO

ALTER TABLE [hub].[User] ADD  DEFAULT (getutcdate()) FOR [VersionStartTime]
GO

ALTER TABLE [hub].[User] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [VersionEndTime]
GO

CREATE INDEX idx_User_Deleted ON hub.[User] (Deleted) include (UserName) WITH (FILLFACTOR = 95);
GO

CREATE INDEX idx_User_Username ON [hub].[User] (UserName) WITH (FILLFACTOR = 95);
GO