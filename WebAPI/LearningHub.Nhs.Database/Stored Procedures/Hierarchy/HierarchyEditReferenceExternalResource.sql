-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      02-07-2024
-- Purpose      References and external resource within a Hierarchy Edit.
--
-- Modification History
--
-- 02-07-2024  DB	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditReferenceExternalResource]
(
	@ResourceId int,
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
		SELECT	@HierarchyEditId = HierarchyEditId
		FROM	[hierarchy].[HierarchyEditDetail]
		WHERE	Id = @MoveToHierarchyEditDetailId

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

		-- Reference the resource.
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

		INSERT INTO [hierarchy].[HierarchyEditDetail]
		(
			HierarchyEditId,
			HierarchyEditDetailTypeId,
			HierarchyEditDetailOperationId,
			NodePathId,
			NodeId,
			NodeVersionId,
			InitialParentNodeId,
			ParentNodeId,
			ParentNodePathId,
			NodeLinkId,
			ResourceId,
			ResourceVersionId,
			ResourceReferenceId,
			NodeResourceId,
			DisplayOrder,
			InitialNodePath,
			NewNodePath,
			Deleted,
			CreateUserId,
			CreateDate,
			AmendUserId,
			AmendDate)
		SELECT
			@HierarchyEditId, 
			5 AS HierarchyEditDetailTypeId, -- Node Resource
			4 AS HierarchyEditDetailOperationId, -- Add Reference											
			NULL AS NodePathId,
			NULL AS NodeId,
			NULL AS NodeVersionId,
			NULL AS InitialParentNodeId,
			@NewParentNodeId AS ParentNodeId,
			@NewParentNodePathId AS ParentNodePathId,
			NULL AS NodeLinkId,
			r.Id AS ResourceId,
			rv.Id AS ResourceVersionId,
			NULL AS ResourceReferenceId,
			@nodeResourceId AS NodeResourceId,
			1 AS DisplayOrder,
			NULL AS InitialNodePath,
			@NewParentNodePath AS NewNodePath,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM
			[resources].[Resource] r
		INNER JOIN
			[resources].ResourceVersion rv ON rv.Id = r.CurrentResourceVersionId
		WHERE
			r.Id = @ResourceId
			AND rv.VersionStatusId = 2 -- Published
			AND r.Deleted = 0


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