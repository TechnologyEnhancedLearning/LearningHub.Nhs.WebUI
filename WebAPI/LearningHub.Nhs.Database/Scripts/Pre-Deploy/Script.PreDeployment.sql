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
-- =========================================
-- Pre-deployment: Disable Change Tracking for all tables
-- =========================================

DECLARE @schemaName NVARCHAR(128), @tableName NVARCHAR(128);
DECLARE ct_cursor CURSOR FOR
SELECT s.name AS schema_name, t.name AS table_name
FROM sys.change_tracking_tables ct
JOIN sys.tables t ON ct.object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id;

OPEN ct_cursor;
FETCH NEXT FROM ct_cursor INTO @schemaName, @tableName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Disabling Change Tracking for table: ' + @schemaName + '.' + @tableName;
    EXEC('ALTER TABLE [' + @schemaName + '].[' + @tableName + '] DISABLE CHANGE_TRACKING;');

    FETCH NEXT FROM ct_cursor INTO @schemaName, @tableName;
END

CLOSE ct_cursor;
DEALLOCATE ct_cursor;

PRINT 'Change Tracking has been disabled for all tables.';


-- 2️⃣ Disable Change Tracking if enabled
IF EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id = DB_ID())
BEGIN
    PRINT 'Change Tracking is enabled. Disabling Change Tracking for the database...';
    ALTER DATABASE CURRENT
    SET CHANGE_TRACKING = OFF;
    PRINT 'Change Tracking has been disabled.';
END
ELSE
BEGIN
    PRINT 'Change Tracking is not enabled on this database.';
END

PRINT 'Change Tracking disable completed.';
