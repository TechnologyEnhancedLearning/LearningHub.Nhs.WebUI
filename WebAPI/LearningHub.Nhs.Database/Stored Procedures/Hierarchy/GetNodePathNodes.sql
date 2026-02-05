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
-- 17-12-2025  SA   TD-6520 - Refractored the SP to fix the DTU spike.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetNodePathNodes]
(
    @NodePathId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    -- If NodePath is NVARCHAR, keep types consistent to avoid implicit conversions
    ;WITH PathParts AS
    (
        SELECT
            ordinal,
            TRY_CAST(s.value AS int) AS NodeIdInt
        FROM hierarchy.NodePath AS np
        CROSS APPLY STRING_SPLIT(np.NodePath, N'\', 1) AS s   -- 1 = ordinal
        WHERE np.Id = @NodePathId
    )
    SELECT
        pp.NodeIdInt AS NodeId,
        COALESCE(fnv.Name, cnv.Name) AS Name
    FROM PathParts AS pp
    JOIN hierarchy.NodeVersion AS nv
        ON nv.NodeId = pp.NodeIdInt
    LEFT JOIN hierarchy.CatalogueNodeVersion AS cnv
        ON cnv.NodeVersionId = nv.Id
    LEFT JOIN hierarchy.FolderNodeVersion AS fnv
        ON fnv.NodeVersionId = nv.Id
    WHERE pp.NodeIdInt IS NOT NULL
    ORDER BY pp.ordinal
    OPTION (RECOMPILE);  -- helps if @NodePathId cardinality varies a lot
END