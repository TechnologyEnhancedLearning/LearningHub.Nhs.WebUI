CREATE TABLE [elfh].[userHistoryAttributeTBL](
	[userHistoryAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[userHistoryId] [int] NOT NULL,
	[attributeId] [int] NOT NULL,
	[intValue] [int] NULL,
	[textValue] [nvarchar](1000) NULL,
	[booleanValue] [bit] NULL,
	[dateValue] [datetimeoffset](7) NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userHistoryAttributeTBL] PRIMARY KEY CLUSTERED 
(
	[userHistoryAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userHistoryAttributeTBL] ADD  CONSTRAINT [DF_userHistoryAttributeTBL_deleted]  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[userHistoryAttributeTBL] ADD  CONSTRAINT [DF_userHistoryAttributeTBL_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[userHistoryAttributeTBL]  WITH CHECK ADD  CONSTRAINT [FK_userHistoryAttributeTBL_attributeId] FOREIGN KEY([attributeId])
REFERENCES [elfh].[attributeTBL] ([attributeId])
GO

ALTER TABLE [elfh].[userHistoryAttributeTBL] CHECK CONSTRAINT [FK_userHistoryAttributeTBL_attributeId]
GO

ALTER TABLE [elfh].[userHistoryAttributeTBL]  WITH CHECK ADD  CONSTRAINT [FK_userHistoryAttributeTBL_userHistoryId] FOREIGN KEY([userHistoryId])
REFERENCES [elfh].[userHistoryTBL] ([userHistoryId])
GO

ALTER TABLE [elfh].[userHistoryAttributeTBL] CHECK CONSTRAINT [FK_userHistoryAttributeTBL_userHistoryId]
GO


