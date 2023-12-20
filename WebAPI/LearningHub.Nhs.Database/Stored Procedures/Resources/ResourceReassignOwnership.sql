-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Reassigns the ownership of resource versions associated with the supplied reference.
--
-- Modification History
--
-- 27-05-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceReassignOwnership]
(
	@ResourceId int,
	@NewOwnerUserName nvarchar(50),
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	IF NOT EXISTS (SELECT 'X' FROM hub.[User] WHERE Id=@UserId AND Deleted=0)
	BEGIN
		RAISERROR ('Error - Please supply a valid UserId for auditing purposes', -- Message text.  
				16, -- Severity.  
				1 -- State.  
				); 
		RETURN;
	END

	--------------------------------------------------------------------
	-- Validate supplied Owner User details
	--------------------------------------------------------------------
	DECLARE @NewOwnerUserId int

	SELECT @NewOwnerUserId = Id FROM hub.[User] WHERE [UserName]=@NewOwnerUserName AND Deleted=0

	IF @NewOwnerUserId IS NULL
	BEGIN
		RAISERROR ('Error - New Owner User does not exist in hub.[User]', -- Message text.  
				16, -- Severity.  
				1 -- State.  
				); 
		RETURN;
	END

	BEGIN TRY
	
		BEGIN TRAN

		DECLARE @ResourceVersionId int
		DECLARE @ResourceVersionCursor as CURSOR
 
		SET @ResourceVersionCursor = CURSOR FORWARD_ONLY FOR
		SELECT
			Id
		FROM
			resources.ResourceVersion
		WHERE
			ResourceId=@ResourceId
			AND Deleted=0

		OPEN @ResourceVersionCursor;
		FETCH NEXT FROM @ResourceVersionCursor INTO @ResourceVersionId;
		WHILE @@FETCH_STATUS = 0
		BEGIN

			EXECUTE [resources].[ResourceVersionReassignOwnership] @ResourceVersionId, @NewOwnerUserId, @UserId, @UserTimezoneOffset
			FETCH NEXT FROM @ResourceVersionCursor INTO @ResourceVersionId;

		END

		CLOSE @ResourceVersionCursor;
		DEALLOCATE @ResourceVersionCursor;
		
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

		RAISERROR (@ErrorMessage, -- Message text.  
			@ErrorSeverity, -- Severity.  
			@ErrorState -- State.  
			);  
	END CATCH

END

GO