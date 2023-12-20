IF NOT EXISTS (SELECT 1 FROM [hub].[BookmarkType] WHERE Id IN (1, 2, 3))
BEGIN
	SET IDENTITY_INSERT [hub].[BookmarkType] ON

	INSERT [hub].[BookmarkType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (1, N'Folder', N'', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

	INSERT [hub].[BookmarkType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (2, N'Node', N'', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

	INSERT [hub].[BookmarkType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (3, N'Resource', N'', 0, 4,SYSDATETIMEOFFSET(), 4,SYSDATETIMEOFFSET())

	SET IDENTITY_INSERT [hub].[BookmarkType] OFF
END