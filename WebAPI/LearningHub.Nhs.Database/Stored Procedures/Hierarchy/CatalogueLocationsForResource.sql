-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Returns Catalogue Location info for the supplied ResourceId.
--				Temporary means to retrieve this data - MVP.
--		
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueLocationsForResource]
(
	@resourecId int
)

AS

BEGIN

	SELECT DISTINCT
		cn.Id AS CatalogueNodeId,
		COALESCE(npd.DisplayName, cn.[Name]) AS CatalogueName,
		npd.Icon, 
		resourceCount.ResourceCount
	FROM
		hierarchy.NodePathResource npr
	INNER JOIN
		hierarchy.NodePath np ON npr.NodePathId = np.Id
	INNER JOIN 
		hierarchy.[Node] cn on np.CatalogueNodeId = cn.Id
	INNER JOIN
		hierarchy.NodePath cnp ON cnp.NodeId = cn.Id
	LEFT JOIN
		hierarchy.NodePathDisplay npd ON cnp.Id = npd.NodePathId
	INNER JOIN (SELECT 
					np.CatalogueNodeId, ResourceCount = COUNT(DISTINCT npr.ResourceId)
				FROM 
					hierarchy.NodePathResource  npr
				INNER JOIN
					hierarchy.NodePath np ON npr.NodePathId = np.Id
				GROUP BY
					np.CatalogueNodeId) AS resourceCount ON cn.Id = resourceCount.CatalogueNodeId
	WHERE
		ResourceId=@resourecId

END

GO