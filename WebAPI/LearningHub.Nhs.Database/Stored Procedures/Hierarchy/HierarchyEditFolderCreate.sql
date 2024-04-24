-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Creates a Folder within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 22-04-2024  DB	Included NULL NodeId in call to [hierarchy].[FolderNodeVersionCreate].
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditFolderCreate]
(
	@HierarchyEditId int,
	@Name nvarchar(255), 
	@Description nvarchar(4000), 
	@ParentNodeId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@HierarchyEditDetailId int OUTPUT
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @CreatedNodeVersionId int

		EXECUTE [hierarchy].[FolderNodeVersionCreate] NULL, @Name, @Description, @CreatedNodeVersionId OUTPUT

		-- Create new HierarchyEditDetail with details of the link to parent node.
		UPDATE [hierarchy].[HierarchyEditDetail] SET DisplayOrder = DisplayOrder + 1 WHERE ParentNodeId = @ParentNodeId AND HierarchyEditId = @HierarchyEditId AND ResourceId IS NULL

		INSERT INTO [hierarchy].[HierarchyEditDetail](HierarchyEditId,
														HierarchyEditDetailTypeId,
														HierarchyEditDetailOperationId,
														NodeId,
														NodeVersionId,
														ParentNodeId,
														NodeLinkId,
														DisplayOrder,
														InitialNodePath,
														Deleted,
														CreateUserId,
														CreateDate,
														AmendUserId,
														AmendDate)
		SELECT
			@HierarchyEditId, 
			3 AS HierarchyEditDetailTypeId,			-- Folder Node
			1 AS HierarchyEditDetailOperationId,	-- Add
			NodeId,
			@CreatedNodeVersionId AS NodeVersionId,
			@ParentNodeId AS ParentNodeId,
			NULL AS NodeLinkId,
			1 AS DisplayOrder,
			NULL AS InitialNodePath,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM 
			hierarchy.NodeVersion
		WHERE
			Id = @CreatedNodeVersionId

		SELECT @HierarchyEditDetailId = SCOPE_IDENTITY()

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