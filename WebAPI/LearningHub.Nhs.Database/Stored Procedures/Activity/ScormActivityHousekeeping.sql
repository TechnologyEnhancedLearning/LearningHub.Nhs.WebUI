-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      02-06-2021
-- Purpose      Resolves any completed ScormActivity that has not processed an
--				an LMSFinish event.
--
-- Modification History
--
-- 11-06-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[ScormActivityHousekeeping]

AS

BEGIN

	-- Identify ScormActivity records to close:
	-- i.e. those that have exceeded the activity duration limit and have a 'Complete' / 'Passed' / 'Failed' 
	-- status but no corresponding ResourceActivity record.
	DECLARE @SystemUserId int = 4
	DECLARE @ScormActivityDurationLimitHours int = 10
	DECLARE @ScormActivityDurationLimitSeconds int = @ScormActivityDurationLimitHours * 60 * 60

	BEGIN TRY

		INSERT INTO [activity].[ResourceActivity] (UserId,
												   LaunchResourceActivityId,
												   ResourceId,
												   ResourceVersionId,
												   MajorVersion,
												   MinorVersion,
												   NodePathId,
												   ActivityStatusId,
												   ActivityStart,
												   ActivityEnd,
												   DurationSeconds,
												   Score,
												   Deleted,
												   CreateUserId,
												   CreateDate,
												   AmendUserId,
												   AmendDate)
		SELECT 
			UserId = ra_launch.UserId,
			LaunchResourceActivityId = ra_launch.Id,
			ra_launch.ResourceId,
			ra_launch.ResourceVersionId,
			ra_launch.MajorVersion,
			ra_launch.MinorVersion,
			ra_launch.NodePathId,
			ActivityStatusId = sa.CmiCoreLesson_status,
			ActivityStart = ra_launch.ActivityStart,
			ActivityEnd = DATEADD(second, (@ScormActivityDurationLimitSeconds - 1), ra_launch.ActivityStart),
			DurationSeconds = @ScormActivityDurationLimitSeconds - 1,
			Score = CASE WHEN sa.CmiCoreScoreRaw = 0 AND sa.CmiCoreLesson_status NOT IN (4,5) THEN NULL ELSE sa.CmiCoreScoreRaw END,
			Deleted = 0,
			CreateUserId = @SystemUserId,
			CreateDate = SYSDATETIMEOFFSET(),
			AmendUserId = @SystemUserId,
			AmendDate = SYSDATETIMEOFFSET()
		FROM 
			activity.ScormActivity sa
		INNER JOIN
			activity.ResourceActivity ra_launch ON sa.ResourceActivityId = ra_launch.Id
		LEFT JOIN	
			activity.ResourceActivity ra_complete ON sa.ResourceActivityId = ra_complete.LaunchResourceActivityId
										 AND sa.CmiCoreLesson_status = ra_complete.ActivityStatusId
										 AND ra_complete.Deleted = 0
		WHERE 
			sa.CmiCoreLesson_status IN (3, 4, 5) -- Completed / Passed / Failed
			AND ra_complete.Id IS NULL
			AND activity.ScormTimeToSeconds(sa.CmiCoreSession_time) >= @ScormActivityDurationLimitSeconds
			AND sa.Deleted = 0

	END TRY

	BEGIN CATCH
		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT @ErrorMessage = ERROR_MESSAGE()
			,@ErrorSeverity = ERROR_SEVERITY()
			,@ErrorState = ERROR_STATE();

		IF @@TRANCOUNT > 0
			ROLLBACK TRAN;

		RAISERROR (
				@ErrorMessage
				,@ErrorSeverity
				,@ErrorState
				);
	END CATCH
END

GO