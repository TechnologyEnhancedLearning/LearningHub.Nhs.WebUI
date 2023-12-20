IF NOT EXISTS(SELECT 'X' FROM [hub].[EmailTemplateType] WHERE Id = 23)
BEGIN
	INSERT INTO [hub].[EmailTemplateType]([Id], [Name], [AvailableTags], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
	VALUES(23, 'CatalogueAccessRequest', '[FullName]', 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [hub].[EmailTemplate] WHERE Id = 2000)
BEGIN
	INSERT INTO [hub].[EmailTemplate]([Id],[EmailTemplateTypeId],[Title],[Subject],[Body],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(2000, 23, 'CatalogueAccessRequest', 'subject', 'body', 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
END