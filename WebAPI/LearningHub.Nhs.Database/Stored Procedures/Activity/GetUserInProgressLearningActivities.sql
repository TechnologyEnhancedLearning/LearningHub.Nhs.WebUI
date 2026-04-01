-------------------------------------------------------------------------------
-- Author       Swapnamol Abraham
-- Created      10-09-2025
-- Purpose      Get Users In progress learning acrtivities
--
-- Modification History
-- 01-10-2025  SA added assesment score and passmark and provider details
-- 05-02-2025  SA TD-6860 : Fixed the null issue with the search history
-- 31-03-2026  OA TD-7057 Script Optimization
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

        rrRef.OriginalResourceReferenceId AS ResourceReferenceID,

        ra.MajorVersion AS MajorVersion,
        ra.MinorVersion AS MinorVersion,
        ra.NodePathId AS NodePathId,
        r.ResourceTypeId AS ResourceType,
        rv.Title AS Title,
        rv.CertificateEnabled AS CertificateEnabled,
        ISNULL(ara.ActivityStatusId, ra.ActivityStatusId) AS ActivityStatus,
        ra.ActivityStart AS ActivityDate,
        ISNULL(ara.DurationSeconds, 0) AS ActivityDurationSeconds,
        ara.Score AS ScorePercentage,
        arv.AssessmentType AS AssessmentType,
        arv.PassMark AS AssessmentPassMark,
        asra.score AS AssesmentScore,
        mar.SecondsPlayed AS SecondsPlayed,
        ISNULL(CAST(mar.PercentComplete AS INT), 0) AS PercentComplete,
        sa.CmiCoreLesson_status AS CmiCoreLessonstatus,
        sa.CmiCoreScoreMax AS CmiCoreScoreMax,
        sa.CmiCoreSession_time AS CmiCoreSessiontime,
        sa.DurationSeconds AS DurationSeconds,
        rpAgg.ProvidersJson,

        ROW_NUMBER() OVER (
            PARTITION BY ra.ResourceId 
            ORDER BY ISNULL(ara.ActivityEnd, ra.ActivityStart) DESC
        ) AS rn

    FROM activity.ResourceActivity ra
    LEFT JOIN activity.ResourceActivity ara ON ara.LaunchResourceActivityId = ra.Id
    INNER JOIN resources.Resource r ON ra.ResourceId = r.Id
    INNER JOIN resources.ResourceVersion rv ON rv.Id = ra.ResourceVersionId AND rv.Deleted = 0

   LEFT JOIN (
    SELECT 
        ResourceId,
        MAX(Id) AS LatestRefId
    FROM resources.ResourceReference
    WHERE Deleted = 0
    GROUP BY ResourceId
     ) rrLatest ON rrLatest.ResourceId = rv.ResourceId

    LEFT JOIN resources.ResourceReference rrRef
    ON rrRef.Id = rrLatest.LatestRefId

      LEFT JOIN (
        SELECT 
            rp.ResourceVersionId,
            (
                SELECT 
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Logo
                FROM resources.ResourceVersionProvider rp2
                JOIN hub.Provider p ON p.Id = rp2.ProviderId
                WHERE rp2.ResourceVersionId = rp.ResourceVersionId
                  AND rp2.Deleted = 0
                  AND p.Deleted = 0
                FOR JSON PATH
            ) AS ProvidersJson
        FROM resources.ResourceVersionProvider rp
        GROUP BY rp.ResourceVersionId
    ) rpAgg ON rpAgg.ResourceVersionId = r.CurrentResourceVersionId


    LEFT JOIN resources.AssessmentResourceVersion arv ON arv.ResourceVersionId = ra.ResourceVersionId
    LEFT JOIN activity.AssessmentResourceActivity asra ON asra.ResourceActivityId = ra.Id
    LEFT JOIN activity.MediaResourceActivity mar ON mar.ResourceActivityId = ra.Id
    LEFT JOIN activity.ScormActivity sa ON sa.ResourceActivityId = ra.Id

    WHERE ra.LaunchResourceActivityId IS NULL
      AND ra.UserId = @userId
      AND ra.Deleted = 0
      AND r.ResourceTypeId IN (6)
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
	   ActivityStatus,
	   ActivityDate,
	   ActivityDurationSeconds,
	   ScorePercentage,
	   0 AS ResourceDurationMilliseconds,
	   PercentComplete AS CompletionPercentage,
	   ProvidersJson,
	   AssessmentType,
	   AssessmentPassMark,
	   AssesmentScore
FROM CTERecentActivities
WHERE rn = 1 AND ActivityStatus IN (2,4,7)
			order by ActivityDate desc;
		
END