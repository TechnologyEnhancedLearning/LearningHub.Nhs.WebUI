-------------------------------------------------------------------------------
-- Author       KD
-- Created      05-01-2022
-- Purpose      Returns the resources and sub-nodes located under a specified node. 
--				Returns data for Admin purposes - read only view.
--
-- Modification History
--
-- 05-01-2022  KD	Initial Revision.
-- 09-02-2022  KD	Explicitly exclude External Orgs from NodePath lookup.
-- 22-02-2022  RS	Explicitly exclude External Orgs from ResourceReference lookup.
-- 23-04-2022  DB	Restrict published resourceVersions by current published node version Id.
-- 07-05-2024  DB	Change input parameter to NodePathId to prevent all referenced resources and child nodes being returned multiple times
--					Also return Child NodePathId to allow the client to navigate to the child node.
-- 03-06-2024  DB	Return the NodePathDisplayVersion properties (where they exist).
-- 13-06-2024  DB	Return the ResourceReferenceDisplayVersion properties (where they exist).
-- 02-07-2024  DB   Allow catalogue node types to be nested.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodeContentsAdminReadOnly]
(
	@NodePathId INT
)

AS

BEGIN

	SELECT 
		ROW_NUMBER() OVER(ORDER BY DisplayOrder) AS Id,
		NodePathId,
		[Name],
		[Description],
		NodeTypeId,
		NodeId,
		NodeVersionId,
		NodePathDisplayVersionId,
		ResourceId,
		ResourceVersionId,
		ResourceReferenceId,
		ResourceReferenceDisplayVersionId,
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
		-- Published Folder Node/s
		SELECT 
			cnp.Id AS NodePathId,
			COALESCE(p_npdv.DisplayName, fnv.[Name], cnv.[Name]) AS [Name],
			ISNULL(fnv.[Description], cnv.[Description]) AS [Description],
			cn.NodeTypeId,
			nl.ChildNodeId AS NodeId,
			ISNULL(fnv.NodeVersionId, cnv.NodeVersionId) AS NodeVersionId,
			ISNULL(p_npdv.Id, 0) AS NodePathDisplayVersionId,
			NULL AS ResourceId,
			NULL AS ResourceVersionId,
			NULL AS ResourceReferenceId,
			NULL AS ResourceReferenceDisplayVersionId,
			nl.DisplayOrder,
			NULL AS ResourceTypeId,
			NULL AS VersionStatusId,
			NULL AS UnpublishedByAdmin,
			NULL AS ResourceInEdit,
			NULL AS DraftResourceVersionId,
			CASE WHEN nr.NodeId IS NULL THEN 0 ELSE 1 END AS HasResourcesInd,
			CASE WHEN nrl.NodeId IS NULL THEN 0 ELSE 1 END AS HasResourcesInBranchInd,
			NULL AS HierarchyEditDetailId
		FROM 
            hierarchy.NodePath pnp
		INNER JOIN
			hierarchy.NodeLink nl ON pnp.NodeId = nl.ParentNodeId
		INNER JOIN 
			hierarchy.[Node] cn ON nl.ChildNodeId = cn.Id
		INNER JOIN 
			hierarchy.NodePath cnp ON cn.Id = cnp.NodeId AND pnp.NodePath + '\' + CONVERT(VARCHAR(10), cn.Id) = cnp.NodePath
		INNER JOIN 
			hierarchy.NodeVersion nv ON nv.Id = cn.CurrentNodeVersionId
		LEFT JOIN
			hierarchy.CatalogueNodeVersion cnv ON nv.Id = cnv.NodeVersionId AND cnv.Deleted = 0
		LEFT JOIN
			hierarchy.FolderNodeVersion fnv ON nv.Id = fnv.NodeVersionId AND fnv.Deleted = 0
		LEFT JOIN
			(SELECT DISTINCT NodeId FROM hierarchy.NodeResource WHERE Deleted = 0 AND VersionStatusId = 2) nr ON cn.Id = nr.NodeId
		LEFT JOIN
			(SELECT DISTINCT NodeId FROM hierarchy.NodeResourceLookup WHERE Deleted = 0) nrl ON cn.Id = nrl.NodeId
		LEFT OUTER JOIN
			hierarchy.NodePathDisplayVersion p_npdv ON cnp.Id = p_npdv.NodePathId AND p_npdv.VersionStatusId = 2 /* Published */ AND p_npdv.Deleted = 0
		WHERE
			pnp.Id = @NodePathId 
			AND nv.VersionStatusId = 2 -- Published
			AND nl.Deleted = 0
            and cnp.Deleted = 0
			AND cn.Deleted = 0

		UNION

		-- Resources
		SELECT 
			np.Id AS NodePathId,
			ISNULL(p_rrdv.DisplayName, rv.Title) as [Name],
			NULL As [Description],
			0 as NodeTypeId, 
			NULL AS NodeId,
			NULL AS NodeVersionId,
			NULL AS NodePathDisplayVersionId,
			r.Id AS ResourceId,
			rv.Id AS ResourceVersionId,
			rr.OriginalResourceReferenceId AS ResourceReferenceId,
			ISNULL(p_rrdv.Id, 0) AS ResourceReferenceDisplayVersionId,
			nr.DisplayOrder,
			r.ResourceTypeId,
			CASE WHEN rvd.Id IS NOT NULL AND rvd.VersionStatusId > 1 THEN rvd.VersionStatusId ELSE rv.VersionStatusId END AS VersionStatusId, --rv.VersionStatusId,
			CASE WHEN rv.VersionStatusId = 3 AND rve.Id IS NOT NULL THEN 1 ELSE 0 END AS UnpublishedByAdmin,
			CASE WHEN rvd.Id IS NOT NULL THEN 1 ELSE 0 END AS ResourceInEdit,
			ISNULL(rvd.Id, rv.Id) AS DraftResourceVersionId,
			0 AS HasResourcesInd,
			0 AS HasResourcesInBranchInd,
			NULL AS HierarchyEditDetailId
		FROM 
            hierarchy.NodePath np
        INNER JOIN
			hierarchy.NodeResource nr ON np.NodeId = nr.NodeId
		INNER JOIN 
			resources.[Resource] r ON nr.ResourceId = r.Id
		INNER JOIN 
			resources.ResourceVersion rv ON rv.resourceId = nr.ResourceId
		LEFT JOIN 
			resources.ResourceReference rr ON rr.ResourceId = nr.ResourceId AND rr.NodePathId = np.Id AND rr.Deleted = 0
		LEFT JOIN
			resources.ResourceVersionEvent rve ON rve.ResourceVersionId = rv.Id AND rve.ResourceVersionEventTypeId = 6 /* Unpublished by admin */
		LEFT JOIN 
			resources.ResourceVersion rvd ON r.Id = rvd.ResourceId AND rvd.Id > rv.Id AND rvd.Deleted = 0
		LEFT OUTER JOIN
			resources.ResourceReferenceDisplayVersion p_rrdv ON rr.Id = p_rrdv.ResourceReferenceId AND p_rrdv.VersionStatusId = 2 /* Published */ AND p_rrdv.Deleted = 0
		WHERE 
			np.Id = @NodePathId
			AND (r.CurrentResourceVersionId = rv.Id OR r.CurrentResourceVersionId IS NULL)
			AND ((nr.VersionStatusId IN (2,3) AND np.Id IS NOT NULL) OR nr.VersionStatusId = 1)
			AND nr.Deleted = 0
			AND r.Deleted = 0
			AND rv.Deleted = 0
	) AS t1
	ORDER BY CASE WHEN NodeTypeId = 0 THEN 1 ELSE 0 END ASC, DisplayOrder ASC

END
GO