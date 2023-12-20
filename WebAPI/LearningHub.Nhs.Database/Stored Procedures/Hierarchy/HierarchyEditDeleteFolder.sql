-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Delete a folder within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 04-10-2021  KD   Correction for updating display order of sibling nodes on delete.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditDeleteFolder] 
(
	@HierarchyEditDetailId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
		
		-- Decrement display order of sibling nodes with higher display order
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
			[hierarchy].[HierarchyEditDetail] hed_delete ON hed.HierarchyEditId = hed_delete.HierarchyEditId AND hed.ParentNodeId = hed_delete.ParentNodeId
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON hed.NodeVersionId = fnv.NodeVersionId
		WHERE	
			hed_delete.Id = @HierarchyEditDetailId
			AND hed.DisplayOrder > hed_delete.DisplayOrder
			AND hed.ResourceId IS NULL
			AND hed.Deleted = 0
			AND fnv.Deleted = 0

		-- Delete folder node:
		-- If the folder node is newly added within the hierarchy edit then delete, otherwise
		-- mark for a 'Delete' operation.
		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 3 END, -- Delete
			Deleted = CASE WHEN HierarchyEditDetailOperationId = 1 THEN 1 ELSE Deleted END, -- Delete
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		WHERE	
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0

		UPDATE 
			fnv
		SET
			Deleted = 1,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.FolderNodeVersion fnv
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON fnv.NodeVersionId = hed.NodeVersionId
		WHERE
			hed.Id = @HierarchyEditDetailId
			AND hed.HierarchyEditDetailOperationId = 1

		UPDATE 
			nv
		SET
			Deleted = 1,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.NodeVersion nv
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON nv.Id = hed.NodeVersionId
		WHERE
			hed.Id = @HierarchyEditDetailId
			AND hed.HierarchyEditDetailOperationId = 1

		UPDATE 
			n
		SET
			Deleted = 1,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.[Node] n
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON n.Id = hed.NodeId
		WHERE
			hed.Id = @HierarchyEditDetailId
			AND hed.HierarchyEditDetailOperationId = 1

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