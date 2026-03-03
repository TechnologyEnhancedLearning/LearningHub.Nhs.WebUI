CREATE TABLE [elfh].[emailTemplateTBL](
	[emailTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[emailTemplateTypeId] [int] NOT NULL,
	[programmeComponentId] [int] NOT NULL,
	[title] [nvarchar](256) NULL,
	[subject] [nvarchar](256) NOT NULL,
	[body] [ntext] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
	[tenantId] [int] NULL,
 CONSTRAINT [PK_EmailTemplateTBL] PRIMARY KEY CLUSTERED 
(
	[emailTemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [elfh].[emailTemplateTBL] ADD  CONSTRAINT [DF_emailTemplateTBL_Deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[emailTemplateTBL] ADD  CONSTRAINT [DF_EmailTemplate_AmendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[emailTemplateTBL]  WITH CHECK ADD  CONSTRAINT [FK_emailTemplateTBL_emailTemplateTypeTBL] FOREIGN KEY([emailTemplateTypeId])
REFERENCES [elfh].[emailTemplateTypeTBL] ([emailTemplateTypeId])
GO

ALTER TABLE [elfh].[emailTemplateTBL] CHECK CONSTRAINT [FK_emailTemplateTBL_emailTemplateTypeTBL]
GO