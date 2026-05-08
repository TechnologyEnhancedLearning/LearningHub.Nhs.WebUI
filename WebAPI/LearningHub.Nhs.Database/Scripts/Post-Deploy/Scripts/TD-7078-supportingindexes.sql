
  -- readmodels.DashboardResources


IF OBJECT_ID('readmodels.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_ResourceId'
      AND object_id = OBJECT_ID('readmodels.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_ResourceId
    ON readmodels.DashboardResources (ResourceId);
END;
GO

IF OBJECT_ID('readmodels.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_Recent'
      AND object_id = OBJECT_ID('readmodels.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_Recent
    ON readmodels.DashboardResources (PublishedDate DESC, Title)
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

IF OBJECT_ID('readmodels.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_Popular'
      AND object_id = OBJECT_ID('readmodels.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_Popular
    ON readmodels.DashboardResources (ActivityCount DESC, Title)
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

IF OBJECT_ID('readmodels.DashboardResources', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardResources_Rated'
      AND object_id = OBJECT_ID('readmodels.DashboardResources')
)
BEGIN
    CREATE INDEX IX_DashboardResources_Rated
    ON readmodels.DashboardResources (AverageRating DESC, RatingCount DESC, Title)
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

 --readmodels.UserResourceActivity


IF OBJECT_ID('readmodels.UserResourceActivity', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_UserResourceActivity_UserCompletedLatest'
      AND object_id = OBJECT_ID('readmodels.UserResourceActivity')
)
BEGIN
    CREATE INDEX IX_UserResourceActivity_UserCompletedLatest
    ON readmodels.UserResourceActivity (UserId, IsCompleted, LatestActivityId DESC)
    INCLUDE (ResourceId, LastAccessedDate);
END;
GO


IF OBJECT_ID('readmodels.DashboardCatalogues', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardCatalogues_Name'
      AND object_id = OBJECT_ID('readmodels.DashboardCatalogues')
)
BEGIN
    CREATE INDEX IX_DashboardCatalogues_Name
    ON readmodels.DashboardCatalogues (Name);
END;
GO

IF OBJECT_ID('readmodels.DashboardCatalogues', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardCatalogues_Popular'
      AND object_id = OBJECT_ID('readmodels.DashboardCatalogues')
)
BEGIN
    CREATE INDEX IX_DashboardCatalogues_Popular
    ON readmodels.DashboardCatalogues (CatalogueActivityCount DESC, Name)
    INCLUDE
    (
        NodeVersionId,
        CatalogueNodeVersionId,
        Description,
        BannerUrl,
        BadgeUrl,
        CardImageUrl,
        Url,
        RestrictedAccess,
        LastShownDate,
        ContributedResourceCount,
        SumResourceAverageRating
    );
END;
GO

IF OBJECT_ID('readmodels.DashboardCatalogues', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardCatalogues_Recent'
      AND object_id = OBJECT_ID('readmodels.DashboardCatalogues')
)
BEGIN
    CREATE INDEX IX_DashboardCatalogues_Recent
    ON readmodels.DashboardCatalogues (LastShownDate DESC, Name)
    INCLUDE
    (
        NodeVersionId,
        CatalogueNodeVersionId,
        Description,
        BannerUrl,
        BadgeUrl,
        CardImageUrl,
        Url,
        RestrictedAccess,
        CatalogueActivityCount,
        ContributedResourceCount,
        SumResourceAverageRating
    )
    WHERE LastShownDate IS NOT NULL;
END;
GO

IF OBJECT_ID('readmodels.DashboardCatalogues', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_DashboardCatalogues_HighlyContributed'
      AND object_id = OBJECT_ID('readmodels.DashboardCatalogues')
)
BEGIN
    CREATE INDEX IX_DashboardCatalogues_HighlyContributed
    ON readmodels.DashboardCatalogues (ContributedResourceCount DESC, SumResourceAverageRating DESC, Name)
    INCLUDE
    (
        NodeVersionId,
        CatalogueNodeVersionId,
        Description,
        BannerUrl,
        BadgeUrl,
        CardImageUrl,
        Url,
        RestrictedAccess,
        LastShownDate,
        CatalogueActivityCount
    );
END;
GO

IF OBJECT_ID('readmodels.UserCatalogueActivity', 'U') IS NOT NULL
AND NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_UserCatalogueActivity_User_Latest'
      AND object_id = OBJECT_ID('readmodels.UserCatalogueActivity')
)
BEGIN
    CREATE INDEX IX_UserCatalogueActivity_User_Latest
    ON readmodels.UserCatalogueActivity (UserId, LatestActivityId DESC)
    INCLUDE (CatalogueNodeId, LastAccessedDate);
END;
GO