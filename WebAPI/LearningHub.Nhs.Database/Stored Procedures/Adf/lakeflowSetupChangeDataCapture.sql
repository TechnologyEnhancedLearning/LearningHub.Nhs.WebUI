-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowSetupChangeDataCapture] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[lakeflowSetupChangeDataCapture]
    @Tables NVARCHAR(MAX) = NULL,
    @User NVARCHAR(128) = NULL,
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
    DECLARE @captureInstanceTableName NVARCHAR(100) = 'lakeflowCaptureInstanceInfo' + @versionSuffix;
    DECLARE @alterTableTriggerName NVARCHAR(100) = 'lakeflowAlterTableTrigger' + @versionSuffix;
    DECLARE @disableOldCaptureInstanceProcName NVARCHAR(100) = 'lakeflowDisableOldCaptureInstance' + @versionSuffix;
    DECLARE @mergeCaptureInstancesProcName NVARCHAR(100) = 'lakeflowMergeCaptureInstances' + @versionSuffix;
    DECLARE @refreshCaptureInstanceProcName NVARCHAR(100) = 'lakeflowRefreshCaptureInstance' + @versionSuffix;

    -- Error codes and messages
    DECLARE @invalidModeErrorCode INT = 100000;
    DECLARE @invalidModeErrorMessage NVARCHAR(200);
    DECLARE @insufficientUserPrivilegesCode INT = 100400;
    DECLARE @insufficientUserPrivilegesErrorMessage NVARCHAR(200);

    SET @invalidModeErrorMessage = CONCAT('Provided execution mode: ', @Mode, ', is not recognized. Allowed values are: INSTALL, CLEANUP');
    SET @insufficientUserPrivilegesErrorMessage = 'User executing this script is not a ''db_owner'' role member. To execute this script, please use a user that is.';

    PRINT N'Starting Change Data Capture setup for: ' + @CatalogName;
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
            PRINT N'Cleaning up CDC DDL support objects...';

            -- Drop procedures
            IF OBJECT_ID('dbo.' + @refreshCaptureInstanceProcName, 'P') IS NOT NULL
            BEGIN
                SET @SQL = N'DROP PROCEDURE [dbo].[' + @refreshCaptureInstanceProcName + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped procedure: ' + @refreshCaptureInstanceProcName;
            END

            IF OBJECT_ID('dbo.' + @mergeCaptureInstancesProcName, 'P') IS NOT NULL
            BEGIN
                SET @SQL = N'DROP PROCEDURE [dbo].[' + @mergeCaptureInstancesProcName + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped procedure: ' + @mergeCaptureInstancesProcName;
            END

            IF OBJECT_ID('dbo.' + @disableOldCaptureInstanceProcName, 'P') IS NOT NULL
            BEGIN
                SET @SQL = N'DROP PROCEDURE [dbo].[' + @disableOldCaptureInstanceProcName + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped procedure: ' + @disableOldCaptureInstanceProcName;
            END

            -- Drop ALTER TABLE trigger
            IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = @alterTableTriggerName AND parent_class = 0)
            BEGIN
                SET @SQL = N'DROP TRIGGER [' + @alterTableTriggerName + '] ON DATABASE';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped ALTER TABLE trigger: ' + @alterTableTriggerName;
            END

            -- Drop capture instance table
            IF OBJECT_ID('dbo.' + @captureInstanceTableName, 'U') IS NOT NULL
            BEGIN
                SET @SQL = N'DROP TABLE [dbo].[' + @captureInstanceTableName + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Dropped capture instance table: ' + @captureInstanceTableName;
            END

            -- Pattern-based cleanup for any remaining CDC objects across versions
            DECLARE @cdcCleanupSql NVARCHAR(MAX) = '';

            -- Clean up any remaining capture instance tables across versions
            SELECT @cdcCleanupSql = @cdcCleanupSql + 'DROP TABLE [dbo].[' + name + '];' + CHAR(13)
            FROM sys.tables
            WHERE name LIKE 'lakeflowCaptureInstanceInfo_%_%' AND name != @captureInstanceTableName;

            IF LEN(@cdcCleanupSql) > 0
            BEGIN
                EXEC sp_executesql @cdcCleanupSql;
                PRINT N'? Cleaned up remaining capture instance tables across versions';
            END

            -- Clean up any remaining CDC procedures across versions
            SET @cdcCleanupSql = '';
            SELECT @cdcCleanupSql = @cdcCleanupSql + 'DROP PROCEDURE [dbo].[' + name + '];' + CHAR(13)
            FROM sys.procedures
            WHERE (name LIKE 'lakeflowDisableOldCaptureInstance_%_%' AND name != @disableOldCaptureInstanceProcName)
               OR (name LIKE 'lakeflowMergeCaptureInstances_%_%' AND name != @mergeCaptureInstancesProcName)
               OR (name LIKE 'lakeflowRefreshCaptureInstance_%_%' AND name != @refreshCaptureInstanceProcName);

            IF LEN(@cdcCleanupSql) > 0
            BEGIN
                EXEC sp_executesql @cdcCleanupSql;
                PRINT N'? Cleaned up remaining CDC procedures across versions';
            END

            -- Clean up any remaining ALTER TABLE triggers across versions
            SET @cdcCleanupSql = '';
            SELECT @cdcCleanupSql = @cdcCleanupSql + 'DROP TRIGGER [' + name + '] ON DATABASE;' + CHAR(13)
            FROM sys.triggers
            WHERE name LIKE 'lakeflowAlterTableTrigger_%_%' AND name != @alterTableTriggerName AND parent_class = 0;

            IF LEN(@cdcCleanupSql) > 0
            BEGIN
                EXEC sp_executesql @cdcCleanupSql;
                PRINT N'? Cleaned up remaining ALTER TABLE triggers across versions';
            END

            PRINT N'CDC DDL support objects cleanup completed';
            RETURN;
        END

        -- Install mode: Create/upgrade DDL support objects
        PRINT N'Installing/upgrading CDC DDL support objects...';

        -- Enable CDC at database level if not already enabled
        IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = DB_NAME() AND is_cdc_enabled = 1)
        BEGIN
            PRINT N'Enabling Change Data Capture at database level...';
            EXEC sys.sp_cdc_enable_db;
            PRINT N'? Change Data Capture enabled at database level';
        END
        ELSE
        BEGIN
            PRINT N'? Change Data Capture already enabled at database level';
        END

        -- Create capture instance table
        IF OBJECT_ID('dbo.' + @captureInstanceTableName, 'U') IS NULL
        BEGIN
            SET @SQL = N'CREATE TABLE [dbo].[' + @captureInstanceTableName + '](
                [oldCaptureInstance] VARCHAR(MAX) NULL,
                [newCaptureInstance] VARCHAR(MAX) NULL,
                [schemaName] VARCHAR(100) NOT NULL,
                [tableName] VARCHAR(255) NOT NULL,
                [committedCursor] VARCHAR(MAX) NULL,
                [triggerReinit] BIT NULL,
                CONSTRAINT replicantCaptureInstanceInfoPrimaryKey PRIMARY KEY (schemaName, tableName)
            )';
            EXEC sp_executesql @SQL;
            PRINT N'? Created capture instance table: ' + @captureInstanceTableName;
        END

        -- Create lakeflowDisableOldCaptureInstance procedure
        IF OBJECT_ID('dbo.' + @disableOldCaptureInstanceProcName, 'P') IS NULL
        BEGIN
            SET @SQL = N'CREATE PROCEDURE [dbo].[' + @disableOldCaptureInstanceProcName + ']
                @schemaName VARCHAR(MAX), @tableName VARCHAR(MAX)
            WITH EXECUTE AS OWNER
            AS
            SET NOCOUNT ON

            DECLARE @oldCaptureInstance NVARCHAR(MAX);

            BEGIN TRAN
                SET @oldCaptureInstance = (SELECT oldCaptureInstance FROM dbo.[' + @captureInstanceTableName + '] WHERE schemaName=@schemaName AND tableName=@tableName);

                IF @oldCaptureInstance IS NOT NULL
                BEGIN
                    EXEC sys.sp_cdc_disable_table
                        @source_schema = @schemaName,
                        @source_name = @tableName,
                        @capture_instance = @oldCaptureInstance;
                    UPDATE dbo.[' + @captureInstanceTableName + '] SET oldCaptureInstance=NULL WHERE schemaName=@schemaName AND tableName=@tableName;
                END
            COMMIT TRAN';
            EXEC sp_executesql @SQL;
            PRINT N'? Created procedure: ' + @disableOldCaptureInstanceProcName;
        END

        -- Create lakeflowMergeCaptureInstances procedure
        IF OBJECT_ID('dbo.' + @mergeCaptureInstancesProcName, 'P') IS NULL
        BEGIN
            SET @SQL = N'CREATE PROCEDURE [dbo].[' + @mergeCaptureInstancesProcName + ']
                @schemaName VARCHAR(MAX), @tableName VARCHAR(MAX)
            AS
            SET NOCOUNT ON
            BEGIN TRAN
                DECLARE @newCaptureInstanceFullPath NVARCHAR(MAX),
                    @oldCaptureInstanceFullPath NVARCHAR(MAX),
                    @columnList NVARCHAR(MAX),
                    @columnListValues NVARCHAR(MAX),
                    @oldCaptureInstanceName NVARCHAR(MAX),
                    @newCaptureInstanceName NVARCHAR(MAX),
                    @captureInstanceCount INT,
                    @minLSN VARCHAR(MAX),
                    @quotedFullTableName nvarchar(max),
                    @mergeSQL NVARCHAR(MAX);

                SET @quotedFullTableName = QUOTENAME(@schemaName) + ''.'' + QUOTENAME(@tableName);
                SET @captureInstanceCount = (SELECT COUNT(*) FROM cdc.change_tables WHERE source_object_id = OBJECT_ID(@quotedFullTableName));
                IF (@captureInstanceCount = 2)
                BEGIN
                    SET @oldCaptureInstanceName = (SELECT oldCaptureInstance
                                           FROM dbo.[' + @captureInstanceTableName + ']
                                           WHERE schemaName = @schemaName and tableName = @tableName) + ''_CT'';
                    SET @newCaptureInstanceName = (SELECT newCaptureInstance
                                           FROM dbo.[' + @captureInstanceTableName + ']
                                           WHERE schemaName = @schemaName and tableName = @tableName) + ''_CT'';
                    SET @newCaptureInstanceFullPath = ''[cdc].'' + QUOTENAME(@newCaptureInstanceName);
	                SET @oldCaptureInstanceFullPath = ''[cdc].'' + QUOTENAME(@oldCaptureInstanceName);
                    SET @minLSN = (SELECT committedCursor FROM dbo.[' + @captureInstanceTableName + '] WHERE schemaName=@schemaName and tableName=@tableName);

                    IF @minLSN is NULL OR @minLSN = ''''
                    BEGIN
                        SET @minLSN = ''0x00000000000000000000''
                    END

						SET @columnList = (SELECT STUFF((SELECT '','' + QUOTENAME(A.COLUMN_NAME)
												   FROM INFORMATION_SCHEMA.COLUMNS A
													   JOIN INFORMATION_SCHEMA.COLUMNS B ON
														   A.COLUMN_NAME=B.COLUMN_NAME AND
														   A.DATA_TYPE=B.DATA_TYPE
													   WHERE A.TABLE_NAME=@newCaptureInstanceName AND
														   A.TABLE_SCHEMA=''cdc'' AND
														   B.TABLE_NAME=@oldCaptureInstanceName AND
														   B.TABLE_SCHEMA=''cdc'' FOR XML PATH(''''), TYPE).value(''.'', ''nvarchar(max)''), 1, 1, ''''));

						SET @columnListValues = (SELECT STUFF((SELECT '',source.'' + QUOTENAME(A.COLUMN_NAME)
														 FROM INFORMATION_SCHEMA.COLUMNS A
															 JOIN INFORMATION_SCHEMA.COLUMNS B ON
																 A.COLUMN_NAME=B.COLUMN_NAME AND
																 A.DATA_TYPE=B.DATA_TYPE
														 WHERE
															 A.TABLE_NAME=@newCaptureInstanceName AND
															 A.TABLE_SCHEMA=''cdc'' AND
															 B.TABLE_NAME=@oldCaptureInstanceName AND
															 B.TABLE_SCHEMA=''cdc'' FOR XML PATH(''''), TYPE).value(''.'', ''nvarchar(max)''), 1, 1, ''''));

                    SET @mergeSQL = ''MERGE '' + @newCaptureInstanceFullPath + '' AS target USING '' + @oldCaptureInstanceFullPath + '' AS source ON source.__$start_lsn = target.__$start_lsn AND source.__$seqval = target.__$seqval AND source.__$operation = target.__$operation WHEN NOT MATCHED AND source.__$start_lsn > '' + @minLSN + '' THEN INSERT ('' + @columnList + '') VALUES ('' + @columnListValues + '');'';
                    EXEC sp_executesql @mergeSQL;
                END
            COMMIT TRAN';
            EXEC sp_executesql @SQL;
            PRINT N'? Created procedure: ' + @mergeCaptureInstancesProcName;
        END

        -- Create lakeflowRefreshCaptureInstance procedure
        IF OBJECT_ID('dbo.' + @refreshCaptureInstanceProcName, 'P') IS NULL
        BEGIN
            SET @SQL = N'CREATE PROCEDURE [dbo].[' + @refreshCaptureInstanceProcName + ']
                @schemaName NVARCHAR(MAX),
                @tableName NVARCHAR(MAX),
                @reinit INT = 0
            WITH EXECUTE AS OWNER
            AS
            SET NOCOUNT ON
            BEGIN TRAN
                DECLARE @OldCaptureInstance NVARCHAR(MAX),
                    @NewCaptureInstance NVARCHAR(MAX),
                    @FileGroupName NVARCHAR(255),
                    @SupportNetChanges BIT,
                    @RoleName VARCHAR(255),
                    @CaptureInstanceCount INT,
                    @TriggerReinit INT,
                    @QuotedFullName nvarchar(max);

                SET @QuotedFullName = QUOTENAME(@schemaName) + ''.'' + QUOTENAME(@tableName);
                SET @CaptureInstanceCount = (SELECT COUNT(capture_instance) FROM cdc.change_tables WHERE source_object_id = object_id(@QuotedFullName));
                IF (@CaptureInstanceCount = 2)
                BEGIN
                    SET @TriggerReinit = 1;
                    EXEC dbo.[' + @mergeCaptureInstancesProcName + '] @schemaName, @tableName;
                    EXEC dbo.[' + @disableOldCaptureInstanceProcName + '] @schemaName, @tableName;
                END

                SET @OldCaptureInstance = (select top 1 capture_instance from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);
                SET @SupportNetChanges = (select top 1 supports_net_changes from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);
                SET @FileGroupName = (select top 1 filegroup_name from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);
                SET @RoleName = (select top 1 role_name from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);

                IF @OldCaptureInstance = @schemaName + ''_'' + @tableName
                BEGIN
                    SET @NewCaptureInstance = ''New_'' + @schemaName + ''_'' + @tableName
                END
                ELSE
                BEGIN
                    SET @NewCaptureInstance = @schemaName + ''_'' + @tableName
                END

                DECLARE @CommittedCursor VARCHAR(MAX);

                BEGIN TRAN
                    EXEC sys.sp_cdc_enable_table
                        @source_schema = @schemaName,
                        @source_name   = @tableName,
                        @role_name     = @RoleName,
                        @capture_instance = @NewCaptureInstance,
                        @filegroup_name = @FileGroupName,
                        @supports_net_changes = @SupportNetChanges

                    SET @CommittedCursor = (SELECT committedCursor FROM dbo.[' + @captureInstanceTableName + '] WHERE schemaName=@schemaName AND tableName=@tableName);
                    DELETE FROM dbo.[' + @captureInstanceTableName + '] WHERE schemaName=@schemaName AND tableName=@tableName;
                    INSERT INTO dbo.[' + @captureInstanceTableName + '] VALUES (@OldCaptureInstance, @NewCaptureInstance, @schemaName, @tableName, @CommittedCursor, @TriggerReinit);
                   IF (@reinit = 0)
                       EXEC dbo.' + @mergeCaptureInstancesProcName + ' @schemaName, @tableName;
                COMMIT TRAN
            COMMIT TRAN';
            EXEC sp_executesql @SQL;
            PRINT N'? Created procedure: ' + @refreshCaptureInstanceProcName;
        END

        -- Create ALTER TABLE trigger
        IF NOT EXISTS (SELECT 1 FROM sys.triggers WHERE name = @alterTableTriggerName AND parent_class = 0)
        BEGIN
            SET @SQL = N'CREATE TRIGGER [' + @alterTableTriggerName + ']
                ON DATABASE FOR ALTER_TABLE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DECLARE @data XML = EVENTDATA();
                    DECLARE @DbName NVARCHAR(255) = DB_NAME();
                    DECLARE @schemaName NVARCHAR(MAX) = @data.value(''(/EVENT_INSTANCE/SchemaName)[1]'', ''NVARCHAR(MAX)'');
                    DECLARE @tableName NVARCHAR(255) = @data.value(''(/EVENT_INSTANCE/ObjectName)[1]'', ''NVARCHAR(255)'');
                    DECLARE @isColumnAdd NVARCHAR(255) = @data.value(''(/EVENT_INSTANCE/AlterTableActionList/Create)[1]'', ''NVARCHAR(255)'');
                    DECLARE @IsCdcEnabledDBLevel BIT = (SELECT is_cdc_enabled FROM sys.databases WHERE name=@DbName);
                    DECLARE @IsCdcEnabledTableLevel BIT = (SELECT is_tracked_by_cdc FROM sys.tables WHERE schema_id = SCHEMA_ID(@SchemaName) AND name = @TableName);

                    -- Only trigger refresh if CDC is enabled at both levels AND column add detected
                    IF (@IsCdcEnabledDBLevel = 1 AND @IsCdcEnabledTableLevel = 1 AND @isColumnAdd IS NOT NULL)
                    BEGIN
                        -- Refresh the capture instance for this table
                        EXEC [dbo].[' + @refreshCaptureInstanceProcName + '] @SchemaName, @TableName;
                    END
                END';
            EXEC sp_executesql @SQL;
            PRINT N'? Created ALTER TABLE trigger: ' + @alterTableTriggerName;
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

                -- If still no database user found, warn and exit
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

            -- Special handling for dbo user - cannot grant permissions to dbo
            IF @DatabaseUser = 'dbo'
            BEGIN
                PRINT N'Skipping permission grants (dbo already has all permissions).';
                SET @DatabaseUser = NULL;
            END
        END

        -- Grant permissions to user if specified
        IF @DatabaseUser IS NOT NULL
        BEGIN
            PRINT N'Granting CDC DDL support object permissions to user: ' + @DatabaseUser;
            BEGIN TRY
                SET @SQL = N'GRANT SELECT, UPDATE ON [dbo].[' + @captureInstanceTableName + '] TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT VIEW DEFINITION TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT VIEW DATABASE STATE TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT SELECT ON SCHEMA::dbo TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT SELECT, INSERT ON SCHEMA::cdc TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT EXECUTE ON [dbo].[' + @disableOldCaptureInstanceProcName + '] TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT EXECUTE ON [dbo].[' + @mergeCaptureInstancesProcName + '] TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                SET @SQL = N'GRANT EXECUTE ON [dbo].[' + @refreshCaptureInstanceProcName + '] TO [' + @DatabaseUser + ']';
                EXEC sp_executesql @SQL;
                PRINT N'? Granted CDC permissions to ' + @DatabaseUser;
            END TRY
            BEGIN CATCH
                PRINT N'? Could not grant CDC permissions to ' + @DatabaseUser + ': ' + ERROR_MESSAGE();
            END CATCH
        END

        -- Process tables if specified
        IF @Tables IS NOT NULL
        BEGIN
            PRINT N'Processing tables for Change Data Capture enablement...';

            DECLARE @TargetTables TABLE (
                SchemaName NVARCHAR(128),
                TableName NVARCHAR(128),
                HasPrimaryKey BIT
            );

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

            -- Process each table for CDC enablement
            DECLARE @CurrentSchema NVARCHAR(128), @CurrentTable NVARCHAR(128);
            DECLARE @ProcessedCount INT = 0, @SkippedCount INT = 0, @ErrorCount INT = 0;

            DECLARE table_cursor CURSOR FOR
                SELECT SchemaName, TableName FROM @TargetTables ORDER BY SchemaName, TableName;

            OPEN table_cursor;
            FETCH NEXT FROM table_cursor INTO @CurrentSchema, @CurrentTable;

            WHILE @@FETCH_STATUS = 0
            BEGIN
                BEGIN TRY
                    IF NOT EXISTS (
                        SELECT 1 FROM cdc.change_tables ct
                        INNER JOIN sys.tables t ON ct.source_object_id = t.object_id
                        INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                        WHERE s.name = @CurrentSchema AND t.name = @CurrentTable
                    )
                    BEGIN
                        EXEC sys.sp_cdc_enable_table
                            @source_schema = @CurrentSchema,
                            @source_name = @CurrentTable,
                            @role_name = NULL;
                        PRINT N'? Enabled CDC on [' + @CurrentSchema + '].[' + @CurrentTable + ']';
                        SET @ProcessedCount = @ProcessedCount + 1;
                    END
                    ELSE
                    BEGIN
                        PRINT N'? CDC already enabled on [' + @CurrentSchema + '].[' + @CurrentTable + ']';
                        SET @SkippedCount = @SkippedCount + 1;
                    END
                END TRY
                BEGIN CATCH
                    PRINT N'? Error enabling CDC on [' + @CurrentSchema + '].[' + @CurrentTable + ']: ' + ERROR_MESSAGE();
                    SET @ErrorCount = @ErrorCount + 1;
                END CATCH

                FETCH NEXT FROM table_cursor INTO @CurrentSchema, @CurrentTable;
            END

            CLOSE table_cursor;
            DEALLOCATE table_cursor;

            -- Summary
            PRINT N'';
            PRINT N'CDC setup summary:';
            PRINT N'  - Tables processed: ' + CAST(@ProcessedCount AS NVARCHAR(10));
            PRINT N'  - Tables already enabled: ' + CAST(@SkippedCount AS NVARCHAR(10));
            PRINT N'  - Tables with errors: ' + CAST(@ErrorCount AS NVARCHAR(10));
        END

        PRINT N'Change Data Capture setup completed successfully';

    END TRY
    BEGIN CATCH
        SET @ErrorMessage = 'Error in lakeflowSetupChangeDataCapture: ' + ERROR_MESSAGE();
        PRINT @ErrorMessage;
        THROW;
    END CATCH
END
GO
