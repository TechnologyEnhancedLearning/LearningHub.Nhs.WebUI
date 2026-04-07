-------------------------------------------------------------------------------
-- Author       HV
-- Created      18 Nov 2020
-- Purpose      Gets the catalogues to be displayed on dashboard
--
-- Modification History
--
-- 19 Nov 2020	HV	Initial Revision
-- 19 Feb 2021	DB	Total number of records returned
-- 28 Apr 2021	KD	Added restricted access indication
-- 17 Nov 2022  RS  Added offset-fetch functionality for paging
-- 28 Mar 2023  RS  Removed BadgeUrl column as no longer used
-- 15 Jun 2023  RS  Re-added BadgeUrl column following design change
-- 11 Aug 2023  RS  Added CardImageUrl column
-- 27 Sep 2023  HV  Included Paging and user accessed catalogues
-- 13 Nov 2023  SA  Included Node VersionId in also.
-- 29 Sep 2025  SA  Integrated the provider dertails 
-- 31 Mar 2026  OA  TD-7057 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetDashboardCatalogues]
    @DashboardType nvarchar(30),
    @UserId INT,
    @PageNumber INT = 1,
    @TotalRecords INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxPageNumber INT = 4;
    DECLARE @FetchRows INT = 3;

    IF @DashboardType = 'all-catalogues'
        SET @FetchRows = CASE WHEN @PageNumber = -1 THEN 100000 ELSE 9 END;

    IF @PageNumber > @MaxPageNumber AND @DashboardType <> 'all-catalogues'
        SET @PageNumber = @MaxPageNumber;

    DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows;
    DECLARE @MaxRows INT = @MaxPageNumber * @FetchRows;

---providers
    SELECT 
        cnp.CatalogueNodeVersionId,
        JSON_QUERY('[' + STRING_AGG(
            '{"Id":' + CAST(p.Id AS NVARCHAR) +
            ',"Name":"' + p.Name + '"' +
            ',"Description":"' + p.Description + '"' +
            ',"Logo":"' + ISNULL(p.Logo,'') + '"}',
        ',') + ']') AS ProvidersJson
    INTO #Providers
    FROM hierarchy.CatalogueNodeVersionProvider cnp
    JOIN hub.Provider p ON p.Id = cnp.ProviderId
    WHERE p.Deleted = 0 AND cnp.Deleted = 0
    GROUP BY cnp.CatalogueNodeVersionId;

   --auth
    SELECT DISTINCT CatalogueNodeId
    INTO #Auth
    FROM hub.RoleUserGroupView rug
    JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
    WHERE rug.ScopeTypeId = 1
        AND rug.RoleId IN (1,2,3)
        AND uug.Deleted = 0
        AND uug.UserId = @UserId;

   --base catalogue
    SELECT
        n.Id AS NodeId,
        cnv.Id AS NodeVersionId,   
        cnv.Id AS CatalogueNodeVersionId,
        cnv.Name,
        cnv.Description,
        cnv.BannerUrl,
        cnv.BadgeUrl,
        cnv.CardImageUrl,
        cnv.Url,
        cnv.RestrictedAccess
    INTO #BaseCatalogues
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
    WHERE n.Id <> 1 
        AND n.Hidden = 0 
        AND n.Deleted = 0;

   --popular
    IF @DashboardType = 'popular-catalogues'
    BEGIN
        SELECT na.NodeId, COUNT(*) NodeCount
        INTO #Popular
        FROM activity.NodeActivity na
        JOIN hierarchy.Node n ON n.Id = na.NodeId
        WHERE na.CatalogueNodeVersionId = n.CurrentNodeVersionId
            AND n.Hidden = 0
            AND n.Deleted = 0
            AND na.NodeId <> 113
        GROUP BY na.NodeId;

        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM #Popular;

        SELECT 
            b.NodeId,
            b.NodeVersionId,
            b.Name,
            b.Description,
            b.BannerUrl,
            b.BadgeUrl,
            b.CardImageUrl,
            b.Url,
            b.RestrictedAccess,

            CAST(CASE 
                WHEN b.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0 
                ELSE 1 
            END AS BIT) AS HasAccess,

            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            p.ProvidersJson

        FROM #Popular pop
        JOIN #BaseCatalogues b ON b.NodeId = pop.NodeId
        LEFT JOIN #Providers p ON p.CatalogueNodeVersionId = b.CatalogueNodeVersionId
        LEFT JOIN #Auth a ON a.CatalogueNodeId = b.NodeId
        LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = b.NodeId

        ORDER BY pop.NodeCount DESC, b.Name
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END

    --recent
    ELSE IF @DashboardType = 'recent-catalogues'
    BEGIN
        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM #BaseCatalogues b
        JOIN hierarchy.CatalogueNodeVersion cnv 
            ON cnv.Id = b.CatalogueNodeVersionId
        WHERE cnv.LastShownDate IS NOT NULL;

        SELECT 
            b.NodeId,
            b.NodeVersionId,
            b.Name,
            b.Description,
            b.BannerUrl,
            b.BadgeUrl,
            b.CardImageUrl,
            b.Url,
            b.RestrictedAccess,

            CAST(CASE 
                WHEN b.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0 
                ELSE 1 
            END AS BIT) AS HasAccess,

            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            p.ProvidersJson

        FROM #BaseCatalogues b
        JOIN hierarchy.CatalogueNodeVersion cnv 
            ON cnv.Id = b.CatalogueNodeVersionId
        LEFT JOIN #Providers p ON p.CatalogueNodeVersionId = b.CatalogueNodeVersionId
        LEFT JOIN #Auth a ON a.CatalogueNodeId = b.NodeId
        LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = b.NodeId

        WHERE cnv.LastShownDate IS NOT NULL
        ORDER BY b.NodeId DESC   -- matches original final ordering
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END

    ELSE IF @DashboardType = 'highly-contributed-catalogues'
    BEGIN
        SELECT nr.NodeId, COUNT(*) NodeCount
        INTO #HC
        FROM hierarchy.Node n
        JOIN hierarchy.NodeResource nr ON nr.NodeId = n.Id
        JOIN resources.Resource r ON r.Id = nr.ResourceId
        JOIN resources.ResourceVersion rv 
            ON rv.Id = r.CurrentResourceVersionId 
            AND rv.VersionStatusId = 2 
            AND rv.Deleted = 0
        JOIN hierarchy.Publication p ON p.Id = nr.PublicationId AND p.Deleted = 0
        WHERE n.Id <> 1 AND nr.Deleted = 0 AND n.Deleted = 0 AND n.Hidden = 0
        GROUP BY nr.NodeId;

        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM #HC;

        SELECT 
            b.NodeId,
            b.NodeVersionId,
            b.Name,
            b.Description,
            b.BannerUrl,
            b.BadgeUrl,
            b.CardImageUrl,
            b.Url,
            b.RestrictedAccess,

            CAST(CASE 
                WHEN b.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0 
                ELSE 1 
            END AS BIT) AS HasAccess,

            (SELECT SUM(rvrs.AverageRating)
             FROM resources.ResourceVersionRatingSummary rvrs
             WHERE rvrs.ResourceVersionId IN (
                 SELECT r.CurrentResourceVersionId
                 FROM resources.Resource r
                 WHERE r.Deleted = 0
                 AND r.Id IN (
                     SELECT nr.ResourceId
                     FROM hierarchy.NodeResource nr
                     WHERE nr.NodeId = b.NodeId
                 )
             )) AS AverageRating,

            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            p.ProvidersJson

        FROM #HC hc
        JOIN #BaseCatalogues b ON b.NodeId = hc.NodeId
        LEFT JOIN #Providers p ON p.CatalogueNodeVersionId = b.CatalogueNodeVersionId
        LEFT JOIN #Auth a ON a.CatalogueNodeId = b.NodeId
        LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = b.NodeId

        ORDER BY hc.NodeCount DESC, AverageRating DESC, b.Name
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END

    ELSE IF @DashboardType = 'my-catalogues'
    BEGIN
        WITH Latest AS (
            SELECT 
                np.NodeId,
                np.CatalogueNodeId,
                ROW_NUMBER() OVER (
                    PARTITION BY np.NodeId, np.CatalogueNodeId
                    ORDER BY ra.Id DESC
                ) rn
            FROM activity.ResourceActivity ra
            JOIN hierarchy.NodePath np ON np.Id = ra.NodePathId
            WHERE ra.UserId = @UserId
        )
        SELECT DISTINCT NodeId, CatalogueNodeId
        INTO #MyCatalogues
        FROM Latest
        WHERE rn = 1;

        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM #MyCatalogues;

        SELECT 
            b.NodeId,
            b.NodeVersionId,
            b.Name,
            b.Description,
            b.BannerUrl,
            b.BadgeUrl,
            b.CardImageUrl,
            b.Url,
            b.RestrictedAccess,

            CAST(CASE 
                WHEN b.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0 
                ELSE 1 
            END AS BIT) AS HasAccess,

            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            p.ProvidersJson

        FROM #MyCatalogues mc
        JOIN #BaseCatalogues b ON b.NodeId = mc.CatalogueNodeId
        LEFT JOIN #Providers p ON p.CatalogueNodeVersionId = b.CatalogueNodeVersionId
        LEFT JOIN #Auth a ON a.CatalogueNodeId = b.NodeId
        LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = b.NodeId

        ORDER BY b.NodeId DESC
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END

    ELSE
    BEGIN
        SELECT @TotalRecords = COUNT(*) FROM #BaseCatalogues;

        SELECT 
            b.NodeId,
            b.NodeVersionId,
            b.Name,
            b.Description,
            b.BannerUrl,
            b.BadgeUrl,
            b.CardImageUrl,
            b.Url,
            b.RestrictedAccess,

            CAST(CASE 
                WHEN b.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0 
                ELSE 1 
            END AS BIT) AS HasAccess,

            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            p.ProvidersJson

        FROM #BaseCatalogues b
        LEFT JOIN #Providers p ON p.CatalogueNodeVersionId = b.CatalogueNodeVersionId
        LEFT JOIN #Auth a ON a.CatalogueNodeId = b.NodeId
        LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = b.NodeId

        ORDER BY b.Name
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END
END