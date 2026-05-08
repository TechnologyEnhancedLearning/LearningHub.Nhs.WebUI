CREATE TABLE [readmodels].[UserResourceActivity] (
    UserId INT NOT NULL,
    ResourceId INT NOT NULL,
    LatestActivityId INT NOT NULL,
    IsCompleted BIT NOT NULL,
    LastAccessedDate DATETIME2 NOT NULL,
    CONSTRAINT PK_UserResourceActivity PRIMARY KEY (UserId, ResourceId)
);
GO