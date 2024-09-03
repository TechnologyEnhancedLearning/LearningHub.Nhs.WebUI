-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      29 August 2024
-- Purpose      Return if Node has an external reference
--
-- Modification History
--
-- 29-08-2024  SS	Initial Revision.

-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CheckCatalogueHasExternalReference]
(
	@CatalogueNodeId int,
	@HasExternalCatalogueReference bit OUTPUT  
)
AS

BEGIN
		;WITH
		  cteNode(NodeId,  ParentNodeId, PrimaryCatalogueNodeId, InitialNodePath)
		  AS
		  (
			SELECT 
				n.Id AS NodeId,
				ParentNodeId = NULL,
                nv.PrimaryCatalogueNodeId,
				CAST(n.Id AS nvarchar(128)) AS InitialNodePath
			 FROM 
                 hierarchy.NodePath np
             INNER JOIN
			 	hierarchy.[Node] n ON np.NodeId = n.Id
             INNER JOIN
                hierarchy.NodeVersion nv ON n.CurrentNodeVersionId = nv.Id
			 WHERE 
			 	nv.NodeId=@catalogueNodeId
                AND nv.VersionStatusId = 2 -- Published
                AND np.Deleted = 0
                AND n.Deleted = 0
                AND nv.Deleted = 0

			UNION ALL

			SELECT 
				ChildNodeId AS NodeId,
				nl.ParentNodeId,
                nv.PrimaryCatalogueNodeId,
				CAST(cte.InitialNodePath + '\' + CAST(ChildNodeId AS nvarchar(8)) AS nvarchar(128)) AS InitialNodePath
			FROM 
				hierarchy.NodeLink nl
			INNER JOIN
				hierarchy.[Node] n ON nl.ChildNodeId = n.Id
			INNER JOIN	
				cteNode cte ON nl.ParentNodeId = cte.NodeId
            INNER JOIN
                hierarchy.NodeVersion nv ON n.CurrentNodeVersionId = nv.Id
			WHERE 
				n.CurrentNodeVersionId IS NOT NULL
                AND nv.VersionStatusId = 2 -- Published
                AND n.Deleted = 0
                AND nl.Deleted = 0

			),
		
		 cteResource
		 AS (
				SELECT cte.PrimaryCatalogueNodeId 
							FROM
								cteNode cte
				UNION 

				SELECT 
					rv.PrimaryCatalogueNodeId AS PrimaryCatalogueNodeId
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
					resources.VideoResourceVersion vrv ON vrv.ResourceVersionId = rv.Id AND vrv.Deleted = 0
				LEFT JOIN 
					resources.AudioResourceVersion arv ON arv.ResourceVersionId = rv.Id AND arv.Deleted = 0
				LEFT JOIN
					resources.ResourceVersionEvent rve ON rve.ResourceVersionId = rv.Id AND rve.ResourceVersionEventTypeId = 6 /* Unpublished by admin */
				LEFT JOIN 
					resources.ResourceVersion rvd ON r.Id = rvd.ResourceId AND rvd.Id > rv.Id AND rvd.Deleted = 0
				WHERE 
					np.CatalogueNodeId = @CatalogueNodeId
					 and rv.PrimaryCatalogueNodeId <>np.CatalogueNodeId
					AND r.CurrentResourceVersionId IS not NULL
					AND nr.VersionStatusId=2
					AND nr.Deleted = 0
					AND r.Deleted = 0
					AND rv.Deleted = 0
			)
	SELECT @HasExternalCatalogueReference=case when  count(distinct cte.PrimaryCatalogueNodeId ) > 1 then 1 ELSE 0 END 

	FROM
			cteResource cte
END
GO