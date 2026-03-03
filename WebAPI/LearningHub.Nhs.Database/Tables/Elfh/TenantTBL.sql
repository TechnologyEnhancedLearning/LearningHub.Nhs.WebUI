CREATE TABLE [elfh].[tenantTBL](
	[tenantId] [int] NOT NULL,
	[tenantCode] [nvarchar](20) NOT NULL,
	[tenantName] [nvarchar](64) NOT NULL,
	[tenantDescription] [nvarchar](1024) NOT NULL,
	[showFullCatalogInfoMessageInd] [bit] NOT NULL,
	[catalogUrl] [nvarchar](256) NOT NULL,
	[quickStartGuideUrl] [nvarchar](1024) NULL,
	[supportFormUrl] [nvarchar](1024) NULL,
	[liveChatStatus] [int] NOT NULL,
	[liveChatSnippet] [nvarchar](2048) NULL,
	[myElearningDefaultView] [int] NOT NULL,
	[preLoginCatalogueDefaultView] [int] NOT NULL,
	[postLoginCatalogueDefaultView] [int] NOT NULL,
	[authSignInUrlRelative] [nvarchar](1024) NULL,
	[authSignOutUrlRelative] [nvarchar](1024) NULL,
	[authSecret] [uniqueidentifier] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_tenantTBL] PRIMARY KEY CLUSTERED 
(
	[tenantId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[tenantTBL] ADD  DEFAULT ((0)) FOR [liveChatStatus]
GO

ALTER TABLE [elfh].[tenantTBL] ADD  DEFAULT ((0)) FOR [myElearningDefaultView]
GO

ALTER TABLE [elfh].[tenantTBL] ADD  DEFAULT ((0)) FOR [preLoginCatalogueDefaultView]
GO

ALTER TABLE [elfh].[tenantTBL] ADD  DEFAULT ((0)) FOR [postLoginCatalogueDefaultView]
GO

ALTER TABLE [elfh].[tenantTBL] ADD  DEFAULT (newid()) FOR [authSecret]
GO


