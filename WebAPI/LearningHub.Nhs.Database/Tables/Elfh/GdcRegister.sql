CREATE TABLE [elfh].[gdcRegisterTBL](
	[reg_number] [nvarchar](50) NOT NULL,
	[Dentist] [bit] NOT NULL,
	[Title] [nvarchar](50) NULL,
	[Surname] [nvarchar](255) NULL,
	[Forenames] [nvarchar](255) NULL,
	[honorifics] [nvarchar](50) NULL,
	[house_name] [nvarchar](255) NULL,
	[address_line1] [nvarchar](255) NULL,
	[address_line2] [nvarchar](255) NULL,
	[address_line3] [nvarchar](255) NULL,
	[address_line4] [nvarchar](255) NULL,
	[Town] [nvarchar](50) NULL,
	[County] [nvarchar](50) NULL,
	[PostCode] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[regdate] [nvarchar](50) NULL,
	[qualifications] [nvarchar](1000) NULL,
	[dcp_titles] [nvarchar](100) NULL,
	[specialties] [nvarchar](100) NULL,
	[condition] [nvarchar](50) NULL,
	[suspension] [nvarchar](50) NULL,
	[dateProcessed] [datetimeoffset](7) NOT NULL,
	[action] [nvarchar](1) NULL,
 CONSTRAINT [PK_gdcRegisterTBL] PRIMARY KEY CLUSTERED 
(
	[reg_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[gdcRegisterTBL] ADD  DEFAULT ((0)) FOR [Dentist]
GO

ALTER TABLE [elfh].[gdcRegisterTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [dateProcessed]
GO