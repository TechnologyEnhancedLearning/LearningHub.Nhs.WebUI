CREATE TABLE [elfh].[gmclrmpTBL](
	[GMC_Ref_No] [nvarchar](50) NOT NULL,
	[Surname] [nvarchar](255) NULL,
	[Given_Name] [nvarchar](255) NULL,
	[Year_Of_Qualification] [float] NULL,
	[GP_Register_Date] [nvarchar](255) NULL,
	[Registration_Status] [nvarchar](255) NULL,
	[Other_Names] [nvarchar](255) NULL,
	[dateProcessed] [datetime] NULL,
	[action] [nchar](1) NULL,
 CONSTRAINT [PK_gmclrmpTBL] PRIMARY KEY CLUSTERED 
(
	[GMC_Ref_No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO