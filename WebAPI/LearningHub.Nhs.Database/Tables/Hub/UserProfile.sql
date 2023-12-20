CREATE TABLE hub.[UserProfile](
	[Id] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[VersionStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[VersionEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([VersionStartTime], [VersionEndTime])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hub].[UserProfileHistory] )
)
GO

ALTER TABLE hub.[UserProfile] ADD  DEFAULT (getutcdate()) FOR [VersionStartTime]
GO

ALTER TABLE hub.[UserProfile] ADD  DEFAULT (CONVERT([datetime2],'9999-12-31 23:59:59.9999999')) FOR [VersionEndTime]
GO

CREATE NONCLUSTERED INDEX [idx_UserProfile_EmailAddress]
    ON [hub].[UserProfile]([EmailAddress] ASC) WITH (FILLFACTOR = 95);
GO