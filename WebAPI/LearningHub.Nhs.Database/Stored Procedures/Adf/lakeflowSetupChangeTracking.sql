-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowSetupChangeTracking] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[lakeflowSetupChangeTracking]
    @Tables NVARCHAR(MAX) = NULL,
    @User NVARCHAR(128) = NULL,
    @Retention NVARCHAR(50) = '2 DAYS',
    @Mode NVARCHAR(10) = 'INSTALL'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DatabaseUser NVARCHAR(128) = @User;
    DECLARE @Platform NVARCHAR(50) = dbo.lakeflowDetectPlatform();
    DECLARE @CatalogName NVARCHAR(128) = DB_NAME();
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @SQL NVARCHAR(MAX);
    DECLARE @versionSuffix NVARCHAR(10) = '_1_2';
    DECLARE @ddlAuditTableName NVARCHAR(100) = 'lakeflowDdlAudit' + @versionSuffix;
    DECLARE @ddlAuditTriggerName NVARCHAR(100) = 'lakeflowDdlAuditTrigger' + @versionSuffix;

    -- Error codes and messages
    DECLARE @invalidModeErrorCode INT = 100000;
    DECLARE @invalidModeErrorMessage NVARCHAR(200);
    DECLARE @insufficientUserPrivilegesCode INT = 100400;
    DECLARE @insufficientUserPrivilegesErrorMessage NVARCHAR(200);

    SET @invalidModeErrorMessage = CONCAT('Provided execution mode: ', @Mode, ', is not recognized. Allowed values are: INSTALL, CLEANUP');
    SET @insufficientUserPrivilegesErrorMessage = 'User executing this script is not a ''db_owner'' role member. To execute this script, please use a user that is.';

    PRINT N'Starting change tracking setup for: ' + @CatalogName;
    PRINT N'Platform: ' + @Platform;
    PRINT N'Mode: ' + @Mode;
    IF @Tables IS NOT NULL
        PRINT N'Tables: ' + @Tables;

    BEGIN TRY
        -- Validate execution mode
        IF (@Mode != 'INSTALL' AND @Mode != 'CLEANUP')
        BEGIN
            THROW @invalidModeErrorCode, @invalidModeErrorMessage, 1;
        END

        -- Validate that current user is db_owner
        IF (IS_ROLEMEMBER('db_owner') = 0)
        BEGIN
            THROW @insufficientUserPrivilegesCode, @insufficientUserPrivilegesErrorMessage, 1;
        END

        -- Cleanup legacy DDL support objects
        IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'replicate_io_audit_ddl_trigger_1' AND parent_class = 0)
            OR OBJECT_ID('dbo.replicate_io_audit_ddl_1', 'U') IS NOT NULL
            OR OBJECT_ID('dbo.replicate_io_audit_tbl_cons_1', 'U') IS NOT NULL
            OR OBJECT_ID('dbo.replicate_io_audit_tbl_schema_1', 'U') IS NOT NULL
            OR EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'alterTableTrigger_1' AND parent_class = 0)
            OR OBJECT_ID('dbo.disableOldCaptureInstance_1', 'P') IS NOT NULL
            OR OBJECT_ID('dbo.refreshCaptureInstance_1', 'P') IS NOT NULL
            OR OBJECT_ID('dbo.mergeCaptureInstance_1', 'P') IS NOT NULL
            OR OBJECT_ID('dbo.captureInstanceTracker_1', 'U') IS NOT NULL
        BEGIN
            PRINT N'Cleaning up legacy DDL support objects...';

            IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'replicate_io_audit_ddl_trigger_1' AND parent_class = 0)
            BEGIN
                EXEC('DROP TRIGGER replicate_io_audit_ddl_trigger_1 ON DATABASE');
                PRINT N'? Dropped legacy trigger: replicate_io_audit_ddl_trigger_1';
            END

            IF OBJECT_ID('dbo.replicate_io_audit_ddl_1', 'U') IS NOT NULL
            BEGIN
                EXEC('DROP TABLE dbo.replicate_io_audit_ddl_1');
                PRINT N'? Dropped legacy table: replicate_io_audit_ddl_1';
            END

            IF OBJECT_ID('dbo.replicate_io_audit_tbl_cons_1', 'U') IS NOT NULL
            BEGIN
                EXEC('DROP TABLE dbo.replicate_io_audit_tbl_cons_1');
                PRINT N'? Dropped legacy table: replicate_io_audit_tbl_cons_1';
            END

            IF OBJECT_ID('dbo.replicate_io_audit_tbl_schema_1', 'U') IS NOT NULL
            BEGIN
                EXEC('DROP TABLE dbo.replicate_io_audit_tbl_schema_1');
                PRINT N'? Dropped legacy table: replicate_io_audit_tbl_schema_1';
            END

            IF EXISTS (SELECT name FROM sys.triggers WHERE name = 'alterTableTrigger_1' AND type = 'TR')
            BEGIN
                EXEC('DROP TRIGGER alterTableTrigger_1 ON DATABASE');
                PRINT N'? Dropped legacy trigger: alterTableTrigger_1';
            END

            IF OBJECT_ID('dbo.disableOldCaptureInstance_1', 'P') IS NOT NULL
            BEGIN
                EXEC('DROP PROCEDURE dbo.disableOldCaptureInstance_1');
                PRINT N'? Dropped legacy procedure: disableOldCaptureInstance_1';
            END

            IF OBJECT_ID('dbo.refreshCaptureInstance_1', 'P') IS NOT NULL
            BEGIN
                EXEC('DROP PROCEDURE dbo.refreshCaptureInstance_1');
                PRINT N'? Dropped legacy procedure: refreshCaptureInstance_1';
            END

            IF OBJECT_ID('dbo.mergeCaptureInstance_1', 'P') IS NOT NULL
            BEGIN
                EXEC('DROP PROCEDURE dbo.mergeCaptureInstance_1');
                PRINT N'? Dropped legacy procedure: mergeCaptureInstance_1';
            END

            IF OBJECT_ID('dbo.captureInstanceTracker_1', 'U') IS NOT NULL
            BEGIN
                EXEC('DROP TABLE dbo.captureInstanceTracker_1');
                PRINT N'? Dropped legacy table: captureInstanceTracker_1';
            END

            PRINT N'Legacy DDL support objects cleanup completed';
        END

        -- Cleanup mode: Remove DDL support objects
        IF @Mode = 'CLEANUP'
        BEGIN
            PRINT N'Cleaning up CT DDL support objects...';

            -- Drop DDL audit trigger
            IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = @ddlAuditTriggerName AND parent_class = 0)
            BEGIN
                SET @SQL = N'DROP TRIGGER [' + @ddlAuditTriggerName + '] ON DATABASE';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped trigger: ' + @ddlAuditTriggerName;
            END

            -- Drop DDL audit table
            IF OBJECT_ID('dbo.' + @ddlAuditTableName, 'U') IS NOT NULL
            BEGIN
                SET @SQL = N'DROP TABLE [dbo].[' + @ddlAuditTableName + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped table: ' + @ddlAuditTableName;
            END

            -- Pattern-based cleanup for any remaining CT objects across versions
            DECLARE @ctCleanupSql NVARCHAR(MAX) = '';

            -- Clean up any remaining DDL audit tables across versions (excluding current and legacy)
            SELECT @ctCleanupSql = @ctCleanupSql + 'DROP TABLE [dbo].[' + name + '];' + CHAR(13)
            FROM sys.tables
            WHERE name LIKE 'lakeflowDdlAudit_%_%'
              AND name != @ddlAuditTableName;

            IF LEN(@ctCleanupSql) > 0
            BEGIN
                EXEC sp_executesql @ctCleanupSql;
                PRINT N'? Cleaned up remaining DDL audit tables across versions';
            END

            -- Clean up any remaining DDL audit triggers across versions (excluding current and legacy)
            SET @ctCleanupSql = '';
            SELECT @ctCleanupSql = @ctCleanupSql + 'DROP TRIGGER [' + name + '] ON DATABASE;' + CHAR(13)
            FROM sys.triggers
            WHERE name LIKE 'lakeflowDdlAuditTrigger_%_%'
              AND name != @ddlAuditTriggerName
              AND parent_class = 0;

            IF LEN(@ctCleanupSql) > 0
            BEGIN
                EXEC sp_executesql @ctCleanupSql;
                PRINT N'? Cleaned up remaining DDL audit triggers across versions';
            END

            PRINT N'CT DDL support objects cleanup completed';
            RETURN;
        END

        -- Install mode continues here
        PRINT N'Setting up change tracking infrastructure...';

        -- Check if change tracking is enabled at database level
        IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_databases ctd
                       INNER JOIN sys.databases d ON ctd.database_id = d.database_id
                       WHERE d.name = DB_NAME())
        BEGIN
            PRINT N'Enabling change tracking at database level...';
            SET @SQL = N'ALTER DATABASE ' + QUOTENAME(@CatalogName) + ' SET CHANGE_TRACKING = ON (CHANGE_RETENTION = ' + @Retention + ', AUTO_CLEANUP = ON)';
            EXEC sp_executesql @SQL;
            PRINT N'? Change tracking enabled at database level';
        END
        ELSE
        BEGIN
            PRINT N'? Change tracking already enabled at database level';
        END

        -- Create DDL audit table if it does not exist
        IF OBJECT_ID('dbo.' + @ddlAuditTableName, 'U') IS NULL
        BEGIN
            SET @sql = N'CREATE TABLE [dbo].[' + @ddlAuditTableName + '](
                [SERIAL_NUMBER] INT IDENTITY NOT NULL,
                [CURRENT_USER] NVARCHAR(128) NULL,
                [SCHEMA_NAME] NVARCHAR(128) NULL,
                [TABLE_NAME] NVARCHAR(128) NULL,
                [TYPE] NVARCHAR(30) NULL,
                [OPERATION_TYPE] NVARCHAR(30) NULL,
                [SQL_TXT] NVARCHAR(2000) NULL,
                [LOGICAL_POSITION] BIGINT NOT NULL,
                CONSTRAINT [replicantDdlAuditPrimaryKey_' + @versionSuffix + '] PRIMARY KEY ([SERIAL_NUMBER], [LOGICAL_POSITION]))';
            EXEC sp_executesql @sql;
            PRINT N'? Created DDL audit table: ' + @ddlAuditTableName;

            -- Enable change tracking on DDL audit table
            SET @sql = N'ALTER TABLE [dbo].[' + @ddlAuditTableName + '] ENABLE CHANGE_TRACKING';
            EXEC sp_executesql @sql;
            PRINT N'? Enabled change tracking on DDL audit table';
        END

        -- Create DDL audit trigger
        IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE name = @ddlAuditTriggerName)
        BEGIN
            DECLARE @QuotedDbName NVARCHAR(255) = QUOTENAME(DB_NAME());
            SET @sql = N'CREATE TRIGGER [' + @ddlAuditTriggerName + '] ON DATABASE
                FOR ALTER_TABLE
                AS
                SET NOCOUNT ON;
                DECLARE @DbName NVARCHAR(255),
                        @SchemaName NVARCHAR(max),
                        @TableName NVARCHAR(255),
                        @QuotedFullName NVARCHAR(max),
                        @objectType NVARCHAR(30),
                        @data XML,
                        @changeVersion NVARCHAR(30),
                        @operation NVARCHAR(30),
                        @capturedSql NVARCHAR(2000),
                        @isCTEnabledDBLevel bit,
                        @isCTEnabledTableLevel bit,
                        @isColumnAdd nvarchar(255),
                        @isAlterColumn nvarchar(255),
                        @isDropColumn nvarchar(255);

                    SET @data = EVENTDATA();
                    SET @changeVersion = CHANGE_TRACKING_CURRENT_VERSION();
                    SET @DbName = DB_NAME();
                    SET @SchemaName = @data.value(''(/EVENT_INSTANCE/SchemaName)[1]'',  ''NVARCHAR(MAX)'');
                    SET @TableName = @data.value(''(/EVENT_INSTANCE/ObjectName)[1]'',  ''NVARCHAR(255)'');
                    SET @objectType = @data.value(''(/EVENT_INSTANCE/ObjectType)[1]'', ''NVARCHAR(30)'');
                    SET @QuotedFullName = QUOTENAME(@SchemaName) + ''.'' + QUOTENAME(@TableName);
                    SET @operation = @data.value(''(/EVENT_INSTANCE/EventType)[1]'', ''NVARCHAR(30)'');
                    SET @capturedSql = @data.value(''(/EVENT_INSTANCE/TSQLCommand/CommandText)[1]'', ''NVARCHAR(2000)'');
                    SET @isCTEnabledDBLevel = (SELECT COUNT(*) FROM sys.change_tracking_databases ctd
                                                INNER JOIN sys.databases d ON ctd.database_id = d.database_id
                                                WHERE d.name = @DbName);
                    SET @isCTEnabledTableLevel = (SELECT COUNT(*) FROM sys.change_tracking_tables WHERE object_id = object_id(@QuotedFullName));
                    SET @isColumnAdd = @data.value(''(/EVENT_INSTANCE/AlterTableActionList/Create)[1]'', ''NVARCHAR(255)'');
                    SET @isAlterColumn = @data.value(''(/EVENT_INSTANCE/AlterTableActionList/Alter)[1]'', ''NVARCHAR(255)'');
                    SET @isDropColumn = @data.value(''(/EVENT_INSTANCE/AlterTableActionList/Drop)[1]'', ''NVARCHAR(255)'');

                IF ((@isCTEnabledDBLevel = 1 AND @isCTEnabledTableLevel = 1) AND ((@isColumnAdd IS NOT NULL) OR (@isAlterColumn IS NOT NULL) OR (@isDropColumn IS NOT NULL)))
                BEGIN
                    INSERT INTO ' + @QuotedDbName + '.dbo.[' + @ddlAuditTableName + '] (
                        [CURRENT_USER],
                        [SCHEMA_NAME],
                        [TABLE_NAME],
                        [TYPE],
                        [OPERATION_TYPE],
                        [SQL_TXT],
                        [LOGICAL_POSITION]
                    )
                    VALUES (
                        SUSER_NAME(),
                        @SchemaName,
                        @TableName,
                        @objectType,
                        @operation,
                        @capturedSql,
                        @changeVersion
                    );
                END';
            EXEC sp_executesql @sql;
            PRINT N'? Created DDL audit trigger: ' + @ddlAuditTriggerName;
        END

        -- User resolution
        IF @User IS NOT NULL AND @User != ''
        BEGIN
            -- Check if user exists as database user
            IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @User)
            BEGIN
                -- Check if it is a server login and find its mapped database user
                SELECT @DatabaseUser = dp.name
                FROM sys.database_principals dp
                INNER JOIN sys.server_principals sp ON dp.sid = sp.sid
                WHERE sp.name = @User
                    AND dp.type IN ('S', 'U', 'G')
                    AND dp.name NOT IN ('guest');

                -- If still no database user found, warn
                IF @DatabaseUser IS NULL OR @DatabaseUser = @User
                BEGIN
                    PRINT N'? Warning: User/Login [' + @User + '] not found as database user. Skipping permission grants.';
                    PRINT N'  To fix: CREATE USER [' + @User + '] FOR LOGIN [' + @User + '];';
                    SET @DatabaseUser = NULL;
                END
                ELSE
                BEGIN
                    PRINT N'Server login [' + @User + '] maps to database user [' + @DatabaseUser + '].';
                END
            END

            IF @DatabaseUser = 'dbo'
            BEGIN
                PRINT N'Skipping permission grants (dbo already has all permissions).';
                SET @DatabaseUser = NULL;
            END
        END

        -- Grant permissions to user if specified
        IF @DatabaseUser IS NOT NULL
        BEGIN
            PRINT N'Granting permissions to user: ' + @DatabaseUser;

            -- Grant SELECT on DDL audit table
            SET @SQL = N'GRANT SELECT ON [dbo].[' + @ddlAuditTableName + '] TO ' + QUOTENAME(@DatabaseUser);
            EXEC sp_executesql @SQL;
            PRINT N'? Granted SELECT on ' + @ddlAuditTableName + ' to ' + @DatabaseUser;

            -- Grant VIEW CHANGE TRACKING on DDL audit table
            SET @SQL = N'GRANT VIEW CHANGE TRACKING ON [dbo].[' + @ddlAuditTableName + '] TO ' + QUOTENAME(@DatabaseUser);
            EXEC sp_executesql @SQL;
            PRINT N'? Granted VIEW CHANGE TRACKING on ' + @ddlAuditTableName + ' to ' + @DatabaseUser;

            -- Grant VIEW DEFINITION to see database-level triggers
            SET @SQL = N'GRANT VIEW DEFINITION TO ' + QUOTENAME(@DatabaseUser);
            EXEC sp_executesql @SQL;
            PRINT N'? Granted VIEW DEFINITION to ' + @DatabaseUser;
        END

        -- Process tables if specified
        IF @Tables IS NOT NULL
        BEGIN
            PRINT N'Processing tables for change tracking enablement...';

            -- Declare variables for table processing
            DECLARE @TargetTables TABLE (
                SchemaName NVARCHAR(128),
                TableName NVARCHAR(128),
                HasPrimaryKey BIT
            );

            DECLARE @SkippedTables NVARCHAR(MAX) = '';
            DECLARE @SkippedTablesCount INT = 0;
            DECLARE @CurrentSchema NVARCHAR(128);
            DECLARE @CurrentTable NVARCHAR(128);
            DECLARE @ProcessedCount INT = 0;
            DECLARE @SkippedCount INT = 0;
            DECLARE @ErrorCount INT = 0;

            -- Parse table list and populate target tables
            IF @Tables = 'ALL'
            BEGIN
                INSERT INTO @TargetTables (SchemaName, TableName, HasPrimaryKey)
                SELECT
                    s.name,
                    t.name,
                    CASE WHEN EXISTS (
                        SELECT 1 FROM sys.key_constraints kc
                        WHERE kc.parent_object_id = t.object_id
                        AND kc.type = 'PK'
                    ) THEN 1 ELSE 0 END
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0;
            END
            ELSE IF @Tables LIKE 'SCHEMAS:%'
            BEGIN
                DECLARE @SchemaList NVARCHAR(MAX) = SUBSTRING(@Tables, 9, LEN(@Tables));
                INSERT INTO @TargetTables (SchemaName, TableName, HasPrimaryKey)
                SELECT
                    s.name, t.name,
                    CASE WHEN pk.CONSTRAINT_NAME IS NOT NULL THEN 1 ELSE 0 END
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk ON
                    pk.TABLE_SCHEMA = s.name AND pk.TABLE_NAME = t.name AND pk.CONSTRAINT_TYPE = 'PRIMARY KEY'
                WHERE t.type = 'U'
                    AND s.name IN (SELECT LTRIM(RTRIM(Split.a.value('.', 'NVARCHAR(MAX)'))) AS value
                FROM (
                    SELECT CAST('<M>' + REPLACE(@SchemaList, ',', '</M><M>') + '</M>' AS XML) AS Data
                ) AS A
                CROSS APPLY Data.nodes('/M') AS Split(a)
                WHERE LTRIM(RTRIM(Split.a.value('.', 'NVARCHAR(MAX)'))) != '');
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

                INSERT INTO @TargetTables (SchemaName, TableName, HasPrimaryKey)
                SELECT
                    s.name, t.name,
                    CASE WHEN pk.CONSTRAINT_NAME IS NOT NULL THEN 1 ELSE 0 END
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                INNER JOIN @TableList tl ON
                    (tl.FullTableName = s.name + '.*' OR
                     tl.FullTableName = s.name + '.' + t.name OR
                     (CHARINDEX('.', tl.FullTableName) = 0 AND tl.FullTableName = t.name AND s.name = 'dbo'))
                LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk ON
                    pk.TABLE_SCHEMA = s.name AND pk.TABLE_NAME = t.name AND pk.CONSTRAINT_TYPE = 'PRIMARY KEY'
                WHERE t.type = 'U';
            END

            -- Check for tables without primary keys
            SELECT @SkippedTables = COALESCE(@SkippedTables + ',', '') + QUOTENAME(SchemaName) + '.' + QUOTENAME(TableName)
            FROM @TargetTables
            WHERE HasPrimaryKey = 0;

            SELECT @SkippedTablesCount = COUNT(*)
            FROM @TargetTables
            WHERE HasPrimaryKey = 0;

            IF @SkippedTablesCount > 0
            BEGIN
                DECLARE @SkippedTableWord NVARCHAR(10) = CASE WHEN @SkippedTablesCount = 1 THEN 'table' ELSE 'tables' END;
                PRINT N'? WARNING: Skipping ' + CAST(@SkippedTablesCount AS NVARCHAR(10)) + ' ' + @SkippedTableWord + ' without primary keys:';
                PRINT N'   ' + @SkippedTables;
                PRINT N'   Consider using lakeflowSetupChangeDataCapture for these tables.';

                DELETE FROM @TargetTables WHERE HasPrimaryKey = 0;
            END

            -- Process each table for change tracking enablement
            DECLARE table_cursor CURSOR FOR
                SELECT SchemaName, TableName FROM @TargetTables ORDER BY SchemaName, TableName;

            OPEN table_cursor;
            FETCH NEXT FROM table_cursor INTO @CurrentSchema, @CurrentTable;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY
                    IF NOT EXISTS (
                        SELECT 1 FROM sys.change_tracking_tables ct
                        INNER JOIN sys.tables t ON ct.object_id = t.object_id
                        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                        WHERE s.name = @CurrentSchema AND t.name = @CurrentTable
                    )
                    BEGIN
                        SET @SQL = N'ALTER TABLE ' + QUOTENAME(@CurrentSchema) + '.' + QUOTENAME(@CurrentTable) + ' ENABLE CHANGE_TRACKING';
                        EXEC sp_executesql @SQL;
                        PRINT N'? Enabled change tracking on [' + @CurrentSchema + '].[' + @CurrentTable + ']';
                        SET @ProcessedCount = @ProcessedCount + 1;
                    END
                    ELSE
                    BEGIN
                        PRINT N'? Change tracking already enabled on [' + @CurrentSchema + '].[' + @CurrentTable + ']';
                        SET @SkippedCount = @SkippedCount + 1;
                    END
                END TRY
                BEGIN CATCH
                    PRINT N'? Error enabling change tracking on [' + @CurrentSchema + '].[' + @CurrentTable + ']: ' + ERROR_MESSAGE();
                    SET @ErrorCount = @ErrorCount + 1;
                END CATCH

                FETCH NEXT FROM table_cursor INTO @CurrentSchema, @CurrentTable;
            END

            CLOSE table_cursor;
            DEALLOCATE table_cursor;

            -- Grant VIEW CHANGE TRACKING permissions to user (if @User is specified)
            IF @DatabaseUser IS NOT NULL
            BEGIN
                PRINT N'';
                PRINT N'=== Granting VIEW CHANGE TRACKING Permissions ===';

                DECLARE @PermissionGrantCount INT = 0, @PermissionErrorCount INT = 0;

                -- Strategy based on @Tables parameter
                IF @Tables = 'ALL'
                BEGIN
                    -- Grant on all user tables with change tracking enabled
                    PRINT N'Granting VIEW CHANGE TRACKING on all change tracking enabled tables...';

                    DECLARE @CTSchema NVARCHAR(128), @CTTable NVARCHAR(128);
                    DECLARE ct_cursor CURSOR FOR
                        SELECT s.name, t.name
                        FROM sys.change_tracking_tables ct
                        INNER JOIN sys.tables t ON ct.object_id = t.object_id
                        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                        WHERE s.name NOT IN ('sys', 'information_schema', 'cdc', 'INFORMATION_SCHEMA', 'guest')
                        ORDER BY s.name, t.name;

                    OPEN ct_cursor;
                    FETCH NEXT FROM ct_cursor INTO @CTSchema, @CTTable;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        BEGIN TRY
                            SET @SQL = N'GRANT VIEW CHANGE TRACKING ON [' + @CTSchema + '].[' + @CTTable + '] TO ' + QUOTENAME(@DatabaseUser);
                            EXEC sp_executesql @SQL;
                            PRINT N'  ? Granted VIEW CHANGE TRACKING on [' + @CTSchema + '].[' + @CTTable + ']';
                            SET @PermissionGrantCount = @PermissionGrantCount + 1;
                        END TRY
                        BEGIN CATCH
                            PRINT N'  ? Could not grant VIEW CHANGE TRACKING on [' + @CTSchema + '].[' + @CTTable + ']: ' + ERROR_MESSAGE();
                            SET @PermissionErrorCount = @PermissionErrorCount + 1;
                        END CATCH

                        FETCH NEXT FROM ct_cursor INTO @CTSchema, @CTTable;
                    END

                    CLOSE ct_cursor;
                    DEALLOCATE ct_cursor;
                END
                ELSE IF @Tables LIKE 'SCHEMAS:%'
                BEGIN
                    -- Grant on schema level for specified schemas
                    DECLARE @SchemaListForPerms NVARCHAR(MAX) = SUBSTRING(@Tables, 9, LEN(@Tables));
                    PRINT N'Granting VIEW CHANGE TRACKING on schemas: ' + @SchemaListForPerms;

                    -- Parse schema list and grant on each schema''s CT-enabled tables
                    DECLARE @Schema NVARCHAR(128);
                    DECLARE @TempSchemasForPerms NVARCHAR(MAX) = @SchemaListForPerms;

                    WHILE LEN(@TempSchemasForPerms) > 0
                    BEGIN
                        DECLARE @SchemaPosPerm INT = CHARINDEX(',', @TempSchemasForPerms);
                        IF @SchemaPosPerm = 0
                        BEGIN
                            SET @Schema = LTRIM(RTRIM(@TempSchemasForPerms));
                            SET @TempSchemasForPerms = N'';
                        END
                        ELSE
                        BEGIN
                            SET @Schema = LTRIM(RTRIM(LEFT(@TempSchemasForPerms, @SchemaPosPerm - 1)));
                            SET @TempSchemasForPerms = SUBSTRING(@TempSchemasForPerms, @SchemaPosPerm + 1, LEN(@TempSchemasForPerms));
                        END

                        IF LEN(@Schema) > 0
                        BEGIN
                            -- Grant on all CT-enabled tables in this schema
                            DECLARE schema_ct_cursor CURSOR FOR
                                SELECT t.name
                                FROM sys.change_tracking_tables ct
                                INNER JOIN sys.tables t ON ct.object_id = t.object_id
                                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                                WHERE s.name = @Schema
                                ORDER BY t.name;

                            OPEN schema_ct_cursor;
                            FETCH NEXT FROM schema_ct_cursor INTO @CTTable;

                            WHILE @@FETCH_STATUS = 0
                            BEGIN
                                BEGIN TRY
                                    SET @SQL = N'GRANT VIEW CHANGE TRACKING ON [' + @Schema + '].[' + @CTTable + '] TO ' + QUOTENAME(@DatabaseUser);
                                    EXEC sp_executesql @SQL;
                                    PRINT N'  ? Granted VIEW CHANGE TRACKING on [' + @Schema + '].[' + @CTTable + ']';
                                    SET @PermissionGrantCount = @PermissionGrantCount + 1;
                                END TRY
                                BEGIN CATCH
                                    PRINT N'  ? Could not grant VIEW CHANGE TRACKING on [' + @Schema + '].[' + @CTTable + ']: ' + ERROR_MESSAGE();
                                    SET @PermissionErrorCount = @PermissionErrorCount + 1;
                                END CATCH

                                FETCH NEXT FROM schema_ct_cursor INTO @CTTable;
                            END

                            CLOSE schema_ct_cursor;
                            DEALLOCATE schema_ct_cursor;
                        END
                    END
                END
                ELSE
                BEGIN
                    -- Grant on specific tables listed in @Tables
                    PRINT N'Granting VIEW CHANGE TRACKING on specified tables...';

                    -- Use the same @TargetTables that were processed for CT enablement
                    DECLARE specific_ct_cursor CURSOR FOR
                        SELECT SchemaName, TableName FROM @TargetTables
                        WHERE EXISTS (
                            SELECT 1 FROM sys.change_tracking_tables ct
                            INNER JOIN sys.tables t ON ct.object_id = t.object_id
                            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                            WHERE s.name = SchemaName AND t.name = TableName
                        )
                        ORDER BY SchemaName, TableName;

                    OPEN specific_ct_cursor;
                    FETCH NEXT FROM specific_ct_cursor INTO @CTSchema, @CTTable;

                    WHILE @@FETCH_STATUS = 0
                    BEGIN
                        BEGIN TRY
                            SET @SQL = N'GRANT VIEW CHANGE TRACKING ON [' + @CTSchema + '].[' + @CTTable + '] TO ' + QUOTENAME(@DatabaseUser);
                            EXEC sp_executesql @SQL;
                            PRINT N'  ? Granted VIEW CHANGE TRACKING on [' + @CTSchema + '].[' + @CTTable + ']';
                            SET @PermissionGrantCount = @PermissionGrantCount + 1;
                        END TRY
                        BEGIN CATCH
                            PRINT N'  ? Could not grant VIEW CHANGE TRACKING on [' + @CTSchema + '].[' + @CTTable + ']: ' + ERROR_MESSAGE();
                            SET @PermissionErrorCount = @PermissionErrorCount + 1;
                        END CATCH

                        FETCH NEXT FROM specific_ct_cursor INTO @CTSchema, @CTTable;
                    END

                    CLOSE specific_ct_cursor;
                    DEALLOCATE specific_ct_cursor;
                END

                -- Permission grant summary report
                PRINT N'';
                PRINT N'VIEW CHANGE TRACKING permission summary:';
                PRINT N'  - Tables granted: ' + CAST(@PermissionGrantCount AS NVARCHAR(10));
                PRINT N'  - Tables with permission errors: ' + CAST(@PermissionErrorCount AS NVARCHAR(10));

                IF @PermissionGrantCount > 0
                    PRINT N'? VIEW CHANGE TRACKING permissions granted to user: ' + @DatabaseUser;
            END

            -- Final summary report
            PRINT N'';
            PRINT N'CT setup summary:';
            PRINT N'  - Tables processed: ' + CAST(@ProcessedCount AS NVARCHAR(10));
            PRINT N'  - Tables already enabled: ' + CAST(@SkippedCount AS NVARCHAR(10));
            PRINT N'  - Tables with processing errors: ' + CAST(@ErrorCount AS NVARCHAR(10));
            PRINT N'  - Tables skipped (no PK): ' + CAST(@SkippedTablesCount AS NVARCHAR(10));
        END

        PRINT N'Change tracking setup completed successfully';

    END TRY
    BEGIN CATCH
        SET @ErrorMessage = 'Error in lakeflowSetupChangeTracking: ' + ERROR_MESSAGE();
        PRINT @ErrorMessage;
        THROW;
    END CATCH
END
GO
