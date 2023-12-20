
-- Migration Statuses

IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 0)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (0, 'Created', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 1)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (1, 'Validating', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 2)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (2, 'Validated', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 3)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (3, 'CreatingLHMetadata', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 4)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (4, 'CreatedLHMetadata', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 5)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (5, 'PublishingLHResources', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 6)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (6, 'PublishedLHResources', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationStatus] WHERE Id = 7)
BEGIN
	INSERT INTO [migrations].[MigrationStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (7, 'Abandoned', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END


-- Migration Input Record Statuses

IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 0)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (0, 'Created', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 1)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (1, 'ValidationFailed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 2)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (2, 'ValidationPassed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 3)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (3, 'LHMetadataCreationFailed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 4)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (4, 'LHMetadataCreationComplete', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 5)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (5, 'LHPublishFailed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 6)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (6, 'LHPublishComplete', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 7)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (7, 'TaxonomySubmissionFailed', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 8)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (8, 'TaxonomySubmissionComplete', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET()) 
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE Id = 9)
BEGIN
	INSERT INTO [migrations].[MigrationInputRecordStatus] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (9, 'Abandoned', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END


-- Migration Sources

IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationSource] WHERE Id = 1)
BEGIN
	INSERT INTO [migrations].[MigrationSource] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (1, 'eLR', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 1 FROM [migrations].[MigrationSource] WHERE Id = 2)
BEGIN
	INSERT INTO [migrations].[MigrationSource] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]) 
	VALUES (2, 'eWIN', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END