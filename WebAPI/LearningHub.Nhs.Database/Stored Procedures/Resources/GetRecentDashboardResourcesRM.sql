CREATE PROCEDURE [resources].[GetRecentDashboardResourcesRM]
(
    @UserId       INT,
    @PageNumber   INT = 1,
    @TotalRecords INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxPageNumber INT = 4;
    DECLARE @PageSize INT = 3;
    DECLARE @MaxRows INT = @MaxPageNumber * @PageSize;

    IF @PageNumber > @MaxPageNumber
        SET @PageNumber = @MaxPageNumber;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    SELECT DISTINCT
        rug.CatalogueNodeId
    INTO #Auth
    FROM hub.RoleUserGroupView rug
    JOIN hub.UserUserGroup uug
        ON rug.UserGroupId = uug.UserGroupId
    WHERE rug.ScopeTypeId = 1
      AND rug.RoleId IN (1,2,3)
      AND uug.Deleted = 0
      AND uug.UserId = @UserId;

    CREATE CLUSTERED INDEX IX_Auth ON #Auth (CatalogueNodeId);

    SELECT TOP (@MaxRows)
        dr.ResourceId,
        dr.ResourceVersionId,
        dr.ResourceTypeId,
        dr.Title,
        dr.Description,
        CAST(dr.AverageRating AS DECIMAL(3,2)) AS AverageRating,
        dr.RatingCount,
        dr.PublishedDate AS CreateDate,
        dr.CatalogueName,
        dr.Url,
        dr.BadgeUrl,
        dr.RestrictedAccess,
        dr.ProvidersJson,
        dr.DurationInMilliseconds,
        dr.ResourceReferenceId,
        ub.Id AS BookMarkId,
        CAST(ISNULL(ub.Deleted, 1) ^ 1 AS BIT) AS IsBookmarked,
        CAST(
            CASE
                WHEN dr.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT
        ) AS HasAccess
    INTO #Base
    FROM reports.DashboardResources dr
    LEFT JOIN #Auth auth
        ON auth.CatalogueNodeId = dr.CatalogueNodeId
    LEFT JOIN hub.UserBookmark ub
        ON ub.UserId = @UserId
       AND ub.ResourceReferenceId = dr.ResourceReferenceId
    ORDER BY dr.PublishedDate DESC, dr.Title;

    SELECT @TotalRecords = COUNT(*) FROM #Base;

    SELECT
        ResourceId,
        ResourceVersionId,
        ResourceTypeId,
        Title,
        Description,
        AverageRating,
        RatingCount,
        CreateDate,
        CatalogueName,
        Url,
        BadgeUrl,
        RestrictedAccess,
        ProvidersJson,
        DurationInMilliseconds,
        ResourceReferenceId,
        BookMarkId,
        IsBookmarked,
        HasAccess
    FROM #Base
    ORDER BY CreateDate DESC, Title
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;