CREATE TABLE [elfh].[jobRoleTBL](
	[jobRoleId] [int] IDENTITY(1,1) NOT NULL,
	[staffGroupId] [int] NULL,
	[jobRoleName] [nvarchar](100) NOT NULL,
	[medicalCouncilId] [int] NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_jobRoleTBL] PRIMARY KEY CLUSTERED 
(
	[jobRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[jobRoleTBL] ADD  CONSTRAINT [DF_jobRoleTBL_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[jobRoleTBL] ADD  CONSTRAINT [DF_jobRoleTBL_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[jobRoleTBL]  WITH CHECK ADD  CONSTRAINT [FK_jobRoleTBL_medicalCouncilTBL] FOREIGN KEY([medicalCouncilId])
REFERENCES [elfh].[medicalCouncilTBL] ([medicalCouncilId])
GO

ALTER TABLE [elfh].[jobRoleTBL] CHECK CONSTRAINT [FK_jobRoleTBL_medicalCouncilTBL]
GO

ALTER TABLE [elfh].[jobRoleTBL]  WITH CHECK ADD  CONSTRAINT [FK_jobRoleTBL_staffGroupTBL] FOREIGN KEY([staffGroupId])
REFERENCES [elfh].[staffGroupTBL] ([staffGroupId])
GO

ALTER TABLE [elfh].[jobRoleTBL] CHECK CONSTRAINT [FK_jobRoleTBL_staffGroupTBL]
GO
