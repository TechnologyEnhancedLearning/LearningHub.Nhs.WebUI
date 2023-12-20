IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 1)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (1, N'Search', N'Basic Search', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 2)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (2, N'SearchSort', N'Sort the current search ', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 3)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (3, N'SearchFilter', N'Search by Filter', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 4)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (4, N'SearchLoadMore', N'Search Load More Records', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 5)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (5, N'SearchLaunchResource', N'Search Launch Resource', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 6)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (6, N'SearchSubmitFeedback', N'Search Submit Feedback', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 7)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (7, N'SearchLaunchCatalogue', N'Search Launch Catalogue', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [analytics].[EventType] WHERE Id = 8)
BEGIN
	INSERT [analytics].[EventType] ([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) VALUES (8, N'LaunchCatalogueResource', N'Launch Catalogue Resource', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END