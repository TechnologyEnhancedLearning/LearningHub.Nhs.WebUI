CREATE TABLE [elfh].[tenantUrlTBL](
	[tenantUrlId] [int] IDENTITY(1,1) NOT NULL,
	[tenantId] [int] NOT NULL,
	[urlHostName] [nvarchar](128) NOT NULL,
	[useHostForAuth] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_tenantUrlTBL] PRIMARY KEY CLUSTERED 
(
	[tenantUrlId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[tenantUrlTBL] ADD  DEFAULT ((0)) FOR [useHostForAuth]
GO

ALTER TABLE [elfh].[tenantUrlTBL]  WITH CHECK ADD  CONSTRAINT [FK_tenantUrlTBL_tenantTBL] FOREIGN KEY([tenantId])
REFERENCES [elfh].[tenantTBL] ([tenantId])
GO

ALTER TABLE [elfh].[tenantUrlTBL] CHECK CONSTRAINT [FK_tenantUrlTBL_tenantTBL]
GO