--------------------------------------------------------------------------------
-- Killian Davies	08-February-2021	Initial Revision - services LH user history
--------------------------------------------------------------------------------
CREATE PROCEDURE  [elfh].[proc_UserHistoryLoadForLearningHubUser]
	@userId			int,
	@startPage		int=1,
	@PageSize		float=10.0,
	@TotalResults	int output
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
		@TotalResults = Count(userHistoryId)
	FROM	
		@History 		

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
