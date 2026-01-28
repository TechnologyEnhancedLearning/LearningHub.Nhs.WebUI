-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowFixPermissions] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[lakeflowFixPermissions]
    @User NVARCHAR(128),
    @Tables NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DatabaseUser NVARCHAR(128) = @User;
    DECLARE @Platform NVARCHAR(50) = dbo.lakeflowDetectPlatform();
    DECLARE @CatalogName NVARCHAR(128) = DB_NAME();
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @CurrentObject NVARCHAR(255);

    -- Error codes and messages
    DECLARE @invalidModeErrorCode INT = 100000;
    DECLARE @insufficientUserPrivilegesCode INT = 100400;
    DECLARE @insufficientUserPrivilegesErrorMessage NVARCHAR(200);

    SET @insufficientUserPrivilegesErrorMessage = 'User executing this script is not a ''db_owner'' role member. To execute this script, please use a user that is.';

    PRINT N'Starting permission fixes for: ' + @CatalogName;
    PRINT N'Platform: ' + @Platform;
    PRINT N'User: ' + @User;
    IF @Tables IS NOT NULL
        PRINT N'Tables parameter: ' + @Tables;

    BEGIN TRY
        -- Validate that current user is db_owner
        IF (IS_ROLEMEMBER('db_owner') = 0)
        BEGIN
            THROW @insufficientUserPrivilegesCode, @insufficientUserPrivilegesErrorMessage, 1;
        END

        -- User resolution
        IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @User)
        BEGIN
            -- Check if user exists as database user
            SELECT @DatabaseUser = dp.name
            FROM sys.database_principals dp
            INNER JOIN sys.server_principals sp ON dp.sid = sp.sid
            WHERE sp.name = @User
                AND dp.type IN ('S', 'U', 'G')
                AND dp.name NOT IN ('guest');

            -- If still no database user found, warn and skip
            IF @DatabaseUser IS NULL OR @DatabaseUser = @User
            BEGIN
                PRINT N'? Warning: User/Login [' + @User + '] not found as database user. Skipping permission grants.';
                PRINT N'  To fix: CREATE USER [' + @User + '] FOR LOGIN [' + @User + '];';
                RETURN;
            END
            ELSE
            BEGIN
                PRINT N'Server login [' + @User + '] maps to database user [' + @DatabaseUser + '].';
            END
        END

        IF @DatabaseUser = 'dbo'
        BEGIN
            PRINT N'Skipping permission grants (dbo already has all permissions).';
            PRINT N'Permission setup completed for user: ' + @User;
            RETURN;
        END

        -- Grant SELECT permissions on required system views and tables
        DECLARE @SystemObjects TABLE (ObjectName NVARCHAR(255), IsServerScoped BIT);
        INSERT INTO @SystemObjects VALUES
            ('sys.objects', 0), ('sys.schemas', 0), ('sys.tables', 0), ('sys.columns', 0),
            ('sys.key_constraints', 0), ('sys.foreign_keys', 0), ('sys.check_constraints', 0),
            ('sys.default_constraints', 0), ('sys.triggers', 0), ('sys.indexes', 0),
            ('sys.index_columns', 0), ('sys.fulltext_index_columns', 0), ('sys.fulltext_indexes', 0),
            ('sys.change_tracking_databases', 1), ('sys.change_tracking_tables', 0),
            ('cdc.change_tables', 0), ('cdc.captured_columns', 0), ('cdc.index_columns', 0);

        -- Grant EXECUTE permissions on required system stored procedures (all server-scoped)
        DECLARE @SystemProcedures TABLE (ProcedureName NVARCHAR(255));
        INSERT INTO @SystemProcedures VALUES
            ('sp_tables'), ('sp_columns_100'), ('sp_pkeys'), ('sp_statistics_100');

        PRINT N'';
        PRINT N'=== System Object Permissions ===';

        DECLARE sys_cursor CURSOR FOR
            SELECT ObjectName FROM @SystemObjects
            WHERE IsServerScoped = 0 OR @Platform NOT IN ('AZURE_SQL_DATABASE');

        OPEN sys_cursor;
        FETCH NEXT FROM sys_cursor INTO @CurrentObject;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            BEGIN TRY
                -- Check if object exists before trying to grant (helps with CDC objects)
                IF @CurrentObject LIKE 'cdc.%'
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'cdc')
                    BEGIN
                        PRINT N'? Skipping ' + @CurrentObject + ' (CDC not enabled)';
                        FETCH NEXT FROM sys_cursor INTO @CurrentObject;
                        CONTINUE;
                    END
                END

                SET @SQL = 'GRANT SELECT ON ' + @CurrentObject + ' TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Granted SELECT on ' + @CurrentObject;
            END TRY
            BEGIN CATCH
                PRINT N'? Could not grant SELECT on ' + @CurrentObject + ': ' + ERROR_MESSAGE();
            END CATCH

            FETCH NEXT FROM sys_cursor INTO @CurrentObject;
        END

        CLOSE sys_cursor;
        DEALLOCATE sys_cursor;

        -- Grant EXECUTE permissions on system procedures (skip for Azure SQL Database - implicit access)
        IF @Platform NOT IN ('AZURE_SQL_DATABASE')
        BEGIN
            DECLARE proc_cursor CURSOR FOR
                SELECT ProcedureName FROM @SystemProcedures;

            OPEN proc_cursor;
            FETCH NEXT FROM proc_cursor INTO @CurrentObject;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY
                    SET @SQL = 'GRANT EXECUTE ON ' + @CurrentObject + ' TO [' + @DatabaseUser + ']';
                    EXEC sp_executesql @SQL;
                    PRINT N'? Granted EXECUTE on ' + @CurrentObject;
                END TRY
                BEGIN CATCH
                    PRINT N'? Could not grant EXECUTE on ' + @CurrentObject + ': ' + ERROR_MESSAGE();
                END CATCH

                FETCH NEXT FROM proc_cursor INTO @CurrentObject;
            END

            CLOSE proc_cursor;
            DEALLOCATE proc_cursor;
        END
        ELSE
        BEGIN
            PRINT N'? Skipping system stored procedure permissions on Azure SQL Database';
            PRINT N'  Database users have implicit EXECUTE access to system stored procedures';
        END

        -- Handle table-specific permissions if @Tables parameter is provided
        IF @Tables IS NOT NULL
        BEGIN
            PRINT N'';
            PRINT N'=== Table-Level SELECT Permissions ===';

            DECLARE @TargetTables TABLE (
                SchemaName NVARCHAR(128),
                TableName NVARCHAR(128),
                FullName NVARCHAR(261),
                ObjectId INT
            );

            -- Table discovery logic
            IF @Tables = 'ALL'
            BEGIN
                PRINT N'Discovering all user tables in database...';
                INSERT INTO @TargetTables (SchemaName, TableName, FullName, ObjectId)
                SELECT
                    s.name, t.name,
                    QUOTENAME(s.name) + '.' + QUOTENAME(t.name),
                    t.object_id
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.type = 'U'
                    AND s.name NOT IN ('sys', 'information_schema', 'cdc', 'INFORMATION_SCHEMA', 'guest');
            END
            ELSE IF @Tables LIKE 'SCHEMAS:%'
            BEGIN
                DECLARE @SchemaList NVARCHAR(MAX) = SUBSTRING(@Tables, 9, LEN(@Tables));
                PRINT N'Discovering tables in schemas: ' + @SchemaList;
                DECLARE @SchemaXML XML;
                SET @SchemaXML = CAST('<schema>' + REPLACE(@SchemaList, ',', '</schema><schema>') + '</schema>' AS XML);
                INSERT INTO @TargetTables (SchemaName, TableName, FullName, ObjectId)
                SELECT
                    s.name, t.name,
                    QUOTENAME(s.name) + '.' + QUOTENAME(t.name),
                    t.object_id
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.type = 'U'
                    AND s.name IN (
                        SELECT LTRIM(RTRIM(x.value('(./text())[1]', 'NVARCHAR(MAX)')))
                        FROM @SchemaXML.nodes('/schema') AS T(x)
                        WHERE LTRIM(RTRIM(x.value('(./text())[1]', 'NVARCHAR(MAX)'))) != ''
                    );
            END
            ELSE
            BEGIN
                PRINT N'Processing specified tables: ' + @Tables;
                DECLARE @TableList TABLE (FullTableName NVARCHAR(261));
                INSERT INTO @TableList (FullTableName)
                SELECT LTRIM(RTRIM(Split.a.value('.', 'NVARCHAR(MAX)'))) AS value
                FROM (
                    SELECT CAST('<M>' + REPLACE(@Tables, ',', '</M><M>') + '</M>' AS XML) AS Data
                ) AS A
                CROSS APPLY Data.nodes('/M') AS Split(a)
                WHERE LTRIM(RTRIM(Split.a.value('.', 'NVARCHAR(MAX)'))) != '';

                INSERT INTO @TargetTables (SchemaName, TableName, FullName, ObjectId)
                SELECT
                    s.name, t.name,
                    QUOTENAME(s.name) + '.' + QUOTENAME(t.name),
                    t.object_id
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                INNER JOIN @TableList tl ON
                    (tl.FullTableName = s.name + '.*' OR
                     tl.FullTableName = s.name + '.' + t.name OR
                     (CHARINDEX('.', tl.FullTableName) = 0 AND tl.FullTableName = t.name AND s.name = 'dbo'))
                WHERE t.type = 'U';
            END

            -- Grant SELECT permissions on discovered tables
            DECLARE @ProcessedCount INT = 0, @ErrorCount INT = 0;
            DECLARE @CurrentSchema NVARCHAR(128), @CurrentTable NVARCHAR(128), @CurrentFullName NVARCHAR(261);

            DECLARE table_cursor CURSOR FOR
                SELECT SchemaName, TableName, FullName FROM @TargetTables ORDER BY SchemaName, TableName;

            OPEN table_cursor;
            FETCH NEXT FROM table_cursor INTO @CurrentSchema, @CurrentTable, @CurrentFullName;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY
                    SET @SQL = N'GRANT SELECT ON ' + @CurrentFullName + ' TO [' + @DatabaseUser + ']';
                    EXEC sp_executesql @SQL;
                    PRINT N'? Granted SELECT on ' + @CurrentFullName;
                    SET @ProcessedCount = @ProcessedCount + 1;
                END TRY
                BEGIN CATCH
                    PRINT N'? Error granting SELECT on ' + @CurrentFullName + ': ' + ERROR_MESSAGE();
                    SET @ErrorCount = @ErrorCount + 1;
                END CATCH

                FETCH NEXT FROM table_cursor INTO @CurrentSchema, @CurrentTable, @CurrentFullName;
            END

            CLOSE table_cursor;
            DEALLOCATE table_cursor;

            -- Summary for table permissions
            PRINT N'';
            PRINT N'Table permission summary:';
            PRINT N'  - Tables processed: ' + CAST(@ProcessedCount AS NVARCHAR(10));
            PRINT N'  - Tables with errors: ' + CAST(@ErrorCount AS NVARCHAR(10));
        END

        PRINT N'';
        PRINT N'Permission fixes completed for user: ' + @User;

        -- Platform-specific guidance
        IF @Platform = 'AZURE_SQL_DATABASE'
        BEGIN
            PRINT N'';
            PRINT N'=== Azure SQL Database Platform Notes ===';
            PRINT N'• System stored procedures: Accessible by default to database users (no grants needed)';
            PRINT N'• Server-scoped catalog views: Limited access in Azure SQL Database';
            PRINT N'• Consider granting db_datareader role for broader access';
            PRINT N'• CDC objects are only available when CDC is enabled on the database';
            PRINT N'';
            PRINT N'=== Recommended Additional Access ===';
            PRINT N'-- Grant broader database-level access for comprehensive permissions:';
            PRINT N'USE [' + @CatalogName + '];';
            PRINT N'ALTER ROLE db_datareader ADD MEMBER [' + @DatabaseUser + '];';
            PRINT N'';
            PRINT N'=== Server-Scoped Limitations ===';
            PRINT N'• sys.change_tracking_databases: Requires server-level access (typically not available)';
            PRINT N'• Most Azure SQL Database deployments cannot grant server-level permissions';
            PRINT N'• Contact your Azure administrator if server-level access is specifically required';
        END
        ELSE IF @Platform = 'AZURE_SQL_MANAGED_INSTANCE'
        BEGIN
            PRINT N'';
            PRINT N'=== Azure SQL Managed Instance Platform Notes ===';
            PRINT N'• Most permissions granted successfully at database level';
            PRINT N'• If server-scoped permissions are needed, connect to master:';
            PRINT N'USE master;';
            PRINT N'GRANT SELECT ON sys.change_tracking_databases TO [' + @DatabaseUser + '];';
        END
        ELSE
        BEGIN
            PRINT N'';
            PRINT N'=== Platform Notes ===';
            PRINT N'• All permissions granted successfully at database level';
            PRINT N'• No additional server-level configuration required';
        END

    END TRY
    BEGIN CATCH
        SET @ErrorMessage = 'Error in lakeflowFixPermissions: ' + ERROR_MESSAGE();
        PRINT @ErrorMessage;
        THROW;
    END CATCH
END
GO
