CREATE TABLE [elfh].[specialtyTBL](
	[specialtyId] [int] IDENTITY(1,1) NOT NULL,
	[specialtyName] [nvarchar](50) NOT NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_specialtyTBL] PRIMARY KEY CLUSTERED 
(
	[specialtyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[specialtyTBL] ADD  CONSTRAINT [DF_specialty_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[specialtyTBL] ADD  CONSTRAINT [DF_specialtyTBL_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO