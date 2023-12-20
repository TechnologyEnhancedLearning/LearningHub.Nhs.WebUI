-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Move node within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 05-01-2021  KD	IT2 - refresh hierarchy.HierarchyEditNodeResourceLookup if required.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditMoveNode]
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
		DECLARE @NodeId int
		SELECT @HierarchyEditId = HierarchyEditId, @NodeId = NodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @HierarchyEditDetailId

		-- Decrement display order of sibling nodes with higher display order.
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
			[hierarchy].[HierarchyEditDetail] hed_moveFrom ON hed.HierarchyEditId = hed_moveFrom.HierarchyEditId AND hed.ParentNodeId = hed_moveFrom.ParentNodeId AND hed.Id != hed_moveFrom.Id
		WHERE	
			hed_moveFrom.Id = @HierarchyEditDetailId
			AND hed.DisplayOrder > hed_moveFrom.DisplayOrder
			AND hed.ResourceId IS NULL
			AND hed.Deleted = 0
			AND hed_moveFrom.Deleted = 0

		-- Increment display order of nodes in destination.
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
															AND hed_moveTo_children.ParentNodeId = hed_moveTo.NodeId
															AND ISNULL(hed_moveTo_children.HierarchyEditDetailOperationId, 0) != 3 -- ignore deletes.
		WHERE	
			hed_moveTo.Id = @MoveToHierarchyEditDetailId
			AND hed_moveTo.ResourceId IS NULL
			AND hed_moveTo.Deleted = 0
			AND hed_moveTo_children.Deleted = 0

		-- Move the node.
		-- Is there an existing NodeLink between the Nodes (i.e. from delete / move away & reinstate scenario)
		DECLARE @nodeLinkId int
		SELECT 
			@nodeLinkId = Id
		FROM 
			hierarchy.NodeLink
		WHERE
			ParentNodeId = (SELECT NodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @MoveToHierarchyEditDetailId)
			AND ChildNodeId = (SELECT NodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @hierarchyEditDetailId)
			AND Deleted = 0

		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
			ParentNodeId = (SELECT NodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @MoveToHierarchyEditDetailId),
			DisplayOrder = 1,
			NodeLinkId = CASE WHEN @nodeLinkId IS NOT NULL THEN @nodeLinkId ELSE hed.NodeLinkId END,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		WHERE	
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0

		------------------------------------------------------------ 
		-- Refresh HierarchyEditNodeResourceLookup
		------------------------------------------------------------
		IF EXISTS (SELECT 'X' FROM hierarchy.HierarchyEditNodeResourceLookup WHERE HierarchyEditId = @HierarchyEditId AND NodeId = @NodeId)
		BEGIN
			EXEC hierarchy.HierarchyEditRefreshNodeResourceLookup @HierarchyEditId
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