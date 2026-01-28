-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowRefreshCaptureInstance_1_2] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[lakeflowRefreshCaptureInstance_1_2]
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

                SET @QuotedFullName = QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName);
                SET @CaptureInstanceCount = (SELECT COUNT(capture_instance) FROM cdc.change_tables WHERE source_object_id = object_id(@QuotedFullName));
                IF (@CaptureInstanceCount = 2)
                BEGIN
                    SET @TriggerReinit = 1;
                    EXEC dbo.[lakeflowMergeCaptureInstances_1_2] @schemaName, @tableName;
                    EXEC dbo.[lakeflowDisableOldCaptureInstance_1_2] @schemaName, @tableName;
                END

                SET @OldCaptureInstance = (select top 1 capture_instance from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);
                SET @SupportNetChanges = (select top 1 supports_net_changes from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);
                SET @FileGroupName = (select top 1 filegroup_name from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);
                SET @RoleName = (select top 1 role_name from cdc.change_tables where source_object_id=OBJECT_ID(@QuotedFullName) order by create_date ASC);

                IF @OldCaptureInstance = @schemaName + '_' + @tableName
                BEGIN
                    SET @NewCaptureInstance = 'New_' + @schemaName + '_' + @tableName
                END
                ELSE
                BEGIN
                    SET @NewCaptureInstance = @schemaName + '_' + @tableName
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

                    SET @CommittedCursor = (SELECT committedCursor FROM dbo.[lakeflowCaptureInstanceInfo_1_2] WHERE schemaName=@schemaName AND tableName=@tableName);
                    DELETE FROM dbo.[lakeflowCaptureInstanceInfo_1_2] WHERE schemaName=@schemaName AND tableName=@tableName;
                    INSERT INTO dbo.[lakeflowCaptureInstanceInfo_1_2] VALUES (@OldCaptureInstance, @NewCaptureInstance, @schemaName, @tableName, @CommittedCursor, @TriggerReinit);
                   IF (@reinit = 0)
                       EXEC dbo.lakeflowMergeCaptureInstances_1_2 @schemaName, @tableName;
                COMMIT TRAN
            COMMIT TRAN
GO
