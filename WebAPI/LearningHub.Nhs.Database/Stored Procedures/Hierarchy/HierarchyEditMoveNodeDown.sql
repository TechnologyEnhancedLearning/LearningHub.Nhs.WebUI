-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Move node down within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 01-09-2023  SA	Changes for the Catalogue structure - folders always displayed at the top.
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

		DECLARE @siblingHierarchyEditDetailId int, @parentnodeId INT ,@currentDisplayOrder INT,@HierarchyEditId INT

		SELECT @parentnodeId = ParentNodeId,@CurrentDisplayOrder = DisplayOrder,@HierarchyEditId = HierarchyEditId from [hierarchy].[HierarchyEditDetail] Where Id = @HierarchyEditDetailId

		SELECT @SiblingHierarchyEditDetailId = Id FROM [hierarchy].[HierarchyEditDetail] 
										WHERE (ParentNodeId = @parentnodeId OR (ParentNodeId IS NULL AND ResourceId IS NOT NULL)) 
										AND HierarchyEditId = @HierarchyEditId AND DisplayOrder = (@CurrentDisplayOrder + 1)
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
		WHERE	
			(hed.Id = @HierarchyEditDetailId
			OR hed.Id = @SiblingHierarchyEditDetailId)
			AND hed.Deleted = 0

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