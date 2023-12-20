-------------------------------------------------------------------------------
-- Author       Jignesh Jethwani
-- Created      21-07-2023
-- Purpose      Check User ScormActivity Suspend Data to be cleared on next launch of resource activity
--				check if new resource version created has cleared suspend data flag and if last resource activity was before cleared suspend data flag set then don't copy suspend data
--				this applies to only if last activity was incomplete, this has already being checked at code level
-- 21-07-2023  Jignesh Jethwani - Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[UserScormActivitySuspendDataToBeCleared]
(
	@LastScormActivityId int,
	@CurrentResourceVersionId int,	
	@Clear bit output
)
AS
BEGIN

	DECLARE @CurrentResourceVersionNumber BIGINT
	DECLARE @LastScormActivityResourceVersionNumber BIGINT
	DECLARE @ResourceId INT
	DECLARE @ClearSuspendData bit

	SET @Clear = 0;	

	-- get last scorm activity resource vesion number	
	SELECT  
		@LastScormActivityResourceVersionNumber = [activity].[CalculateResourceVersionNumber] (activity.ResourceActivity.MajorVersion, activity.ResourceActivity.MinorVersion)  
	FROM
		activity.ScormActivity 
	INNER JOIN
        activity.ResourceActivity 
	ON 
		activity.ScormActivity.ResourceActivityId = activity.ResourceActivity.Id
	WHERE
		activity.ScormActivity.Id = @LastScormActivityId

		
	-- get current resource version number
	SELECT 
		@CurrentResourceVersionNumber = [activity].[CalculateResourceVersionNumber] (resources.ResourceVersion.MajorVersion, resources.ResourceVersion.MinorVersion),
		@ResourceId = resources.ResourceVersion.ResourceId
	FROM    
		resources.ResourceVersion 
	INNER JOIN
        resources.ScormResourceVersion 
	ON 
		resources.ResourceVersion.Id = resources.ScormResourceVersion.ResourceVersionId
	WHERE 
		resources.ResourceVersion.Id  = @CurrentResourceVersionId


     -- check current version number is greater than last scorm activity version number, if so then check clear suspend data was checked on any version between these 2 versions
	IF @CurrentResourceVersionNumber > @LastScormActivityResourceVersionNumber
	BEGIN

		IF (EXISTS (
			SELECT 
				1 
			FROM 
				resources.ScormResourceVersion 
			INNER JOIN
				resources.ResourceVersion 
			ON 
				resources.ScormResourceVersion.ResourceVersionId = resources.ResourceVersion.Id
			WHERE
				resources.ResourceVersion.ResourceId = @ResourceId 
			AND 
				resources.ResourceVersion.Deleted = 0 
			AND 
				resources.ScormResourceVersion.Deleted  = 0 
			AND 
				[activity].[CalculateResourceVersionNumber] (resources.ResourceVersion.MajorVersion, resources.ResourceVersion.MinorVersion) > @LastScormActivityResourceVersionNumber and resources.ScormResourceVersion.ClearSuspendData = 1
		)) 

		BEGIN
			SET @Clear = 1;
		END

	END
END
