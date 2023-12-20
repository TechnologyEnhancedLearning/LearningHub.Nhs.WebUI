-------------------------------------------------------------------------------
-- Author       RS
-- Created      23-12-2021
-- Purpose      Returns the resources and sub-nodes located in a specified node. 
--				Only returns the first level items, doesn't recurse through subfolders.
--              Returns the data needed by the My Contributions screen (All content tab).
--
-- Modification History
--
-- 23-12-2021  RS	Initial Revision. Split out from original GetNodeContents proc.
-- 09-02-2022  KD	Explicitly exclude External Orgs from NodePath lookup.
-- 22-02-2022  RS	Explicitly exclude External Orgs from ResourceReference lookup.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodeContentsForCatalogueEditor]
(
	@NodeId INT
)

AS

BEGIN

	SELECT 
		ROW_NUMBER() OVER(ORDER BY DisplayOrder) AS Id,
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
		DurationInMilliseconds
	FROM
	(
		-- Published Folder Node/s
		SELECT 
			fnv.[Name],
			fnv.[Description],
			cn.NodeTypeId,
			nl.ChildNodeId AS NodeId,
			fnv.NodeVersionId,
			NULL AS ResourceId,
			NULL AS ResourceVersionId,
			NULL AS ResourceReferenceId,
			nl.DisplayOrder,
			NULL AS ResourceTypeId,
			NULL AS VersionStatusId,
			NULL AS UnpublishedByAdmin,
			NULL AS ResourceInEdit,
			NULL AS DraftResourceVersionId,
			NULL AS DurationInMilliseconds
		FROM 
			hierarchy.NodeLink nl
		INNER JOIN 
			hierarchy.[Node] cn ON nl.ChildNodeId = cn.Id
		INNER JOIN 
			hierarchy.NodeVersion nv ON nv.NodeId = cn.Id
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = nv.Id
		WHERE
			nl.ParentNodeId = @NodeId 
			AND nl.Deleted = 0
			AND cn.Deleted = 0
			AND fnv.Deleted = 0

		UNION

		-- Resources
		SELECT 
			rv.Title as [Name],
			NULL As [Description],
			0 as NodeTypeId, 
			NULL AS NodeId,
			NULL AS NodeVersionId,
			r.Id AS ResourceId,
			rv.Id AS ResourceVersionId,
			rr.OriginalResourceReferenceId AS ResourceReferenceId,
			nr.DisplayOrder,
			r.ResourceTypeId,
			CASE WHEN rvd.Id IS NOT NULL AND rvd.VersionStatusId > 1 THEN rvd.VersionStatusId ELSE rv.VersionStatusId END AS VersionStatusId, --rv.VersionStatusId,
			CASE WHEN rv.VersionStatusId = 3 AND rve.Id IS NOT NULL THEN 1 ELSE 0 END AS UnpublishedByAdmin,
			CASE WHEN rvd.Id IS NOT NULL THEN 1 ELSE 0 END AS ResourceInEdit,
			ISNULL(rvd.Id, rv.Id) AS DraftResourceVersionId,
			COALESCE(vrv.DurationInMilliseconds, arv.DurationInMilliseconds) AS DurationInMilliseconds
		FROM 
			hierarchy.NodeResource nr 
		INNER JOIN 
			resources.[Resource] r ON nr.ResourceId = r.Id
		INNER JOIN 
			resources.ResourceVersion rv ON rv.resourceId = nr.ResourceId
		LEFT JOIN 
			resources.ResourceReference rr ON rr.ResourceId = nr.ResourceId AND rr.Deleted = 0
								  AND rr.NodePathId NOT IN (SELECT np.Id FROM hierarchy.NodePath np INNER JOIN [hub].[ExternalOrganisation] eo ON eo.NodeId = np.NodeId)
		LEFT JOIN 
			hierarchy.NodePath np ON rr.NodePathId = np.Id AND nr.NodeId = np.NodeId AND np.IsActive = 1 AND np.Deleted = 0
								  AND np.Id NOT IN (SELECT np.Id FROM hierarchy.NodePath np INNER JOIN [hub].[ExternalOrganisation] eo ON eo.NodeId = np.NodeId)
		LEFT JOIN
			resources.VideoResourceVersion vrv ON vrv.ResourceVersionId = rv.Id AND vrv.Deleted = 0
		LEFT JOIN 
			resources.AudioResourceVersion arv ON arv.ResourceVersionId = rv.Id AND arv.Deleted = 0
		LEFT JOIN
			resources.ResourceVersionEvent rve ON rve.ResourceVersionId = rv.Id AND rve.ResourceVersionEventTypeId = 6 /* Unpublished by admin */
		LEFT JOIN 
			resources.ResourceVersion rvd ON r.Id = rvd.ResourceId AND rvd.Id > rv.Id AND rvd.Deleted = 0
		WHERE 
			nr.NodeId = @NodeId
			AND (r.CurrentResourceVersionId = rv.Id OR r.CurrentResourceVersionId IS NULL)
			AND ((nr.VersionStatusId IN (2,3) AND np.Id IS NOT NULL) OR nr.VersionStatusId = 1)
			AND nr.Deleted = 0
			AND r.Deleted = 0
			AND rv.Deleted = 0

	) AS t1
	ORDER BY NodeTypeId DESC, DisplayOrder ASC

END
GO