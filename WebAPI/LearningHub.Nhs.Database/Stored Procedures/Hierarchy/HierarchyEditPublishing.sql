-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-09-2021
-- Purpose      Publishing a HierarchyEdit that is currently in "Submitted to Publishing Queue"
--
-- Modification History
--
-- 23-09-2021  Killian Davies	Initial revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditPublishing]
(
	@HierarchyEditId int,
	@UserId int
)

AS

BEGIN

	BEGIN TRY
			
		DECLARE @CurrentStatusId int

		SELECT @CurrentStatusId = HierarchyEditStatusId
		FROM hierarchy.HierarchyEdit
		WHERE Id = @HierarchyEditId

		-- Must be in 'SubmittedToPublishingQueue' status to be set to Publishing
		IF @CurrentStatusId <> 5
		BEGIN
			RAISERROR ('Error - HierarchyEdit must be in "SubmittedToPublishingQueue" to be set to Publishing status', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN
		
			UPDATE hierarchy.HierarchyEdit
			SET HierarchyEditStatusId = 4 -- Publishing
			WHERE Id = @HierarchyEditId
			
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