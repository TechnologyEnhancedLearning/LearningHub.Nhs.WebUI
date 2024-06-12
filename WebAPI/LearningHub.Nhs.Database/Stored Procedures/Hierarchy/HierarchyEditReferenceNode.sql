-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      29-05-2024
-- Purpose      Create a reference to a node within a Hierarchy Edit.
--
-- Modification History
--
-- 29-04-2024  DB	Initial Revision.
-- 13-05-2024  DB	Set the parent node path id for the new reference node.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditReferenceNode]
(
	@HierarchyEditDetailId int,
	@ReferenceToHierarchyEditDetailId int,
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
		DECLARE @NewParentNodeId int
		DECLARE @NewParentNodePathId int
		DECLARE @RootNodeId INT

		SELECT	@HierarchyEditId = hed.HierarchyEditId,
				@NodeId = hed.NodeId,
                @RootNodeId = np.NodeId
		FROM [hierarchy].[HierarchyEditDetail] hed
        INNER JOIN [hierarchy].[HierarchyEdit] he ON he.Id = hed.HierarchyEditId
		INNER JOIN hierarchy.NodePath np ON he.RootNodePathId = np.Id
		WHERE hed.Id = @HierarchyEditDetailId
		
		SELECT	@NewParentNodeId = NodeId,
				@NewParentNodePathId = NodePathId
		FROM	[hierarchy].[HierarchyEditDetail]
		WHERE	Id = @ReferenceToHierarchyEditDetailId

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
			hed_moveTo.Id = @ReferenceToHierarchyEditDetailId
			AND hed_moveTo.ResourceId IS NULL
			AND hed_moveTo.Deleted = 0
			AND hed_moveTo_children.Deleted = 0


		-- Create the new Hierarchy Edit Detail records for the reference nodes and resources.
		DECLARE @OriginalNodePath NVARCHAR(256)
		DECLARE @OriginalParentNodePath NVARCHAR(256)
		DECLARE @DestinationParentNodePath NVARCHAR(256)

		SELECT  @OriginalNodePath = ISNULL(hed.NewNodePath, hed.InitialNodePath),
				@OriginalParentNodePath = ISNULL(p_hed.NewNodePath, p_hed.InitialNodePath)
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.HierarchyEditDetail p_hed ON hed.ParentNodePathId = p_hed.NodePathId AND hed.HierarchyEditId = p_hed.HierarchyEditId
		WHERE   hed.Id = @HierarchyEditDetailId

		SELECT  @DestinationParentNodePath = ISNULL(NewNodePath, InitialNodePath)
		FROM    hierarchy.HierarchyEditDetail
		WHERE   Id = @ReferenceToHierarchyEditDetailId

		INSERT INTO hierarchy.HierarchyEditDetail (HierarchyEditId,HierarchyEditDetailTypeId,HierarchyEditDetailOperationId,NodeId,NodePathId,NodeVersionId,ParentNodeId,ParentNodePathId,NodeLinkId,ResourceId,ResourceVersionId,ResourceReferenceId,NodeResourceId,DisplayOrder,InitialNodePath,NewNodePath,Deleted,CreateUserId,CreateDate,AmendUserId,AmendDate)
		SELECT  HierarchyEditId,
				HierarchyEditDetailTypeId, -- Folder Node (3) OR Node Link (4) OR OR Node Resource (5)
				4 AS HierarchyEditDetailOperationId,  -- Add Reference
				NodeId, 
				NULL as NodePathId, 
				NodeVersionId, 
				NULL AS ParentNodeId, -- Populated further down
				NULL AS ParentNodePathId, -- Populated further down
				NULL AS NodeLinkId, 
				ResourceId,
				ResourceVersionId,
				NULL AS ResourceReferenceId,
				NodeResourceId,
				DisplayOrder,
				NULL AS InitialNodePath,
				@DestinationParentNodePath + SUBSTRING(ISNULL(NewNodePath, InitialNodePath), LEN(@OriginalParentNodePath)+1, LEN(ISNULL(NewNodePath, InitialNodePath))) AS NewNodePath, -- Not using REPLACE incase number of digits in nodeIds are not consistant
				0 AS Deleted,
				@UserId AS CreateUserId,
				@AmendDate AS CreateDate,
				@UserId AS AmendUserId,
				@AmendDate AS AmendDate
		FROM hierarchy.HierarchyEditDetail
		where HierarchyEditID = @HierarchyEditID
			AND (HierarchyEditDetailTypeId = 3 OR HierarchyEditDetailTypeId = 4 OR HierarchyEditDetailTypeId = 5) -- Folder Node OR Node Link OR Node Resource
			AND ISNULL(NewNodePath, InitialNodePath) like @OriginalNodePath +'%'
			AND Deleted = 0

		-- Create the new NodePath records for the new referenced nodes (Created as IsActive = 0).
		INSERT INTO hierarchy.NodePath (NodeId, NodePath, CatalogueNodeId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT  hed.NodeId, ISNULL(hed.NewNodePath, hed.InitialNodePath), @RootNodeId AS CatalogueNodeId, 0 AS IsActive, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM hierarchy.HierarchyEditDetail hed
		LEFT OUTER JOIN hierarchy.NodePath np ON hed.NodeId = np.NodeId AND ISNULL(hed.NewNodePath, hed.InitialNodePath) = np.NodePath AND np.Deleted = 0
		WHERE hed.HierarchyEditID = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND (
				hed.HierarchyEditDetailTypeId = 3 -- Folder Node
				OR
				hed.HierarchyEditDetailTypeId = 4 -- Node Link
				)
			AND hed.NodePathId is NULL
			AND np.Id IS NULL

		-- Update HierarchyEditDetail records with the new NodePathIds.
		UPDATE	hed
		SET		NodePathId = np.Id,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.NodePath np ON hed.NodeId = np.NodeId AND ISNULL(hed.NewNodePath, hed.InitialNodePath) = np.NodePath AND np.Deleted = 0
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND hed.NodePathId is NULL

		-- Populate the Parent Node Path Ids for the new reference nodes.
		UPDATE  hed
		SET     ParentNodeId = p_hed.NodeId,
				ParentNodePathId = p_hed.NodePathId,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.HierarchyEditDetail p_hed ON ISNULL(hed.NewNodePath, hed.InitialNodePath) = ISNULL(p_hed.NewNodePath, p_hed.InitialNodePath) + '\' + CAST(hed.NodeId AS VARCHAR(10))
		WHERE   hed.HierarchyEditID = @HierarchyEditID
			AND p_hed.HierarchyEditId = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND (
				hed.HierarchyEditDetailTypeId = 3 -- Folder Node
				OR
				hed.HierarchyEditDetailTypeId = 4 -- Node Link
				)
			AND hed.ParentNodePathId IS NULL

		-- Populate the Parent Node Path Ids for the new reference resources.
		UPDATE  hed
		SET     ParentNodeId = p_hed.NodeId,
				ParentNodePathId = p_hed.NodePathId,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.HierarchyEditDetail p_hed ON ISNULL(hed.NewNodePath, hed.InitialNodePath) = ISNULL(p_hed.NewNodePath, p_hed.InitialNodePath)
		WHERE   hed.HierarchyEditID = @HierarchyEditID
			AND p_hed.HierarchyEditId = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
			AND p_hed.HierarchyEditDetailTypeId = 4 -- Node link
			AND hed.ParentNodePathId IS NULL

		-- Add existing NodeLink between the Nodes if one exists (i.e. from delete / move away & reinstate scenario)
		UPDATE  hed
		SET     NodeLinkId = nl.Id,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.NodeLink nl ON hed.ParentNodeId = nl.ParentNodeId AND hed.NodeId = nl.ChildNodeId AND nl.Deleted = 0
		WHERE   hed.HierarchyEditID = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND hed.HierarchyEditDetailTypeId = 4 -- Node link
			AND hed.NodeLinkId IS NULL

		------ Create new ResourceReference records for the new referenced resources.
		----INSERT INTO resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		----SELECT  hed.ResourceId, hed.NodePathId, NULL AS OriginalResourceReferenceId, 0, @UserId, @AmendDate, @UserId, @AmendDate
		----FROM hierarchy.HierarchyEditDetail hed
		----LEFT OUTER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.NodePathId = rr.NodePathId AND rr.Deleted = 0
		----WHERE hed.HierarchyEditID = @HierarchyEditID
		----	AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
		----	AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
		----	AND hed.ResourceReferenceId is NULL
		----	AND rr.Id IS NULL

		---- Create new ResourceReference records for the new referenced resources.
		--INSERT INTO resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		--SELECT  hed.ResourceId, hed.ParentNodePathId, NULL AS OriginalResourceReferenceId, 0, @UserId, @AmendDate, @UserId, @AmendDate
		--FROM hierarchy.HierarchyEditDetail hed
		--LEFT OUTER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
  --      LEFT OUTER JOIN hierarchy.NodePath np ON rr.NodePathId = np.Id 
  --      LEFT OUTER JOIN hierarchy.HierarchyEditDetail hed2 ON hed2.HierarchyEditID = @HierarchyEditID
  --                                                          AND hed2.HierarchyEditDetailTypeId = 5 -- Node Resource
  --                                                          AND hed2.Id != hed.Id
  --                                                          AND np.NodePath = ISNULL(hed2.NewNodePath, hed2.InitialNodePath)
		--WHERE hed.HierarchyEditID = @HierarchyEditID
		--	AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
		--	AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
		--	AND hed.ResourceReferenceId is NULL
		--	AND (
  --              rr.Id IS NULL
  --              OR
  --              hed2.Id IS NULL-- If rr.Id is NOT NULL check if the original resource has been moved, so the original ResourceReference will be deleted during publish
  --          )

		---- Update the HierarcyEditDetail records with the new ResourceReferenceIds.
		--UPDATE  hed
		--SET     ResourceReferenceId = MAX(rr.Id), -- MAX is needed as the resorce reference for a moved resource will not have been deleted yet.
		--		AmendUserId = @UserId,
		--		AmendDate = @AmendDate
		--FROM	hierarchy.HierarchyEditDetail hed
		--INNER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.NodePathId = rr.NodePathId AND rr.Deleted = 0
		--WHERE	hed.HierarchyEditID = @HierarchyEditID
		--	AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
		--	AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
		--	AND hed.ResourceReferenceId IS NULL

		-- Update the HierarcyEditDetail records with the new ResourceReferenceIds.
		UPDATE  hed
		SET     ResourceReferenceId = (	SELECT MAX(rr.Id) -- MAX is needed as the resource reference for a moved resource will not have been deleted yet.
										FROM resources.ResourceReference rr
										WHERE hed.ResourceId = rr.ResourceId AND hed.NodePathId = rr.NodePathId AND rr.Deleted = 0),
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		WHERE   hed.HierarchyEditID = @HierarchyEditID
				AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
				AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
				AND hed.ResourceReferenceId IS NULL

		-- Update the OriginalResourceReferenceId for the new ResourceReference records.
		UPDATE  rr
		SET     OriginalResourceReferenceId = rr.Id,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
		WHERE hed.HierarchyEditID = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
			AND hed.ResourceReferenceId = rr.Id
			AND rr.OriginalResourceReferenceId IS NULL

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