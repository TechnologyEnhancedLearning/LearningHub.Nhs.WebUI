-------------------------------------------------------------------------------
-- Author       Swapnamol Abraham
-- Created      08-08-2025
-- Purpose      Get Users learning history
--
-- Modification History
-- 23-09-2025  SA Added new columns for displaying video/Audio Progress
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[GetUsersLearningHistory] (
	  @userId INT,
	  @activityStatuses varchar(50) = NULL,
	  @resourceTypes varchar(50) = NULL
	)
AS
BEGIN 
			SELECT 
			ra.Id AS ActivityId,
			ara.LaunchResourceActivityId AS LaunchResourceActivityId,
			ra.UserId AS UserId,
			ra.ResourceId AS ResourceId,
			r.CurrentResourceVersionId AS ResourceVersionId,
			CAST(CASE WHEN r.CurrentResourceVersionId = ra.ResourceVersionId THEN 1 ELSE 0 END AS BIT) AS IsCurrentResourceVersion,
			(
				SELECT TOP 1 rr.OriginalResourceReferenceId
				FROM [resources].[ResourceReference] rr		
				WHERE rr.ResourceId = rv.ResourceId AND rr.Deleted = 0
			) AS ResourceReferenceId,
			ra.MajorVersion AS MajorVersion,
			ra.MinorVersion AS MinorVersion,
			ra.NodePathId AS NodePathId,
			r.ResourceTypeId AS ResourceType,
			rv.Title AS Title,
			rv.CertificateEnabled AS CertificateEnabled,
			rvp.ProviderId AS ProviderId,
			ISNULL(ara.ActivityStatusId,  ra.ActivityStatusId) AS ActivityStatus,
			ra.ActivityStart AS ActivityDate,
			ISNULL(ara.DurationSeconds, 0) ActivityDurationSeconds,
			ara.Score AS ScorePercentage,
			CASE 
			WHEN ResourceTypeId = 2 THEN ISNULL(audiorv.DurationInMilliseconds, 0)
			WHEN ResourceTypeId = 7 THEN ISNULL(videorv.DurationInMilliseconds, 0)
			ELSE 0
		    END AS ResourceDurationMilliseconds,
			ISNULL( CAST(mar.PercentComplete AS INT) ,0) AS CompletionPercentage
		FROM activity.ResourceActivity ra
		LEFT JOIN activity.ResourceActivity ara
			ON ara.LaunchResourceActivityId = ra.Id
		INNER JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
		INNER JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId AND rv.deleted =0
		LEFT JOIN [resources].[ResourceVersionProvider] rvp on rv.Id = rvp.ResourceVersionId
		LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
		LEFT JOIN [resources].[AudioResourceVersion] audiorv ON audiorv.ResourceVersionId = ra.ResourceVersionId
		LEFT JOIN [resources].[VideoResourceVersion] videorv ON videorv.ResourceVersionId = ra.ResourceVersionId
		LEFT JOIN [activity].[AssessmentResourceActivity] asra ON asra.ResourceActivityId = ra.Id
		LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = ra.Id
		LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id
		WHERE ra.LaunchResourceActivityId IS NULL and ra.userid = @userId AND ra.deleted = 0
		 AND (
						@activityStatuses IS NULL OR
						ISNULL(ara.ActivityStatusId, ra.ActivityStatusId) IN (
							SELECT TRY_CAST(value AS INT)
							FROM STRING_SPLIT(@activityStatuses, ',')
							WHERE TRY_CAST(value AS INT) IS NOT NULL)
			)
		AND (
					@resourceTypes IS NULL OR
					r.ResourceTypeId IN (
						SELECT value
						FROM STRING_SPLIT(@resourceTypes, ','))
				)
		ORDER BY ra.ActivityStart desc
END