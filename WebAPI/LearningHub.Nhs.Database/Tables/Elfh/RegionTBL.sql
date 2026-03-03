CREATE TABLE [elfh].[regionTBL](
	[regionId] [int] NOT NULL,
	[regionName] [nvarchar](100) NOT NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_regionTbl] PRIMARY KEY CLUSTERED 
(
	[regionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[regionTBL] ADD  CONSTRAINT [DF_regionTbl_deleted]  DEFAULT ((0)) FOR [deleted]
GO