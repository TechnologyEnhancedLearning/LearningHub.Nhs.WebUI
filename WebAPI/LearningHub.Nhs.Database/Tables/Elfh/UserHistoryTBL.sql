CREATE TABLE [elfh].[userHistoryTBL](
	[userHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[userHistoryTypeId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[createdDate] [datetimeoffset](7) NOT NULL,
	[tenantId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[userHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userHistoryTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [createdDate]
GO

ALTER TABLE [elfh].[userHistoryTBL] ADD  DEFAULT ((0)) FOR [tenantId]
GO

ALTER TABLE [elfh].[userHistoryTBL]  WITH CHECK ADD  CONSTRAINT [FK_userHistoryTBL_userHistoryTypeTBL] FOREIGN KEY([userHistoryTypeId])
REFERENCES [elfh].[userHistoryTypeTBL] ([UserHistoryTypeId])
GO

ALTER TABLE [elfh].[userHistoryTBL] CHECK CONSTRAINT [FK_userHistoryTBL_userHistoryTypeTBL]
GO