-----------------------------------------------------------------------------------
------ Author       RS
------ Created      30-03-2023
------ Purpose      Returns the basic details of the current version of a hierarchy 
------              node. System currently only supports catalogues and folders but 
------              may be extended to include modules and courses in the future.
------
------ Modification History
------
------ 30-03-2023  RS	Initial Revision.
------ 30-05-2023  RS   Added NodePathId.
-----------------------------------------------------------------------------------
----CREATE PROCEDURE [hierarchy].[GetNodeDetails]
----(
----	@NodeId INT
----)

----AS

----BEGIN

----	DECLARE @NodePathId INT
----	SELECT	@NodePathId = Id
----		FROM	hierarchy.NodePath
----		WHERE	NodeId = @NodeId AND Deleted = 0 AND IsActive = 1

----	SELECT 
----		nv.NodeId AS NodeId,
----		@NodePathId AS NodePathId,
----		COALESCE(fnv.Name, cnv.Name) AS Name,
----		COALESCE(fnv.Description, cnv.Description) As Description
----	FROM hierarchy.Node n
----		INNER JOIN hierarchy.NodeVersion nv ON nv.Id = n.CurrentNodeVersionId
----		LEFT JOIN hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = nv.Id
----		LEFT JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id
----	WHERE n.Deleted = 0 AND n.Id = @NodeId

----END