-------------------------------------------------------------------------------
-- Author       OA
-- Created      24 JUN 2024 Nov 2020
-- Purpose      Break down the GetDashboardResources SP to smaller SP for a specific data type
--
-- Modification History
--
-- 24 Jun 2024	OA	Initial Revision
-- 31 Jun 2024  PT  Extracting functionality of certification with optional pagination so can be used on openapi and be single source of truth
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetMyCertificatesDashboardResources]
	@UserId					INT,	
	@PageNumber				INT = 1,
	@TotalRecords			INT OUTPUT
AS
BEGIN
	DECLARE @MaxPageNumber INT = 4
	
	IF @PageNumber > 4
	BEGIN
		SET @PageNumber = @MaxPageNumber
	END
		
	DECLARE @FetchRows INT = 3
	DECLARE @MaxRows INT = @MaxPageNUmber * @FetchRows
	DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows

    EXEC [resources].[GetAchievedcertificatedResourcesWithOptionalPagination] 
    @UserId = @UserId,
    @MaxRows= @MaxRows,
    @OffsetRows = @OffsetRows,
    @FetchRows = @FetchRows,
    @TotalRecords = @TotalRecords;

END