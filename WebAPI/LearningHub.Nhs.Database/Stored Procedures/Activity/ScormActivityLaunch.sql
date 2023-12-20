-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      02-06-2021
-- Purpose      Creates new ScormActivity 'Launch' event
--
-- Modification History
--
-- 23-02-2021  Killian Davies	Initial Revision
-- 09-02-2022  Killian Davies	Use OriginalResourceReference.
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[ScormActivityLaunch]
(
	@UserId int,
	@ResourceReferenceId int,
	@ScormActivityId int output
)
AS
BEGIN

	DECLARE @ResourceVersionId int
	DECLARE @NodePathId int

	SELECT 
		@ResourceVersionId = r.CurrentResourceVersionId,
		@NodePathId = rr.NodePathId
	FROM 
		resources.ResourceReference rr
	INNER JOIN 
		resources.[Resource] r ON r.id = rr.ResourceId
	INNER JOIN
		resources.[ResourceVersion] rv ON rv.id = r.CurrentResourceVersionId
	WHERE 
		rr.OriginalResourceReferenceId = @ResourceReferenceId
		AND rr.Deleted = 0
		AND r.Deleted = 0
		AND rv.Deleted = 0
		AND rv.VersionStatusId = 2 -- Published

	-- Ensure a valid activity location is resolved.
	IF @ResourceVersionId IS NULL OR @NodePathId IS NULL
	BEGIN
		DECLARE @ActivityLocationErrorMessage nvarchar(1024)
		SELECT @ActivityLocationErrorMessage = 'Error obtaining Scorm Activity location for ResoureReference=' + @ResourceReferenceId
		RAISERROR (@ActivityLocationErrorMessage,   
				   16, -- Severity.  
				   1 -- State.  
				   );  
	END

	BEGIN TRY
		BEGIN TRAN

		DECLARE @ResourceActivityId INT

		INSERT INTO [activity].[ResourceActivity] ([UserId],
												   [ResourceId],
												   [ResourceVersionId],
												   [MajorVersion],
												   [MinorVersion],
												   [NodePathId],
												   [ActivityStatusId],
												   [ActivityStart],
												   [DurationSeconds],
												   [Deleted],
												   [CreateUserId],
												   [CreateDate],
												   [AmendUserId],
												   [AmendDate])
		SELECT 
			UserId = @UserId,
			rv.ResourceId,
			@ResourceVersionId,
			rv.MajorVersion,
			rv.MinorVersion,
			@NodePathId,
			1, -- Launched
			ActivityStart = SYSDATETIMEOFFSET(),
			DurationSeconds = 0,
			Deleted = 0,
			CreateUserId = @UserId,
			CreateDate = SYSDATETIMEOFFSET(),
			AmendUserId = @UserId,
			AmendDate = SYSDATETIMEOFFSET()
		FROM 
			resources.ResourceVersion rv
		WHERE 
			Id = @ResourceVersionId
			AND Deleted = 0

		SELECT @ResourceActivityId = SCOPE_IDENTITY()

		INSERT INTO [activity].[ScormActivity]([ResourceActivityId],
											   [Deleted],
											   [CreateUserId],
											   [CreateDate],
											   [AmendUserId],
											   [AmendDate])
		VALUES (@ResourceActivityId, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

		SELECT @ScormActivityId = SCOPE_IDENTITY()

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

