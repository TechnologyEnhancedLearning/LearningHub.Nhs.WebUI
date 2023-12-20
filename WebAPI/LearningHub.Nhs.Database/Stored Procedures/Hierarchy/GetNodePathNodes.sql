-------------------------------------------------------------------------------
-- Author       RS
-- Created      30-03-2023
-- Purpose      Returns the basic details of all nodes in a hierarchy NodePath.
--              Used for constructing catalogue breadcrumbs, for example.
--
-- Modification History
--
-- 30-03-2023  RS	Initial Revision.
-- 30-05-2023  RS   Changed to use NodePathId.
-- 22-06-2023  RS   Switched order of joins to ensure catalogue node is always returned first.
-- 24-08-2023  RS   Proper fix for ordering issue - STRING_SPLIT doesn't return substrings in order.
-- 08-09-2023  RS   A further fix for ordering issue that works on SQL Server 2019 (for developer installs).
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodePathNodes]
(
	@NodePathId INT
)

AS

BEGIN

	SELECT 
		CAST(value AS INT) AS NodeId,
		COALESCE(fnv.Name, cnv.Name) AS Name
	FROM hierarchy.NodePath np
		CROSS APPLY hub.fn_Split(NodePath, '\') as ss
		INNER JOIN hierarchy.NodeVersion nv ON nv.NodeId = ss.value
		LEFT JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id
		LEFT JOIN hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = nv.Id
	WHERE np.Id = @NodePathId
	ORDER BY ss.idx

END