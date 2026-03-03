
CREATE TABLE [elfh].[locationTBL](
	[locationId] [int] NOT NULL,
	[locationCode] [nvarchar](50) NOT NULL,
	[locationName] [nvarchar](200) NOT NULL,
	[locationSubName] [nvarchar](200) NULL,
	[locationTypeId] [int] NOT NULL,
	[address1] [nvarchar](100) NULL,
	[address2] [nvarchar](100) NULL,
	[address3] [nvarchar](100) NULL,
	[address4] [nvarchar](100) NULL,
	[town] [nvarchar](100) NULL,
	[county] [nvarchar](100) NULL,
	[postCode] [nvarchar](8) NULL,
	[telephone] [nvarchar](50) NULL,
	[acute] [bit] NOT NULL,
	[ambulance] [bit] NOT NULL,
	[mental] [bit] NOT NULL,
	[care] [bit] NOT NULL,
	[mainHosp] [bit] NOT NULL,
	[nhsCode] [nvarchar](50) NULL,
	[parentId] [int] NOT NULL,
	[dataSource] [nvarchar](50) NOT NULL,
	[active] [bit] NOT NULL,
	[importExclusion] [bit] NOT NULL,
	[depth] [int] NULL,
	[lineage] [nvarchar](max) NULL,
	[created] [datetimeoffset](7) NOT NULL,
	[updated] [datetimeoffset](7) NOT NULL,
	[archivedDate] [datetimeoffset](7) NULL,
	[countryId] [int] NULL,
	[iguId] [int] NOT NULL,
	[letbId] [int] NOT NULL,
	[ccgId] [int] NOT NULL,
	[healthServiceId] [int] NOT NULL,
	[healthBoardId] [int] NOT NULL,
	[primaryTrustId] [int] NOT NULL,
	[secondaryTrustId] [int] NOT NULL,
	[islandId] [int] NOT NULL,
	[otherNHSOrganisationId] [int] NOT NULL,
 CONSTRAINT [PK_locationTBL] PRIMARY KEY CLUSTERED 
(
	[locationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [locationTypeId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_Acute]  DEFAULT ((0)) FOR [acute]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_Ambulance]  DEFAULT ((0)) FOR [ambulance]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_Mental]  DEFAULT ((0)) FOR [mental]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_Care]  DEFAULT ((0)) FOR [care]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_MainHosp]  DEFAULT ((0)) FOR [mainHosp]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_Active]  DEFAULT ((1)) FOR [active]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_ImportExclusion]  DEFAULT ((0)) FOR [importExclusion]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_created]  DEFAULT (sysdatetimeoffset()) FOR [created]
GO

ALTER TABLE [elfh].[locationTBL] ADD  CONSTRAINT [DF_locationTBL_updated]  DEFAULT (sysdatetimeoffset()) FOR [updated]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [iguId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [letbId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [ccgId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [healthServiceId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [healthBoardId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [primaryTrustId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [secondaryTrustId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [islandId]
GO

ALTER TABLE [elfh].[locationTBL] ADD  DEFAULT ((0)) FOR [otherNHSOrganisationId]
GO

ALTER TABLE [elfh].[locationTBL]  WITH CHECK ADD  CONSTRAINT [FK_locationTBL_countryTBL] FOREIGN KEY([countryId])
REFERENCES [elfh].[countryTBL] ([countryId])
GO

ALTER TABLE [elfh].[locationTBL] CHECK CONSTRAINT [FK_locationTBL_countryTBL]
GO

ALTER TABLE [elfh].[locationTBL]  WITH CHECK ADD  CONSTRAINT [FK_locationTBL_locationTypeTBL] FOREIGN KEY([locationTypeId])
REFERENCES [elfh].[locationTypeTBL] ([locationTypeID])
GO

ALTER TABLE [elfh].[locationTBL] CHECK CONSTRAINT [FK_locationTBL_locationTypeTBL]
GO