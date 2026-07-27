
CREATE TABLE [elfh].[userAdminLocationTBL](
	[userId] [int] NOT NULL,
	[adminLocationId] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
	[createdUserId] [int] NOT NULL,
	[createdDate] [datetimeoffset](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[adminLocationId] ASC,
	[userId] ASC,
	[deleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userAdminLocationTBL] ADD  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[userAdminLocationTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[userAdminLocationTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [createdDate]
GO

ALTER TABLE [elfh].[userAdminLocationTBL]  WITH CHECK ADD  CONSTRAINT [FK_userAdminLocationTBL_locationTBL] FOREIGN KEY([adminLocationId])
REFERENCES [elfh].[locationTBL] ([locationId])
GO

ALTER TABLE [elfh].[userAdminLocationTBL] CHECK CONSTRAINT [FK_userAdminLocationTBL_locationTBL]
GO

ALTER TABLE [elfh].[userAdminLocationTBL]  WITH CHECK ADD  CONSTRAINT [FK_userAdminLocationTBL_userTBL] FOREIGN KEY([userId])
REFERENCES [hub].[user] ([Id])
GO

ALTER TABLE [elfh].[userAdminLocationTBL] CHECK CONSTRAINT [FK_userAdminLocationTBL_userTBL]
GO


