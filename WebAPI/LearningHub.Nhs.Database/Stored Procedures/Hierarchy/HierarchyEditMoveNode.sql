-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Move node within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 05-01-2021  KD	IT2 - refresh hierarchy.HierarchyEditNodeResourceLookup if required.
-- 13-05-2024  DB	Addition of ParentNodePathId to the update statement.
-- 20-05-2024  DB	Added the creation of a new NodePath record for the moved node and update child nodes.
-- 29-05-2024  DB	Clear the NodePathId for moved nodes. NodePaths can not be updated incase susequent references are made to the original path.
-- 07-08-2024  SA   Remove all instance of the referenced path when moving a referenced folder
-- 23-08-2024  SA   Moving a folder into a referenced folder should affect all instances of the referenced folder.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditMoveNode]
(
	@HierarchyEditDetailId int,
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
		DECLARE @ChildNodeId int
		DECLARE @OriginalNodePath varchar(256)
		DECLARE @RootNodeId INT

		SELECT	@HierarchyEditId = HierarchyEditId,
				@ChildNodeId = hed.NodeId,
				@OriginalNodePath = ISNULL(hed.NewNodePath, hed.InitialNodePath),
                @RootNodeId = np.NodeId
		FROM
			[hierarchy].[HierarchyEditDetail] hed
        INNER JOIN
			[hierarchy].[HierarchyEdit] he ON he.Id = hed.HierarchyEditId
		INNER JOIN
			hierarchy.NodePath np ON he.RootNodePathId = np.Id
		WHERE hed.Id = @HierarchyEditDetailId

		-- Decrement display order of sibling nodes with higher display order.
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
			[hierarchy].[HierarchyEditDetail] hed_moveFrom ON hed.HierarchyEditId = hed_moveFrom.HierarchyEditId AND hed.ParentNodeId = hed_moveFrom.ParentNodeId AND hed.Id != hed_moveFrom.Id
		WHERE	
			hed_moveFrom.Id = @HierarchyEditDetailId
			AND hed.DisplayOrder > hed_moveFrom.DisplayOrder
			AND hed.ResourceId IS NULL
			AND hed.Deleted = 0
			AND hed_moveFrom.Deleted = 0

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
			hed_moveTo.Id = @MoveToHierarchyEditDetailId
			AND hed_moveTo.ResourceId IS NULL
			AND hed_moveTo.Deleted = 0
			AND hed_moveTo_children.Deleted = 0

		DECLARE @NewParentNodeId int
		DECLARE @NewParentNodePathId int
		DECLARE @NewParentNodePath varchar(256)
		SELECT	@NewParentNodeId = NodeId,
				@NewParentNodePathId = NodePathId,
				@NewParentNodePath = ISNULL(NewNodePath, InitialNodePath)
		FROM	[hierarchy].[HierarchyEditDetail]
		WHERE	Id = @MoveToHierarchyEditDetailId

		-- Move the node.
		-- Is there an existing NodeLink between the Nodes (i.e. from delete / move away & reinstate scenario)
		DECLARE @nodeLinkId int
		SELECT 
			@nodeLinkId = Id
		FROM 
			hierarchy.NodeLink
		WHERE
			ParentNodeId = @NewParentNodeId
			AND ChildNodeId = @ChildNodeId
			AND Deleted = 0

		UPDATE 
			hed
		SET
			HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
			ParentNodeId = CASE WHEN hed.Id = @HierarchyEditDetailId THEN null ELSE hed.ParentNodeId END, -- Populated further down if root of move
			ParentNodePathId = NULL, -- Populated further down for nodes
			NodePathId = NULL, -- Populated further down for nodes
			DisplayOrder = CASE WHEN hed.Id = @HierarchyEditDetailId THEN 1 ELSE hed.DisplayOrder END,
			NodeLinkId = CASE WHEN hed.Id = @HierarchyEditDetailId THEN @nodeLinkId ELSE hed.NodeLinkId END,
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		WHERE	
			HierarchyEditID = @HierarchyEditID
			AND hed.Deleted = 0
			AND (HierarchyEditDetailTypeId = 4 OR HierarchyEditDetailTypeId = 5) -- Node Link OR Node Resource
			AND ISNULL(NewNodePath, InitialNodePath) like @OriginalNodePath +'%'
			AND Deleted = 0

		-- Set the ParentNodeId and @NewParentNodePathId for the moved Node
		UPDATE 
			[hierarchy].[HierarchyEditDetail]
		SET
			ParentNodeId = @NewParentNodeId,
			ParentNodePathId = @NewParentNodePathId,
			HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId IS NULL THEN 2 ELSE HierarchyEditDetailOperationId END, -- Set to Edit if first update
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		WHERE	
			Id = @HierarchyEditDetailId

		-- Set NewNodePath column for all the children of the moved NodePath and clear the NodePathId
		UPDATE	hierarchy.HierarchyEditDetail
		SET		NewNodePath = REPLACE(ISNULL(NewNodePath, InitialNodePath), @OriginalNodePath, CONCAT(@NewParentNodePath, '\', @ChildNodeId)),
				HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId IS NULL THEN 2 ELSE HierarchyEditDetailOperationId END, -- Set to Edit if first update
				NodePathId = NULL, -- Populated further down
				ParentNodePathId = NULL, -- Populated further down
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		WHERE	HierarchyEditId = @HierarchyEditId
    		AND ISNULL(NewNodePath, InitialNodePath) Like CONCAT(@OriginalNodePath, '%')

		-- Create the new NodePath records resulting from the move (Created as IsActive = 0).
		INSERT INTO hierarchy.NodePath (NodeId, NodePath, CatalogueNodeId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT  hed.NodeId, ISNULL(hed.NewNodePath, hed.InitialNodePath), @RootNodeId AS CatalogueNodeId, 0 AS IsActive, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM hierarchy.HierarchyEditDetail hed
		LEFT OUTER JOIN hierarchy.NodePath np ON hed.NodeId = np.NodeId AND ISNULL(hed.NewNodePath, hed.InitialNodePath) = np.NodePath AND np.Deleted = 0
		WHERE hed.HierarchyEditID = @HierarchyEditID
			AND HierarchyEditDetailTypeId != 5 -- Exclude Node Resource
			AND hed.NodePathId is NULL
			AND np.Id IS NULL

		-- Update HierarchyEditDetail records with the new NodePathIds.
		UPDATE	hed
		SET		NodePathId = np.Id,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.NodePath np ON hed.NodeId = np.NodeId AND ISNULL(hed.NewNodePath, hed.InitialNodePath) = np.NodePath AND np.Deleted = 0
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND hed.NodePathId IS NULL

		-- Populate the Parent Node Path Ids for the new reference nodes.
		UPDATE  hed
		SET     ParentNodePathId = p_hed.NodePathId,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.HierarchyEditDetail p_hed ON ISNULL(hed.NewNodePath, hed.InitialNodePath) = ISNULL(p_hed.NewNodePath, p_hed.InitialNodePath) + '\' + CAST(hed.NodeId AS VARCHAR(10)) AND hed.HierarchyEditDetailTypeId != 5 -- Exclude Node Resources
                                                            OR
                                                          ISNULL(hed.NewNodePath, hed.InitialNodePath) = ISNULL(p_hed.NewNodePath, p_hed.InitialNodePath) AND hed.HierarchyEditDetailTypeId = 5 -- Node Resources
		WHERE   hed.HierarchyEditID = @HierarchyEditID
			AND hed.ParentNodePathId IS NULL

		-- Create new NodePathDisplayVersion records where the NodePathId has changed. i.e. the NodePathId against the NodePathDisplayVersion record is different
		INSERT INTO hierarchy.NodePathDisplayVersion (NodePathId, DisplayName, VersionStatusId, PublicationId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT hed.NodePathId, DisplayName, 1 /* Draft */, NULL, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	hierarchy.NodePathDisplayVersion npdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON npdv.Id = hed.NodePathDisplayVersionId
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND npdv.NodePathId != hed.NodePathId
			AND npdv.Deleted = 0
			AND hed.Deleted = 0

		-- Delete any Draft and Unused NodePathDisplayVersion records (resulting from creation and then subsequent move of node to different NodePath)
		UPDATE npdv
		SET		Deleted = 1,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	hierarchy.NodePathDisplayVersion npdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON npdv.Id = hed.NodePathDisplayVersionId
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND npdv.NodePathId != hed.NodePathId
			AND npdv.VersionStatusId = 1 -- Draft
			AND npdv.Deleted = 0
			AND hed.Deleted = 0

		-- Update to the new NodePathDisplayVersion records where the NodePathId has changed.
		UPDATE	hed
		SET		NodePathDisplayVersionId = npdv.Id,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	hierarchy.NodePathDisplayVersion npdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON npdv.NodePathId = hed.NodePathId
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND npdv.Deleted = 0
			AND hed.Deleted = 0


		-- Create the new ResourceReference records resulting from the move (Created as IsActive = 0).
		INSERT INTO resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT  hed.ResourceId, hed.ParentNodePathId, o_rr.OriginalResourceReferenceId, 0 AS IsActive, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReference o_rr ON hed.ResourceReferenceId = o_rr.Id AND o_rr.Deleted = 0
		LEFT OUTER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
		WHERE hed.HierarchyEditID = @HierarchyEditID
			AND HierarchyEditDetailTypeId = 5 -- Node Resource
			AND rr.Id IS NULL


		-- Update HierarchyEditDetail records with the new ResourceReferenceIds.
		UPDATE	hed
		SET		ResourceReferenceId = rr.Id,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND hed.ResourceReferenceId != rr.Id


		-- Create new ResourceReferenceDisplayVersion records where the ResourceReferenceId has changed. i.e. the ResourceReferenceId against the ResourceReferenceDisplayVersion record is different
		INSERT INTO resources.ResourceReferenceDisplayVersion (ResourceReferenceId, DisplayName, VersionStatusId, PublicationId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT hed.ResourceReferenceId, DisplayName, 1 /* Draft */, NULL, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON rrdv.Id = hed.ResourceReferenceDisplayVersionId
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND rrdv.ResourceReferenceId != hed.ResourceReferenceId
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0


		-- Delete any Draft and Unused ResourceReferenceDisplayVersion records (resulting from creation and then subsequent move of node to different ResourceReference)
		UPDATE	rrdv
		SET		Deleted = 1,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON rrdv.Id = hed.ResourceReferenceDisplayVersionId
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND rrdv.ResourceReferenceId != hed.ResourceReferenceId
			AND rrdv.VersionStatusId = 1 -- Draft
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0


		-- Update to the new ResourceReferenceDisplayVersion records where the ResourceReferenceId has changed.
		UPDATE	hed
		SET		ResourceReferenceDisplayVersionId = rrdv.Id,
				HierarchyEditDetailOperationId = CASE WHEN hed.HierarchyEditDetailOperationId IS NULL THEN 2 ELSE hed.HierarchyEditDetailOperationId END, -- Set to Edit if first update
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM	resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN hierarchy.HierarchyEditDetail hed ON rrdv.ResourceReferenceId = hed.ResourceReferenceId
		WHERE	hed.HierarchyEditID = @HierarchyEditID
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0

		 -- Remove all instance of the referenced path when moving a referenced folder

		 EXEC hierarchy.HierarchyEditRemoveNodeReferencesOnMoveNode @HierarchyEditDetailId,@UserId,@UserTimezoneOffset

		-- Moving a folder into a referenced folder should affect all instances of the referenced folder.

		DECLARE @CurrentNodeId INT,
                @ReferenceHierarchyEditDetailId INT

        SELECT @CurrentNodeId = hed.NodeId
        FROM [hierarchy].[HierarchyEditDetail] hed
        WHERE hed.Id = @MoveToHierarchyEditDetailId
		
        -- Declare the cursor

		 DECLARE NodeCursor CURSOR FOR 
                  SELECT Id AS ReferenceHierarchyEditDetailId
                  FROM hierarchy.HierarchyEditDetail
                                         WHERE NodeId = @CurrentNodeId
                                               AND Id != @MoveToHierarchyEditDetailId         
        -- Open the cursor
        OPEN NodeCursor;

        -- Fetch the first row from the cursor
        FETCH NEXT FROM NodeCursor
        INTO @ReferenceHierarchyEditDetailId;

        -- Loop until no more rows are returned
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Execute the script to add new folder references in all referenced instances
            EXEC [hierarchy].[HierarchyEditReferenceNode] @HierarchyEditDetailId, @ReferenceHierarchyEditDetailId, @UserId,@UserTimezoneOffset

            -- Fetch the next row from the cursor
            FETCH NEXT FROM NodeCursor
            INTO @ReferenceHierarchyEditDetailId;
        END

        -- Close and deallocate the cursor
        CLOSE NodeCursor;
        DEALLOCATE NodeCursor;
       
		------------------------------------------------------------ 
		-- Refresh HierarchyEditNodeResourceLookup
		------------------------------------------------------------
		IF EXISTS (SELECT 'X' FROM hierarchy.HierarchyEditNodeResourceLookup WHERE HierarchyEditId = @HierarchyEditId AND NodeId = @ChildNodeId)
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