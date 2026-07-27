CREATE TABLE [elfh].[locationTypeTBL](
	[locationTypeID] [int] IDENTITY(1,1) NOT NULL,
	[locationType] [nvarchar](50) NOT NULL,
	[countryId] [int] NULL,
	[healthService] [bit] NOT NULL,
	[healthBoard] [bit] NOT NULL,
	[primaryTrust] [bit] NOT NULL,
	[secondaryTrust] [bit] NOT NULL,
 CONSTRAINT [PK_locationType] PRIMARY KEY CLUSTERED 
(
	[locationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[locationTypeTBL] ADD  DEFAULT ((0)) FOR [healthService]
GO

ALTER TABLE [elfh].[locationTypeTBL] ADD  DEFAULT ((0)) FOR [healthBoard]
GO

ALTER TABLE [elfh].[locationTypeTBL] ADD  DEFAULT ((0)) FOR [primaryTrust]
GO

ALTER TABLE [elfh].[locationTypeTBL] ADD  DEFAULT ((0)) FOR [secondaryTrust]
GO

ALTER TABLE [elfh].[locationTypeTBL]  WITH CHECK ADD  CONSTRAINT [FK_locationTypeTBL_countryTBL] FOREIGN KEY([countryId])
REFERENCES [elfh].[countryTBL] ([countryId])
GO

ALTER TABLE [elfh].[locationTypeTBL] CHECK CONSTRAINT [FK_locationTypeTBL_countryTBL]
GO

