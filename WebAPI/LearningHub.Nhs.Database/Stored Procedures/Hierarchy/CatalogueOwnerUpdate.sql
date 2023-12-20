-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-02-2021
-- Purpose      Updates Catalogue Owner details.
--
-- Modification History
--
-- 01-02-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueOwnerUpdate]
(
	@OwnerName nvarchar(250),
	@OwnerEmailAddress nvarchar(250),
	@Notes nvarchar(max),
	@UserId int,
	@CatalogueNodeVersionId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN
			UPDATE [hierarchy].[CatalogueNodeVersion]
			SET
				[Notes] = @Notes,
				[OwnerName] = @OwnerName,
				[OwnerEmailAddress] = @OwnerEmailAddress,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate
			WHERE Id = @CatalogueNodeVersionId
		COMMIT
	END TRY
	BEGIN CATCH
	    DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  
  
		SELECT   
			@ErrorMessage = ERROR_MESSAGE(),  
			@ErrorSeverity = ERROR_SEVERITY(),  
			@ErrorState = ERROR_STATE();  
  
		IF @@TRANCOUNT > 0  
			ROLLBACK TRAN;  

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  

	END CATCH
END
GO