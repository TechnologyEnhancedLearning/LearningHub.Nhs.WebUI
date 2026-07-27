CREATE TABLE [elfh].[medicalCouncilTBL](
	[medicalCouncilId] [int] NOT NULL,
	[medicalCouncilName] [nvarchar](50) NOT NULL,
	[medicalCouncilCode] [nvarchar](50) NOT NULL,
	[uploadPrefix] [nvarchar](3) NOT NULL,
	[includeOnCerts] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_medicalCouncilTBL] PRIMARY KEY CLUSTERED 
(
	[medicalCouncilId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[medicalCouncilTBL] ADD  CONSTRAINT [DF_medicalCouncilTBL_includeOnCerts]  DEFAULT ((0)) FOR [includeOnCerts]
GO

ALTER TABLE [elfh].[medicalCouncilTBL] ADD  CONSTRAINT [DF_medicalCouncilTBL_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[medicalCouncilTBL] ADD  CONSTRAINT [DF_medicalCouncilTBL_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO