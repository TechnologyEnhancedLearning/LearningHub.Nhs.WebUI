﻿-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Move Resource within a Hierarchy Edit.
--
-- Modification History
--
-- 11-01-2022  KD	Initial Revision.
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
															  AND hed.NodeId = hed_moveFrom.NodeId
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
															AND hed_moveTo_children.NodeId = hed_moveTo.NodeId
		WHERE	
			hed_moveTo.Id = @MoveToHierarchyEditDetailId
			AND hed_moveTo_children.ResourceId IS NOT NULL
			AND hed_moveTo.Deleted = 0
			AND hed_moveTo_children.Deleted = 0

		-- Move the resource.
		-- Is there an existing link between the Node and Resource (i.e. from delete / move away & reinstate scenario)
		DECLARE @nodeResourceId int
		SELECT 
			@nodeResourceId = Id
		FROM 
			hierarchy.NodeResource
		WHERE
			NodeId = (SELECT NodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @MoveToHierarchyEditDetailId)
			AND ResourceId = @ResourceId
			AND Deleted = 0

		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
			NodeId = (SELECT NodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @MoveToHierarchyEditDetailId),
			NodeVersionId = (SELECT NodeVersionId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @MoveToHierarchyEditDetailId),
			DisplayOrder = 1,
			NodeResourceId = CASE WHEN @nodeResourceId IS NOT NULL THEN @nodeResourceId ELSE hed.NodeResourceId END,
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