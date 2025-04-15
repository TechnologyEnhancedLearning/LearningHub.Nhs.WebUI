-------------------------------------------------------------------------------
-- Author      Swapnamol Abraham
-- Created      07-04-2025
-- Purpose      Creates a External System user
--
-- Modification History
--
-- 07-04-2025  SA	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [external].[ExternalSystemUserCreate]
(
	@userId int,
	@externalSystemId	int,
	@amendUserId		INT,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	BEGIN TRY
		BEGIN TRAN
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
			 -- Insert into ExternalSystemUser table
                INSERT INTO [external].[ExternalSystemUser] 
                    (UserId, ExternalSystemId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                SELECT @userId, @externalSystemId, 0, @amendUserId, @amendDate, @amendUserId, @amendDate

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