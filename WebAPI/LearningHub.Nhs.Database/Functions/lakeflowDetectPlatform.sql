-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowDetectPlatform] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[lakeflowDetectPlatform]()
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @engineEdition INT = CAST(SERVERPROPERTY('EngineEdition') AS INT);
    DECLARE @serverName NVARCHAR(255) = @@SERVERNAME;
    DECLARE @platform NVARCHAR(50);

    IF @engineEdition = 5
        SET @platform = 'AZURE_SQL_DATABASE';
    ELSE IF @engineEdition = 8
        SET @platform = 'AZURE_SQL_MANAGED_INSTANCE';
    ELSE IF @serverName LIKE '%.rds.amazonaws.com'
        SET @platform = 'AMAZON_RDS';
    ELSE IF @engineEdition IN (1, 2, 3, 4)
        SET @platform = 'ON_PREMISES';
    ELSE
        SET @platform = 'UNKNOWN';

    RETURN @platform;
END