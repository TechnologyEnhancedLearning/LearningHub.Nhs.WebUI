-------------------------------------------------------------------------------
-- Author       Hari Vaka
-- Created      16-09-2020
-- Purpose       Gets summary details of the incomplete activity
--
-- Modification History
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[ScormActivityGetSummary] (
	@UserId INT
	,@ResourceReferenceId INT
	,@IncompleteActivityId INT = NULL OUTPUT
	,@TotalTime NVARCHAR(20) = NULL OUTPUT
	)
AS
BEGIN

	DECLARE @MajorVersion int
	DECLARE @ResourceId int

	SELECT 
		@ResourceId = r.Id,
		@MajorVersion = rv.MajorVersion
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

	SET @IncompleteActivityId = [activity].[ScormActivityGetLastIncomplete](@ResourceId, @MajorVersion, @UserId)

	IF @IncompleteActivityId IS NOT NULL
	BEGIN
		DECLARE @CompleteActivityId INT

		SET @CompleteActivityId = [activity].[ScormActivityGetLastCompleted](@ResourceId, @MajorVersion, @UserId)

		SET @TotalTime = (
				SELECT
					SUM(activity.ScormTimeToSeconds(sa.CmiCoreSession_time)) AS TotalTime
				FROM 
					activity.ScormActivity sa
				INNER JOIN 
					activity.ResourceActivity ra ON ra.id = sa.ResourceActivityId
				WHERE 
					ra.ResourceId = @ResourceId
					AND ra.MajorVersion = @MajorVersion
					AND sa.CreateUserID = @UserId
					AND sa.CmiCoreLesson_status IN (1, 2)
					AND sa.ID > @CompleteActivityId
					AND ISNULL(sa.CmiCoreSession_time,'') != ''
					AND sa.Deleted = 0
					AND ra.Deleted = 0
				)
	END
END

GO
