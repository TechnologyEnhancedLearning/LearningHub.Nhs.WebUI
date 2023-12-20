-- RS 12/8/20
-- Update the MigrationInputRecordStatus records end up as:
--
-- Id	Description
-- 0	Created						(Unchanged)
-- 1	ValidationFailed			(Unchanged)
-- 2	ValidationPassed			(Unchanged)
-- 3	LHMetadataCreationFailed	(Unchanged)
-- 4	LHMetadataCreationComplete	(Unchanged)
-- 5	LHQueuedForPublish			(Changed)
-- 6	LHPublishFailed				(Changed)
-- 7	LHPublishComplete			(Changed)
-- 8	Abandoned					(Changed)


IF EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE ID = 5 AND Description <> 'LHQueuedForPublish')
BEGIN
	UPDATE [migrations].[MigrationInputRecordStatus] SET [Description] = 'LHQueuedForPublish' WHERE ID = 5
END

IF EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE ID = 6 AND Description <> 'LHPublishFailed')
BEGIN
	UPDATE [migrations].[MigrationInputRecordStatus] SET [Description] = 'LHPublishFailed' WHERE ID = 6
END

IF EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE ID = 7 AND Description <> 'LHPublishComplete')
BEGIN
	UPDATE [migrations].[MigrationInputRecordStatus] SET [Description] = 'LHPublishComplete' WHERE ID = 7
END

IF EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE ID = 8 AND Description <> 'Abandoned')
BEGIN
	UPDATE [migrations].[MigrationInputRecordStatus] SET [Description] = 'Abandoned' WHERE ID = 8
END

IF EXISTS(SELECT 1 FROM [migrations].[MigrationInputRecordStatus] WHERE ID = 9)
BEGIN
	DELETE FROM [migrations].[MigrationInputRecordStatus] WHERE [Id] = 9
END