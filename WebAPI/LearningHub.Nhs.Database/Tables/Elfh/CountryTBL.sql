CREATE TABLE [elfh].[countryTBL](
	[countryId] [int] IDENTITY(1,1) NOT NULL,
	[countryName] [nvarchar](50) NULL,
	[alpha2] [nvarchar](2) NULL,
	[alpha3] [nvarchar](3) NULL,
	[numeric] [nvarchar](3) NULL,
	[EUVatRate] [float] NOT NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_countryTBL] PRIMARY KEY CLUSTERED 
(
	[countryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[countryTBL] ADD  DEFAULT ((0)) FOR [EUVatRate]
GO

ALTER TABLE [elfh].[countryTBL] ADD  CONSTRAINT [DF_countryTBL_displayOrder]  DEFAULT ((0)) FOR [displayOrder]
GO

