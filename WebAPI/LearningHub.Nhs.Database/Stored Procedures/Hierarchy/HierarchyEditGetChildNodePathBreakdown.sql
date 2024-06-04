-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      28 May 2024
-- Purpose      Return the breakdown of the child NodePaths
--              based on an in-progress Hierarchy Edit.
--              This includes the node path breakdown for duplicate
--              nodes elsewhere in the structure.
--
-- Modification History
--
-- 28-05-2024  DB	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditGetChildNodePathBreakdown]
(
	@NodePathId int
)
AS

BEGIN

	DECLARE @HierarchyEditId int
    DECLARE @RootNodePathId INT

	-- Get the latest Hierarchy Edit in Draft status, the NodePathId can only be in one Draft Hierarchy Edit at a time
	SELECT  TOP 1 
            @HierarchyEditId = he.Id ,
            @RootNodePathId = he.RootNodePathId
	FROM	hierarchy.HierarchyEdit he
    INNER JOIN hierarchy.HierarchyEditDetail hed ON he.Id = hed.HierarchyEditId 
	WHERE	hed.NodePathId = @NodePathId
		AND he.HierarchyEditStatusId = 1 -- Draft
		AND he.Deleted = 0
	ORDER BY he.Id DESC


    ; WITH cteNodepathDetails (NodePathId, NodePathIdPath, NodeVersionIdPath, CompletePathInd)
    AS
    (

    	SELECT 
    		hed.NodePathId,
    		NodePathIdPath = CAST(hed.NodePathId AS nvarchar(256)),
            NodeVersionIdPath = CAST(hed.NodeVersionId AS nvarchar(256)),
    		CompletePathInd = CASE WHEN hed.NodePathID = @RootNodePathId THEN 1 ELSE 0 END
    	FROM
    		[hierarchy].[HierarchyEditDetail] hed
        INNER JOIN
            [hierarchy].[HierarchyEditDetail] hed2 ON hed.Nodeid = hed2.NodeId
        WHERE
                hed2.ParentNodePathId = @NodePathId
            AND hed.HierarchyEditDetailTypeId != 5 -- exclude Node Resource
            AND hed2.HierarchyEditDetailTypeId != 5 -- exclude Node Resource
            AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND ISNULL(hed2.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND hed.HierarchyEditId = @HierarchyEditId
            AND hed2.HierarchyEditId = @HierarchyEditId
            AND hed.Deleted = 0
            AND hed2.Deleted = 0

    	UNION ALL

    	SELECT 
    		hed.ParentNodePathId AS NodePathId,
    		CAST(CAST(hed.ParentNodePathID AS nvarchar(10)) + '\' + cte.NodePathIdPath AS nvarchar(256)) AS NodePathIdPath,
            CAST(CAST(p_hed.NodeVersionId AS nvarchar(10)) + '\' + cte.NodeVersionIdPath AS nvarchar(256)) AS NodeVersionIdPath,
    		CompletePathInd = CASE WHEN hed.ParentNodePathID = @RootNodePathId THEN 1 ELSE 0 END
    	FROM
    		[hierarchy].[HierarchyEditDetail] hed
        INNER JOIN
            [hierarchy].[HierarchyEditDetail] p_hed ON hed.ParentNodePathId = p_hed.NodePathId
    	INNER JOIN	
    		cteNodepathDetails cte ON hed.NodePathId = cte.NodePathId
        WHERE
                hed.HierarchyEditDetailTypeId != 5 -- exclude Node Resource
            AND p_hed.HierarchyEditDetailTypeId != 5 -- exclude Node Resource
            AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND ISNULL(p_hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND hed.HierarchyEditId = @HierarchyEditId
            AND p_hed.HierarchyEditId = @HierarchyEditId
            AND hed.Deleted = 0
            AND p_hed.Deleted = 0
    )

    SELECT  CAST(SUBSTRING(NodePathIdPath, LEN(NodePathIdPath) - (CHARINDEX('\', REVERSE(NodePathIdPath)) - 2), LEN(NodePathIdPath)) AS INT) AS NodePathId, 
            sp2.[value] AS DepthNodePathId,
            NodePathIdPath,
            NodeVersionIdPath,
            sp1.idx AS Depth,
            sp1.[value] as NodeVersionId,
            COALESCE(d_npdv.DisplayName, p_npdv.DisplayName, cnv.Name, fnv.Name) AS NodeName
    INTO    #NodePathDetails
    FROM    cteNodepathDetails cte
    CROSS APPLY hub.fn_Split(NodeVersionIdPath, '\') as sp1
    CROSS APPLY hub.fn_Split(NodePathIdPath, '\') as sp2
    LEFT JOIN
            hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = sp1.[value]
    LEFT JOIN
            hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = sp1.[value]
    LEFT JOIN
            hierarchy.NodePathDisplayVersion d_npdv ON sp2.[value] = d_npdv.NodePathId AND d_npdv.VersionStatusId = 1 /* Draft */ AND d_npdv.Deleted = 0
    LEFT JOIN
            hierarchy.NodePathDisplayVersion p_npdv ON sp2.[value] = p_npdv.NodePathId AND p_npdv.VersionStatusId = 2 /* Published */ AND p_npdv.Deleted = 0

    WHERE   sp1.idx = sp2.idx
        AND CompletePathInd = 1
   
    SELECT  ROW_NUMBER() OVER(ORDER BY hed.NodeId, npd.NodePathId) AS Id,
            hed.NodeId,
            npd.NodePathId,
            npd.Depth,
            npd.NodeName
    FROM    #NodePathDetails npd
    INNER JOIN
            [hierarchy].[HierarchyEditDetail] hed ON npd.NodePathId = hed.NodePathId
    WHERE
            hed.HierarchyEditDetailTypeId != 5 -- exclude Node Resource
        AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
        AND hed.HierarchyEditId = @HierarchyEditId
        AND hed.Deleted = 0
    ORDER BY hed.NodeId, npd.NodePathId, npd.Depth

    DROP table #NodePathDetails

END

