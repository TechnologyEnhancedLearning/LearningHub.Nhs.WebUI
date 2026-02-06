CREATE VIEW [dbo].[SearchCataloguesView] AS
WITH Catalogues AS (
    SELECT
        nv.NodeId AS Id,
        cnv.Name,
        cnv.Description,
        cnv.URL,
        n.Hidden,
		amend.MaxAmendDate AS AmendDate,
		cnv.Deleted,
        cnv.Id AS CatalogueNodeVersionId
    FROM hierarchy.CatalogueNodeVersion cnv
    INNER JOIN hierarchy.Node n ON n.CurrentNodeVersionId = cnv.NodeVersionId
    INNER JOIN hierarchy.NodeVersion nv ON nv.NodeId = n.Id
	CROSS APPLY (
        SELECT MAX(v) AS MaxAmendDate
        FROM (VALUES (cnv.AmendDate), (nv.AmendDate), (n.AmendDate)) AS value(v)
    ) AS amend
    WHERE nv.VersionStatusId = 2
      AND cnv.Deleted = 0 
      AND n.Deleted = 0 
      AND nv.Deleted = 0	
)
SELECT
    c.Id,
    c.Name,
    c.Description,
    c.URL,
    c.Hidden,
    c.CatalogueNodeVersionId,
	c.AmendDate,
	c.Deleted,
    -- Aggregate keywords into JSON array
    (
        SELECT STRING_AGG(cnvk.Keyword, ',')
        FROM hierarchy.CatalogueNodeVersionKeyword cnvk
        WHERE cnvk.CatalogueNodeVersionId = c.CatalogueNodeVersionId AND cnvk.Deleted = 0
    ) AS Keywords,

	-- Aggregate providers into JSON array
    (
		SELECT STRING_AGG(CAST(cvp.ProviderId AS varchar(10)), ',') 
        FROM hierarchy.CatalogueNodeVersionProvider cvp
        WHERE cvp.CatalogueNodeVersionId = c.CatalogueNodeVersionId AND cvp.Deleted = 0        
    ) AS Providers

FROM Catalogues c;
GO;

