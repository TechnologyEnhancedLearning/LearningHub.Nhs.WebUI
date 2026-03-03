CREATE TABLE [elfh].[userAttributeTBL](
	[userAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[attributeId] [int] NOT NULL,
	[intValue] [int] NULL,
	[textValue] [nvarchar](255) NULL,
	[booleanValue] [bit] NULL,
	[dateValue] [datetimeoffset](7) NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userAttributeTBL] PRIMARY KEY CLUSTERED 
(
	[userAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userAttributeTBL] ADD  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[userAttributeTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[userAttributeTBL]  WITH CHECK ADD  CONSTRAINT [FK_userAttributeTBL_attributeId] FOREIGN KEY([attributeId])
REFERENCES [elfh].[attributeTBL] ([attributeId])
GO

ALTER TABLE [elfh].[userAttributeTBL] CHECK CONSTRAINT [FK_userAttributeTBL_attributeId]
GO

ALTER TABLE [elfh].[userAttributeTBL]  WITH CHECK ADD  CONSTRAINT [FK_userAttributeTBL_userId] FOREIGN KEY([userId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [elfh].[userAttributeTBL] CHECK CONSTRAINT [FK_userAttributeTBL_userId]
GO