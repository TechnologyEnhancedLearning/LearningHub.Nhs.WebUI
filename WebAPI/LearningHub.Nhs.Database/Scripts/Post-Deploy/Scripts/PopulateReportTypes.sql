IF NOT EXISTS(SELECT 1 FROM [reports].[ReportType] WHERE Id = 1)
BEGIN
	INSERT INTO [reports].[ReportType] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (1, 'LearningHubCertificate', 'Certificate for Learning Hub Resource', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [reports].[ReportType] WHERE Id = 2)
BEGIN
	INSERT INTO [reports].[ReportType] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (2, 'DLSCertificate', 'Certificate for DLS Resource', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 1 FROM [reports].[ReportType] WHERE Id = 3)
BEGIN
	INSERT INTO [reports].[ReportType] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (3, 'Report', 'Report Pdf', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END