-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Creates a Folder within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 22-04-2024  DB	Included NULL NodeId and UserId in call to [hierarchy].[FolderNodeVersionCreate].
-- 15-05-2024  DB	Accept @ParentNodePathId as input parameter, create NodePath and populate NodePathId and ParentNodePathId in HierarchyEditDetail.
-- 10-07-2024  DB	Added PrimaryCatalogueNodeId to the NodeVersion table.
-- 04-09-2024  SA   The problem with creating the subfolder has been resolved.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditFolderCreate]
(
	@HierarchyEditId int,
	@Name nvarchar(255), 
	@Description nvarchar(4000),
	@ParentNodePathId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@HierarchyEditDetailId int OUTPUT
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @CreatedNodeId int
		DECLARE @CreatedNodeVersionId int
		DECLARE @ParentNodeId int
		DECLARE @ParentNodePath varchar(256)
		DECLARE @PrimaryCatalogueNodeId INT

		SELECT	@ParentNodeId = NodeId,
				@ParentNodePath = ISNULL(NewNodePath, InitialNodePath),
				@PrimaryCatalogueNodeId = PrimaryCatalogueNodeId
		FROM	hierarchy.HierarchyEditDetail
		WHERE	HierarchyEditId = @HierarchyEditId
			AND NodePathId = @ParentNodePathId
			AND HierarchyEditDetailTypeId = 4 -- Node Link
			AND Deleted = 0

		-- Sub folder node creation (HierarchyEditDetailTypeId 3)

		IF @ParentNodeId IS NULL
		BEGIN

		SELECT	@ParentNodeId = NodeId,
				@ParentNodePath = NodePath
		FROM hierarchy.NodePath
		WHERE Id = @ParentNodePathId
			 AND Deleted = 0

        SELECT TOP 1
				 @PrimaryCatalogueNodeId = PrimaryCatalogueNodeId
		FROM	hierarchy.HierarchyEditDetail
		WHERE	HierarchyEditId = @HierarchyEditId
			   AND Deleted = 0
		END

		EXECUTE [hierarchy].[FolderNodeVersionCreate] NULL, @Name, @Description, @PrimaryCatalogueNodeId, @UserId, @CreatedNodeVersionId OUTPUT

		SELECT @CreatedNodeId = NodeId FROM hierarchy.NodeVersion WHERE Id = @CreatedNodeVersionId

		-- Create new NodePath for the new Folder Node.
		DECLARE @NewNodePathId int
		INSERT INTO [hierarchy].[NodePath](NodeId, NodePath, CatalogueNodeId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)	
        SELECT 
			@CreatedNodeId AS NodeId,
			CONCAT(NodePath, '\', CAST(@CreatedNodeId AS VARCHAR(10))) AS NodePath,
			CatalogueNodeId,
			0 AS IsActive,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
        FROM hierarchy.NodePath
        WHERE Id = @ParentNodePathId

		SET @NewNodePathId = SCOPE_IDENTITY()

		-- Create new HierarchyEditDetail with details of the link to parent node.
		UPDATE	[hierarchy].[HierarchyEditDetail]
		SET		DisplayOrder = DisplayOrder + 1
		WHERE	ParentNodeId = @ParentNodeId
			AND HierarchyEditId = @HierarchyEditId
			AND ResourceId IS NULL

		INSERT INTO [hierarchy].[HierarchyEditDetail](HierarchyEditId,
														HierarchyEditDetailTypeId,
														HierarchyEditDetailOperationId,
														PrimaryCatalogueNodeId,
														NodeId,
														NodePathId,
														NodeVersionId,
														ParentNodeId,
														ParentNodePathId,
														NodeLinkId,
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
			3 AS HierarchyEditDetailTypeId,			-- Folder Node
			1 AS HierarchyEditDetailOperationId,	-- Add
			@PrimaryCatalogueNodeId AS PrimaryCatalogueNodeId,
			NodeId,
			@NewNodePathId AS NodePathId,
			@CreatedNodeVersionId AS NodeVersionId,
			@ParentNodeId AS ParentNodeId,
			@ParentNodePathId AS ParentNodePathId,
			NULL AS NodeLinkId,
			1 AS DisplayOrder,
			NULL AS InitialNodePath,
			CONCAT(@ParentNodePath, '\', CAST(@CreatedNodeId AS VARCHAR(10))) AS NewNodePath,
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