-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      18-08-2020
-- Purpose      Set ResourceVersion to "Failed to Publish"
--
-- Modification History
--
-- 18-08-2020	Killian Davies	Initial Revision
-- 18-11-2020   Killian Davies	Comment out check for current status = "Publishing"
--								Reinstate when db publish and findwise publish have been refatored to Azure Function
-- 11-12-2020	Dave Brown		Previous FailedToPublish records marked as deleted
-- 01-04-2021	RobS			Added call to UpdateMigrationStatus stored proc for migration tool improvements.
-- 25-05-2021   RobS            Fixed deadlock issue when called as a result of the migration tool.
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionFailedToPublish]
(
	@ResourceVersionId int,
	@UserId int
)

AS

BEGIN

	BEGIN TRY
			
		DECLARE @CurrentVersionStatusId int
		DECLARE @ResourceId int

		SELECT @CurrentVersionStatusId = VersionStatusId,
				@ResourceId = ResourceId
		FROM resources.ResourceVersion
		WHERE Id = @ResourceVersionId

		-- Resource version must be in 'Publishing' status to be set to FailedToPublish
		-- TODO - reinstate when db publish and findwise publish have been refatored to Azure Function
		--IF @CurrentVersionStatusId <> 4
		--BEGIN
		--	RAISERROR ('Error - ResourceVersion must be in "Publishing" to be set to "FailedToPublish" status', -- Message text.  
		--			   16, -- Severity.  
		--			   1 -- State.  
		--			   );  
		--END

		BEGIN TRAN
		
			-- Mark the resource version as failed to publish. Mark all other failed versions of same resource as deleted.
			UPDATE 
				resources.ResourceVersion
			SET
				VersionStatusId = 
					case 
						when Id=@ResourceVersionId then 6 -- Failed to Publish
						else VersionStatusId 
					end, 
				Deleted = 
					case 
						when VersionStatusId=6 then 1 -- Mark previously failed versions as deleted.
						else Deleted
					end
			WHERE
				ResourceId=@ResourceId
						
			-- If this resource version was created via the Migration Tool, we need to update the status of the corresponding MigrationInputRecord.
			EXEC migrations.UpdateMigrationStatus @ResourceVersionId, 0

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
