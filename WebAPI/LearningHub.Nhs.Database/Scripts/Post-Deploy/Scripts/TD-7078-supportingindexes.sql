
  -- reports.DashboardResources


IF OBJECT_ID('reports.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_ResourceId'
      AND object_id = OBJECT_ID('reports.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_ResourceId
    ON reports.DashboardResources (ResourceId);
END;
GO

IF OBJECT_ID('reports.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_Recent'
      AND object_id = OBJECT_ID('reports.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_Recent
    ON reports.DashboardResources (PublishedDate DESC, Title)
    INCLUDE
    (
        ResourceId,
        CatalogueNodeId,
        ResourceReferenceId,
        ResourceVersionId,
        ResourceTypeId,
        Description,
        DurationInMilliseconds,
        CatalogueName,
        Url,
        BadgeUrl,
        RestrictedAccess,
        AverageRating,
        RatingCount
    );
END;
GO

IF OBJECT_ID('reports.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_Popular'
      AND object_id = OBJECT_ID('reports.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_Popular
    ON reports.DashboardResources (ActivityCount DESC, Title)
    INCLUDE
    (
        ResourceId,
        CatalogueNodeId,
        ResourceReferenceId,
        ResourceVersionId,
        ResourceTypeId,
        Description,
        DurationInMilliseconds,
        CatalogueName,
        Url,
        BadgeUrl,
        RestrictedAccess,
        AverageRating,
        RatingCount
    );
END;
GO

IF OBJECT_ID('reports.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_Rated'
      AND object_id = OBJECT_ID('reports.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_Rated
    ON reports.DashboardResources (AverageRating DESC, RatingCount DESC, Title)
    INCLUDE
    (
        ResourceId,
        CatalogueNodeId,
        ResourceReferenceId,
        ResourceVersionId,
        ResourceTypeId,
        Description,
        DurationInMilliseconds,
        CatalogueName,
        Url,
        BadgeUrl,
        RestrictedAccess
    )
    WHERE RatingCount > 0;
END;
GO

 --reports.UserResourceActivity


IF OBJECT_ID('reports.UserResourceActivity', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_UserResourceActivity_UserCompletedLatest'
      AND object_id = OBJECT_ID('reports.UserResourceActivity')
)
BEGIN
    CREATE INDEX IX_UserResourceActivity_UserCompletedLatest
    ON reports.UserResourceActivity (UserId, IsCompleted, LatestActivityId DESC)
    INCLUDE (ResourceId, LastAccessedDate);
END;
GO