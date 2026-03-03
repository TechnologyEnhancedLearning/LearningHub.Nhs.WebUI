CREATE TABLE [elfh].[userTermsAndConditionsTBL](
	[userTermsAndConditionsId] [int] IDENTITY(1,1) NOT NULL,
	[termsAndConditionsId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[acceptanceDate] [datetimeoffset](7) NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userTermsAndConditions] PRIMARY KEY CLUSTERED 
(
	[userTermsAndConditionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userTermsAndConditionsTBL]  WITH CHECK ADD  CONSTRAINT [FK_userTermsAndConditionsTBL_portalUserTBL] FOREIGN KEY([userId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [elfh].[userTermsAndConditionsTBL] CHECK CONSTRAINT [FK_userTermsAndConditionsTBL_portalUserTBL]
GO

ALTER TABLE [elfh].[userTermsAndConditionsTBL]  WITH CHECK ADD  CONSTRAINT [FK_userTermsAndConditionsTBL_termsAndConditionsTBL] FOREIGN KEY([termsAndConditionsId])
REFERENCES [elfh].[termsAndConditionsTBL] ([termsAndConditionsId])
GO

ALTER TABLE [elfh].[userTermsAndConditionsTBL] CHECK CONSTRAINT [FK_userTermsAndConditionsTBL_termsAndConditionsTBL]
GO