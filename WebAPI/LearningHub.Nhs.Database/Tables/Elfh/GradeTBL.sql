CREATE TABLE [elfh].[gradeTBL](
	[gradeId] [int] IDENTITY(1,1) NOT NULL,
	[gradeName] [nvarchar](50) NOT NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_gradeTBL] PRIMARY KEY CLUSTERED 
(
	[gradeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[gradeTBL] ADD  CONSTRAINT [DF_gradeTBL_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[gradeTBL] ADD  CONSTRAINT [DF_gradeTBL_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

