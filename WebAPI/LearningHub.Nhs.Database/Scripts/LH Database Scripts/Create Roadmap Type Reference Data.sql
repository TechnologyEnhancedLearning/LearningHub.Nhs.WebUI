IF NOT EXISTS(SELECT 'X' FROM [hub].[RoadmapType] WHERE Id = 1)
BEGIN
	set IDENTITY_INSERT [hub].[RoadmapType] ON
	INSERT INTO [hub].[RoadmapType]([Id],[Name],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(1, 'Updates', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET());
END