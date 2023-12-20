-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      02-06-2021
-- Purpose      Completes ScormActivity
--
-- Modification History
--
-- 11-06-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[ScormActivityComplete]
(
	@UserId int,
	@ScormActivityId int,
	@UserTimezoneOffset int = NULL,
	@ResourceActivityId int output
)
AS
BEGIN

	DECLARE @ScormResourceActivityId int
	DECLARE @ActivityStatusId int
	DECLARE @Score decimal(16,2)
	DECLARE @DurationSeconds int
	DECLARE @ScormActivityDurationLimitHours int = 10 * 60 * 60
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

	SELECT 
		@ScormResourceActivityId = [ResourceActivityId],
		@ActivityStatusId = CmiCoreLesson_status,
		@DurationSeconds = CASE 
							WHEN CmiCoreSession_time IS NULL THEN 0 
							WHEN (activity.ScormTimeToSeconds(CmiCoreSession_time) >= @ScormActivityDurationLimitHours) THEN (@ScormActivityDurationLimitHours - 1)
							ELSE activity.ScormTimeToSeconds(CmiCoreSession_time) 
						   END,
		@Score = CmiCoreScoreRaw
	FROM 
		activity.ScormActivity sa
	INNER JOIN	
		activity.ResourceActivity ra ON sa.ResourceActivityId = ra.Id
	WHERE 
		sa.Id = @ScormActivityId
		AND sa.Deleted = 0
		
	IF EXISTS (SELECT 'X' FROM activity.ResourceActivity WHERE LaunchResourceActivityId = @ScormResourceActivityId AND ActivityStatusId IN (3, 4, 5))
	BEGIN
		DECLARE @ActivityStatusErrorMessage nvarchar(1024)
		SELECT @ActivityStatusErrorMessage = 'ResourceActivity entry with Completed status already exists for ScormActivityId=' + @ScormActivityId
		RAISERROR (@ActivityStatusErrorMessage,   
				   16, -- Severity.  
				   1 -- State.  
				   );  
	END

	-- Validation ported from e-LfH: completed status requires duration > 0
	IF (@ActivityStatusId IN (3, 4, 5) AND @DurationSeconds > 0)
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
			UserId = @UserId,
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
			CreateUserId = @UserId,
			CreateDate = @AmendDate,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM 
			activity.ResourceActivity ra
		WHERE 
			Id = @ScormResourceActivityId -- Base completed activity on associated launch event
			AND Deleted = 0

		SELECT @ResourceActivityId = SCOPE_IDENTITY()

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