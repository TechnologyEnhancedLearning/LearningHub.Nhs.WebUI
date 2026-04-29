CREATE TABLE [reports].[DashboardResources] (
    ResourceId INT NOT NULL,
    ResourceReferenceId INT NULL,
    ResourceVersionId INT NOT NULL,
    ResourceTypeId INT NOT NULL,
    Title NVARCHAR(500) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    DurationInMilliseconds INT NULL,
    CatalogueNodeId INT NOT NULL,
    CatalogueName NVARCHAR(255) NULL,
    Url NVARCHAR(500) NULL,
    BadgeUrl NVARCHAR(500) NULL,
    RestrictedAccess BIT NOT NULL,
    PublishedDate DATETIME2 NOT NULL,
    AverageRating DECIMAL(3,2) NULL,
    RatingCount INT NULL,
    ActivityCount INT NOT NULL DEFAULT 0,
    ProvidersJson NVARCHAR(MAX) NULL,
    CONSTRAINT PK_DashboardResources PRIMARY KEY (ResourceId, CatalogueNodeId)
);
GO