IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 1)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(1, 'Launched', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 2)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(2, 'In Progress', 0, 2, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 3)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(3, 'Completed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 4)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(4, 'Failed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 5)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(5, 'Passed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 6)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(6, 'Downloaded', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END