CREATE TABLE [elfh].[employmentReferenceTypeTBL](
	[EmploymentReferenceTypeId] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[RefAccess] [int] NOT NULL,
 CONSTRAINT [PK_EmploymentReferenceType] PRIMARY KEY CLUSTERED 
(
	[EmploymentReferenceTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[employmentReferenceTypeTBL] ADD  CONSTRAINT [DF_employmentReferenceTypeTBL_RefAccess]  DEFAULT ((0)) FOR [RefAccess]
GO
