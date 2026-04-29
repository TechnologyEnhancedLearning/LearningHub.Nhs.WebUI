CREATE TABLE [reports].[DashboardCatalogues]
(
    CatalogueNodeId INT NOT NULL,
    NodeVersionId INT NOT NULL,
    CatalogueNodeVersionId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    BannerUrl NVARCHAR(500) NULL,
    BadgeUrl NVARCHAR(500) NULL,
    CardImageUrl NVARCHAR(500) NULL,
    Url NVARCHAR(500) NULL,
    RestrictedAccess BIT NOT NULL,
    LastShownDate DATETIME2 NULL,
    CatalogueActivityCount INT NOT NULL DEFAULT 0,
    ContributedResourceCount INT NOT NULL DEFAULT 0,
    SumResourceAverageRating DECIMAL(18,2) NULL,
    ProvidersJson NVARCHAR(MAX) NULL,
    CONSTRAINT PK_DashboardCatalogues PRIMARY KEY (CatalogueNodeId)
);