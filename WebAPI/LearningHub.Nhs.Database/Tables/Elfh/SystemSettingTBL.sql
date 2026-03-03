CREATE TABLE [elfh].[systemSettingTBL](
	[systemSettingId] [int] NOT NULL,
	[systemSettingName] [nvarchar](50) NOT NULL,
	[intValue] [int] NULL,
	[textValue] [nvarchar](255) NULL,
	[booleanValue] [bit] NULL,
	[dateValue] [datetimeoffset](7) NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_systemSettingsTBL] PRIMARY KEY CLUSTERED 
(
	[systemSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[systemSettingTBL] ADD  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[systemSettingTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO


