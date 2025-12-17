-------------------------------------------------------------------------------
-- Author       Swapnamol Abraham
-- Created      29-07-2025
-- Purpose      Get Users recent learning acrtivities
--
-- Modification History
-- 02-Sep-2025       SA        Incorrect Syntax 
-- 23-09-2025  SA Added new columns for displaying video/Audio Progress
-- 01-10-2025  SA added assesment score and passmark and provider details
-- 16-12-2025  SA TD-6322 added the condition to validate the certificate enabled is true for the latest resourceversion.
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[GetUserRecentLearningActivities] (
	 @userId INT,
	 @activityStatuses varchar(50) = NULL
	)
AS
BEGIN

	;WITH CTERecentActivities AS (
    SELECT 
    ra.Id AS ActivityId,
    ara.LaunchResourceActivityId AS LaunchResourceActivityId,
    ra.UserId AS UserId,
    ra.ResourceId AS ResourceId,
    r.CurrentResourceVersionId AS ResourceVersionId,
	CASE WHEN r.CurrentResourceVersionId = ra.ResourceVersionId THEN 1 ELSE 0 END AS IsCurrentResourceVersion,
    (
        SELECT TOP 1 rr.OriginalResourceReferenceId
		FROM [resources].[ResourceReference] rr		
		WHERE rr.ResourceId = rv.ResourceId AND rr.Deleted = 0
    ) AS ResourceReferenceID,
    ra.MajorVersion AS MajorVersion,
    ra.MinorVersion AS MinorVersion,
    ra.NodePathId AS NodePathId,
	r.ResourceTypeId AS ResourceType,
	rv.Title AS Title,
	--rv.[Description] AS ResourceDescription,
	 -- Get CertificateEnabled from the latest resource version
    rvCurrent.CertificateEnabled AS CertificateEnabled,
	ISNULL(ara.ActivityStatusId,  ra.ActivityStatusId) AS ActivityStatus,
    ra.ActivityStart AS ActivityDate,
   -- ara.ActivityEnd,
    ISNULL(ara.DurationSeconds, 0) AS ActivityDurationSeconds,
    ara.Score AS ScorePercentage,
	arv.AssessmentType AS AssessmentType,
	arv.PassMark AS AssessmentPassMark,
	asra.score AS AssesmentScore,
	mar.SecondsPlayed AS SecondsPlayed,
	ISNULL( CAST(mar.PercentComplete AS INT) ,0) AS PercentComplete,
	sa.CmiCoreLesson_status AS CmiCoreLessonstatus,
	sa.CmiCoreScoreMax AS CmiCoreScoreMax,
	sa.CmiCoreSession_time AS CmiCoreSessiontime,
	sa.DurationSeconds AS DurationSeconds,
	CASE 
			WHEN ResourceTypeId = 2 THEN ISNULL(audiorv.DurationInMilliseconds, 0)
			WHEN ResourceTypeId = 7 THEN ISNULL(videorv.DurationInMilliseconds, 0)
			ELSE 0
		    END AS ResourceDurationMilliseconds,
			rpAgg.ProvidersJson,
    ROW_NUMBER() OVER (PARTITION BY ra.ResourceId ORDER BY ISNULL(ara.ActivityEnd, ra.ActivityStart) DESC) AS rn
   FROM activity.ResourceActivity ra
	LEFT JOIN activity.ResourceActivity ara ON ara.LaunchResourceActivityId = ra.Id
	INNER JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
	  -- Version used in the activity
    INNER JOIN [resources].[ResourceVersion] rv ON rv.Id = ra.ResourceVersionId AND rv.Deleted = 0
    -- Latest resource version
    INNER JOIN [resources].[ResourceVersion] rvCurrent ON rvCurrent.Id = r.CurrentResourceVersionId
	LEFT JOIN (
		SELECT 
			rp.ResourceVersionId,
			JSON_QUERY('[' + STRING_AGG(
				'{"Id":' + CAST(p.Id AS NVARCHAR) +
				',"Name":"' + p.Name + '"' +
				',"Description":"' + p.Description + '"' +
				',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
			',') + ']') AS ProvidersJson
		FROM resources.ResourceVersionProvider rp
		JOIN hub.Provider p ON p.Id = rp.ProviderId
		WHERE p.Deleted = 0 and rp.Deleted = 0
		GROUP BY rp.ResourceVersionId
		) rpAgg ON rpAgg.ResourceVersionId = r.CurrentResourceVersionId
	LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
	LEFT JOIN [resources].[AudioResourceVersion] audiorv ON audiorv.ResourceVersionId = ra.ResourceVersionId
	LEFT JOIN [resources].[VideoResourceVersion] videorv ON videorv.ResourceVersionId = ra.ResourceVersionId
	LEFT JOIN [activity].[AssessmentResourceActivity] asra ON asra.ResourceActivityId = ra.Id
	LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = ra.Id
	LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id
  WHERE ra.LaunchResourceActivityId IS NULL AND ra.userid = @userId 
  AND ra.deleted = 0
  AND r.ResourceTypeId IN(2,6,7,10,11) AND ra.ActivityStart >= DATEADD(MONTH, -6, SYSDATETIMEOFFSET())  
) 
SELECT ActivityId,
       LaunchResourceActivityId,
	   UserId,
	   ResourceId,
	   ResourceVersionId,
	   CAST(IsCurrentResourceVersion AS BIT) AS IsCurrentResourceVersion,
	   ResourceReferenceId,
	   MajorVersion,
	   MinorVersion,
	   NodePathId,
	   ResourceType,
	   Title,
	   CertificateEnabled,
	   ActivityStatus,
	   ActivityDate,
	   ActivityDurationSeconds,
	   ScorePercentage,
	   ResourceDurationMilliseconds,
	   PercentComplete AS CompletionPercentage,
	   ProvidersJson,
	   AssessmentType,
	   AssessmentPassMark,
	   AssesmentScore
FROM CTERecentActivities
WHERE rn = 1
 AND (
						@activityStatuses IS NULL OR
						ActivityStatus IN (
							SELECT TRY_CAST(value AS INT)
							FROM STRING_SPLIT(@activityStatuses, ',')
							WHERE TRY_CAST(value AS INT) IS NOT NULL)
			)
order by ActivityDate desc;
		
END