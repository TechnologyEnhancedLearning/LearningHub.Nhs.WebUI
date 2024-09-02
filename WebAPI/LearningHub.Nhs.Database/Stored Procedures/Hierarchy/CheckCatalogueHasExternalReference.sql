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
		  cteExternalReference(NodeId,  ParentNodeId, PrimaryCatalogueNodeId, InitialNodePath)
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
				cteExternalReference cte ON nl.ParentNodeId = cte.NodeId
            INNER JOIN
                hierarchy.NodeVersion nv ON n.CurrentNodeVersionId = nv.Id
			WHERE 
				n.CurrentNodeVersionId IS NOT NULL
                AND nv.VersionStatusId = 2 -- Published
                AND n.Deleted = 0
                AND nl.Deleted = 0

			)
		
		SELECT @HasExternalCatalogueReference=case when  count(distinct cte.PrimaryCatalogueNodeId ) > 1 then 1 ELSE 0 END 
		FROM
			cteExternalReference cte
END
GO