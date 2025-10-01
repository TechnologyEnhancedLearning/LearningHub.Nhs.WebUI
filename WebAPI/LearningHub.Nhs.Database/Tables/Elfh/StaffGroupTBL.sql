CREATE TABLE [elfh].[staffGroupTBL](
	[staffGroupId] [int] IDENTITY(1,1) NOT NULL,
	[staffGroupName] [nvarchar](50) NOT NULL,
	[displayOrder] [int] NOT NULL,
	[internalUsersOnly] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserID] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_staffGroupTBL] PRIMARY KEY CLUSTERED 
(
	[staffGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[staffGroupTBL] ADD  CONSTRAINT [DF_staffGroupTBL_internalUsersOnly]  DEFAULT ((0)) FOR [internalUsersOnly]
GO