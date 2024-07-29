-------------------------------------------------------------------------------
-- Author       RS
-- Created      13-01-2022
-- Purpose      Move resource down within a Hierarchy Edit.
--
-- Modification History
--
-- 13-01-2022  RS	Initial Revision.
-- 29-07-2024  DB	Updated to use parent node id to find sibling.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditMoveResourceDown] 
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
															AND hedSibling.ResourceId IS NOT NULL
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