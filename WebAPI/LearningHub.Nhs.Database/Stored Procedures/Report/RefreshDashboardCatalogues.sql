-------------------------------------------------------------------------------
-- Author       OA
-- Created      23 April 2026
-- Purpose      Populate dashbaord catalogues read model
--
-- Modification History
-- 
-- 23 April 2026  OA  TD-7078 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [reports].[RefreshDashboardCatalogues]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRAN;

    BEGIN TRY
        IF OBJECT_ID('tempdb..#Providers') IS NOT NULL DROP TABLE #Providers;
        IF OBJECT_ID('tempdb..#CatalogueActivity') IS NOT NULL DROP TABLE #CatalogueActivity;
        IF OBJECT_ID('tempdb..#Contributed') IS NOT NULL DROP TABLE #Contributed;

        /* Precompute provider JSON */
        SELECT
            cnp.CatalogueNodeVersionId,
            ProvidersJson = (
                SELECT
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Logo
                FROM hierarchy.CatalogueNodeVersionProvider cnp2
                JOIN hub.Provider p
                    ON p.Id = cnp2.ProviderId
                WHERE cnp2.CatalogueNodeVersionId = cnp.CatalogueNodeVersionId
                  AND cnp2.Deleted = 0
                  AND p.Deleted = 0
                FOR JSON PATH
            )
        INTO #Providers
        FROM hierarchy.CatalogueNodeVersionProvider cnp
        WHERE cnp.Deleted = 0
        GROUP BY cnp.CatalogueNodeVersionId;

        CREATE CLUSTERED INDEX IX_Providers
            ON #Providers (CatalogueNodeVersionId);

        /* Precompute catalogue activity count */
        SELECT
            na.NodeId AS CatalogueNodeId,
            COUNT(*) AS CatalogueActivityCount
        INTO #CatalogueActivity
        FROM activity.NodeActivity na
        JOIN hierarchy.Node n
            ON n.Id = na.NodeId
        WHERE na.CatalogueNodeVersionId = n.CurrentNodeVersionId
          AND n.Hidden = 0
          AND n.Deleted = 0
          AND na.NodeId <> 113
        GROUP BY na.NodeId;

        CREATE CLUSTERED INDEX IX_CatalogueActivity
            ON #CatalogueActivity (CatalogueNodeId);

        /* Precompute highly contributed metrics */
        SELECT
            nr.NodeId AS CatalogueNodeId,
            COUNT(*) AS ContributedResourceCount,
            CAST(SUM(CAST(ISNULL(rvrs.AverageRating, 0) AS DECIMAL(18,2))) AS DECIMAL(18,2)) AS SumResourceAverageRating
        INTO #Contributed
        FROM hierarchy.Node n
        JOIN hierarchy.NodeResource nr
            ON nr.NodeId = n.Id
        JOIN resources.Resource r
            ON r.Id = nr.ResourceId
        JOIN resources.ResourceVersion rv
            ON rv.Id = r.CurrentResourceVersionId
           AND rv.VersionStatusId = 2
           AND rv.Deleted = 0
        JOIN hierarchy.Publication p
            ON p.Id = nr.PublicationId
           AND p.Deleted = 0
        LEFT JOIN resources.ResourceVersionRatingSummary rvrs
            ON rvrs.ResourceVersionId = rv.Id
           AND rvrs.Deleted = 0
        WHERE n.Id <> 1
          AND nr.Deleted = 0
          AND n.Deleted = 0
          AND n.Hidden = 0
        GROUP BY nr.NodeId;

        CREATE CLUSTERED INDEX IX_Contributed
            ON #Contributed (CatalogueNodeId);

        TRUNCATE TABLE reports.DashboardCatalogues;

        INSERT INTO reports.DashboardCatalogues
        (
            CatalogueNodeId,
            NodeVersionId,
            CatalogueNodeVersionId,
            Name,
            Description,
            BannerUrl,
            BadgeUrl,
            CardImageUrl,
            Url,
            RestrictedAccess,
            LastShownDate,
            CatalogueActivityCount,
            ContributedResourceCount,
            SumResourceAverageRating,
            ProvidersJson
        )
        SELECT
            n.Id AS CatalogueNodeId,
            cnv.Id AS NodeVersionId,
            cnv.Id AS CatalogueNodeVersionId,
            cnv.Name,
            cnv.Description,
            cnv.BannerUrl,
            cnv.BadgeUrl,
            cnv.CardImageUrl,
            cnv.Url,
            cnv.RestrictedAccess,
            cnv.LastShownDate,
            ISNULL(ca.CatalogueActivityCount, 0) AS CatalogueActivityCount,
            ISNULL(c.ContributedResourceCount, 0) AS ContributedResourceCount,
            c.SumResourceAverageRating,
            p.ProvidersJson
        FROM hierarchy.Node n
        JOIN hierarchy.NodeVersion nv
            ON nv.NodeId = n.Id
           AND nv.VersionStatusId = 2
        JOIN hierarchy.CatalogueNodeVersion cnv
            ON cnv.NodeVersionId = nv.Id
           AND cnv.Deleted = 0
        JOIN hub.Scope s
            ON s.CatalogueNodeId = n.Id
           AND s.Deleted = 0
        LEFT JOIN #Providers p
            ON p.CatalogueNodeVersionId = cnv.Id
        LEFT JOIN #CatalogueActivity ca
            ON ca.CatalogueNodeId = n.Id
        LEFT JOIN #Contributed c
            ON c.CatalogueNodeId = n.Id
        WHERE n.Id <> 1
          AND n.Hidden = 0
          AND n.Deleted = 0;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        THROW;
    END CATCH
END;