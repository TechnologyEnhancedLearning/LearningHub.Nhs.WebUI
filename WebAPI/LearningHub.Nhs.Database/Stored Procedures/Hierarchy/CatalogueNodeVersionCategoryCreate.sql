-------------------------------------------------------------------------------
-- Author       SA
-- Created      14-10-2025 
-- Purpose      Creates a Catalogue NodeVersion Category.
--
-- Modification History
--
-- 14-10-2025  SA	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueNodeVersionCategoryCreate]
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

	INSERT INTO [hierarchy].[CatalogueNodeVersionCategory]
           ([CatalogueNodeVersionId]
           ,[CategoryId]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (@CatalogueNodeVersionId
           ,@CategoryId
           ,0
           ,@userId
           ,@AmendDate
           ,@userId
           ,@AmendDate)
END