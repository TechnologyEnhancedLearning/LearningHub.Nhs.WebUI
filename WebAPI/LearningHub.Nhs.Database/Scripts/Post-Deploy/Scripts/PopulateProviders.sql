
IF NOT EXISTS(SELECT 1 FROM [hub].[Provider] WHERE Id = 1)
BEGIN
	INSERT INTO [hub].[Provider] (Id, [Name], [Description], [Logo], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (1, 'elearning for healthcare', 'elearning for healthcare', 'elfhlogo.svg', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END

