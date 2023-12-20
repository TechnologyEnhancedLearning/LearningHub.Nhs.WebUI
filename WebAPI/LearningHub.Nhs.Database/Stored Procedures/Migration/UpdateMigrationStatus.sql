-------------------------------------------------------------------------------
-- Author       RobS
-- Created      01-04-2020
-- Purpose      Updates the status of a MigrationInputRecord following the publish
--				of a resource version. Also updates status of overall migration
--				if no resources are left.
--
-- Modification History
--
-- 01-04-2020  RobS	- Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[UpdateMigrationStatus]
(
	@ResourceVersionId int,
	@Successful bit
)

AS

BEGIN

	IF @Successful = 1
		UPDATE [migrations].[MigrationInputRecord] SET 
			MigrationInputRecordStatusId = 7 -- LHPublishComplete
		WHERE ResourceVersionId = @ResourceVersionId 
	ELSE
		UPDATE [migrations].[MigrationInputRecord] SET 
			ExceptionDetails = 'Error occurred in publish Azure Function. See ResourceVersionEvent table or error log table for details.',
			MigrationInputRecordStatusId = 6 -- LHPublishFailed
		WHERE ResourceVersionId = @ResourceVersionId
		

	-- If there are no  queued input records left, mark the migration as complete (PublishedLHResources).
	DECLARE @MigrationId INT
	SELECT @MigrationId = MigrationId FROM [migrations].[MigrationInputRecord] WHERE ResourceVersionId = @ResourceVersionId

	IF @MigrationId > 0 AND NOT EXISTS (SELECT 1 FROM [migrations].[MigrationInputRecord] WHERE MigrationId = @MigrationId AND MigrationInputRecordStatusId = 5) -- 5 = LHQueuedForPublish
	BEGIN
		UPDATE [migrations].[Migration] SET MigrationStatusId = 6 WHERE Id = @MigrationId -- 6 = PublishedLHResources
	END

END