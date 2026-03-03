-------------------------------------------------------------------------------
-- Author       SA
-- Created      14-10-2025 
-- Purpose      Removes a Catalogue NodeVersion Category.
--
-- Modification History
--
-- 05-11-2025  SA	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[RemoveCatalogueCategory]
(
	@userId INT, 
	@CatalogueNodeVersionId INT,
	@CategoryId INT,
	@UserTimezoneOffset int = NULL
)

AS
BEGIN
    DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
	IF EXISTS (SELECT 1 
		FROM [hierarchy].[CatalogueNodeVersionCategory] 
		WHERE CatalogueNodeVersionId = @CatalogueNodeVersionId AND deleted =0
		)
	BEGIN
		UPDATE [hierarchy].[CatalogueNodeVersionCategory]
		SET 
			Deleted = 1,
			AmendDate = @AmendDate,
			AmendUserId = @UserId
		WHERE 
			CatalogueNodeVersionId = @CatalogueNodeVersionId
	END
END