-------------------------------------------------------------------------------
-- Author       Jignesh Jethwani
-- Created      29-04-2020
-- Purpose      Publishing a ResourceVersion that is currently in "Draft"
--
-- Modification History
--
-- 29-04-2020  Jignesh Jethwani	Initial Revision
-- 03-09-2020  Killian Davies	Modified to require initial ""SubmittedToPublishingQueue" status.
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionPublishing]
(
	@ResourceVersionId int,
	@UserId int
)

AS

BEGIN

	BEGIN TRY
			
		DECLARE @CurrentVersionStatusId int

		SELECT @CurrentVersionStatusId = VersionStatusId
		FROM resources.ResourceVersion
		WHERE Id = @ResourceVersionId

		-- Resource version must be in 'SubmittedToPublishingQueue' status to be set to Publishing
		IF @CurrentVersionStatusId <> 5
		BEGIN
			RAISERROR ('Error - ResourceVersion must be in "SubmittedToPublishingQueue" to be set to Publishing status', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN
		
			UPDATE 
				resources.ResourceVersion
			SET
				VersionStatusId = 4 -- Publishing
			WHERE
				Id=@ResourceVersionId
			
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
