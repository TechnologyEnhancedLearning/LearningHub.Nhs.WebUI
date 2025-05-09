﻿-------------------------------------------------------------------------------
-- Author       RS
-- Created      23-12-2021
-- Purpose      Returns the resources and sub-nodes located in a specified node. 
--				Only returns the first level items, doesn't recurse through subfolders.
--              Returns the data needed by the Catalogue landing page screen (Browse tab).
--
-- Modification History
--
-- 23-12-2021  RS	Initial Revision. Split out from original GetNodeContents proc.
-- 09-02-2022  KD	Explicitly exclude External Orgs from Resource Reference lookup.
-- 27-03-2023  RS   Added rating data for progressive enhancement work.
-- 11-05-2023  RS   Removed Description and AuthoredBy as no longer required for screen.
-- 15-05-2023  RS   Added AuthoredBy back in following design decision change.
-- 23-06-2023  RS   Removed AverageRating and RatingCount as not required from this proc. That data comes separately from RatingService.
-- 05-06-2023  SA   Modified the sp to fix the sql timeout issues.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodeContentsForCatalogueBrowse]
(
	@NodeId INT
)

AS

BEGIN

    IF @NodeId IS NULL
    BEGIN
        RAISERROR('NodeId cannot be null', 16, 1)
        RETURN
    END

	;WITH CTENode AS(SELECT DISTINCT NodeId FROM hierarchy.NodeResourceLookup WHERE Deleted = 0)

	SELECT 
		ROW_NUMBER() OVER(ORDER BY DisplayOrder) AS Id,
		[Name],
		NodeTypeId,
		NodeId,
		NodeVersionId,
		ResourceId,
		ResourceVersionId,
		ResourceReferenceId,
		ISNULL(DisplayOrder, 0) AS DisplayOrder,
		ResourceTypeId,
		VersionStatusId,
		AuthoredBy,
		DurationInMilliseconds,
		IsEmptyFolder
	FROM
	(
		-- Published Folder Node/s
		SELECT 
			fnv.[Name],
			cn.NodeTypeId,
			nl.ChildNodeId AS NodeId,
			fnv.NodeVersionId,
			NULL AS ResourceId,
			NULL AS ResourceVersionId,
			NULL AS ResourceReferenceId,
			nl.DisplayOrder,
			NULL AS ResourceTypeId,
			NULL AS VersionStatusId,
			NULL AS AuthoredBy,
			NULL AS DurationInMilliseconds,
			CAST(1 AS BIT) AS IsEmptyFolder
		FROM 
			hierarchy.NodeLink nl
		INNER JOIN 
			hierarchy.[Node] cn ON nl.ChildNodeId = cn.Id
		INNER JOIN 
			hierarchy.NodeVersion nv ON nv.NodeId = cn.Id
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = nv.Id
		INNER JOIN -- Exclude folders with no published resources.
			CTENode nrl ON cn.Id = nrl.NodeId
		WHERE
			nl.ParentNodeId = @NodeId 
			AND nl.Deleted = 0
			AND cn.Deleted = 0
			AND fnv.Deleted = 0

		UNION

		-- Resources
		SELECT 
			rv.Title as [Name],
			0 as NodeTypeId, 
			NULL AS NodeId,
			NULL AS NodeVersionId,
			r.Id AS ResourceId,
			rv.Id AS ResourceVersionId,
			rr.OriginalResourceReferenceId AS ResourceReferenceId,
			nr.DisplayOrder,
			r.ResourceTypeId,
			rv.VersionStatusId,
			SUBSTRING(
				(SELECT ' - ' + 
					IIF(rva.AuthorName IS NOT NULL AND LEN(LTRIM(RTRIM(rva.AuthorName))) > 0, rva.AuthorName, '') + 
					IIF(rva.AuthorName IS NOT NULL AND LEN(LTRIM(RTRIM(rva.AuthorName))) > 0 AND rva.Organisation IS NOT NULL AND LEN(LTRIM(RTRIM(rva.Organisation))) > 0, ', ', '') +
					IIF(rva.Organisation IS NOT NULL AND LEN(LTRIM(RTRIM(rva.Organisation))) > 0, rva.Organisation, '') + 
					IIF(rva.Role IS NOT NULL AND LEN(LTRIM(RTRIM(rva.Role))) > 0, ', ' + rva.Role, '')
				FROM resources.ResourceVersionAuthor rva WHERE rva.ResourceVersionId = rv.Id AND rva.Deleted = 0 FOR XML PATH(''))
			, 4,10000) AS AuthoredBy,
			COALESCE(vrv.DurationInMilliseconds, arv.DurationInMilliseconds) AS DurationInMilliseconds,
			CAST(0 AS BIT) AS IsEmptyFolder
		FROM 
			hierarchy.NodeResource nr 
		INNER JOIN 
			resources.[Resource] r ON nr.ResourceId = r.Id
		INNER JOIN 
			resources.ResourceVersion rv ON rv.resourceId = nr.ResourceId
		INNER JOIN 
			resources.ResourceReference rr ON rr.ResourceId = nr.ResourceId AND rr.Deleted = 0
		INNER JOIN 
			hierarchy.NodePath np ON rr.NodePathId = np.Id AND np.Deleted = 0
		INNER JOIN 
			hierarchy.[Node] n ON np.NodeId = n.Id AND n.Deleted = 0
		LEFT JOIN
			resources.VideoResourceVersion vrv ON vrv.ResourceVersionId = rv.Id AND vrv.Deleted = 0
		LEFT JOIN 
			resources.AudioResourceVersion arv ON arv.ResourceVersionId = rv.Id AND arv.Deleted = 0
		WHERE 
			nr.NodeId = @NodeId
			AND r.CurrentResourceVersionId = rv.Id
			AND rv.VersionStatusId = 2 AND nr.VersionStatusId = 2
			AND n.NodeTypeId IN (1, 2, 3) -- Catalogue, Course, Folder only
			AND np.Id NOT IN (SELECT np.Id FROM hierarchy.NodePath np INNER JOIN [hub].[ExternalOrganisation] eo ON eo.NodeId = np.NodeId) -- explicitly exclude Ext Refs
			AND nr.Deleted = 0
			AND r.Deleted = 0
			AND rv.Deleted = 0

	) AS t1
	ORDER BY NodeTypeId DESC,DisplayOrder ASC
END
GO