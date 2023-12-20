IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceLicence] WHERE Id = 1)
BEGIN
	INSERT INTO [resources].[ResourceLicence]([Id],[Title],[DisplayOrder],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(1, 'Creative commons: Attribution-NonCommercial 4.0 International', 1, 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceLicence] WHERE Id = 2)
BEGIN
	INSERT INTO [resources].[ResourceLicence]([Id],[Title],[DisplayOrder],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(2, 'Creative Commons: Attribution-NonCommercial-ShareAlike 4.0 International', 2, 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceLicence] WHERE Id = 3)
BEGIN
	INSERT INTO [resources].[ResourceLicence]([Id],[Title],[DisplayOrder],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(3, 'Creative Commons: Attribution-NonCommercial-NoDerivatives 4.0 International', 3, 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [resources].[ResourceLicence] WHERE Id = 4)
BEGIN
	INSERT INTO [resources].[ResourceLicence]([Id],[Title],[DisplayOrder],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(4, '© All rights reserved', 4, 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
