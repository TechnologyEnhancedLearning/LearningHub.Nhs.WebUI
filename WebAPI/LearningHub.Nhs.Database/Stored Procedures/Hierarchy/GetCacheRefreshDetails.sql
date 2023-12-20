-------------------------------------------------------------------------------
-- Author       KD
-- Created      09-10-2021
-- Purpose      Returns Node information for cache refresh, based on supplied Publication.
--
-- Modification History
--
-- 09-10-2021  KD	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetCacheRefreshDetails]
(
	@PublicationId int
)

AS

BEGIN

	SELECT DISTINCT
		npn.NodeId
	FROM
		hierarchy.PublicationLog pl
	INNER JOIN
		hierarchy.NodePath np ON pl.NodeId = np.NodeId
	INNER JOIN
		hierarchy.NodePathNode npn ON np.Id = npn.NodePathId
	WHERE
		pl.PublicationId = @PublicationId
		AND pl.Deleted = 0
		AND np.Deleted = 0
		AND npn.Deleted = 0

END
GO
