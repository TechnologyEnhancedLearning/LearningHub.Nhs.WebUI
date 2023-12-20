IF NOT EXISTS(SELECT 1 FROM [maintenance].[InternalSystem])
BEGIN
	DECLARE @dbName NVARCHAR(100)
	SELECT @dbName = DB_NAME()

	IF @dbName LIKE '%dev%'
	BEGIN
		INSERT [maintenance].[InternalSystem] ([Id], [Name], [Description], [IsOffline], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
		VALUES
		(1, N'Learning Hub', N'Learning Hub', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(2, N'resourcepublishqueuedev', N'Queue to manage resourse publishing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(3, N'hierarchyeditpublishqueuedev', N'Queue to manage content structure publishing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(4, N'contentmanagementqueuedev', N'Queue to manage admin landing page video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(5, N'wholeslideimageprocessingqueuedev', N'Queue to manage .NET based whole slide image processing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(6, N'wholeslideimageprocessingjavaqueuedev', N'Queue to manage java based Z stack whole slide image processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(7, N'videoprocessingqueuedev', N'Queue to manage video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
	END

	IF @dbName LIKE '%test%'
	BEGIN
		INSERT [maintenance].[InternalSystem] ([Id], [Name], [Description], [IsOffline], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
		VALUES
		(1, N'Learning Hub', N'Learning Hub', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(2, N'resourcepublishqueuetest', N'Queue to manage resourse publishing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(3, N'hierarchyeditpublishqueuetest', N'Queue to manage content structure publishing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(4, N'contentmanagementqueuetest', N'Queue to manage admin landing page video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(5, N'wholeslideimageprocessingqueuetest', N'Queue to manage .NET based whole slide image processing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(6, N'wholeslideimageprocessingjavaqueuetest', N'Queue to manage java based Z stack whole slide image processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(7, N'videoprocessingqueuetest', N'Queue to manage video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
	END

	IF @dbName LIKE '%uat%'
	BEGIN
		INSERT [maintenance].[InternalSystem] ([Id], [Name], [Description], [IsOffline], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
		VALUES
		(1, N'Learning Hub', N'Learning Hub', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(2, N'resourcepublishqueueuat', N'Queue to manage resourse publishing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(3, N'hierarchyeditpublishqueueuat', N'Queue to manage content structure publishing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(4, N'contentmanagementqueueuat', N'Queue to manage admin landing page video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(5, N'wholeslideimageprocessingqueueuat', N'Queue to manage .NET based whole slide image processing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(6, N'wholeslideimageprocessingjavaqueueuat', N'Queue to manage java based Z stack whole slide image processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(7, N'videoprocessingqueueuat', N'Queue to manage video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
	END

	IF @dbName LIKE '%prod%'
	BEGIN
		INSERT [maintenance].[InternalSystem] ([Id], [Name], [Description], [IsOffline], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
		VALUES
		(1, N'Learning Hub', N'Learning Hub', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(2, N'resourcepublishqueueprod', N'Queue to manage resourse publishing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(3, N'hierarchyeditpublishqueueprod', N'Queue to manage content structure publishing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(4, N'contentmanagementqueueprod', N'Queue to manage admin landing page video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(5, N'wholeslideimageprocessingqueueprod', N'Queue to manage .NET based whole slide image processing', 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(6, N'wholeslideimageprocessingjavaqueueprod', N'Queue to manage java based Z stack whole slide image processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()),
		(7, N'videoprocessingqueueprod', N'Queue to manage video processing', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
	END
END