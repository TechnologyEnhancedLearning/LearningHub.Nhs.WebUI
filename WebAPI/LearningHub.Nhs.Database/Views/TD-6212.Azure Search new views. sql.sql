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

---------------------------------------------------------------------------------------------------------------------

CREATE VIEW [dbo].[SearchResourcesView]
AS
WITH BaseResource AS (
    SELECT
        r.Id,
		r.ResourceTypeId,
		-- r.ResourceTypeId as ContentType,
        LOWER(rt.Name) AS ContentType,
        rv.Id AS ResourceVersionId,
        rv.Title,
        rv.Description,
        rv.ResourceAccessibilityId AS ResourceAccessLevel,
        p.Id AS PublicationId,
        p.CreateDate AS PublicationDate,
        rvrs.AverageRating,
		amend.MaxAmendDate AS AmendDate,
		CASE WHEN rv.VersionStatusId = 2 THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS Deleted,		
        ROW_NUMBER() OVER(PARTITION BY r.Id ORDER BY p.Id DESC) AS RowNumber
    FROM resources.ResourceVersion rv
    INNER JOIN hub.[User] u ON u.Id = rv.CreateUserId
    INNER JOIN resources.[Resource] r ON r.Id = rv.ResourceId
    INNER JOIN resources.ResourceVersionRatingSummary rvrs ON rvrs.ResourceVersionId = rv.Id
    INNER JOIN hierarchy.Publication p ON p.Id = rv.PublicationId
	INNER JOIN resources.ResourceType rt ON r.ResourceTypeId = rt.id
	CROSS APPLY (
        SELECT MAX(v) AS MaxAmendDate
        FROM (VALUES (rv.AmendDate), (r.AmendDate), (rvrs.AmendDate)) AS value(v)
    ) AS amend
    WHERE
        rv.VersionStatusId in (2,3)
        AND rv.Deleted = 0 AND r.Deleted = 0 AND rvrs.Deleted = 0 AND p.Deleted = 0
)
SELECT
    CAST(b.Id AS NVARCHAR(50)) AS ResourceId,
    b.ContentType,
	b.ResourceVersionId,
    b.Title,
    b.Description,
    b.ResourceAccessLevel,
    b.PublicationId,
    b.PublicationDate,
    b.AverageRating,
	b.AmendDate,
	b.Deleted,
    -- Keywords (JSON array)
    (	 
        SELECT  STRING_AGG(rvk.Keyword, ',') 
        FROM resources.ResourceVersionKeyword rvk
        WHERE rvk.ResourceVersionId = b.ResourceVersionId AND rvk.Deleted = 0
    ) AS Keywords,

    -- Authors (JSON array)
    (
		SELECT STRING_AGG(
			CASE
				WHEN (rva.AuthorName IS NULL OR rva.AuthorName = '') THEN rva.Organisation
				WHEN (rva.Organisation IS NULL OR rva.Organisation = '') THEN rva.AuthorName
				ELSE rva.AuthorName + ', ' + rva.Organisation
			END, '; '
		) 
		FROM resources.ResourceVersionAuthor rva
		WHERE rva.ResourceVersionId = b.ResourceVersionId AND rva.Deleted = 0
	) AS Authors,

    -- Active Catalogues (JSON array)
    (
        SELECT DISTINCT np.CatalogueNodeId,  rr.OriginalResourceReferenceId, cv.Name as LocationPaths
        FROM resources.ResourceReference rr
        INNER JOIN hierarchy.NodeResource nr ON nr.ResourceId = b.Id
        INNER JOIN hierarchy.Node n ON n.Id = nr.NodeId
        INNER JOIN hierarchy.NodePath np ON np.NodeId = n.Id
		LEFT JOIN hierarchy.CatalogueNodeVersion cv ON  n.CurrentNodeVersionId = cv.NodeVersionId
        WHERE rr.ResourceId = b.Id
          AND nr.VersionStatusId = 2
          AND np.IsActive = 1
          AND rr.Deleted = 0 AND nr.Deleted = 0 AND n.Deleted = 0 AND np.Deleted = 0
          AND np.Id = rr.NodePathId
		  FOR JSON PATH
    ) AS Catalogues,

    -- Author Date (only if type = 9, JSON object)
    (
    SELECT 
        TRY_CONVERT(date, 
            CONCAT(
                gfrv.AuthoredYear, '-', 
                RIGHT('0' + CAST(gfrv.AuthoredMonth AS varchar(2)), 2), '-', 
                RIGHT('0' + CAST(gfrv.AuthoredDayOfMonth AS varchar(2)), 2)
            )
        ) AS AuthoredDate
    FROM resources.GenericFileResourceVersion gfrv
    INNER JOIN resources.[File] f ON f.Id = gfrv.FileId
    INNER JOIN resources.FileType ft ON ft.Id = f.FileTypeId
    WHERE gfrv.ResourceVersionId = b.ResourceVersionId
      AND b.ResourceTypeId = 9
      AND gfrv.Deleted = 0 
      AND f.Deleted = 0 
      AND ft.Deleted = 0
)
 AS AuthoredDate,


    -- Providers (JSON array)
    (
        SELECT STRING_AGG(CAST(rvp.ProviderId AS varchar(10)), ',') 
        FROM resources.ResourceVersionProvider rvp
        WHERE rvp.ResourceVersionId = b.ResourceVersionId AND rvp.Deleted = 0        
    ) AS Providers

FROM BaseResource b
WHERE b.RowNumber = 1;
GO;

-------------------------------------------------------------------------------------------------------------------------------

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


-----------------------------------------------------------------------------------------------------------------



