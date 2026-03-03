CREATE TABLE [elfh].[userEmploymentResponsibilityTBL](
	[userEmploymentResponsibilityId] [int] IDENTITY(1,1) NOT NULL,
	[userEmploymentId] [int] NOT NULL,
	[additionalResponsibilityId] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[userEmploymentResponsibilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userEmploymentResponsibilityTBL] ADD  DEFAULT ((0)) FOR [deleted]
GO