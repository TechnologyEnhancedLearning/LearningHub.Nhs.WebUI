-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-09-2021
-- Purpose      Submit a HierarchyEdit to Publishing Queue
--
-- Modification History
--
-- 23-09-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditSubmitForPublishing]
(
	@HierarchyEditId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
						
		DECLARE @CurrentStatusId int

		SELECT @CurrentStatusId = HierarchyEditStatusId
		FROM hierarchy.HierarchyEdit
		WHERE Id = @HierarchyEditId

		-- Must be in 'Draft' status to be submitted to publishing queue.
		IF @CurrentStatusId <> 1
		BEGIN
			RAISERROR ('Error - HierarchyEdit must be in "Draft" to be set to SubmittedToPublishingQueue status', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN

			DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

			UPDATE 
				hierarchy.HierarchyEdit
			SET 
				HierarchyEditStatusId = 5, -- SubmittedToPublishingQueue
				AmendDate = @AmendDate
			WHERE 
				Id = @HierarchyEditId
			
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