CREATE TABLE [elfh].[loginWizardStageActivityTBL](
	[loginWizardStageActivityId] [int] IDENTITY(1,1) NOT NULL,
	[loginWizardStageId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[activityDatetime] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_loginWizardStageActivityTBL] PRIMARY KEY CLUSTERED 
(
	[loginWizardStageActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[loginWizardStageActivityTBL]  WITH CHECK ADD  CONSTRAINT [FK_loginWizardStageActivityTBL_loginWizardStageTBL] FOREIGN KEY([loginWizardStageId])
REFERENCES [elfh].[loginWizardStageTBL] ([loginWizardStageId])
GO

ALTER TABLE [elfh].[loginWizardStageActivityTBL] CHECK CONSTRAINT [FK_loginWizardStageActivityTBL_loginWizardStageTBL]
GO

ALTER TABLE [elfh].[loginWizardStageActivityTBL]  WITH CHECK ADD  CONSTRAINT [FK_loginWizardStageActivityTBL_userTBL] FOREIGN KEY([userId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [elfh].[loginWizardStageActivityTBL] CHECK CONSTRAINT [FK_loginWizardStageActivityTBL_userTBL]
GO