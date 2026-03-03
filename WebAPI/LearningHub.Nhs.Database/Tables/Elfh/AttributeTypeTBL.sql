CREATE TABLE [elfh].[attributeTypeTBL](
	[attributeTypeId] [int] NOT NULL,
	[attributeTypeName] [nvarchar](50) NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_attributeTypeTBL] PRIMARY KEY CLUSTERED 
(
	[attributeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[attributeTypeTBL] ADD  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[attributeTypeTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO
