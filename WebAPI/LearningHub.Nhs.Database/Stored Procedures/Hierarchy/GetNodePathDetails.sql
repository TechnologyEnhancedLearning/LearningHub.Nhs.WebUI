-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      09-05-2024
-- Purpose      Returns the basic details of the current version of a hierarchy 
--              node path.
--
-- Modification History
--
-- 09-05-2024  DB	Initial version.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodePathDetails]
(
	@NodePathId INT
)

AS

BEGIN


	SELECT 
		np.Id,
		np.NodeId,
		np.NodePath AS NodePathString,
		np.IsActive,
		np.CatalogueNodeId,
		COALESCE(fnv.Name, cnv.Name) AS Name,
		COALESCE(fnv.Description, cnv.Description) As Description
	FROM 
		hierarchy.NodePath np
	INNER JOIN
		hierarchy.Node n ON n.Id = np.NodeId
	INNER JOIN
		hierarchy.NodeVersion nv ON nv.Id = n.CurrentNodeVersionId
	LEFT JOIN
		hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = nv.Id
	LEFT JOIN
		hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id
	WHERE	np.Id = @NodePathId
		AND n.Deleted = 0
		AND np.Deleted = 0

END