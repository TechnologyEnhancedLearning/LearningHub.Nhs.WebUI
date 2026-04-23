-------------------------------------------------------------------------------
-- Author       OA
-- Created      23 April 2026
-- Purpose      Populate user catalogue activity read model
--
-- Modification History
-- 
-- 23 April 2026  OA  TD-7078 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [reports].[RefreshUserCatalogueActivity]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @LastProcessedActivityId INT;

    SELECT @LastProcessedActivityId = ISNULL(MAX(LatestActivityId), 0)
    FROM reports.UserCatalogueActivity;

    IF OBJECT_ID('tempdb..#ChangedPairs') IS NOT NULL DROP TABLE #ChangedPairs;
    IF OBJECT_ID('tempdb..#LatestIds') IS NOT NULL DROP TABLE #LatestIds;
    IF OBJECT_ID('tempdb..#Latest') IS NOT NULL DROP TABLE #Latest;

   
    SELECT DISTINCT
        ra.UserId,
        np.CatalogueNodeId
    INTO #ChangedPairs
    FROM activity.ResourceActivity ra
    JOIN hierarchy.NodePath np
        ON np.Id = ra.NodePathId
    WHERE ra.Id > @LastProcessedActivityId
      AND ra.UserId IS NOT NULL
      AND ra.Deleted = 0
      AND np.Deleted = 0;

    IF NOT EXISTS (SELECT 1 FROM #ChangedPairs)
        RETURN;

    CREATE CLUSTERED INDEX IX_ChangedPairs
        ON #ChangedPairs (UserId, CatalogueNodeId);

    /* Latest activity id per changed (UserId, CatalogueNodeId) */
    SELECT
        ra.UserId,
        np.CatalogueNodeId,
        MAX(ra.Id) AS LatestActivityId
    INTO #LatestIds
    FROM activity.ResourceActivity ra
    JOIN hierarchy.NodePath np
        ON np.Id = ra.NodePathId
    JOIN #ChangedPairs cp
        ON cp.UserId = ra.UserId
       AND cp.CatalogueNodeId = np.CatalogueNodeId
    WHERE ra.Deleted = 0
      AND np.Deleted = 0
    GROUP BY
        ra.UserId,
        np.CatalogueNodeId;

    CREATE CLUSTERED INDEX IX_LatestIds
        ON #LatestIds (UserId, CatalogueNodeId);

    SELECT
        li.UserId,
        li.CatalogueNodeId,
        ra.Id AS LatestActivityId,
        CAST(COALESCE(ra.ActivityStart, ra.ActivityEnd) AS DATETIME2) AS LastAccessedDate
    INTO #Latest
    FROM #LatestIds li
    JOIN activity.ResourceActivity ra
        ON ra.Id = li.LatestActivityId
    WHERE COALESCE(ra.ActivityStart, ra.ActivityEnd) IS NOT NULL;

    CREATE CLUSTERED INDEX IX_Latest
        ON #Latest (UserId, CatalogueNodeId);

    UPDATE uca
    SET
        uca.LatestActivityId = l.LatestActivityId,
        uca.LastAccessedDate = l.LastAccessedDate
    FROM reports.UserCatalogueActivity uca
    JOIN #Latest l
        ON l.UserId = uca.UserId
       AND l.CatalogueNodeId = uca.CatalogueNodeId;

    INSERT INTO reports.UserCatalogueActivity
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
    FROM #Latest l
    LEFT JOIN reports.UserCatalogueActivity uca
        ON uca.UserId = l.UserId
       AND uca.CatalogueNodeId = l.CatalogueNodeId
    WHERE uca.UserId IS NULL;
END;