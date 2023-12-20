--------------------------------------------------------------------------
--	01.04.19	Killian Davies		Initial Revision
--------------------------------------------------------------------------
CREATE PROCEDURE [hub].[PermissionSearch]
(
	@page				int = 1,
	@pageSize			int = 20,
	@searchText			nvarchar(50),
	@pagesReturned		int OUTPUT
)
AS
BEGIN

	DECLARE @ItemsReturned INT
	DECLARE @StartRow INT
	DECLARE @EndRow INT
	SET		@StartRow = ((@Page - 1) * @PageSize)
	SET		@EndRow = (@Page * @PageSize) + 1

	SET NOCOUNT OFF
	DECLARE @Err int

	SELECT	@ItemsReturned = Count('x')
	FROM	hub.permission 
	WHERE	((Code like '%'+@searchText+'%')
	OR		([Name] like '%'+@searchText+'%')
	OR		([Description] like '%'+@searchText+'%'))
	AND		deleted = 0

	SET @PagesReturned = CEILING(@ItemsReturned / CAST(@PageSize AS float));

	WITH PermissionsTable AS
	(
		SELECT	Id,
				ISNULL(Code,'') AS 'Code',
				[Name],
				[Description],
				deleted,
				amendUserId,
				amendDate,
				createUserId,
				createDate,
				ROW_NUMBER() OVER(ORDER BY [Name]) AS RowNumber
		FROM	hub.permission
		WHERE	((Code like '%'+@searchText+'%')
		OR		([Name] like '%'+@searchText+'%')
		OR		([Description] like '%'+@searchText+'%'))
		AND		deleted = 0
	)

	SELECT	*
	FROM	PermissionsTable
	WHERE	RowNumber > @StartRow
		AND RowNumber < @EndRow
	ORDER BY [Name]

	SET @Err = @@Error

	RETURN @Err

END
GO