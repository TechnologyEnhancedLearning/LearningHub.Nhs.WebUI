-------------------------------------------------------------------------------
-- Author       Binon Yesudhas
-- Created      06-02-2026
-- Purpose      View of resources for Azure AI search
--
-- Modification History
-- TD-6212 - https://hee-tis.atlassian.net/browse/TD-6212
-- 06-02-2026  Binon Yesudhas  Initial Revision
-- 13-02-2026  Binon Yesudhas  Handle inconsistenrt resource type
-------------------------------------------------------------------------------
CREATE VIEW [dbo].[SearchResourcesView]
AS
WITH BaseResource AS (
    SELECT
        r.Id,
		r.ResourceTypeId,
		-- r.ResourceTypeId as ContentType,
        CASE 
			WHEN LOWER(rt.Name) LIKE '%scorm%' THEN 'scorm'
			ELSE LOWER(rt.Name)
		END AS ContentType,
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
