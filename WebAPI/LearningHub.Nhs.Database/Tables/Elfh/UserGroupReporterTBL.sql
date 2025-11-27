CREATE TABLE [elfh].[userGroupReporterTBL](
	[userGroupReporterId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[userId] [int] NOT NULL,
	[userGroupId] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userGroupReporterTBL] PRIMARY KEY CLUSTERED 
(
	[userGroupReporterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userGroupReporterTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[userGroupReporterTBL]  WITH CHECK ADD  CONSTRAINT [FK_userGroupReporterTBL_userGroupTBL] FOREIGN KEY([userGroupId])
REFERENCES [hub].[userGroup] ([Id])
GO

ALTER TABLE [elfh].[userGroupReporterTBL] CHECK CONSTRAINT [FK_userGroupReporterTBL_userGroupTBL]
GO

ALTER TABLE [elfh].[userGroupReporterTBL]  WITH CHECK ADD  CONSTRAINT [FK_userGroupReporterTBL_userTBL] FOREIGN KEY([userId])
REFERENCES [hub].[user] ([Id])
GO

ALTER TABLE [elfh].[userGroupReporterTBL] CHECK CONSTRAINT [FK_userGroupReporterTBL_userTBL]
GO