CREATE TABLE [elfh].[schoolTBL](
	[schoolId] [int] IDENTITY(1,1) NOT NULL,
	[deaneryId] [int] NOT NULL,
	[specialtyId] [int] NOT NULL,
	[schoolName] [nvarchar](50) NOT NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_schoolTBL] PRIMARY KEY CLUSTERED 
(
	[schoolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[schoolTBL]  WITH CHECK ADD  CONSTRAINT [FK_schoolTBL_deaneryTBL] FOREIGN KEY([deaneryId])
REFERENCES [elfh].[deaneryTBL] ([deaneryId])
GO

ALTER TABLE [elfh].[schoolTBL] CHECK CONSTRAINT [FK_schoolTBL_deaneryTBL]
GO

ALTER TABLE [elfh].[schoolTBL]  WITH CHECK ADD  CONSTRAINT [FK_schoolTBL_specialtyTBL] FOREIGN KEY([specialtyId])
REFERENCES [elfh].[specialtyTBL] ([specialtyId])
GO

ALTER TABLE [elfh].[schoolTBL] CHECK CONSTRAINT [FK_schoolTBL_specialtyTBL]
GO