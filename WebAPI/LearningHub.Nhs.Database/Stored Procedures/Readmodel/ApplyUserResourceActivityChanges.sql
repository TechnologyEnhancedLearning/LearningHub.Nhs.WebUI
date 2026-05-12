-------------------------------------------------------------------------------
-- Author       OA
-- Created      23 April 2026
-- Purpose      Populate user resource activity read model
--
-- Modification History
-- 
-- 23 April 2026  OA  TD-7078 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [readmodels].[ApplyUserResourceActivityChanges]
(
    @ChangedActivityIdsJson NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @ChangedActivityIdsJson IS NULL OR ISJSON(@ChangedActivityIdsJson) <> 1
        RETURN;

    IF OBJECT_ID('tempdb..#ChangedActivityIds') IS NOT NULL DROP TABLE #ChangedActivityIds;
    IF OBJECT_ID('tempdb..#ChangedPairs') IS NOT NULL DROP TABLE #ChangedPairs;
    IF OBJECT_ID('tempdb..#LatestIds') IS NOT NULL DROP TABLE #LatestIds;
    IF OBJECT_ID('tempdb..#Latest') IS NOT NULL DROP TABLE #Latest;
    IF OBJECT_ID('tempdb..#Ara') IS NOT NULL DROP TABLE #Ara;
    IF OBJECT_ID('tempdb..#Mar') IS NOT NULL DROP TABLE #Mar;
    IF OBJECT_ID('tempdb..#Sa') IS NOT NULL DROP TABLE #Sa;
    IF OBJECT_ID('tempdb..#Completion') IS NOT NULL DROP TABLE #Completion;

    SELECT DISTINCT
        TRY_CAST([value] AS INT) AS ActivityId
    INTO #ChangedActivityIds
    FROM OPENJSON(@ChangedActivityIdsJson)
    WHERE TRY_CAST([value] AS INT) IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM #ChangedActivityIds)
        RETURN;

    CREATE CLUSTERED INDEX IX_ChangedActivityIds
        ON #ChangedActivityIds (ActivityId);

    /* Derive affected user-resource pairs */
    SELECT DISTINCT
        ra.UserId,
        ra.ResourceId
    INTO #ChangedPairs
    FROM #ChangedActivityIds c
    JOIN activity.ResourceActivity ra
        ON ra.Id = c.ActivityId
    WHERE ra.UserId IS NOT NULL
      AND ra.Deleted = 0;

    IF NOT EXISTS (SELECT 1 FROM #ChangedPairs)
        RETURN;

    CREATE CLUSTERED INDEX IX_ChangedPairs
        ON #ChangedPairs (UserId, ResourceId);

    /* Latest activity id per affected (UserId, ResourceId) */
    SELECT
        ra.UserId,
        ra.ResourceId,
        MAX(ra.Id) AS LatestActivityId
    INTO #LatestIds
    FROM activity.ResourceActivity ra
    JOIN #ChangedPairs cp
        ON cp.UserId = ra.UserId
       AND cp.ResourceId = ra.ResourceId
    WHERE ra.Deleted = 0
    GROUP BY
        ra.UserId,
        ra.ResourceId;

    CREATE CLUSTERED INDEX IX_LatestIds
        ON #LatestIds (UserId, ResourceId);

    /* Latest full row */
    SELECT
        ra.UserId,
        ra.ResourceId,
        ra.Id AS ActivityId,
        ra.ResourceVersionId,
        ra.LaunchResourceActivityId,
        ra.ActivityStatusId,
        ra.ActivityStart,
        ra.ActivityEnd,
        r.ResourceTypeId
    INTO #Latest
    FROM #LatestIds li
    JOIN activity.ResourceActivity ra
        ON ra.Id = li.LatestActivityId
    JOIN resources.Resource r
        ON r.Id = ra.ResourceId;

    CREATE CLUSTERED INDEX IX_Latest
        ON #Latest (UserId, ResourceId);

    /* Collapse child tables */
    SELECT
        ara.ResourceActivityId,
        MAX(ara.Score) AS Score
    INTO #Ara
    FROM activity.AssessmentResourceActivity ara
    JOIN #Latest l
        ON l.ResourceTypeId = 11
       AND ara.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
    WHERE ara.Score IS NOT NULL
    GROUP BY ara.ResourceActivityId;

    CREATE CLUSTERED INDEX IX_Ara
        ON #Ara (ResourceActivityId);

    SELECT
        mar.ResourceActivityId,
        MAX(mar.PercentComplete) AS PercentComplete
    INTO #Mar
    FROM activity.MediaResourceActivity mar
    JOIN #Latest l
        ON l.ResourceTypeId IN (2,7)
       AND mar.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
    WHERE mar.PercentComplete IS NOT NULL
    GROUP BY mar.ResourceActivityId;

    CREATE CLUSTERED INDEX IX_Mar
        ON #Mar (ResourceActivityId);

    SELECT
        sa.ResourceActivityId,
        MAX(sa.CmiCoreLesson_status) AS CmiCoreLesson_status
    INTO #Sa
    FROM activity.ScormActivity sa
    JOIN #Latest l
        ON l.ResourceTypeId = 6
       AND sa.ResourceActivityId = l.ActivityId
    WHERE sa.CmiCoreLesson_status IS NOT NULL
    GROUP BY sa.ResourceActivityId;

    CREATE CLUSTERED INDEX IX_Sa
        ON #Sa (ResourceActivityId);

    /* Recompute completion */
    SELECT
        l.UserId,
        l.ResourceId,
        l.ActivityId AS LatestActivityId,
        CAST(COALESCE(l.ActivityStart, l.ActivityEnd) AS DATETIME2) AS LastAccessedDate,
        CAST(
            CASE
                WHEN l.ResourceTypeId IN (2,7)
                     AND l.ActivityStatusId = 3
                     AND (
                         (mar.ResourceActivityId IS NOT NULL AND mar.PercentComplete = 100)
                         OR l.ActivityStart < '2020-09-07'
                     )
                    THEN 1
                WHEN l.ResourceTypeId = 6
                     AND (
                         sa.CmiCoreLesson_status IN (3,5)
                         OR l.ActivityStatusId IN (3,5)
                     )
                    THEN 1
                WHEN l.ResourceTypeId = 11
                     AND (
                         ara.Score >= arv.PassMark
                         OR l.ActivityStatusId IN (3,5)
                     )
                    THEN 1
                WHEN l.ResourceTypeId IN (1,5,8,9,10,12)
                     AND l.ActivityStatusId = 3
                    THEN 1
                ELSE 0
            END AS BIT
        ) AS IsCompleted
    INTO #Completion
    FROM #Latest l
    LEFT JOIN resources.AssessmentResourceVersion arv
        ON arv.ResourceVersionId = l.ResourceVersionId
    LEFT JOIN #Ara ara
        ON ara.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
    LEFT JOIN #Mar mar
        ON mar.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
    LEFT JOIN #Sa sa
        ON sa.ResourceActivityId = l.ActivityId
    WHERE COALESCE(l.ActivityStart, l.ActivityEnd) IS NOT NULL;

    CREATE CLUSTERED INDEX IX_Completion
        ON #Completion (UserId, ResourceId);

    /* Update existing */
    UPDATE ura
    SET
        ura.LatestActivityId = c.LatestActivityId,
        ura.IsCompleted = c.IsCompleted,
        ura.LastAccessedDate = c.LastAccessedDate
    FROM readmodels.UserResourceActivity ura
    JOIN #Completion c
        ON c.UserId = ura.UserId
       AND c.ResourceId = ura.ResourceId;

    /* Insert new */
    INSERT INTO readmodels.UserResourceActivity
    (
        UserId,
        ResourceId,
        LatestActivityId,
        IsCompleted,
        LastAccessedDate
    )
    SELECT
        c.UserId,
        c.ResourceId,
        c.LatestActivityId,
        c.IsCompleted,
        c.LastAccessedDate
    FROM #Completion c
    LEFT JOIN readmodels.UserResourceActivity ura
        ON ura.UserId = c.UserId
       AND ura.ResourceId = c.ResourceId
    WHERE ura.UserId IS NULL;
END;
GO