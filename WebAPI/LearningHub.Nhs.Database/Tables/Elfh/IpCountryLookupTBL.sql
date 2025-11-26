CREATE TABLE [elfh].[ipCountryLookupTBL](
	[fromIP] [varchar](20) NOT NULL,
	[toIP] [varchar](20) NOT NULL,
	[country] [varchar](10) NOT NULL,
	[fromInt] [bigint] NULL,
	[toInt] [bigint] NULL
) ON [PRIMARY]
GO