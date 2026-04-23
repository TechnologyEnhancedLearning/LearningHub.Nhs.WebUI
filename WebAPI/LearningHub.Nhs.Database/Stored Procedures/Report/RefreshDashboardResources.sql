-------------------------------------------------------------------------------
-- Author       OA
-- Created      23 April 2026
-- Purpose      Populate dashbaord resources read model
--
-- Modification History
-- 
-- 23 April 2026  OA  TD-7078 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [reports].[RefreshDashboardResources]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRAN;

    BEGIN TRY
        IF OBJECT_ID('tempdb..#ActivityCounts') IS NOT NULL DROP TABLE #ActivityCounts;
        IF OBJECT_ID('tempdb..#Providers') IS NOT NULL DROP TABLE #Providers;
        IF OBJECT_ID('tempdb..#RefMax') IS NOT NULL DROP TABLE #RefMax;
        IF OBJECT_ID('tempdb..#Ref') IS NOT NULL DROP TABLE #Ref;

        /* Precompute activity counts */
        SELECT
            ra.ResourceId,
            COUNT(ra.ResourceVersionId) AS ActivityCount
        INTO #ActivityCounts
        FROM activity.ResourceActivity ra
        GROUP BY ra.ResourceId;

        CREATE CLUSTERED INDEX IX_ActivityCounts
            ON #ActivityCounts (ResourceId);

        /* Precompute providers JSON */
        SELECT
            rp.ResourceVersionId,
            ProvidersJson = (
                SELECT
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Logo
                FROM resources.ResourceVersionProvider rp2
                JOIN hub.Provider p
                    ON p.Id = rp2.ProviderId
                WHERE rp2.ResourceVersionId = rp.ResourceVersionId
                  AND rp2.Deleted = 0
                  AND p.Deleted = 0
                FOR JSON PATH
            )
        INTO #Providers
        FROM resources.ResourceVersionProvider rp
        WHERE rp.Deleted = 0
        GROUP BY rp.ResourceVersionId;

        CREATE CLUSTERED INDEX IX_Providers
            ON #Providers (ResourceVersionId);

        /* Latest ResourceReference per (ResourceId, CatalogueNodeId) */
        SELECT
            rr.ResourceId,
            np.NodeId AS CatalogueNodeId,
            MAX(rr.Id) AS MaxResourceReferenceId
        INTO #RefMax
        FROM resources.ResourceReference rr
        JOIN hierarchy.NodePath np
            ON np.Id = rr.NodePathId
        WHERE rr.Deleted = 0
          AND np.Deleted = 0
        GROUP BY
            rr.ResourceId,
            np.NodeId;

        CREATE CLUSTERED INDEX IX_RefMax
            ON #RefMax (ResourceId, CatalogueNodeId);

        SELECT
            rm.ResourceId,
            rm.CatalogueNodeId,
            rr.OriginalResourceReferenceId
        INTO #Ref
        FROM #RefMax rm
        JOIN resources.ResourceReference rr
            ON rr.Id = rm.MaxResourceReferenceId;

        CREATE CLUSTERED INDEX IX_Ref
            ON #Ref (ResourceId, CatalogueNodeId);

       
        TRUNCATE TABLE reports.DashboardResources;

        INSERT INTO reports.DashboardResources
        (
            ResourceId,
            ResourceReferenceId,
            ResourceVersionId,
            ResourceTypeId,
            Title,
            Description,
            DurationInMilliseconds,
            CatalogueNodeId,
            CatalogueName,
            Url,
            BadgeUrl,
            RestrictedAccess,
            PublishedDate,
            AverageRating,
            RatingCount,
            ActivityCount,
            ProvidersJson
        )
        SELECT
            r.Id AS ResourceId,
            rf.OriginalResourceReferenceId AS ResourceReferenceId,
            rv.Id AS ResourceVersionId,
            r.ResourceTypeId,
            rv.Title,
            rv.Description,
            CASE
                WHEN r.ResourceTypeId = 7 THEN vrv.DurationInMilliseconds
                WHEN r.ResourceTypeId = 2 THEN arv.DurationInMilliseconds
                ELSE NULL
            END AS DurationInMilliseconds,
            n.Id AS CatalogueNodeId,
            CASE WHEN n.Id = 1 THEN NULL ELSE cnv.Name END AS CatalogueName,
            cnv.Url,
            CASE WHEN n.Id = 1 THEN NULL ELSE cnv.BadgeUrl END AS BadgeUrl,
            cnv.RestrictedAccess,
            pub.CreateDate AS PublishedDate,
            CAST(rvrs.AverageRating AS DECIMAL(3,2)) AS AverageRating,
            rvrs.RatingCount,
            ISNULL(ac.ActivityCount, 0) AS ActivityCount,
            prov.ProvidersJson
        FROM resources.Resource r
        JOIN resources.ResourceVersion rv
            ON rv.Id = r.CurrentResourceVersionId
           AND rv.Deleted = 0
           AND rv.VersionStatusId = 2
        JOIN hierarchy.Publication pub
            ON pub.Id = rv.PublicationId
           AND pub.Deleted = 0
        JOIN hierarchy.NodeResource nr
            ON nr.ResourceId = r.Id
           AND nr.Deleted = 0
        JOIN hierarchy.Node n
            ON n.Id = nr.NodeId
           AND n.Hidden = 0
           AND n.Deleted = 0
        JOIN hierarchy.NodePath np
            ON np.NodeId = n.Id
           AND np.Deleted = 0
           AND np.IsActive = 1
        JOIN hierarchy.NodeVersion nv
            ON nv.NodeId = np.CatalogueNodeId
           AND nv.VersionStatusId = 2
           AND nv.Deleted = 0
        JOIN hierarchy.CatalogueNodeVersion cnv
            ON cnv.NodeVersionId = nv.Id
           AND cnv.Deleted = 0
        LEFT JOIN #Ref rf
            ON rf.ResourceId = r.Id
           AND rf.CatalogueNodeId = n.Id
        LEFT JOIN #ActivityCounts ac
            ON ac.ResourceId = r.Id
        LEFT JOIN #Providers prov
            ON prov.ResourceVersionId = rv.Id
        LEFT JOIN resources.VideoResourceVersion vrv
            ON vrv.ResourceVersionId = rv.Id
        LEFT JOIN resources.AudioResourceVersion arv
            ON arv.ResourceVersionId = rv.Id
        LEFT JOIN resources.ResourceVersionRatingSummary rvrs
            ON rvrs.ResourceVersionId = rv.Id
           AND rvrs.Deleted = 0;

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        THROW;
    END CATCH
END;
GO