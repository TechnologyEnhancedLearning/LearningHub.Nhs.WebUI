-------------------------------------------------------------------------------
-- Author       KD
-- Created      05-01-2022
-- Purpose      Returns the resources and sub-nodes located in a specified node. 
--				Returns data for Admin purposes.
--
-- Modification History
--
-- 05-01-2022  KD	Initial Revision.
-- 09-02-2022  KD	Explicitly exclude External Orgs from Resource Reference lookup.
-- 07-05-2024  DB	Change input parameter to NodePathId to prevent all referenced resources and child nodes being returned multiple times
--					Also return Child NodePathId to allow the client to navigate to the child node.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodeContentsAdmin]
(
	@NodePathId INT
)

AS

BEGIN

	-- Get the latest Hierarchy Edit in Draft status, the NodePathId can only be in one Draft Hierarchy Edit at a time
	DECLARE @HierarchyEditId int
	SELECT TOP 1 @HierarchyEditId = he.Id 
	FROM	hierarchy.HierarchyEdit he
    INNER JOIN hierarchy.HierarchyEditDetail hed ON he.Id = hed.HierarchyEditId 
	WHERE	hed.NodePathId = @NodePathId
		AND he.HierarchyEditStatusId = 1 -- Draft
		AND he.Deleted = 0
	ORDER BY he.Id DESC

	SELECT 
		ROW_NUMBER() OVER(ORDER BY DisplayOrder) AS Id,
		NodePathId,
		[Name],
		[Description],
		NodeTypeId,
		NodeId,
		NodeVersionId,
		ResourceId,
		ResourceVersionId,
		ResourceReferenceId,
		DisplayOrder,
		ResourceTypeId,
		VersionStatusId,
		CAST(UnpublishedByAdmin as bit) AS UnpublishedByAdmin,
		CAST(ResourceInEdit as bit) AS ResourceInEdit,
		DraftResourceVersionId,
		CAST(HasResourcesInd as bit) AS HasResourcesInd,
		CAST(HasResourcesInBranchInd as bit) AS HasResourcesInBranchInd,
		HierarchyEditDetailId
	FROM
	(
		-- Folder Node/s in Edit
		SELECT 
			hed.NodePathId,
			fnv.[Name],
			fnv.[Description],
			n.NodeTypeId,
			hed.NodeId,
			fnv.NodeVersionId,
			NULL AS ResourceId,
			NULL AS ResourceVersionId,
			NULL AS ResourceReferenceId,
			hed.DisplayOrder,
			NULL AS ResourceTypeId,
			NULL AS VersionStatusId,
			NULL AS UnpublishedByAdmin,
			NULL AS ResourceInEdit,
			NULL AS DraftResourceVersionId,
			CASE WHEN nr.NodeId IS NULL THEN 0 ELSE 1 END AS HasResourcesInd,
			CASE WHEN nrl.NodeId IS NULL THEN 0 ELSE 1 END AS HasResourcesInBranchInd,
			hed.Id AS HierarchyEditDetailId
		FROM 
			hierarchy.HierarchyEditDetail  hed
		INNER JOIN
			hierarchy.HierarchyEdit he ON hed.HierarchyEditId = he.Id
		INNER JOIN
			hierarchy.[Node] n ON hed.NodeId = n.Id
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = hed.NodeVersionId
		LEFT JOIN
			(SELECT DISTINCT NodeId FROM hierarchy.HierarchyEditDetail WHERE HierarchyEditId = @HierarchyEditId AND Deleted = 0 AND ResourceId IS NOT NULL) nr ON n.Id = nr.NodeId
		LEFT JOIN
			(SELECT DISTINCT NodeId FROM hierarchy.HierarchyEditNodeResourceLookup WHERE HierarchyEditId = @HierarchyEditId) nrl ON n.Id = nrl.NodeId
		WHERE 
			hed.ParentNodePathId = @NodePathId
			AND he.Id = @HierarchyEditId
			AND he.HierarchyEditStatusId = 1 -- Draft
			AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- excluded deleted
			AND hed.ResourceId IS NULL
			AND hed.Deleted = 0 
			AND he.Deleted = 0
			AND n.Deleted = 0 
			AND fnv.Deleted = 0

		UNION

		-- Resources
		SELECT 
			hed.NodePathId AS NodePathId,
			rv.Title as [Name],
			NULL As [Description],
			0 as NodeTypeId, 
			NULL AS NodeId,
			NULL AS NodeVersionId,
			hed.ResourceId AS ResourceId,
			hed.ResourceVersionId AS ResourceVersionId,
			rr.OriginalResourceReferenceId AS ResourceReferenceId,
			hed.DisplayOrder,
			r.ResourceTypeId,
			CASE WHEN rvd.Id IS NOT NULL AND rvd.VersionStatusId > 1 THEN rvd.VersionStatusId ELSE rv.VersionStatusId END AS VersionStatusId, --rv.VersionStatusId,
			CASE WHEN rv.VersionStatusId = 3 AND rve.Id IS NOT NULL THEN 1 ELSE 0 END AS UnpublishedByAdmin,
			CASE WHEN rvd.Id IS NOT NULL THEN 1 ELSE 0 END AS ResourceInEdit,
			ISNULL(rvd.Id, rv.Id) AS DraftResourceVersionId,
			0 AS HasResourcesInd,
			0 AS HasResourcesInBranchInd,
			hed.Id AS HierarchyEditDetailId
		FROM 
			hierarchy.HierarchyEditDetail  hed
		INNER JOIN
			hierarchy.HierarchyEdit he ON hed.HierarchyEditId = he.Id
		INNER JOIN 
			resources.[Resource] r ON hed.ResourceId = r.Id
		INNER JOIN 
			resources.ResourceVersion rv ON rv.Id = hed.ResourceVersionId
		LEFT JOIN 
			resources.ResourceReference rr ON rr.Id = hed.ResourceReferenceId AND rr.Deleted = 0
		LEFT JOIN
			resources.ResourceVersionEvent rve ON rve.ResourceVersionId = rv.Id AND rve.ResourceVersionEventTypeId = 6 /* Unpublished by admin */
		LEFT JOIN 
			resources.ResourceVersion rvd ON r.Id = rvd.ResourceId AND rvd.Id > rv.Id AND rvd.Deleted = 0
		WHERE 
			hed.NodePathId = @NodePathId
			AND he.Id = @HierarchyEditId
			AND he.HierarchyEditStatusId = 1 -- Draft
			AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- excluded deleted
			AND hed.ResourceId IS NOT NULL
			AND hed.Deleted = 0 
			AND he.Deleted = 0
	) AS t1
	ORDER BY NodeTypeId DESC, DisplayOrder ASC

END
GO