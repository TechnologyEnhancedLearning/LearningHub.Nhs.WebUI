IF NOT EXISTS(SELECT 1 FROM [reports].[Client] WHERE Id = 1)
BEGIN
	INSERT [reports].[Client] ([Id], [ClientId], [Name], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) 
	VALUES (1, N'ff6b5370-6274-4b09-a617-fd7b72e63af9', N'lh-ui', 0, 4,SYSDATETIMEOFFSET() , 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [reports].[Client] WHERE Id = 2)
BEGIN
	INSERT [reports].[Client] ([Id], [ClientId], [Name], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) 
	VALUES (2, N'f70c3c29-c0aa-4398-a934-ccf42616b0f3', N'lh-admin', 0, 4,SYSDATETIMEOFFSET() , 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [reports].[Client] WHERE Id = 3)
BEGIN
	INSERT [reports].[Client] ([Id], [ClientId], [Name], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate]) 
	VALUES (3, N'bcde972f-e007-4dff-b233-8870474aeeaa', N'dls', 0, 4,SYSDATETIMEOFFSET() , 4, SYSDATETIMEOFFSET())
END