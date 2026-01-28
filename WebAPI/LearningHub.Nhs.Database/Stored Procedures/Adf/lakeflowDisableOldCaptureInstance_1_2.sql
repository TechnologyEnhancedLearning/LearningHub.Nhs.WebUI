-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowDisableOldCaptureInstance_1_2] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[lakeflowDisableOldCaptureInstance_1_2]
                @schemaName VARCHAR(MAX), @tableName VARCHAR(MAX)
            WITH EXECUTE AS OWNER
            AS
            SET NOCOUNT ON

            DECLARE @oldCaptureInstance NVARCHAR(MAX);

            BEGIN TRAN
                SET @oldCaptureInstance = (SELECT oldCaptureInstance FROM dbo.[lakeflowCaptureInstanceInfo_1_2] WHERE schemaName=@schemaName AND tableName=@tableName);

                IF @oldCaptureInstance IS NOT NULL
                BEGIN
                    EXEC sys.sp_cdc_disable_table
                        @source_schema = @schemaName,
                        @source_name = @tableName,
                        @capture_instance = @oldCaptureInstance;
                    UPDATE dbo.[lakeflowCaptureInstanceInfo_1_2] SET oldCaptureInstance=NULL WHERE schemaName=@schemaName AND tableName=@tableName;
                END
            COMMIT TRAN
GO
