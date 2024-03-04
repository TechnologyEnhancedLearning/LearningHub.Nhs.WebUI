-------------------------------------------------------------------------------
-- Author       RS
-- Created      19-10-2021
-- Purpose      Move a resource into a different folder in real-time (Content Structure iteration 1)
--
-- Modification History
--
-- 19-10-2021  RS	Initial Revision.
-- 01-09-2023  SA	Changes for the Catalogue structure - folders always displayed at the top.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[MoveResource] 
(
	@SourceNodeId int,
	@DestinationNodeId int,
	@ResourceId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	
	BEGIN TRY

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		-- Check the user is an editor of the destination catalogue
		DECLARE @EditorRoleId int = 1 -- Editor Role
		DECLARE @CatalogueNodeId int
		DECLARE @ScopeId int
		SELECT @CatalogueNodeId = CatalogueNodeId FROM hierarchy.NodePath WHERE NodeId = @DestinationNodeId AND Deleted = 0
		SELECT @ScopeId = Id FROM hub.Scope WHERE CatalogueNodeId = @CatalogueNodeId AND Deleted = 0

		IF @CatalogueNodeId != 1 AND hub.UserIsInRole(@UserId, @EditorRoleId, @ScopeId) = 0
		BEGIN
			RAISERROR ('Access to destination catalogue denied',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Check the user is an editor of the source catalogue. In reality the source and destination catalogues will be the same, so this is just a safeguard.
		DECLARE @SourceCatalogueNodeId int, @SourceDisplayOrder int
		SELECT	@SourceCatalogueNodeId = np.CatalogueNodeId,
				@SourceDisplayOrder = nr.DisplayOrder
		FROM	hierarchy.NodeResource nr 
				INNER JOIN hierarchy.NodePath np ON np.NodeId = nr.NodeId 
		WHERE nr.NodeId = @SourceNodeId AND nr.ResourceId = @ResourceId and nr.Deleted = 0 and np.Deleted = 0 and np.IsActive = 1

		SELECT @ScopeId = Id FROM hub.Scope WHERE CatalogueNodeId = @SourceCatalogueNodeId AND Deleted = 0
		IF @SourceCatalogueNodeId != 1 AND hub.UserIsInRole(@UserId, @EditorRoleId, @ScopeId) = 0
		BEGIN
			RAISERROR ('Access to source catalogue denied',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Check if the catalogue is currently locked for editing by admin.
		IF EXISTS (SELECT Id FROM hierarchy.HierarchyEdit WHERE RootNodeId = @CatalogueNodeId AND HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */) AND Deleted = 0)
		BEGIN
			RAISERROR ('Unable to move the resource because the catalogue is currently locked for editing',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Check that the resource does in fact exist in the specified node.
		IF NOT EXISTS (SELECT 1 FROM [hierarchy].[NodeResource] WHERE NodeId = @SourceNodeId AND ResourceId = @ResourceId AND Deleted = 0)
		BEGIN
			RAISERROR ('The resource is not located in the specified node',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Different implementation required for draft and published resources.
		DECLARE @VersionStatusId int, @DestinationHierarchyEditDetailId INT,@SourceHierarchyEditDetailId INT, @HierarchyEditId INT
		SELECT	@VersionStatusId = VersionStatusId 
		FROM	hierarchy.NodeResource 
		WHERE	NodeId = @SourceNodeId AND ResourceId = @ResourceId AND Deleted = 0

		IF @VersionStatusId > 1
		BEGIN
			-- For published resources move the resource via the existing publishing proc.
			EXECUTE hierarchy.NodeResourcePublish @DestinationNodeId, @ResourceId, NULL, @UserId
		END
		ELSE
		BEGIN
			-- For draft resources, just update the existing NodeResource. No NodeResourceLookup records to maintain.
			UPDATE	hierarchy.NodeResource
			SET 	NodeId = @DestinationNodeId,
					DisplayOrder = 
						CASE @DestinationNodeId 
							WHEN 1 THEN NULL 
							ELSE 1 
						END,
					AmendDate	=	@AmendDate,
					AmendUserId	=	@UserId
			WHERE	ResourceId = @ResourceId
					AND	VersionStatusId = 1
					AND	Deleted = 0

			SELECT @SourceHierarchyEditDetailId = Id, @HierarchyEditId = HierarchyEditId  FROM [hierarchy].[HierarchyEditDetail] Where NodeId = @SourceNodeId and ResourceId = @ResourceId
			SELECT @DestinationHierarchyEditDetailId = Id  FROM [hierarchy].[HierarchyEditDetail] Where NodeId = @DestinationNodeId 
			IF @DestinationNodeId > 1
			BEGIN
				-- Update NodeResources in destination node which will appear after this one - increment by 1.
				UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @UserId 
				WHERE NodeId = @DestinationNodeId AND Deleted = 0  AND ResourceId != @ResourceId
			END

			IF @SourceNodeId > 1
			BEGIN
				-- Update NodeResources in source node which appeared after this one - decrement by 1.
				UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder - 1, AmendDate = @AmendDate, AmendUserId = @UserId 
				WHERE NodeId = @SourceNodeId AND Deleted = 0 AND DisplayOrder > @SourceDisplayOrder

				UPDATE [hierarchy].[HierarchyEditDetail] SET NodeId = @DestinationNodeId, DisplayOrder = DisplayOrder + 1 ,AmendDate = @AmendDate, AmendUserId = @UserId 
				WHERE NodeId = @SourceNodeId AND Id = @SourceHierarchyEditDetailId

				UPDATE HED SET HED.DisplayOrder = NR.DisplayOrder 
				 FROM [hierarchy].[HierarchyEditDetail] HED INNER JOIN 
				 hierarchy.NodeResource NR ON NR.NodeId = HED.ParentNodeId AND NR.ResourceId= HED.ResourceId
				 WHERE HED.ParentNodeId = @SourceNodeId AND NR.Deleted = 0 AND HED.HierarchyEditId = @HierarchyEditId
			END
		END

		-- Return the node IDs that this move operation affects (the ones that the UI will need to refresh on screen)
		SELECT DISTINCT
			npn.NodeId
		FROM
			hierarchy.NodePath np
		INNER JOIN
			hierarchy.NodePathNode npn ON np.Id = npn.NodePathId
		WHERE
			(np.NodeId = @SourceNodeId OR np.NodeId = @DestinationNodeId)
			AND np.Deleted = 0
			AND npn.Deleted = 0
	
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