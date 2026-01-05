INSERT [hub].[NotificationType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (8, N'AccessRequest', N'Access request', 0, 4, CAST(N'2021-07-14T13:33:51.0233333+00:00' AS DateTimeOffset), 4, CAST(N'2021-07-14T13:33:51.0233333+00:00' AS DateTimeOffset))
GO



INSERT [messaging].[EmailTemplate] ([Id], [LayoutId], [Title], [Subject], [Body], [AvailableTags], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (2007, 1, N'ReportProcessed', N'[ReportName] report for [ReportContent] is ready', N'<p>Dear [AdminFirstName],</p>
<p>Your report [ReportName] report for [ReportContent] is ready. You can view and download the report in the <a href=''[ReportSection]'' target=''_blank''>reports section</a></p>', N'[UserFirstName][ReportSection][ReportName][ReportContent]', 0, 4, CAST(N'2025-12-22T00:00:00.0000000+00:00' AS DateTimeOffset), 4, CAST(N'2025-12-22T00:00:00.0000000+00:00' AS DateTimeOffset))
GO