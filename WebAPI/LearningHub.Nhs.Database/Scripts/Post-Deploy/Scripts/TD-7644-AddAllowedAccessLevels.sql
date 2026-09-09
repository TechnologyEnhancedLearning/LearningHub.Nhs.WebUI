IF NOT EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE Name = N'AllowedAccessLevels'
      AND Object_ID = Object_ID(N'dbo.MoodleInstanceConfigs')
)
BEGIN
    ALTER TABLE dbo.MoodleInstanceConfigs
    ADD AllowedAccessLevels nvarchar(1000) NULL;
END
