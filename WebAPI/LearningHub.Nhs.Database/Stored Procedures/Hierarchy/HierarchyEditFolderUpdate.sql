-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Update a Folder within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditFolderUpdate]
(
	@HierarchyEditDetailId int,
	@Name nvarchar(255), 
	@Description nvarchar(4000), 
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		-- Update folder details.
		-- IT1: maintaining a single published node version.
		-- Future iterations will require refactoring to create a new 'draft' node version to cater for updates.
		UPDATE
			fnv
		SET
			[Name] = @Name,
			[Description] = @Description,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.FolderNodeVersion fnv
		INNER JOIN
			 [hierarchy].[HierarchyEditDetail] hed ON hed.NodeVersionId = fnv.NodeVersionId
		WHERE	
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0
			AND fnv.Deleted = 0

		UPDATE 
			hed
		SET
			[HierarchyEditDetailOperationId] = 
				CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE HierarchyEditDetailOperationId END,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON hed.NodeVersionId = fnv.NodeVersionId
		WHERE	
			hed.Id = @HierarchyEditDetailId
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