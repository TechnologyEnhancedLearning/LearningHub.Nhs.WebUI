-------------------------------------------------------------------------------
-- Author       OA
-- Created      23 April 2026
-- Purpose      Gets the catalogues to be displayed on dashboard from read models
--
-- Modification History
-- 
-- 23 April 2026  OA  TD-7078 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetDashboardCataloguesRM]
    @DashboardType NVARCHAR(30),
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

    /* Auth stays live */
    SELECT DISTINCT
        CatalogueNodeId
    INTO #Auth
    FROM hub.RoleUserGroupView rug
    JOIN hub.UserUserGroup uug
        ON rug.UserGroupId = uug.UserGroupId
    WHERE rug.ScopeTypeId = 1
      AND rug.RoleId IN (1,2,3)
      AND uug.Deleted = 0
      AND uug.UserId = @UserId;

    CREATE CLUSTERED INDEX IX_Auth ON #Auth (CatalogueNodeId);

    IF @DashboardType = 'popular-catalogues'
    BEGIN
        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM reports.DashboardCatalogues dc
        WHERE dc.CatalogueActivityCount > 0;

        SELECT
            dc.CatalogueNodeId AS NodeId,
            dc.NodeVersionId,
            dc.Name,
            dc.Description,
            dc.BannerUrl,
            dc.BadgeUrl,
            dc.CardImageUrl,
            dc.Url,
            dc.RestrictedAccess,
            CAST(CASE
                WHEN dc.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT) AS HasAccess,
            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            dc.ProvidersJson
        FROM reports.DashboardCatalogues dc
        LEFT JOIN #Auth a
            ON a.CatalogueNodeId = dc.CatalogueNodeId
        LEFT JOIN hub.UserBookmark ub
            ON ub.UserId = @UserId
           AND ub.NodeId = dc.CatalogueNodeId
        WHERE dc.CatalogueActivityCount > 0
        ORDER BY dc.CatalogueActivityCount DESC, dc.Name
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END
    ELSE IF @DashboardType = 'recent-catalogues'
    BEGIN
        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM reports.DashboardCatalogues dc
        WHERE dc.LastShownDate IS NOT NULL;

        SELECT
            dc.CatalogueNodeId AS NodeId,
            dc.NodeVersionId,
            dc.Name,
            dc.Description,
            dc.BannerUrl,
            dc.BadgeUrl,
            dc.CardImageUrl,
            dc.Url,
            dc.RestrictedAccess,
            CAST(CASE
                WHEN dc.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT) AS HasAccess,
            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            dc.ProvidersJson
        FROM reports.DashboardCatalogues dc
        LEFT JOIN #Auth a
            ON a.CatalogueNodeId = dc.CatalogueNodeId
        LEFT JOIN hub.UserBookmark ub
            ON ub.UserId = @UserId
           AND ub.NodeId = dc.CatalogueNodeId
        WHERE dc.LastShownDate IS NOT NULL
        ORDER BY dc.CatalogueNodeId DESC
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END
    ELSE IF @DashboardType = 'highly-contributed-catalogues'
    BEGIN
        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM reports.DashboardCatalogues dc
        WHERE dc.ContributedResourceCount > 0;

        SELECT
            dc.CatalogueNodeId AS NodeId,
            dc.NodeVersionId,
            dc.Name,
            dc.Description,
            dc.BannerUrl,
            dc.BadgeUrl,
            dc.CardImageUrl,
            dc.Url,
            dc.RestrictedAccess,
            CAST(CASE
                WHEN dc.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT) AS HasAccess,
            dc.SumResourceAverageRating AS AverageRating,
            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            dc.ProvidersJson
        FROM reports.DashboardCatalogues dc
        LEFT JOIN #Auth a
            ON a.CatalogueNodeId = dc.CatalogueNodeId
        LEFT JOIN hub.UserBookmark ub
            ON ub.UserId = @UserId
           AND ub.NodeId = dc.CatalogueNodeId
        WHERE dc.ContributedResourceCount > 0
        ORDER BY dc.ContributedResourceCount DESC, dc.SumResourceAverageRating DESC, dc.Name
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END
    ELSE IF @DashboardType = 'my-catalogues'
    BEGIN
        SELECT @TotalRecords =
            CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
        FROM reports.UserCatalogueActivity uca
        WHERE uca.UserId = @UserId;

        SELECT
            dc.CatalogueNodeId AS NodeId,
            dc.NodeVersionId,
            dc.Name,
            dc.Description,
            dc.BannerUrl,
            dc.BadgeUrl,
            dc.CardImageUrl,
            dc.Url,
            dc.RestrictedAccess,
            CAST(CASE
                WHEN dc.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT) AS HasAccess,
            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            dc.ProvidersJson
        FROM reports.UserCatalogueActivity uca
        JOIN reports.DashboardCatalogues dc
            ON dc.CatalogueNodeId = uca.CatalogueNodeId
        LEFT JOIN #Auth a
            ON a.CatalogueNodeId = dc.CatalogueNodeId
        LEFT JOIN hub.UserBookmark ub
            ON ub.UserId = @UserId
           AND ub.NodeId = dc.CatalogueNodeId
        WHERE uca.UserId = @UserId
        ORDER BY dc.CatalogueNodeId DESC
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END
    ELSE
    BEGIN
        SELECT @TotalRecords = COUNT(*)
        FROM reports.DashboardCatalogues;

        SELECT
            dc.CatalogueNodeId AS NodeId,
            dc.NodeVersionId,
            dc.Name,
            dc.Description,
            dc.BannerUrl,
            dc.BadgeUrl,
            dc.CardImageUrl,
            dc.Url,
            dc.RestrictedAccess,
            CAST(CASE
                WHEN dc.RestrictedAccess = 1 AND a.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT) AS HasAccess,
            ub.Id AS BookMarkId,
            CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
            dc.ProvidersJson
        FROM reports.DashboardCatalogues dc
        LEFT JOIN #Auth a
            ON a.CatalogueNodeId = dc.CatalogueNodeId
        LEFT JOIN hub.UserBookmark ub
            ON ub.UserId = @UserId
           AND ub.NodeId = dc.CatalogueNodeId
        ORDER BY dc.Name
        OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;
    END
END;