CREATE TABLE [elfh].[userReportingUserTBL](
	[userReportingUserId] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[reportingUserId] [int] NOT NULL,
	[reportable] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userReportingUser] PRIMARY KEY CLUSTERED 
(
	[userReportingUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userReportingUserTBL] ADD  DEFAULT ((1)) FOR [reportable]
GO

ALTER TABLE [elfh].[userReportingUserTBL]  WITH CHECK ADD  CONSTRAINT [FK_userReportingUser_reportingUser] FOREIGN KEY([reportingUserId])
REFERENCES [hub].[user] ([Id])
GO

ALTER TABLE [elfh].[userReportingUserTBL] CHECK CONSTRAINT [FK_userReportingUser_reportingUser]
GO

ALTER TABLE [elfh].[userReportingUserTBL]  WITH CHECK ADD  CONSTRAINT [FK_userReportingUser_user] FOREIGN KEY([userId])
REFERENCES [hub].[user] ([Id])
GO

ALTER TABLE [elfh].[userReportingUserTBL] CHECK CONSTRAINT [FK_userReportingUser_user]
GO