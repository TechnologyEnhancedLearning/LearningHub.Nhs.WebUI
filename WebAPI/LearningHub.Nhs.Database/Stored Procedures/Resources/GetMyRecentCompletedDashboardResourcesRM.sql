CREATE PROCEDURE [resources].[GetMyRecentCompletedDashboardResourcesRM]
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
        ura.ResourceId,
        ura.LatestActivityId
    INTO #Candidates
    FROM reports.UserResourceActivity ura
    WHERE ura.UserId = @UserId
      AND ura.IsCompleted = 1
    ORDER BY ura.LatestActivityId DESC;

    CREATE CLUSTERED INDEX IX_Candidates ON #Candidates (ResourceId, LatestActivityId);

    SELECT @TotalRecords = COUNT(*) FROM #Candidates;

    SELECT
        dr.ResourceId,
        dr.ResourceReferenceId AS ResourceReferenceID,
        dr.ResourceVersionId,
        dr.ResourceTypeId,
        dr.Title,
        dr.Description,
        dr.DurationInMilliseconds,
        dr.CatalogueName,
        dr.Url,
        dr.BadgeUrl,
        dr.RestrictedAccess,
        CAST(
            CASE
                WHEN dr.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0
                ELSE 1
            END AS BIT
        ) AS HasAccess,
        ub.Id AS BookMarkId,
        CAST(ISNULL(ub.Deleted, 1) ^ 1 AS BIT) AS IsBookmarked,
        CAST(dr.AverageRating AS DECIMAL(3,2)) AS AverageRating,
        dr.RatingCount,
        dr.ProvidersJson,
        c.LatestActivityId AS SortLatestActivityId
    INTO #Base
    FROM #Candidates c
    JOIN reports.DashboardResources dr
        ON dr.ResourceId = c.ResourceId
    LEFT JOIN #Auth auth
        ON auth.CatalogueNodeId = dr.CatalogueNodeId
    LEFT JOIN hub.UserBookmark ub
        ON ub.UserId = @UserId
       AND ub.ResourceReferenceId = dr.ResourceReferenceId;

    SELECT
        ResourceId,
        ResourceReferenceID,
        ResourceVersionId,
        ResourceTypeId,
        Title,
        Description,
        DurationInMilliseconds,
        CatalogueName,
        Url,
        BadgeUrl,
        RestrictedAccess,
        HasAccess,
        BookMarkId,
        IsBookmarked,
        AverageRating,
        RatingCount,
        ProvidersJson
    FROM #Base
    ORDER BY SortLatestActivityId DESC, Title
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;