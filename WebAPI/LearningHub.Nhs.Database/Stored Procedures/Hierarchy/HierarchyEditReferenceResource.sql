-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      04-06-2024
-- Purpose      References Resource within a Hierarchy Edit.
--
-- Modification History
--
-- 04-06-2024  DB	Initial Revision.
-- 08-07-2024  DB	Populate the PrimaryCatalogueNodeId.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditReferenceResource]
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

		DECLARE @PrimaryCatalogueNodeId INT
		DECLARE @NewHierarchyEditDetailId INT

		DECLARE @HierarchyEditId int
		DECLARE @ResourceId int
		SELECT	@HierarchyEditId = HierarchyEditId,
				@ResourceId = ResourceId,
				@PrimaryCatalogueNodeId = PrimaryCatalogueNodeId
		FROM [hierarchy].[HierarchyEditDetail]
		WHERE Id = @HierarchyEditDetailId

		-- Increment display order of resources in destination.
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
															AND hed_moveTo_children.NodeId = hed_moveTo.NodeId
		WHERE	
			hed_moveTo.Id = @MoveToHierarchyEditDetailId
			AND hed_moveTo_children.ResourceId IS NOT NULL
			AND hed_moveTo.Deleted = 0
			AND hed_moveTo_children.Deleted = 0

		DECLARE @NewParentNodeId int
		DECLARE @NewParentNodePathId int
		DECLARE @NewParentNodePath varchar(256)
		SELECT 
			@NewParentNodeId = NodeId,
			@NewParentNodePathId = NodePathId,
			@NewParentNodePath = ISNULL(NewNodePath, InitialNodePath)
		FROM
			[hierarchy].[HierarchyEditDetail]
		WHERE
			Id = @MoveToHierarchyEditDetailId

		-- Reference the resource.
		-- Is there an existing link between the Node and Resource (i.e. from delete / move away & reinstate scenario)
		DECLARE @nodeResourceId int
		SELECT 
			@nodeResourceId = Id
		FROM 
			hierarchy.NodeResource
		WHERE
			NodeId = @NewParentNodeId
			AND ResourceId = @ResourceId
			AND Deleted = 0

		INSERT INTO [hierarchy].[HierarchyEditDetail]
		(
			HierarchyEditId,
			HierarchyEditDetailTypeId,
			HierarchyEditDetailOperationId,
			PrimaryCatalogueNodeId,
			NodePathId,
			NodeId,
			NodeVersionId,
			InitialParentNodeId,
			ParentNodeId,
			ParentNodePathId,
			NodeLinkId,
			ResourceId,
			ResourceVersionId,
			ResourceReferenceId,
			NodeResourceId,
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
			5 AS HierarchyEditDetailTypeId, -- Node Resource
			4 AS HierarchyEditDetailOperationId, -- Add Reference
			@PrimaryCatalogueNodeId AS PrimaryCatalogueNodeId,
			NULL AS NodePathId,
			NULL AS NodeId,
			NULL AS NodeVersionId,
			NULL AS InitialParentNodeId,
			@NewParentNodeId AS ParentNodeId,
			@NewParentNodePathId AS ParentNodePathId,
			NULL AS NodeLinkId,
			hed.ResourceId AS ResourceId,
			hed.ResourceVersionId AS ResourceVersionId,
			NULL AS ResourceReferenceId,
			@nodeResourceId AS NodeResourceId,
			1 AS DisplayOrder,
			NULL AS InitialNodePath,
			@NewParentNodePath AS NewNodePath,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		WHERE
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0

		SET @NewHierarchyEditDetailId = SCOPE_IDENTITY()

		-- Create new ResourceReference record for the new referenced resources.
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
		WHERE hed.Id = @NewHierarchyEditDetailId
			AND (
                rr.Id IS NULL
                OR
                hed2.Id IS NOT NULL-- If rr.Id is NOT NULL check if the original resource has been moved, so the original ResourceReference will be deleted during publish
            )

		-- Update the HierarcyEditDetail record with the new ResourceReferenceIds.
		UPDATE  hed
		SET     ResourceReferenceId = (	SELECT MAX(rr.Id) -- MAX is needed as the resource reference for a moved resource will not have been deleted yet.
										FROM resources.ResourceReference rr
										WHERE hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0),
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		WHERE   hed.Id = @NewHierarchyEditDetailId

		-- Update the OriginalResourceReferenceId for the new ResourceReference record.
		UPDATE  rr
		SET     OriginalResourceReferenceId = rr.Id,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReference rr ON hed.ResourceId = rr.ResourceId AND hed.ParentNodePathId = rr.NodePathId AND rr.Deleted = 0
		WHERE hed.Id = @NewHierarchyEditDetailId
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