-------------------------------------------------------------------------------
-- Author       RS
-- Created      19-10-2021
-- Purpose      Move a resource down in the display order of a folder in real-time (Content Structure iteration 1)
--
-- Modification History
--
-- 19-10-2021  RS	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[MoveResourceDown] 
(
	@NodeId int,
	@ResourceId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	
	BEGIN TRY

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		-- Check the user is an editor of the Catalogue
		DECLARE @EditorRoleId int = 1 -- Editor Role
		DECLARE @CatalogueNodeId int
		DECLARE @ScopeId int
		SELECT @CatalogueNodeId = CatalogueNodeId FROM hierarchy.NodePath WHERE NodeId = @NodeId AND Deleted = 0
		SELECT @ScopeId = Id FROM hub.Scope WHERE CatalogueNodeId = @CatalogueNodeId AND Deleted = 0

		IF @CatalogueNodeId != 1 AND hub.UserIsInRole(@UserId, @EditorRoleId, @ScopeId) = 0
		BEGIN
			RAISERROR ('Access to catalogue denied',
					16,	-- Severity.  
					1	-- State.  
					); 
			RETURN;
		END

		-- Check if the catalogue is currently locked for editing by admin.
		IF EXISTS (SELECT Id FROM hierarchy.HierarchyEdit WHERE RootNodeId = @CatalogueNodeId AND HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */) AND Deleted = 0)
		BEGIN
			RAISERROR ('Unable to move the resource down because the catalogue is currently locked for editing',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Validate VersionStatusID - can't move drafts up or down within folder.
		DECLARE @CurrentDisplayOrder int 
		SELECT	@CurrentDisplayOrder = DisplayOrder
		FROM	hierarchy.NodeResource 
		WHERE	NodeId = @NodeId AND ResourceId = @ResourceId AND Deleted = 0

		-- Check that the resource isn't already bottom of the list.
		IF NOT EXISTS(SELECT 1 FROM hierarchy.NodeResource WHERE NodeId = @NodeId AND Deleted = 0 AND DisplayOrder = @CurrentDisplayOrder + 1)
		BEGIN
			RAISERROR ('Cannot move the resource down in the folder display order',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		BEGIN TRAN

			-- All validation checks passed. Switch the display orders.
			UPDATE hierarchy.NodeResource SET DisplayOrder = DisplayOrder - 1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE NodeId = @NodeId AND DisplayOrder = @CurrentDisplayOrder + 1 AND Deleted = 0
			UPDATE hierarchy.NodeResource SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE NodeId = @NodeId AND ResourceId = @ResourceId AND Deleted = 0

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