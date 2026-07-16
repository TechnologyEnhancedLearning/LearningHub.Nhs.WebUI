CREATE TABLE [dbo].[DeploymentSmokeTest]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [DeploymentName] NVARCHAR(100) NOT NULL,
    [CreatedDateUtc] DATETIME2(7) NOT NULL CONSTRAINT [DF_DeploymentSmokeTest_CreatedDateUtc] DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_DeploymentSmokeTest] PRIMARY KEY CLUSTERED ([Id] ASC)
);