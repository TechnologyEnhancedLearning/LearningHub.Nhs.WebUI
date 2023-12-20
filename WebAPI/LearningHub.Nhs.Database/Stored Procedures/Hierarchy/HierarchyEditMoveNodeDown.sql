-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Move node down within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditMoveNodeDown] 
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

		DECLARE @siblingHierarchyEditDetailId int
		SELECT 
			@SiblingHierarchyEditDetailId = hedSibling.Id
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN
			[hierarchy].[HierarchyEditDetail] hedSibling ON hed.HierarchyEditId = hedSibling.HierarchyEditId
															AND hed.ParentNodeId = hedSibling.ParentNodeId 
															AND hedSibling.DisplayOrder = hed.DisplayOrder + 1
															AND hedSibling.ResourceId IS NULL
		WHERE
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0
			AND hedSibling.Deleted = 0

		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE HierarchyEditDetailOperationId END,
			DisplayOrder = CASE 
							WHEN hed.Id = @HierarchyEditDetailId THEN DisplayOrder + 1
							WHEN hed.Id = @SiblingHierarchyEditDetailId THEN DisplayOrder - 1
							END,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON hed.NodeVersionId = fnv.NodeVersionId
		WHERE	
			(hed.Id = @HierarchyEditDetailId
			OR hed.Id = @SiblingHierarchyEditDetailId)
			AND hed.Deleted = 0
			AND fnv.Deleted = 0

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