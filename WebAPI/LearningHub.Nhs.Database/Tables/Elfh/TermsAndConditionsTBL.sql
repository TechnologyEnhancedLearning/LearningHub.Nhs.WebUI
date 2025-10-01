CREATE TABLE [elfh].[termsAndConditionsTBL](
	[termsAndConditionsId] [int] IDENTITY(1,1) NOT NULL,
	[createdDate] [datetimeoffset](7) NOT NULL,
	[description] [nvarchar](512) NOT NULL,
	[details] [ntext] NOT NULL,
	[tenantId] [int] NOT NULL,
	[active] [bit] NOT NULL,
	[reportable] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_termsAndConditions] PRIMARY KEY CLUSTERED 
(
	[termsAndConditionsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [elfh].[termsAndConditionsTBL] ADD  DEFAULT ((1)) FOR [reportable]
GO

ALTER TABLE [elfh].[termsAndConditionsTBL]  WITH CHECK ADD  CONSTRAINT [FK_termsAndConditionsTBL_tenantTBL] FOREIGN KEY([tenantId])
REFERENCES [elfh].[tenantTBL] ([tenantId])
GO

ALTER TABLE [elfh].[termsAndConditionsTBL] CHECK CONSTRAINT [FK_termsAndConditionsTBL_tenantTBL]
GO