CREATE TABLE [elfh].[emailTemplateTypeTBL](
	[emailTemplateTypeId] [int] NOT NULL,
	[emailTemplateTypeName] [nvarchar](50) NOT NULL,
	[availableTags] [nvarchar](255) NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_emailTemplateTypeTBL] PRIMARY KEY CLUSTERED 
(
	[emailTemplateTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[emailTemplateTypeTBL] ADD  CONSTRAINT [DF_emailTemplateTypeTBL_Deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[emailTemplateTypeTBL] ADD  CONSTRAINT [DF_emailTemplateTypeTBL_AmendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO