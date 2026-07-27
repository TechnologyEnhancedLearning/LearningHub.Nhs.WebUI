
CREATE TABLE [elfh].[userEmploymentTBL](
	[userEmploymentId] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[jobRoleId] [int] NULL,
	[specialtyId] [int] NULL,
	[gradeId] [int] NULL,
	[schoolId] [int] NULL,
	[locationId] [int] NOT NULL,
	[medicalCouncilId] [int] NULL,
	[medicalCouncilNo] [nvarchar](50) NULL,
	[startDate] [datetimeoffset](7) NULL,
	[endDate] [datetimeoffset](7) NULL,
	[deleted] [bit] NOT NULL,
	[archived] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userEmploymentTBL] PRIMARY KEY CLUSTERED 
(
	[userEmploymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userEmploymentTBL] ADD  CONSTRAINT [DF_userEmploymentTBL_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[userEmploymentTBL] ADD  CONSTRAINT [DF_userEmploymentTBL_archived]  DEFAULT ((0)) FOR [archived]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_gradeTBL] FOREIGN KEY([gradeId])
REFERENCES [elfh].[gradeTBL] ([gradeId])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_gradeTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_jobRoleTBL] FOREIGN KEY([jobRoleId])
REFERENCES [elfh].[jobRoleTBL] ([jobRoleId])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_jobRoleTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_locationTBL] FOREIGN KEY([locationId])
REFERENCES [elfh].[locationTBL] ([locationId])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_locationTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_medicalCouncilTBL] FOREIGN KEY([medicalCouncilId])
REFERENCES [elfh].[medicalCouncilTBL] ([medicalCouncilId])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_medicalCouncilTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_schoolTBL] FOREIGN KEY([schoolId])
REFERENCES [elfh].[schoolTBL] ([schoolId])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_schoolTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_specialtyTBL] FOREIGN KEY([specialtyId])
REFERENCES [elfh].[specialtyTBL] ([specialtyId])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_specialtyTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_userTBL] FOREIGN KEY([userId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_userTBL]
GO

ALTER TABLE [elfh].[userEmploymentTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentTBL_userTBL_AmendUser] FOREIGN KEY([amendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [elfh].[userEmploymentTBL] CHECK CONSTRAINT [FK_userEmploymentTBL_userTBL_AmendUser]
GO