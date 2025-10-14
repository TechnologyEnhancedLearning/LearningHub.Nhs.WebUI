CREATE TABLE [elfh].[externalSystemTBL](
	[externalSystemId] [int] IDENTITY(1,1) NOT NULL,
	[externalSystemName] [nvarchar](50) NOT NULL,
	[url] [nvarchar](256) NULL,
	[tsAndCs] [nvarchar](max) NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendUserDate] [datetimeoffset](7) NOT NULL,
	[securityGuid] [uniqueidentifier] NULL,
	[restrictToSSO] [bit] NOT NULL,
	[defaultUserGroupId] [int] NULL,
	[defaultStaffGroupId] [int] NULL,
	[defaultJobRoleId] [int] NULL,
	[defaultGradingId] [int] NULL,
	[defaultSpecialtyId] [int] NULL,
	[defaultLocationId] [int] NULL,
	[reportUserId] [int] NULL,
	[code] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_externalSystem] PRIMARY KEY CLUSTERED 
(
	[externalSystemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [elfh].[externalSystemTBL] ADD  CONSTRAINT [DF_externalSystemTBL_RestrictToSSO]  DEFAULT ((0)) FOR [restrictToSSO]
GO

ALTER TABLE [elfh].[externalSystemTBL] ADD  DEFAULT ('') FOR [code]
GO

