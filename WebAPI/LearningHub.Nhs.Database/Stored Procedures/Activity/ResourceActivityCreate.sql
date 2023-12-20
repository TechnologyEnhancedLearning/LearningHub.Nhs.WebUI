-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Records Activity for the supplied parameters
--
-- Modification History
--
-- 01-04-2020  Killian Davies	Initial Revision
-- 15-07-2020  Jignesh Jethwani	Card-6040, get output param, set activity start and end date as nullable
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[ResourceActivityCreate]
(
	@UserId int,
	@ResourceVersionId int,
	@NodePathId int = NULL,	
	@ActivityStatusId int,
	@ActivityStart datetimeoffset(7) = NULL,
	@ActivityEnd datetimeoffset(7) = NULL,
	@Score decimal(16,2)  = NULL,
	@LaunchResourceActivityId int = NULL,
	@UserTimezoneOffset int = NULL,
	@ResourceActivityId int output
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN

		IF NOT EXISTS (SELECT 'X' 
					   FROM [activity].[ResourceActivity] 
					   WHERE ResourceVersionId=@ResourceVersionId 
							AND ActivityStatusId=@ActivityStatusId 
							AND ActivityStart=@ActivityStart)
		BEGIN

			DECLARE @DurationSeconds int = 0

			IF (@ActivityStart != null AND @ActivityEnd != null)
			BEGIN
				SET @DurationSeconds = DATEDIFF(SECOND, @ActivityStart, @ActivityEnd)
			END

			INSERT INTO [activity].[ResourceActivity]
					   ([UserId],
						[ResourceId],
						[ResourceVersionId],
						[LaunchResourceActivityId],
						[MajorVersion],
						[MinorVersion],
						[NodePathId],
						[ActivityStatusId],
						[ActivityStart],
						[ActivityEnd],
						[DurationSeconds],
						[Score],
						[Deleted],
						[CreateUserId],
						[CreateDate],
						[AmendUserId],
						[AmendDate])
			SELECT
				UserId = @UserId,
				rv.ResourceId,
				ResourceVersionId = @ResourceVersionId,
				LaunchResourceActivityId = @LaunchResourceActivityId,
				rv.MajorVersion, 
				rv.MinorVersion,
				NodePathId = @NodePathId,
				ActivityStatusId = @ActivityStatusId,
				ActivityStart = @ActivityStart,
				ActivityEnd = @ActivityEnd,
				DurationSeconds = @DurationSeconds,
				Score = @Score,
				Deleted = 0,
				CreateUserId = @UserId,
				CreateDate = @AmendDate,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			FROM
				resources.ResourceVersion rv
			WHERE 
				Id = @ResourceVersionId

			SELECT @ResourceActivityId = SCOPE_IDENTITY()


		END

		COMMIT

	END TRY
	BEGIN CATCH
	    DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  
  
		SELECT   
			@ErrorMessage = ERROR_MESSAGE(),  
			@ErrorSeverity = ERROR_SEVERITY(),  
			@ErrorState = ERROR_STATE();  
  
		IF @@TRANCOUNT > 0  
			ROLLBACK TRAN;  

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  

	END CATCH
END
GO


