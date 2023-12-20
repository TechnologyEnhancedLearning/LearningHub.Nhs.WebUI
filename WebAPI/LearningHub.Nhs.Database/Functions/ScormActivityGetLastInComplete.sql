-------------------------------------------------------------------------------
-- Author       Hari Vaka
-- Created      16-09-2020
-- Purpose      Gets the last incomplete scorm activity Id
--
-- Modification History
-------------------------------------------------------------------------------
CREATE FUNCTION [activity].[ScormActivityGetLastInComplete]
(
	@ResourceId INT,
	@MajorVersion INT,
	@UserId INT
)
RETURNS INT
AS
BEGIN
	
	DECLARE @LastIncompleteActivityId INT
	
	SELECT TOP 1 @LastIncompleteActivityId = CASE WHEN sa.CmiCoreLesson_status IN (3, 4, 5) THEN  NULL ELSE sa.Id END		
	FROM 
		activity.ScormActivity sa
		INNER JOIN activity.ResourceActivity ra ON ra.id = sa.ResourceActivityId
	WHERE 
		ra.ResourceId = @ResourceId
		AND ra.MajorVersion = @MajorVersion
		AND sa.CreateUserID = @UserId		
		AND sa.Deleted = 0
		AND ra.Deleted = 0
	ORDER BY 
		sa.id DESC

	RETURN  @LastIncompleteActivityId

END
GO