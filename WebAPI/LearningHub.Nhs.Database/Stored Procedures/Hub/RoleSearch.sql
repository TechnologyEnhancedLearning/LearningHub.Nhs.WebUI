--------------------------------------------------------------------------
--	01.04.19	Killian Davies		Initial Revision
--------------------------------------------------------------------------
CREATE PROCEDURE [hub].[RoleSearch]
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

	SELECT	@ItemsReturned = Count(*)
	FROM	hub.[Role] r
	WHERE	r.Deleted = 0
		AND	
			(
			[Name] like '%' + @searchText + '%'
			OR	[Description] like '%' + @searchText + '%'
			)
			
	SET @PagesReturned = CEILING(@ItemsReturned / CAST(@PageSize AS float));

	WITH RolesTable AS
	(
		SELECT	Id,
				[Name],
				[Description],
				Deleted,
				CreateUserId,
				CreateDate,
				AmendUserId,
				AmendDate,
				ROW_NUMBER() OVER(ORDER BY [Name], Id) AS RowNumber
		FROM	hub.[Role]
		WHERE	Deleted = 0
		AND	
			(
			[Name] like '%' + @searchText + '%'
			OR	[Description] like '%' + @searchText + '%'
			)
	)

	SELECT	*
	FROM	RolesTable
	WHERE	RowNumber > @StartRow
		AND RowNumber < @EndRow
	ORDER BY Name

	SET @Err = @@Error

	RETURN @Err

END
GO