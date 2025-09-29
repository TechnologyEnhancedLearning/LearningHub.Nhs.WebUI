CREATE TABLE [hub].[User](
	[Id] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[countryId] [int] NULL,
	[registrationCode] [nvarchar](50) NULL,
	[activeFromDate] [datetimeoffset](7) NULL,
	[activeToDate] [datetimeoffset](7) NULL,
	[passwordHash] [nvarchar](255) NULL,
	[mustChangeNextLogin] [bit] NULL,
	[passwordLifeCounter] [int] NULL,
	[securityLifeCounter] [int] NULL,
	[RemoteLoginKey] [nvarchar](50) NULL,
	[RemoteLoginGuid] [uniqueidentifier] NULL,
	[RemoteLoginStart] [datetimeoffset](7) NULL,
	[RestrictToSSO] [bit] NULL,
	[loginTimes] [int] NULL,
	[loginWizardInProgress] [bit] NULL,
	[lastLoginWizardCompleted] [datetimeoffset](7) NULL,
	[primaryUserEmploymentId] [int] NULL,
	[regionId] [int] NULL,
	[preferredTenantId] [int] NULL,
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


ALTER TABLE [hub].[User] ADD  CONSTRAINT [DF_userTBL_passwordLifeCounter]  DEFAULT ((0)) FOR [passwordLifeCounter]
GO

ALTER TABLE [hub].[User] ADD  CONSTRAINT [DF_userTBL_securityLifeCounter]  DEFAULT ((0)) FOR [securityLifeCounter]
GO

ALTER TABLE [hub].[User] ADD  CONSTRAINT [DF_userTBL_RestrictToSSO]  DEFAULT ((0)) FOR [RestrictToSSO]
GO

ALTER TABLE [hub].[User]  WITH CHECK ADD  CONSTRAINT [FK_userTBL_countryTBL] FOREIGN KEY([countryId])
REFERENCES [elfh].[countryTBL] ([countryId])
GO

ALTER TABLE [hub].[User] CHECK CONSTRAINT [FK_userTBL_countryTBL]
GO

ALTER TABLE [hub].[User]  WITH CHECK ADD  CONSTRAINT [FK_userTBL_regionTBL] FOREIGN KEY([regionId])
REFERENCES [elfh].[regionTBL] ([regionId])
GO

ALTER TABLE [hub].[User] CHECK CONSTRAINT [FK_userTBL_regionTBL]
GO

ALTER TABLE [hub].[User]  WITH CHECK ADD  CONSTRAINT [FK_userTBL_userEmploymentTBL] FOREIGN KEY([primaryUserEmploymentId])
REFERENCES [elfh].[userEmploymentTBL] ([userEmploymentId])
GO

ALTER TABLE [hub].[User] CHECK CONSTRAINT [FK_userTBL_userEmploymentTBL]
GO

CREATE INDEX idx_User_Deleted ON hub.[User] (Deleted) include (UserName) WITH (FILLFACTOR = 95);
GO

CREATE INDEX idx_User_Username ON [hub].[User] (UserName) WITH (FILLFACTOR = 95);
GO