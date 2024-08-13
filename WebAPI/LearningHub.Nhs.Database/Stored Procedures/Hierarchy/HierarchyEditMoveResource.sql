-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Move Resource within a Hierarchy Edit.
--
-- Modification History
--
-- 11-01-2022  KD	Initial Revision.
-- 08-08-2024 SA    Remove all instance of the referenced path when moving a referenced resource
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditMoveResource]
(
	@HierarchyEditDetailId int,
	@MoveToHierarchyEditDetailId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @HierarchyEditId int
		DECLARE @ResourceId int
		SELECT @HierarchyEditId = HierarchyEditId, @ResourceId = ResourceId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @HierarchyEditDetailId

		-- Decrement display order of sibling resources with higher display order.
		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END,
			DisplayOrder = hed.DisplayOrder - 1,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN
			[hierarchy].[HierarchyEditDetail] hed_moveFrom ON hed.HierarchyEditId = hed_moveFrom.HierarchyEditId 
															  AND hed.ParentNodeId = hed_moveFrom.ParentNodeId
															  AND hed.Id != hed_moveFrom.Id
		WHERE	
			hed_moveFrom.Id = @HierarchyEditDetailId
			AND hed.ResourceId IS NOT NULL
			AND hed.DisplayOrder > hed_moveFrom.DisplayOrder
			AND hed.Deleted = 0
			AND hed_moveFrom.Deleted = 0

		-- Increment display order of resources in destination.
		UPDATE  
			hed_moveTo_children 
		SET
			HierarchyEditDetailOperationId = CASE WHEN hed_moveTo_children.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed_moveTo_children.HierarchyEditDetailOperationId END,
			DisplayOrder = hed_moveTo_children.DisplayOrder + 1,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed_moveTo
		INNER JOIN
			[hierarchy].[HierarchyEditDetail] hed_moveTo_children ON hed_moveTo_children.HierarchyEditId = hed_moveTo.HierarchyEditId
															AND hed_moveTo_children.ParentNodeId = hed_moveTo.ParentNodeId
		WHERE	
			hed_moveTo.Id = @MoveToHierarchyEditDetailId
			AND hed_moveTo_children.ResourceId IS NOT NULL
			AND hed_moveTo.Deleted = 0
			AND hed_moveTo_children.Deleted = 0

		DECLARE @NewParentNodeId int
		DECLARE @NewParentNodePathId int
		DECLARE @NewParentNodePath varchar(256)
		SELECT 
			@NewParentNodeId = NodeId,
			@NewParentNodePathId = NodePathId,
			@NewParentNodePath = ISNULL(NewNodePath, InitialNodePath)
		FROM
			[hierarchy].[HierarchyEditDetail]
		WHERE
			Id = @MoveToHierarchyEditDetailId

		-- Move the resource.
		-- Is there an existing link between the Node and Resource (i.e. from delete / move away & reinstate scenario)
		DECLARE @nodeResourceId int
		SELECT 
			@nodeResourceId = Id
		FROM 
			hierarchy.NodeResource
		WHERE
			NodeId = @NewParentNodeId
			AND ResourceId = @ResourceId
			AND Deleted = 0

		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
			ParentNodeId = @NewParentNodeId,
			ParentNodePathId = @NewParentNodePathId,
			DisplayOrder = 1,
			NodeResourceId = @nodeResourceId,
			NewNodepath = @NewParentNodePath,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		WHERE	
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0


		-- Create the new ResourceReference record resulting from the move (Created as IsActive = 0).
		INSERT INTO resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT  hed.ResourceId, hed.ParentNodePathId, o_rr.OriginalResourceReferenceId, 0 AS IsActive, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReference o_rr ON hed.ResourceReferenceId = o_rr.Id AND o_rr.Deleted = 0
		LEFT OUTER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
		WHERE 
			hed.Id = @HierarchyEditDetailId
			AND rr.Id IS NULL


		-- Update HierarchyEditDetail records with the new ResourceReferenceIds.
		UPDATE	hed
		SET		ResourceReferenceId = rr.Id,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
		WHERE	
			hed.Id = @HierarchyEditDetailId
			AND hed.ResourceReferenceId != rr.Id


		-- Create new ResourceReferenceDisplayVersion records where the ResourceReferenceId has changed. i.e. the ResourceReferenceId against the ResourceReferenceDisplayVersion record is different
		INSERT INTO resources.ResourceReferenceDisplayVersion (ResourceReferenceId, DisplayName, VersionStatusId, PublicationId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT hed.ResourceReferenceId, DisplayName, 1 /* Draft */, NULL, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON rrdv.Id = hed.ResourceReferenceDisplayVersionId
		WHERE	hed.Id = @HierarchyEditDetailId
			AND rrdv.ResourceReferenceId != hed.ResourceReferenceId
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0


		-- Delete any Draft and Unused ResourceReferenceDisplayVersion records (resulting from creation and then subsequent move of node to different ResourceReference)
		UPDATE	rrdv
		SET		Deleted = 1,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON rrdv.Id = hed.ResourceReferenceDisplayVersionId
		WHERE	hed.Id = @HierarchyEditDetailId
			AND rrdv.ResourceReferenceId != hed.ResourceReferenceId
			AND rrdv.VersionStatusId = 1 -- Draft
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0


		-- Update to the new ResourceReferenceDisplayVersion records where the ResourceReferenceId has changed.
		UPDATE	hed
		SET		ResourceReferenceDisplayVersionId = rrdv.Id,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON rrdv.ResourceReferenceId = hed.ResourceReferenceId
		WHERE	hed.Id = @HierarchyEditDetailId
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0

        EXEC hierarchy.HierarchyEditRemoveResourceReferencesOnMoveResource @HierarchyEditDetailId,@ParentNodeId,@UserId,@UserTimezoneOffset
		------------------------------------------------------------ 
		-- Refresh HierarchyEditNodeResourceLookup
		------------------------------------------------------------
		EXEC hierarchy.HierarchyEditRefreshNodeResourceLookup @HierarchyEditId

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