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
	@DashboardType			nvarchar(30),
	@UserId					INT,
	@PageNumber				INT = 1,
	@TotalRecords			INT OUTPUT
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

    -- Introduce shared temp table datasets for reuse everywhere:
    
    -- Providers aggregation
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

    -- Auth lookup
    SELECT DISTINCT CatalogueNodeId
    INTO #Auth
    FROM hub.RoleUserGroupView rug
    JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
    WHERE rug.ScopeTypeId = 1
        AND rug.RoleId IN (1,2,3)
        AND uug.Deleted = 0
        AND uug.UserId = @UserId;

   
    SELECT
        n.Id AS NodeId,
        nv.Id AS NodeVersionId,
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
    JOIN hierarchy.NodeVersion nv ON nv.NodeId = n.Id AND nv.VersionStatusId = 2 AND nv.Deleted = 0
    JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id AND cnv.Deleted = 0
    WHERE n.Id <> 1 AND n.Hidden = 0 AND n.Deleted = 0;

    IF @DashboardType = 'popular-catalogues'
    BEGIN
        SELECT 
            na.NodeId,
            COUNT(*) AS NodeCount
        INTO #Popular
        FROM activity.NodeActivity na
        JOIN hierarchy.Node n ON n.Id = na.NodeId
        WHERE 
            na.CatalogueNodeVersionId = n.CurrentNodeVersionId
            AND n.Hidden = 0
            AND n.Deleted = 0
            AND na.NodeId <> 113
        GROUP BY na.NodeId;

        SELECT @TotalRecords = COUNT(*) FROM #Popular;

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

    ELSE IF @DashboardType = 'recent-catalogues'
    BEGIN
        SELECT @TotalRecords = COUNT(*)
        FROM #BaseCatalogues
        WHERE CatalogueNodeVersionId IN (
            SELECT Id FROM hierarchy.CatalogueNodeVersion WHERE LastShownDate IS NOT NULL
        );

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

            ub.Id,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            p.ProvidersJson

        FROM #BaseCatalogues b
        LEFT JOIN #Providers p ON p.CatalogueNodeVersionId = b.CatalogueNodeVersionId
        LEFT JOIN #Auth a ON a.CatalogueNodeId = b.NodeId
        LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = b.NodeId
        JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.Id = b.CatalogueNodeVersionId
        WHERE cnv.LastShownDate IS NOT NULL

        ORDER BY cnv.LastShownDate DESC
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END

    -- MY CATALOGUES with fixed MAX pattern
    ELSE IF @DashboardType = 'my-catalogues'
    BEGIN
        WITH Latest AS (
            SELECT 
                np.NodeId,
                np.CatalogueNodeId,
                ROW_NUMBER() OVER (
                    PARTITION BY np.NodeId 
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

        SELECT @TotalRecords = COUNT(*) FROM #MyCatalogues;

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

            ub.Id,
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

            ub.Id,
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