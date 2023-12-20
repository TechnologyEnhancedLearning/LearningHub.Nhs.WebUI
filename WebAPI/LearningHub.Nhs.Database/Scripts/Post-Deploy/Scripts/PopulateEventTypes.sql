
IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 1)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (1, 'Search', 'Basic Search', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 2)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (2, 'SearchSort', 'Sort the current search ', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 3)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (3, 'SearchFilter', 'Search by Filter', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 4)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (4, 'SearchLoadMore', 'Search Load More Records', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 5)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (5, 'SearchLaunchResource', 'Search Launch Resource', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 6)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (6, 'SearchSubmitFeedback', 'Search Submit Feedback', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 7)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (7, 'DashBoardElfh', 'Dashboard Elfh Card', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 8)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (8, 'DashBoardBmj', 'Dashboard Bmk Card', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 9)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (9, 'DashBoardResourceView', 'Dashboard Resource View', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 10)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (10, 'SearchLaunchCatalogue', 'SearchLaunchCatalogue', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 11)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (11, 'LaunchCatalogueResource', 'LaunchCatalogueResource', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 12)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (12, 'SearchWithinCatalogue', 'SearchWithinCatalogue', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 13)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (13, 'ChangeOfflineStatus', 'Change Offline Status', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END


IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 13)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (13, 'ChangeOfflineStatus', 'Change Offline Status', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 14)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (14, 'SearchCatalogue', 'Search Catalogue', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 15)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (15, 'SearchCataloguePageChange', 'Search Catalogue Page Change', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 16)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (16, 'SearchResourcePageChange', 'Search Resource Page Change', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [analytics].[EventType] WHERE Id = 17)
BEGIN
	INSERT INTO [analytics].[EventType] (Id, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (17, 'CacheFlushAll', 'Cache Flush All', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END