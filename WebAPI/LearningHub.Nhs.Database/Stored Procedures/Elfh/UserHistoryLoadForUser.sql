--------------------------------------------------------------------------------
-- Jignesh Jethwani 05 Mar 2018 - Updated, moved userHistory to Attribute Table
-- Killian Davies	30 Oct 2019 - Perf improvement - modified to use table rather than view
--------------------------------------------------------------------------------

CREATE PROCEDURE  [elfh].[proc_UserHistoryLoadForUser]
	@userId			int,
	@startPage		int=1,
	@PageSize		float=10.0,
	@PagesReturned	int output
AS
	DECLARE @History Table(RowNumber int, userHistoryId int)

	DECLARE @ItemsReturned INT
	DECLARE @StartRow INT
	DECLARE @EndRow INT
	SET		@StartRow = ((@startPage - 1) * @PageSize)
	SET		@EndRow = (@startPage * @PageSize) + 1

	INSERT INTO @History
	SELECT
		ROW_NUMBER() OVER( ORDER BY [userHistoryId] DESC) as RowNumber,
		userHistoryId
	FROM
		userHistoryTBL
	WHERE
		userId = @userId
	ORDER BY 
		createdDate DESC

	-- Determine the number of items in the search result set
	SELECT 
		@ItemsReturned = Count(userHistoryId)
	FROM	
		@History 		
			
	SET @PagesReturned = CEILING(@ItemsReturned / @PageSize);


	SELECT 
		uh.*,
		uht.[Description],
		ISNULL(t.tenantName,'Unknown') as tenantName
	FROM userHistoryVW uh
		INNER JOIN @History h ON h.userHistoryId = uh.userHistoryId
		INNER JOIN userHistoryTypeTBL uht ON uht.UserHistoryTypeId = uh.userHistoryTypeId
		LEFT JOIN tenantTBL t ON t.tenantId = uh.tenantId
	WHERE
		h.RowNumber > @StartRow		
	AND 
		h.RowNumber < @EndRow
	ORDER BY 
		h.RowNumber ASC

RETURN 0
