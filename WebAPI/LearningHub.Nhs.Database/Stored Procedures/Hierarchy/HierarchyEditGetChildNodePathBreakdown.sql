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
-- 12-06-2024  DB	Updated to include Node Resources.
-- 08-07-2024  DB	Updated to include external catalogues.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditGetChildNodePathBreakdown]
(
	@NodePathId int
)
AS

BEGIN

	DECLARE @HierarchyEditId int
    DECLARE @RootNodePathId INT
    DECLARE @CatalogueNodeId INT

	-- Get the latest Hierarchy Edit in Draft status, the NodePathId can only be in one Draft Hierarchy Edit at a time
	SELECT  TOP 1 
            @HierarchyEditId = he.Id,
            @RootNodePathId = he.RootNodePathId,
            @CatalogueNodeId = np.CatalogueNodeId
	FROM	hierarchy.HierarchyEdit he
    INNER JOIN hierarchy.HierarchyEditDetail hed ON he.Id = hed.HierarchyEditId 
    INNER JOIN hierarchy.NodePath np ON he.RootNodePathId = np.Id
	WHERE	hed.NodePathId = @NodePathId
		AND he.HierarchyEditStatusId = 1 -- Draft
		AND he.Deleted = 0
	ORDER BY he.Id DESC

    ; WITH cteNodepathDetails (NodeId, NodePathId, ComponentNodePathId, ResourceId, ResourceVersionId, NodePathIdPath, NodeVersionIdPath, CompletePathInd)
    AS
    (

    	SELECT 
            hed.NodeId,
    		ISNULL(hed.NodePathId, hed.ParentNodePathId) AS NodePathId,
            CASE WHEN hed.ResourceId IS NULL THEN hed.NodePathId ELSE hed.ParentNodePathId END AS ComponentNodePathId,
            hed.ResourceId AS ResourceId,
            hed.ResourceVersionId,
    		NodePathIdPath = CAST(ISNULL(hed.NodePathId, hed.ParentNodePathId) AS nvarchar(256)),
            NodeVersionIdPath = CAST(ISNULL(hed.NodeVersionId, hed_ResParent.NodeVersionId) AS nvarchar(256)),
    		CompletePathInd = CASE WHEN hed.NodePathID = @RootNodePathId THEN 1 ELSE 0 END
    	FROM
    		[hierarchy].[HierarchyEditDetail] hed
        INNER JOIN
            [hierarchy].[HierarchyEditDetail] hed2 ON (hed.Nodeid = hed2.NodeId) OR (hed.ResourceId = hed2.ResourceId)
        LEFT JOIN
            [hierarchy].[HierarchyEditDetail] hed_ResParent ON hed.ParentNodePathId = hed_ResParent.NodePathId
                                                        AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
                                                        AND hed.Deleted = 0
                                                        AND hed_ResParent.Deleted = 0
        WHERE
                hed2.ParentNodePathId = @NodePathId
            AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND ISNULL(hed2.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND hed.HierarchyEditId = @HierarchyEditId
            AND hed2.HierarchyEditId = @HierarchyEditId
            AND hed.Deleted = 0
            AND hed2.Deleted = 0

    	UNION ALL

    	SELECT 
            cte.NodeId,
    		hed.ParentNodePathId AS NodePathId,
            cte.ComponentNodePathId,
            cte.ResourceId,
            cte.ResourceVersionId,
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
                hed.HierarchyEditDetailTypeId != 5 -- exclude Node Resource for node path creation
            AND p_hed.HierarchyEditDetailTypeId != 5 -- exclude Node Resource for node path creation
            AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND ISNULL(p_hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
            AND hed.HierarchyEditId = @HierarchyEditId
            AND p_hed.HierarchyEditId = @HierarchyEditId
            AND hed.Deleted = 0
            AND p_hed.Deleted = 0
    )

    SELECT  NodeId, NodePathId, ComponentNodePathId, ResourceId, ResourceVersionId, NodePathIdPath, NodeVersionIdPath, CompletePathInd
    INTO    #NodePathDetails
    FROM    cteNodepathDetails
    WHERE   CompletePathInd = 1;


    -- Add paths from external catalogues
    ; WITH cteExtNodepathDetails (NodeId, NodePathId, ComponentNodePathId, ResourceId, ResourceVersionId, NodePathIdPath, NodeVersionIdPath, CompletePathInd,ParentNodePath)
    AS
    (
        SELECT 
            n_np.NodeId AS NodeId,
    		ISNULL(n_np.Id, r_np.Id) AS NodePathId,
            ISNULL(n_np.Id, r_np.Id) AS ComponentNodePathId,
            hed.ResourceId AS ResourceId,
            hed.ResourceVersionId,
    		NodePathIdPath = CAST(ISNULL(n_np.Id, r_np.Id) AS nvarchar(256)),
            NodeVersionIdPath = CAST(ISNULL(hed.NodeVersionId, nv.Id) AS nvarchar(256)),
                CASE WHEN CHARINDEX('\', ISNULL(n_np.NodePath, r_np.NodePath)) = 0 THEN 1 ELSE 0 END AS CompletePathInd,
                CASE WHEN CHARINDEX('\', ISNULL(n_np.NodePath, r_np.NodePath)) = 0 THEN '' ELSE LEFT(ISNULL(n_np.NodePath, r_np.NodePath), LEN(ISNULL(n_np.NodePath, r_np.NodePath)) - CHARINDEX('\', REVERSE(ISNULL(n_np.NodePath, r_np.NodePath)))) END AS ParentNodePath
    	FROM
    		[hierarchy].[HierarchyEditDetail] hed
        LEFT JOIN
            [hierarchy].[NodePathNode] npn ON hed.NodeId = npn.NodeId
        LEFT JOIN
            [hierarchy].[NodePath] n_np on npn.NodePathId = n_np.Id
                                            AND n_np.IsActive = 1
                                            AND npn.NodeId = n_np.NodeId
                                            AND n_np.CatalogueNodeId != @CatalogueNodeId
                                            AND n_np.Deleted = 0
        LEFT JOIN
            [hierarchy].[NodeResource] nr ON hed.ResourceId = nr.ResourceId
                                            AND nr.VersionStatusId = 2 -- Published
                                            AND nr.Deleted = 0
        LEFT JOIN
            [hierarchy].[NodePath] r_np on nr.NodeId = r_np.NodeId
                                            AND r_np.IsActive = 1
                                            AND r_np.CatalogueNodeId != @CatalogueNodeId
                                            AND r_np.Deleted = 0
        LEFT JOIN
			[hierarchy].[Node] n ON r_np.NodeId = n.Id
                                            AND n.Deleted = 0
        LEFT JOIN
			[hierarchy].[NodeVersion] nv ON n.CurrentNodeVersionId = nv.Id
                                            AND nv.VersionStatusId = 2 -- Published
                                            AND nv.Deleted = 0


        WHERE
            hed.HierarchyEditId = @HierarchyEditId
            AND hed.ParentNodePathId = @NodePathId

    	UNION ALL

        SELECT  
                cte.NodeId,
                np.Id AS NodePathId,
                cte.ComponentNodePathId,
                cte.ResourceId,
                cte.ResourceVersionId,
                CAST(CAST(np.Id AS nvarchar(10)) + '\' + cte.NodePathIdPath AS nvarchar(256)) AS NodePathIdPath,
                CAST(CAST(nv.Id AS nvarchar(10)) + '\' + cte.NodeVersionIdPath AS nvarchar(256)) AS NodeVersionIdPath,
                CASE WHEN CHARINDEX('\', np.NodePath) = 0 THEN 1 ELSE 0 END AS CompletePathInd,
                CASE WHEN CHARINDEX('\', np.NodePath) = 0 THEN '' ELSE LEFT(np.NodePath, LEN(np.NodePath) - CHARINDEX('\', REVERSE(np.NodePath))) END AS ParentNodePath
        FROM    cteExtNodepathDetails cte
        INNER JOIN hierarchy.NodePath np ON cte.ParentNodePath = np.NodePath
        INNER JOIN hierarchy.Node n ON np.NodeId = n.Id
        INNER JOIN hierarchy.NodeVersion nv ON n.CurrentNodeVersionId = nv.Id
        WHERE   np.IsActive = 1
            AND nv.VersionStatusId = 2 -- Published
            AND np.Deleted = 0
            AND n.Deleted = 0
            AND nv.Deleted = 0
    )

    INSERT INTO #NodePathDetails (NodeId, NodePathId, ComponentNodePathId, ResourceId, ResourceVersionId, NodePathIdPath, NodeVersionIdPath, CompletePathInd)
    SELECT  NodeId, NodePathId, ComponentNodePathId, ResourceId, ResourceVersionId, NodePathIdPath, NodeVersionIdPath, CompletePathInd
    FROM    cteExtNodepathDetails
    WHERE   CompletePathInd = 1;


    SELECT  ROW_NUMBER() OVER(ORDER BY cte.NodeId, ComponentNodePathId) AS Id,
            ISNULL(cte.NodeId, 0) AS NodeId,
            ISNULL(cte.ResourceId, 0) AS ResourceId,
            ComponentNodePathId AS NodePathId,
            sp1.idx AS Depth,
            COALESCE(d_npdv.DisplayName, p_npdv.DisplayName, cnv.Name, fnv.Name, rv.Title) AS NodeName
    FROM    #NodePathDetails cte
    CROSS APPLY hub.fn_Split(CASE WHEN cte.ResourceId IS NULL THEN NodeVersionIdPath ELSE NodeVersionIdPath + '\' + CAST(ResourceVersionId AS nvarchar(10)) END, '\') as sp1
    CROSS APPLY hub.fn_Split(CASE WHEN cte.ResourceId IS NULL THEN NodePathIdPath ELSE NodePathIdPath + '\0' END, '\') as sp2
    LEFT JOIN
            hierarchy.FolderNodeVersion fnv ON fnv.NodeVersionId = sp1.[value]
    LEFT JOIN
            hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = sp1.[value]
    LEFT JOIN
            hierarchy.NodePathDisplayVersion d_npdv ON sp2.[value] = d_npdv.NodePathId AND d_npdv.VersionStatusId = 1 /* Draft */ AND d_npdv.Deleted = 0 AND cte.ResourceId IS NULL
    LEFT JOIN
            hierarchy.NodePathDisplayVersion p_npdv ON sp2.[value] = p_npdv.NodePathId AND p_npdv.VersionStatusId = 2 /* Published */ AND p_npdv.Deleted = 0 AND cte.ResourceId IS NULL
    LEFT JOIN
            resources.ResourceVersion rv ON rv.Id = sp1.[value] AND cte.ResourceId IS NOT NULL
    WHERE   sp1.idx = sp2.idx
    ORDER BY NodeId, NodePathId, ResourceId, Depth;

    DROP TABLE #NodePathDetails;
END

