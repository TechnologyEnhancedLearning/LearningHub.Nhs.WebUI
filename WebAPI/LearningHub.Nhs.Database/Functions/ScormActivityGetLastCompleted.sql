-------------------------------------------------------------------------------
-- Author       Hari Vaka
-- Created      16-09-2020
-- Purpose      Gets the last completed scorm activity Id
--
-- Modification History
-------------------------------------------------------------------------------
CREATE FUNCTION [activity].[ScormActivityGetLastCompleted]
(
	@ResourceId INT,
	@MajorVersion INT,
	@UserId INT
)
RETURNS INT
AS
BEGIN

	DECLARE @LastCompletedActivityId INT
	
	SELECT TOP 1 
		@LastCompletedActivityId = sa.Id		
	FROM 
		activity.ScormActivity sa
	INNER JOIN 
		activity.ResourceActivity ra ON ra.id = sa.ResourceActivityId	
	WHERE 
		ra.ResourceId = @ResourceId
		AND ra.MajorVersion = @MajorVersion
		AND sa.CreateUserID = @UserId
		AND sa.CmiCoreLesson_status IN (3, 4, 5) --3 Completed, 4 Failed, 5 Passed
	ORDER BY 
		sa.id DESC

	IF @LastCompletedActivityId IS NULL
	BEGIN
	SELECT @LastCompletedActivityId = 0
	END

	RETURN  @LastCompletedActivityId

END
GO