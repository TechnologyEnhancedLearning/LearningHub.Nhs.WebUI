IF NOT EXISTS(SELECT 1 FROM [reports].[ReportStatus] WHERE Id = 1)
BEGIN
	INSERT INTO [reports].[ReportStatus] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (1, 'Pending', 'Requested by the client', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END


IF NOT EXISTS(SELECT 1 FROM [reports].[ReportStatus] WHERE Id = 2)
BEGIN
	INSERT INTO [reports].[ReportStatus] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (2, 'Ready', 'Report file available', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [reports].[ReportStatus] WHERE Id = 3)
BEGIN
	INSERT INTO [reports].[ReportStatus] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (3, 'Failed', 'Report file generation failed', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END