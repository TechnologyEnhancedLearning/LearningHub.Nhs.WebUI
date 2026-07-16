SET NOCOUNT ON;
SET XACT_ABORT ON;

--Backfill readmodels.UserResourceActivity


IF NOT EXISTS (SELECT 1 FROM readmodels.UserResourceActivity)
BEGIN
    IF OBJECT_ID('tempdb..#URA_LatestIds') IS NOT NULL DROP TABLE #URA_LatestIds;
    IF OBJECT_ID('tempdb..#URA_Latest') IS NOT NULL DROP TABLE #URA_Latest;
    IF OBJECT_ID('tempdb..#URA_Ara') IS NOT NULL DROP TABLE #URA_Ara;
    IF OBJECT_ID('tempdb..#URA_Mar') IS NOT NULL DROP TABLE #URA_Mar;
    IF OBJECT_ID('tempdb..#URA_Sa') IS NOT NULL DROP TABLE #URA_Sa;
    IF OBJECT_ID('tempdb..#URA_Completion') IS NOT NULL DROP TABLE #URA_Completion;

    SELECT
        a.UserId,
        a.ResourceId,
        MAX(a.Id) AS LatestActivityId
    INTO #URA_LatestIds
    FROM activity.ResourceActivity a
    WHERE a.UserId IS NOT NULL
      AND a.Deleted = 0
    GROUP BY
        a.UserId,
        a.ResourceId;

    IF EXISTS (SELECT 1 FROM #URA_LatestIds)
    BEGIN
        CREATE CLUSTERED INDEX IX_URA_LatestIds
            ON #URA_LatestIds (UserId, ResourceId);

        SELECT
            a.UserId,
            a.ResourceId,
            a.Id AS ActivityId,
            a.ResourceVersionId,
            a.LaunchResourceActivityId,
            a.ActivityStatusId,
            a.ActivityStart,
            a.ActivityEnd,
            r.ResourceTypeId
        INTO #URA_Latest
        FROM #URA_LatestIds li
        JOIN activity.ResourceActivity a
            ON a.Id = li.LatestActivityId
        JOIN resources.Resource r
            ON r.Id = a.ResourceId;

        CREATE CLUSTERED INDEX IX_URA_Latest
            ON #URA_Latest (UserId, ResourceId);

        SELECT
            ara.ResourceActivityId,
            MAX(ara.Score) AS Score
        INTO #URA_Ara
        FROM activity.AssessmentResourceActivity ara
        JOIN #URA_Latest l
            ON l.ResourceTypeId = 11
           AND ara.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
        WHERE ara.Score IS NOT NULL
        GROUP BY ara.ResourceActivityId;

        CREATE CLUSTERED INDEX IX_URA_Ara
            ON #URA_Ara (ResourceActivityId);

        SELECT
            mar.ResourceActivityId,
            MAX(mar.PercentComplete) AS PercentComplete
        INTO #URA_Mar
        FROM activity.MediaResourceActivity mar
        JOIN #URA_Latest l
            ON l.ResourceTypeId IN (2,7)
           AND mar.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
        WHERE mar.PercentComplete IS NOT NULL
        GROUP BY mar.ResourceActivityId;

        CREATE CLUSTERED INDEX IX_URA_Mar
            ON #URA_Mar (ResourceActivityId);

        SELECT
            sa.ResourceActivityId,
            MAX(sa.CmiCoreLesson_status) AS CmiCoreLesson_status
        INTO #URA_Sa
        FROM activity.ScormActivity sa
        JOIN #URA_Latest l
            ON l.ResourceTypeId = 6
           AND sa.ResourceActivityId = l.ActivityId
        WHERE sa.CmiCoreLesson_status IS NOT NULL
        GROUP BY sa.ResourceActivityId;

        CREATE CLUSTERED INDEX IX_URA_Sa
            ON #URA_Sa (ResourceActivityId);

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
        INTO #URA_Completion
        FROM #URA_Latest l
        LEFT JOIN resources.AssessmentResourceVersion arv
            ON arv.ResourceVersionId = l.ResourceVersionId
        LEFT JOIN #URA_Ara ara
            ON ara.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
        LEFT JOIN #URA_Mar mar
            ON mar.ResourceActivityId = COALESCE(l.LaunchResourceActivityId, l.ActivityId)
        LEFT JOIN #URA_Sa sa
            ON sa.ResourceActivityId = l.ActivityId
        WHERE COALESCE(l.ActivityStart, l.ActivityEnd) IS NOT NULL;

        CREATE CLUSTERED INDEX IX_URA_Completion
            ON #URA_Completion (UserId, ResourceId);

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
        FROM #URA_Completion c;
    END
END;


--Backfill readmodels.UserCatalogueActivity


IF NOT EXISTS (SELECT 1 FROM readmodels.UserCatalogueActivity)
BEGIN
    IF OBJECT_ID('tempdb..#UCA_LatestIds') IS NOT NULL DROP TABLE #UCA_LatestIds;
    IF OBJECT_ID('tempdb..#UCA_Latest') IS NOT NULL DROP TABLE #UCA_Latest;

    SELECT
        ra.UserId,
        np.CatalogueNodeId,
        MAX(ra.Id) AS LatestActivityId
    INTO #UCA_LatestIds
    FROM activity.ResourceActivity ra
    JOIN hierarchy.NodePath np
        ON np.Id = ra.NodePathId
    WHERE ra.UserId IS NOT NULL
      AND ra.Deleted = 0
      AND np.Deleted = 0
    GROUP BY
        ra.UserId,
        np.CatalogueNodeId;

    IF EXISTS (SELECT 1 FROM #UCA_LatestIds)
    BEGIN
        CREATE CLUSTERED INDEX IX_UCA_LatestIds
            ON #UCA_LatestIds (UserId, CatalogueNodeId);

        SELECT
            li.UserId,
            li.CatalogueNodeId,
            ra.Id AS LatestActivityId,
            CAST(COALESCE(ra.ActivityStart, ra.ActivityEnd) AS DATETIME2) AS LastAccessedDate
        INTO #UCA_Latest
        FROM #UCA_LatestIds li
        JOIN activity.ResourceActivity ra
            ON ra.Id = li.LatestActivityId
        WHERE COALESCE(ra.ActivityStart, ra.ActivityEnd) IS NOT NULL;

        CREATE CLUSTERED INDEX IX_UCA_Latest
            ON #UCA_Latest (UserId, CatalogueNodeId);

        INSERT INTO readmodels.UserCatalogueActivity
        (
            UserId,
            CatalogueNodeId,
            LatestActivityId,
            LastAccessedDate
        )
        SELECT
            l.UserId,
            l.CatalogueNodeId,
            l.LatestActivityId,
            l.LastAccessedDate
        FROM #UCA_Latest l;
    END
END;