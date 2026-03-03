CREATE TABLE [elfh].[deaneryTBL](
	[deaneryId] [int] IDENTITY(1,1) NOT NULL,
	[deaneryName] [nvarchar](50) NOT NULL,
	[displayOrder] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_deaneryTBL] PRIMARY KEY CLUSTERED 
(
	[deaneryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
