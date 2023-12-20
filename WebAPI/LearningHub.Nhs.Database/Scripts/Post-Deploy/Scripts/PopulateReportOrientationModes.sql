IF NOT EXISTS(SELECT 1 FROM [reports].[ReportOrientationMode] WHERE Id = 1)
BEGIN
	INSERT INTO [reports].[ReportOrientationMode] (Id, [Name], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (1, 'Portrait',  0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [reports].[ReportOrientationMode] WHERE Id = 2)
BEGIN
	INSERT INTO [reports].[ReportOrientationMode] (Id, [Name], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (2, 'Landscape', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

