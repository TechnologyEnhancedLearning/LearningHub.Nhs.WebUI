CREATE TABLE [elfh].[userEmploymentReferenceTBL](
	[userEmploymentReferenceId] [int] IDENTITY(1,1) NOT NULL,
	[employmentReferenceTypeId] [int] NOT NULL,
	[userEmploymentId] [int] NOT NULL,
	[referenceValue] [nvarchar](100) NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userEmploymentReferenceTBL] PRIMARY KEY CLUSTERED 
(
	[userEmploymentReferenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userEmploymentReferenceTBL] ADD  CONSTRAINT [DF_userEmploymentReferenceTBL_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[userEmploymentReferenceTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentReferenceTBL_employmentReferenceTypeTBL] FOREIGN KEY([employmentReferenceTypeId])
REFERENCES [elfh].[employmentReferenceTypeTBL] ([EmploymentReferenceTypeId])
GO

ALTER TABLE [elfh].[userEmploymentReferenceTBL] CHECK CONSTRAINT [FK_userEmploymentReferenceTBL_employmentReferenceTypeTBL]
GO

ALTER TABLE [elfh].[userEmploymentReferenceTBL]  WITH CHECK ADD  CONSTRAINT [FK_userEmploymentReferenceTBL_userEmploymentTBL] FOREIGN KEY([userEmploymentId])
REFERENCES [elfh].[userEmploymentTBL] ([userEmploymentId])
GO

ALTER TABLE [elfh].[userEmploymentReferenceTBL] CHECK CONSTRAINT [FK_userEmploymentReferenceTBL_userEmploymentTBL]
GO