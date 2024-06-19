-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      03-06-2024
-- Purpose      Creates or updates a NodePathDisplayVersion within a Hierarchy Edit.
--
-- Modification History
--
-- 03-06-2024  DB	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditNodePathDisplayVersionUpdate]
(
	@HierarchyEditDetailId int,
	@Name nvarchar(255), 
	@NodePathId int,
	@NodePathDisplayVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@NewNodePathDisplayVersionId int OUTPUT
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		IF EXISTS (SELECT 1 FROM hierarchy.NodePathDisplayVersion WHERE Id = @NodePathDisplayVersionId AND VersionStatusId = 1) -- Does a draft version already exist
		BEGIN
			UPDATE	hierarchy.NodePathDisplayVersion
			SET		DisplayName = @Name,
					AmendUserId = @UserId,
					AmendDate = @AmendDate
			WHERE	Id = @NodePathDisplayVersionId
			SET @NewNodePathDisplayVersionId = @NodePathDisplayVersionId
		END
		ELSE
		BEGIN
			INSERT INTO hierarchy.NodePathDisplayVersion (NodePathId, DisplayName, VersionStatusId, PublicationId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			VALUES (@NodePathId, @Name, 1, NULL, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET()) -- Draft VersionStatusId=1
			SELECT @NewNodePathDisplayVersionId = SCOPE_IDENTITY()

			UPDATE	hierarchy.HierarchyEditDetail
			SET		NodePathDisplayVersionId = @NewNodePathDisplayVersionId,
					HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
					AmendUserId = @UserId,
					AmendDate = @AmendDate
			WHERE	Id = @HierarchyEditDetailId
		END

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