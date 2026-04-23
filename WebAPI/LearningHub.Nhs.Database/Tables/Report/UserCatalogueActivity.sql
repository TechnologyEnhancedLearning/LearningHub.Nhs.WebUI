CREATE TABLE [reports].[UserCatalogueActivity]
(
    UserId INT NOT NULL,
    CatalogueNodeId INT NOT NULL,
    LatestActivityId INT NOT NULL,
    LastAccessedDate DATETIME2 NOT NULL,
    CONSTRAINT PK_UserCatalogueActivity PRIMARY KEY (UserId, CatalogueNodeId)
);
