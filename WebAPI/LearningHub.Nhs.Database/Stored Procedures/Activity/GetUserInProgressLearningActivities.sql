-------------------------------------------------------------------------------
-- Author       Swapnamol Abraham
-- Created      10-09-2025
-- Purpose      Get Users In progress learning activities
--
-- Modification History
-------------------------------------------------------------------------------
CREATE PROCEDURE [activity].[GetUserInProgressLearningActivities] (
	 @userId INT
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
	rv.CertificateEnabled AS CertificateEnabled,
	rvp.ProviderId AS ProviderId,
	ISNULL(ara.ActivityStatusId,  ra.ActivityStatusId) AS ActivityStatus,
    ra.ActivityStart AS ActivityDate,
   -- ara.ActivityEnd,
    ISNULL(ara.DurationSeconds, 0) AS ActivityDurationSeconds,
    ara.Score AS ScorePercentage,
	arv.AssessmentType AS AssessmentType,
	arv.PassMark AS PassMark,
	asra.score AS AssesmentScore,
	mar.SecondsPlayed AS SecondsPlayed,
	mar.PercentComplete AS PercentComplete,
	sa.CmiCoreLesson_status AS CmiCoreLessonstatus,
	sa.CmiCoreScoreMax AS CmiCoreScoreMax,
	sa.CmiCoreSession_time AS CmiCoreSessiontime,
	sa.DurationSeconds AS DurationSeconds,
    ROW_NUMBER() OVER (PARTITION BY ra.ResourceId ORDER BY ISNULL(ara.ActivityEnd, ra.ActivityStart) DESC) AS rn
   FROM activity.ResourceActivity ra
	LEFT JOIN activity.ResourceActivity ara ON ara.LaunchResourceActivityId = ra.Id
	INNER JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
	INNER JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId AND rv.Deleted = 0
	LEFT JOIN [resources].[ResourceVersionProvider] rvp on rv.Id = rvp.ResourceVersionId
	LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
	LEFT JOIN [activity].[AssessmentResourceActivity] asra ON asra.ResourceActivityId = ra.Id
	LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = ra.Id
	LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id
  WHERE ra.LaunchResourceActivityId IS NULL AND ra.userid = @userId 
  AND ra.deleted = 0
  AND r.ResourceTypeId IN(6) --AND ra.ActivityStart >= DATEADD(MONTH, -6, SYSDATETIMEOFFSET())  
) 
SELECT Top 8 ActivityId,
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
	   ProviderId,
	   ActivityStatus,
	   ActivityDate,
	   ActivityDurationSeconds,
	   ScorePercentage
FROM CTERecentActivities
WHERE rn = 1 order by ActivityDate desc;
		
END