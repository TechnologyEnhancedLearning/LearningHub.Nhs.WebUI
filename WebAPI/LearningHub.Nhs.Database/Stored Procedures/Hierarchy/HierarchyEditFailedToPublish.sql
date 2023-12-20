-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose		Set HierarchyEdit to "Failed to Publish"
--
-- Modification History
--
-- 23-09-2021  KD	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditFailedToPublish]
(
	@HierarchyEditId int,
	@UserId int
)

AS

BEGIN

	BEGIN TRY
			
		BEGIN TRAN
		
			UPDATE hierarchy.HierarchyEdit
			SET HierarchyEditStatusId =  6 -- Failed to Publish
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