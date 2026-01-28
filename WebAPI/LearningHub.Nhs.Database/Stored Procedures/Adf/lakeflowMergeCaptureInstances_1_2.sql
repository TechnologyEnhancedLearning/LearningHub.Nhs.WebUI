-------------------------------------------------------------------------------
-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowMergeCaptureInstances_1_2] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[lakeflowMergeCaptureInstances_1_2]
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

                SET @quotedFullTableName = QUOTENAME(@schemaName) + '.' + QUOTENAME(@tableName);
                SET @captureInstanceCount = (SELECT COUNT(*) FROM cdc.change_tables WHERE source_object_id = OBJECT_ID(@quotedFullTableName));
                IF (@captureInstanceCount = 2)
                BEGIN
                    SET @oldCaptureInstanceName = (SELECT oldCaptureInstance
                                           FROM dbo.[lakeflowCaptureInstanceInfo_1_2]
                                           WHERE schemaName = @schemaName and tableName = @tableName) + '_CT';
                    SET @newCaptureInstanceName = (SELECT newCaptureInstance
                                           FROM dbo.[lakeflowCaptureInstanceInfo_1_2]
                                           WHERE schemaName = @schemaName and tableName = @tableName) + '_CT';
                    SET @newCaptureInstanceFullPath = '[cdc].' + QUOTENAME(@newCaptureInstanceName);
	                SET @oldCaptureInstanceFullPath = '[cdc].' + QUOTENAME(@oldCaptureInstanceName);
                    SET @minLSN = (SELECT committedCursor FROM dbo.[lakeflowCaptureInstanceInfo_1_2] WHERE schemaName=@schemaName and tableName=@tableName);

                    IF @minLSN is NULL OR @minLSN = ''
                    BEGIN
                        SET @minLSN = '0x00000000000000000000'
                    END

						SET @columnList = (SELECT STUFF((SELECT ',' + QUOTENAME(A.COLUMN_NAME)
												   FROM INFORMATION_SCHEMA.COLUMNS A
													   JOIN INFORMATION_SCHEMA.COLUMNS B ON
														   A.COLUMN_NAME=B.COLUMN_NAME AND
														   A.DATA_TYPE=B.DATA_TYPE
													   WHERE A.TABLE_NAME=@newCaptureInstanceName AND
														   A.TABLE_SCHEMA='cdc' AND
														   B.TABLE_NAME=@oldCaptureInstanceName AND
														   B.TABLE_SCHEMA='cdc' FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, ''));

						SET @columnListValues = (SELECT STUFF((SELECT ',source.' + QUOTENAME(A.COLUMN_NAME)
														 FROM INFORMATION_SCHEMA.COLUMNS A
															 JOIN INFORMATION_SCHEMA.COLUMNS B ON
																 A.COLUMN_NAME=B.COLUMN_NAME AND
																 A.DATA_TYPE=B.DATA_TYPE
														 WHERE
															 A.TABLE_NAME=@newCaptureInstanceName AND
															 A.TABLE_SCHEMA='cdc' AND
															 B.TABLE_NAME=@oldCaptureInstanceName AND
															 B.TABLE_SCHEMA='cdc' FOR XML PATH(''), TYPE).value('.', 'nvarchar(max)'), 1, 1, ''));

                    SET @mergeSQL = 'MERGE ' + @newCaptureInstanceFullPath + ' AS target USING ' + @oldCaptureInstanceFullPath + ' AS source ON source.__$start_lsn = target.__$start_lsn AND source.__$seqval = target.__$seqval AND source.__$operation = target.__$operation WHEN NOT MATCHED AND source.__$start_lsn > ' + @minLSN + ' THEN INSERT (' + @columnList + ') VALUES (' + @columnListValues + ');';
                    EXEC sp_executesql @mergeSQL;
                END
            COMMIT TRAN
GO
