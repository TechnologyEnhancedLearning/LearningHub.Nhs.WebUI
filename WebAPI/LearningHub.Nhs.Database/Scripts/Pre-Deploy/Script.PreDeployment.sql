/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- TD-2902 - IF the [Title] column in [Content].[PageSectionDetail] has not been removed then raise error, script needs to be run manually
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'resources'
                  AND TABLE_NAME = 'ResourceReferenceEvent'))
BEGIN
    RAISERROR (N'TD-2902 Add resource types to Content Server.sql must be run manually before release.', 16, 127) WITH NOWAIT
END
GO
PRINT '*** PRE-DEPLOYMENT: Disable Change Tracking STARTED ***';
GO

-- =========================================
-- Disable Change Tracking on all tables
-- =========================================
DECLARE @schemaName SYSNAME, @tableName SYSNAME;
DECLARE @sql NVARCHAR(MAX);

DECLARE ct_cursor CURSOR FAST_FORWARD FOR
SELECT s.name, t.name
FROM sys.change_tracking_tables ct
JOIN sys.tables t ON ct.object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id;

OPEN ct_cursor;
FETCH NEXT FROM ct_cursor INTO @schemaName, @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'ALTER TABLE [' + @schemaName + '].[' + @tableName + '] DISABLE CHANGE_TRACKING;';
    PRINT 'Disabling Change Tracking for table: ' + @schemaName + '.' + @tableName;
    EXEC (@sql);

    FETCH NEXT FROM ct_cursor INTO @schemaName, @tableName;
END

CLOSE ct_cursor;
DEALLOCATE ct_cursor;
GO

PRINT 'Table-level Change Tracking disabled.';
GO

-- =========================================
-- Disable Change Tracking at database level
-- MUST be in its own batch
-- =========================================
IF EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID())
BEGIN
    PRINT 'Disabling Change Tracking at database level...';
    ALTER DATABASE CURRENT SET CHANGE_TRACKING = OFF;
END
GO

PRINT '*** PRE-DEPLOYMENT: Disable Change Tracking COMPLETED ***';
GO
