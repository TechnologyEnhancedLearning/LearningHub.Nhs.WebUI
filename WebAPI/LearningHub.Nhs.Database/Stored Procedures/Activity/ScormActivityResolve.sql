-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      02-06-2021
-- Purpose      Resolves ScormActivity
--				Create a 'Completed' ResourceActivity event if the ScormActivity
--				has a 'Completed' status but there is no corrsponding ResourceActivity event.
--
-- Modification History
--
-- 11-06-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[ScormActivityResolve]
(
	@ScormActivityId int,
	@UserTimezoneOffset int = NULL
)
AS
BEGIN

	DECLARE @ScormResourceActivityId int
	DECLARE @ActivityStatusId int
	DECLARE @Score decimal(16,2)
	DECLARE @DurationSeconds int
	DECLARE @CreateUserId int
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

	SELECT 
		@CreateUserId = sa.CreateUserId,
		@ScormResourceActivityId = [ResourceActivityId],
		@ActivityStatusId = CmiCoreLesson_status,
		@DurationSeconds = CASE WHEN CmiCoreSession_time IS NULL THEN 0 ELSE activity.ScormTimeToSeconds(CmiCoreSession_time) END,
		@Score = CmiCoreScoreRaw
	FROM 
		activity.ScormActivity sa
	LEFT JOIN
		activity.ResourceActivity ra ON sa.ResourceActivityId = ra.LaunchResourceActivityId AND ra.ActivityStatusId = sa.CmiCoreLesson_status
	WHERE 
		sa.Id = @ScormActivityId
		AND sa.CmiCoreLesson_status IN (3, 4, 5) -- Examine only when ScormActivity has a "Compelted" status.
		AND sa.Deleted = 0
		
	IF @ScormResourceActivityId IS NOT NULL AND @DurationSeconds > 0
	BEGIN TRY
		BEGIN TRAN

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
			UserId = @CreateUserId,
			LaunchResourceActivityId = @ScormResourceActivityId,
			ra.ResourceId,
			ra.ResourceVersionId,
			ra.MajorVersion,
			ra.MinorVersion,
			ra.NodePathId,
			@ActivityStatusId,
			ActivityStart = ra.ActivityStart,
			ActivityEnd = DATEADD(second, @DurationSeconds, ra.ActivityStart),
			DurationSeconds = @DurationSeconds,
			Score = CASE WHEN @Score = 0 AND @ActivityStatusId NOT IN (4,5) THEN NULL ELSE @Score END,
			Deleted = 0,
			CreateUserId = @CreateUserId,
			CreateDate = @AmendDate,
			AmendUserId = @CreateUserId,
			AmendDate = @AmendDate
		FROM 
			activity.ResourceActivity ra
		WHERE 
			Id = @ScormResourceActivityId -- Base completed activity on associated launch event
			AND Deleted = 0

		COMMIT
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