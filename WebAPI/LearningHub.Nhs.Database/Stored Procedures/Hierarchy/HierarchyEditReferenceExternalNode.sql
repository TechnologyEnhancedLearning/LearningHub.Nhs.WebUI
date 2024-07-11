-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      01-07-2024
-- Purpose      Create an external reference to a node within a Hierarchy Edit.
--
-- Modification History
--
-- 01-07-2024  DB	Initial Revision.
-- 08-07-2024  DB	Populate the PrimaryCatalogueNodeId based on the original catalogue.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditReferenceExternalNode]
(
	@NodePathId int,
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
		DECLARE @ExternalCatalogueNodeId INT
		
		SELECT	@HierarchyEditId = hed.HierarchyEditId,
                @RootNodeId = np.NodeId,
				@NewParentNodeId = hed.NodeId,
				@NewParentNodePathId = NodePathId
		FROM	[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN [hierarchy].[HierarchyEdit] he ON he.Id = hed.HierarchyEditId
		INNER JOIN hierarchy.NodePath np ON he.RootNodePathId = np.Id
		WHERE	hed.Id = @ReferenceToHierarchyEditDetailId

		SELECT	@NodeId = np.NodeId,
				@ExternalCatalogueNodeId = nv.PrimaryCatalogueNodeId
		FROM	[hierarchy].[NodePath] np
		INNER JOIN [hierarchy].[Node] n ON np.NodeId = n.Id
		INNER JOIN [hierarchy].NodeVersion nv ON n.CurrentNodeVersionId = nv.Id
		WHERE	np.Id = @NodePathId
			AND nv.VersionStatusId = 2 -- Published
			AND np.Deleted = 0
			AND n.Deleted = 0
			AND nv.Deleted = 0

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

		SELECT  @OriginalNodePath = np.NodePath,
                @OriginalParentNodePath = CASE WHEN CHARINDEX('\', np.NodePath) > 0 THEN LEFT(np.NodePath, LEN(np.NodePath) - LEN(np.NodeId) - 1) ELSE '' END 
		FROM    hierarchy.NodePath np
		WHERE   np.Id = @NodePathId
			AND np.Deleted = 0

		SELECT  @DestinationParentNodePath = ISNULL(NewNodePath, InitialNodePath)
		FROM    hierarchy.HierarchyEditDetail
		WHERE   Id = @ReferenceToHierarchyEditDetailId
		
		-- Create the new HierarchyEditDetail records for the new referenced external nodes.
		INSERT INTO hierarchy.HierarchyEditDetail (HierarchyEditId,HierarchyEditDetailTypeId,HierarchyEditDetailOperationId,PrimaryCatalogueNodeId,NodeId,NodePathId,NodeVersionId,ParentNodeId,ParentNodePathId,NodeLinkId,ResourceId,ResourceVersionId,ResourceReferenceId,NodeResourceId,DisplayOrder,InitialNodePath,NewNodePath,Deleted,CreateUserId,CreateDate,AmendUserId,AmendDate)
		SELECT  @HierarchyEditId,
				4, -- Node Link (4)
				4 AS HierarchyEditDetailOperationId,  -- Add Reference
				@ExternalCatalogueNodeId AS PrimaryCatalogueNodeId,
				np.NodeId, 
				NULL as NodePathId, 
				n.CurrentNodeVersionId, 
				NULL AS ParentNodeId, -- Populated further down
				NULL AS ParentNodePathId, -- Populated further down
				NULL AS NodeLinkId, 
				NULL AS ResourceId,
				NULL AS ResourceVersionId,
				NULL AS ResourceReferenceId,
				NULL AS NodeResourceId,
				NULL AS DisplayOrder, -- Populated further down.
				NULL AS InitialNodePath,
				@DestinationParentNodePath + CASE WHEN @OriginalParentNodePath = '' THEN '\' ELSE '' END + SUBSTRING(np.NodePath, LEN(@OriginalParentNodePath)+1, LEN(np.NodePath)) AS NewNodePath, -- Not using REPLACE incase number of digits in nodeIds are not consistant
				0 AS Deleted,
				@UserId AS CreateUserId,
				@AmendDate AS CreateDate,
				@UserId AS AmendUserId,
				@AmendDate AS AmendDate
		FROM	hierarchy.NodePath np
		INNER JOIN hierarchy.[Node] n on np.NodeId = n.Id
		WHERE	np.NodePath like @OriginalNodePath +'%'
			AND np.IsActive = 1
			AND np.Deleted = 0

		-- Create the new HierarchyEditDetail records for the new referenced external resources.
		INSERT INTO hierarchy.HierarchyEditDetail (HierarchyEditId,HierarchyEditDetailTypeId,HierarchyEditDetailOperationId,PrimaryCatalogueNodeId,NodeId,NodePathId,NodeVersionId,ParentNodeId,ParentNodePathId,NodeLinkId,ResourceId,ResourceVersionId,ResourceReferenceId,NodeResourceId,DisplayOrder,InitialNodePath,NewNodePath,Deleted,CreateUserId,CreateDate,AmendUserId,AmendDate)
		SELECT	@HierarchyEditId AS HierarchyEditId,
				5 AS HierarchyEditDetailTypeId, -- Node Resource (5)
				4 AS HierarchyEditDetailOperationId,  -- Add Reference
				@ExternalCatalogueNodeId AS PrimaryCatalogueNodeId,
				NULL AS NodeId,
				NULL as NodePathId,
				NULL AS NodeVersionId,
				nr.NodeId AS ParentNodeId,
				NULL AS ParentNodePathId, -- Populated further down
				NULL AS NodeLinkId,
				nr.ResourceId,
				rv.Id AS ResourceVersionId,
				NULL AS ResourceReferenceId, -- Populated further down
				nr.Id AS NodeResourceId,
				nr.DisplayOrder,
				NULL AS InitialNodePath,
				@DestinationParentNodePath + CASE WHEN @OriginalParentNodePath = '' THEN '\' ELSE '' END + SUBSTRING(np.NodePath, LEN(@OriginalParentNodePath)+1, LEN(np.NodePath)) AS NewNodePath, -- Not using REPLACE incase number of digits in nodeIds are not consistant
				0 AS Deleted,
				@UserId AS CreateUserId,
				@AmendDate AS CreateDate,
				@UserId AS AmendUserId,
				@AmendDate AS AmendDate
		FROM	hierarchy.NodePath np
		INNER JOIN hierarchy.NodeResource nr on np.NodeId = nr.NodeId
		INNER JOIN resources.Resource r on nr.ResourceId = r.Id
		INNER JOIN resources.ResourceVersion rv on r.CurrentResourceVersionId = rv.Id
		WHERE	np.NodePath like @OriginalNodePath +'%'
			AND rv.VersionStatusId = 2 -- Published
			AND np.IsActive = 1
			AND np.Deleted = 0
			AND nr.Deleted = 0

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

		-- Populate the Parent Node Path Ids and display orders for the new reference nodes.
		UPDATE  hed
		SET     ParentNodeId = p_hed.NodeId,
				ParentNodePathId = p_hed.NodePathId,
				DisplayOrder = ISNULL(nl.DisplayOrder, 1), -- If no display order, set to 1 as it must be the parent for the referenced nodes.
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN	hierarchy.HierarchyEditDetail p_hed ON ISNULL(hed.NewNodePath, hed.InitialNodePath) = ISNULL(p_hed.NewNodePath, p_hed.InitialNodePath) + '\' + CAST(hed.NodeId AS VARCHAR(10))
		LEFT JOIN	hierarchy.NodeLink nl ON p_hed.NodeId = nl.ParentNodeId AND hed.NodeId = nl.ChildNodeId AND nl.Deleted = 0
		WHERE   hed.HierarchyEditID = @HierarchyEditID
			AND p_hed.HierarchyEditId = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND hed.HierarchyEditDetailTypeId = 4 -- Node Link
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

		-- Create new ResourceReference records for the new referenced resources.
		INSERT INTO resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT  hed.ResourceId, hed.ParentNodePathId, NULL AS OriginalResourceReferenceId, 0 AS IsActive, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM hierarchy.HierarchyEditDetail hed
		LEFT OUTER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
        LEFT OUTER JOIN hierarchy.NodePath np ON rr.NodePathId = np.Id 
        LEFT OUTER JOIN hierarchy.HierarchyEditDetail hed2 ON hed2.HierarchyEditID = @HierarchyEditID
                                                            AND hed2.HierarchyEditDetailTypeId = 5 -- Node Resource
                                                            AND hed2.HierarchyEditDetailOperationId = 2 -- Edit
                                                            AND hed2.Id != hed.Id
                                                            AND hed2.ResourceId = hed.ResourceId
                                                            AND ISNULL(hed2.InitialNodePath, '') != ISNULL(hed2.NewNodePath, hed2.InitialNodePath) -- The original resource has been moved
                                                            AND ISNULL(hed2.InitialNodePath, '') = ISNULL(hed.NewNodePath, hed.InitialNodePath) -- The resource has been moved to the original location that it was moved from
		WHERE hed.HierarchyEditID = @HierarchyEditID
			AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
			AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
			AND hed.ResourceReferenceId is NULL
			AND (
                rr.Id IS NULL
                OR
                hed2.Id IS NOT NULL-- If rr.Id is NOT NULL check if the original resource has been moved, so the original ResourceReference will be deleted during publish
            )

		-- Update the HierarcyEditDetail records with the new ResourceReferenceIds.
		UPDATE  hed
		SET     ResourceReferenceId = (	SELECT MAX(rr.Id) -- MAX is needed as the resource reference for a moved resource will not have been deleted yet.
										FROM resources.ResourceReference rr
										WHERE hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0),
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