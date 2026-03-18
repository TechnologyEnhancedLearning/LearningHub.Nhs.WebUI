-------------------------------------------------------------------------------
-- Author       Binon Yesudhas
-- Created      06-02-2026
-- Purpose      Combined view of both catalogues and resources for Azure AI search
--
-- Modification History
-- TD-6212 - https://hee-tis.atlassian.net/browse/TD-6212
-- 06-02-2026  Binon Yesudhas  Initial Revision
-- 05-03-2026  Binon Yesudhas  Added new parameter, ResourceAccessLevel to the view
-------------------------------------------------------------------------------
CREATE VIEW [dbo].[SupersetSearchView]
AS

-- ============================
-- Catalogue Rows
-- ============================
SELECT 
    'cat-'+ CAST(c.Id AS NVARCHAR(50)) AS id,
    c.Name AS title,
	LOWER(c.Name) AS normalised_title,
    c.Description AS description,
    'catalogue' AS resource_collection,    
    c.Keywords as manual_tag,             -- e.g., comma-separated or JSON if multiple
	'catalogue' AS resource_type,
	NULL as resource_access_level,
	NULL AS publication_date,
	NULL AS date_authored, 
	NULL AS rating,
	NULL AS catalogue_id,
	NULL AS resource_reference_id,
	NULL AS location_paths,
	CAST(0 AS bit) AS statutory_mandatory,
	c.Providers AS provider_ids,          -- e.g., comma-separated or JSON if multiple
    NULL AS author,
	CAST(HIdden AS bit) AS hidden,
	URL AS url,
	--c.AmendDate AS last_modified,
	SWITCHOFFSET(CAST(c.AmendDate AS datetimeoffset), '+00:00') AS last_modified,
	CAST(HIdden AS bit) is_deleted
FROM dbo.SearchCataloguesView c

UNION ALL

-- ============================
-- Resource Rows
-- ============================
SELECT 
    'res-'+ CAST(r.ResourceId AS NVARCHAR(50)) AS id,
    r.title,
	LOWER(r.title) AS normalised_title,
    r.Description AS description,
    'resource' AS resource_collection,   
    r.Keywords AS manual_tag,
	r.ContentType AS resource_type,
	r.ResourceAccessLevel as resource_access_level,
	r.PublicationDate AS publication_date,
	r.AuthoredDate AS date_authored,
	CAST(r.AverageRating AS FLOAT) AS rating, 
	JSON_VALUE(r.Catalogues, '$[0].CatalogueNodeId') AS catalogue_id,
	JSON_VALUE(r.Catalogues, '$[0].OriginalResourceReferenceId') AS resource_reference_id,
	JSON_VALUE(r.Catalogues, '$[0].LocationPaths') AS location_paths,
	CAST(0 AS bit) AS statutory_mandatory,
	r.Providers AS provider_ids,  	
    r.Authors AS author,
	CAST(0 AS bit) AS hidden,
	NULL AS url,
	--r.AmendDate AS last_modified,
	SWITCHOFFSET(CAST(r.AmendDate AS datetimeoffset), '+00:00') AS last_modified,
	r.Deleted AS is_deleted
FROM dbo.SearchResourcesView r;
GO;



