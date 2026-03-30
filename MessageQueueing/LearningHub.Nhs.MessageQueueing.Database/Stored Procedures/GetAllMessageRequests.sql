-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      18-03-2026
-- Purpose      Get paginated message requests.
--
-- Modification History
--
-- 18-03-2026  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[GetAllMessageRequests]
	@offSet int,
	@fetchRows int,
	@SortColumn NVARCHAR(50) = 'CreatedAt',
    @SortDirection NVARCHAR(4) = 'DESC',
	@Filters NVARCHAR(MAX) = NULL,
	@totalCount INT OUTPUT
AS
BEGIN
SET NOCOUNT ON;

IF @SortColumn NOT IN (
'Id','RequestType','Recipient','RequestStatus',
'RetryCount','CreatedAt'
)
SET @SortColumn = 'CreatedAt'

IF @SortDirection NOT IN ('ASC','DESC')
SET @SortDirection = 'DESC'

IF @Filters IS NULL OR @Filters = '' OR @Filters = 'null'
BEGIN
    SET @Filters = '[]'
END

DECLARE @WhereClause NVARCHAR(MAX) = ''

------------------------------------------------
-- Build WHERE clause from JSON filters
------------------------------------------------

SELECT @WhereClause =
STRING_AGG(
    CASE 
        WHEN FilterType = 'Text'
            THEN QUOTENAME(ColumnName) + ' LIKE ''%' + FilterValue + '%'''

        WHEN FilterType = 'Number'
			THEN 
                CASE ColumnName
                    WHEN 'Id' THEN 'qr.Id'
                    WHEN 'RetryCount' THEN 'qr.RetryCount'
                END
                + ' = ' + FilterValue

        WHEN FilterType = 'Date'
		THEN
				CASE ColumnName
					WHEN 'CreatedAt' THEN 
					'qr.CreatedAt >= ''' + FilterValue + ''' AND qr.CreatedAt < DATEADD(DAY,1,''' + FilterValue + ''')'
				END

        ELSE NULL
    END
,' AND ')
FROM OPENJSON(@Filters)
WITH
(
    ColumnName NVARCHAR(50) '$.Column',
    FilterType NVARCHAR(50) '$.Type',
    FilterValue NVARCHAR(200) '$.Value'
)

------------------------------------------------
-- Base Query
------------------------------------------------

DECLARE @SQL NVARCHAR(MAX)

SET @SQL = '
SELECT
    qr.Id,
    rt.RequestType,
    qr.Recipient,
    rs.RequestStatus,
    qr.RetryCount,
    qr.CreatedAt,
    qr.DeliverAfter,
    qr.SentAt,
    qr.LastAttemptAt,
    qr.ErrorMessage
FROM QueueRequests qr
JOIN RequestType rt ON qr.RequestTypeId = rt.Id
JOIN RequestStatus rs ON qr.Status = rs.Id
'

IF @WhereClause IS NOT NULL AND LEN(@WhereClause) > 0
BEGIN
    SET @SQL = @SQL + ' WHERE ' + @WhereClause
END


------------------------------------------------
-- Total Count
------------------------------------------------

DECLARE @CountSQL NVARCHAR(MAX)

SET @CountSQL = 'SELECT @TotalCount = COUNT(*) FROM (' + @SQL + ') A'

EXEC sp_executesql
@CountSQL,
N'@TotalCount INT OUTPUT',
@TotalCount OUTPUT


------------------------------------------------
-- Pagination Query
------------------------------------------------

SET @SQL = @SQL + '
 ORDER BY ' + QUOTENAME(@SortColumn) + ' ' + @SortDirection + '
 OFFSET ' + CAST(@Offset AS NVARCHAR) + ' ROWS
 FETCH NEXT ' + CAST(@fetchRows AS NVARCHAR) + ' ROWS ONLY'

EXEC(@SQL)

END
GO
