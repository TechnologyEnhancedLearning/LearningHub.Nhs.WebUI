-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Reverts a ResourceVersion that is currently in "Publishing" to "Draft"
--
-- Modification History
--
-- 14-05-2020  Killian Davies	Initial Revision
-- 03-09-2020  Killian Davies	Modified to require initial ""Failed to Publish" status.
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-- 12-05-2023  Hari Vaka		Modified to include 'Plublishing' Status
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionRevertToDraft]
(
	@ResourceVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
		
		DECLARE @CurrentVersionStatusId int
		SELECT @CurrentVersionStatusId = VersionStatusId 
		FROM resources.ResourceVersion 
		WHERE Id=@ResourceVersionId AND Deleted=0

		IF @CurrentVersionStatusId NOT IN (4, 6)
		BEGIN
			RAISERROR ('Error - ResourceVersion can only revert to a status of ''Draft'' when it has a status of ''Failed to Publish'' or ''Publishing'' ', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN

		UPDATE resources.ResourceVersion SET VersionStatusId=1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE Id = @ResourceVersionId
		
		INSERT INTO [resources].[ResourceVersionEvent] ([ResourceVersionId],[ResourceVersionEventTypeId],[Details],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT @ResourceVersionId, 4, 'Reverted to Draft status', 0, @UserId, @AmendDate, @UserId, @AmendDate

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