-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Deletes a User Group and associated data.
--
-- Modification History
--
-- 21-01-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[UserGroupDelete]
(
	@UserGroupId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	BEGIN TRY
		BEGIN TRAN
			DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

			UPDATE 
				hub.UserGroupAttribute 
			SET		
				Deleted = 1,
				AmendDate = @AmendDate,
				AmendUserId = @UserId
			WHERE 
				UserGroupId = @UserGroupId
				AND Deleted = 0

			UPDATE 
				hub.RoleUserGroup 
			SET		
				Deleted = 1,
				AmendDate = @AmendDate,
				AmendUserId = @UserId
			WHERE 
				UserGroupId = @UserGroupId
				AND Deleted = 0

			UPDATE 
				hub.UserUserGroup 
			SET		
				Deleted = 1,
				AmendDate = @AmendDate,
				AmendUserId = @UserId
			WHERE 
				UserGroupId = @UserGroupId
				AND Deleted = 0
				
			UPDATE 
				hub.UserGroup 
			SET		
				Deleted = 1,
				AmendDate = @AmendDate,
				AmendUserId = @UserId
			WHERE 
				Id = @UserGroupId
				AND Deleted = 0

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