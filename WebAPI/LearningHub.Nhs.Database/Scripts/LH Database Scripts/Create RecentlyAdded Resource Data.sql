IF NOT EXISTS(SELECT 1 FROM [resources].[RecentlyAdded] WHERE Id = 1)
BEGIN
	INSERT INTO [resources].[RecentlyAdded]([DisplayOrder],[ResourceId]) VALUES (1, 1)
END
IF NOT EXISTS(SELECT 1 FROM [resources].[RecentlyAdded] WHERE Id = 2)
BEGIN
	INSERT INTO [resources].[RecentlyAdded]([DisplayOrder],[ResourceId]) VALUES (2, 2)
END
IF NOT EXISTS(SELECT 1 FROM [resources].[RecentlyAdded] WHERE Id = 3)
BEGIN
	INSERT INTO [resources].[RecentlyAdded]([DisplayOrder],[ResourceId]) VALUES (3, 3)
END
IF NOT EXISTS(SELECT 1 FROM [resources].[RecentlyAdded] WHERE Id = 4)
BEGIN
	INSERT INTO [resources].[RecentlyAdded]([DisplayOrder],[ResourceId]) VALUES (4, 4)
END
