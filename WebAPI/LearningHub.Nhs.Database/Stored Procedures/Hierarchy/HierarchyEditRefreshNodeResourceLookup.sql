-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Refreshes the contents of hierarchy.HierarchyEditNodeResourceLookup.
--
-- Modification History
--
-- 05-02-2021  KD	Initial Revision (IT2).
--					Provide details of Resource placement within a HierarchyEdit (for all Resource statuses)
--					Required when lazy loading Node contents.
-- 29-04-2024  DB	Avoid duplicates when the content referencing exists.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditRefreshNodeResourceLookup]
(
	@HierarchyEditId int
)

AS

BEGIN

	DELETE FROM hierarchy.HierarchyEditNodeResourceLookup WHERE HierarchyEditId = @HierarchyEditId

	;WITH cteNodeResourceLookup(NodeId, ParentNodeId, ResourceId)
	AS (
		SELECT 
			hed_node.NodeId,
			hed_node.ParentNodeId,
			hed_resource.ResourceId
		FROM 
			hierarchy.HierarchyEditDetail hed_resource
		INNER JOIN
			hierarchy.HierarchyEditDetail hed_node ON hed_resource.HierarchyEditId = hed_node.HierarchyEditId 
													AND hed_resource.NodeId = hed_node.NodeId
													AND hed_resource.Id != hed_node.Id
		WHERE
			hed_resource.HierarchyEditId = @HierarchyEditId
			AND hed_resource.ResourceId IS NOT NULL
			AND hed_node.ResourceId IS NULL
			AND hed_resource.Deleted = 0
		
		UNION ALL

		SELECT 
			cte.ParentNodeId AS NodeId,
			hed.ParentNodeId AS ParentNodeId,
			cte.ResourceId
		FROM
			cteNodeResourceLookup cte			
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON hed.NodeId = cte.ParentNodeId
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND hed.ResourceId IS NULL
			AND hed.Deleted = 0
		)
	INSERT INTO hierarchy.HierarchyEditNodeResourceLookup ([HierarchyEditId],[NodeId],[ResourceId])
	SELECT DISTINCT @HierarchyEditId AS HierarchyEditId, cte.NodeId, cte.ResourceId
	FROM cteNodeResourceLookup cte

END
GO