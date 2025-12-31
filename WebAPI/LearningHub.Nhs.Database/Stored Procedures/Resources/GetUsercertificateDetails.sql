-------------------------------------------------------------------------------
-- Author       Tobi
-- Created      22-08-2025
-- Purpose      Gets the certificate details for all completed resources.
--
-- Modification History
-- 
-- 22-08-2025	Tobi	Initial Revision
-- 16-09-2025   Tobi    Added null check for ResourceReferenceID
--17-09-2025    Swapna  Added resource version id
-- 01-10-2025  SA added assesment score and passmark and provider details
-- 16=12-2025  SA TD-6322 added the condition to validate the certificate enabled is true for the latest resourceversion.
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[GetUsercertificateDetails]
    @UserId INT,
    @FilterText NVARCHAR(200) = N''
AS
BEGIN
    SET NOCOUNT ON;

    -- Temp table for better stats and indexing
    IF OBJECT_ID('tempdb..#MyActivity') IS NOT NULL 
        DROP TABLE #MyActivity;

    CREATE TABLE #MyActivity (
        ResourceId INT NOT NULL,
        ResourceActivityId INT NOT NULL,
        PRIMARY KEY CLUSTERED (ResourceActivityId)
    );
	-- to get the latest resourceVersion
	;WITH LatestResourceVersion AS (
    SELECT
        rv.ResourceId,
        rv.Id AS ResourceVersionId,
        rv.CertificateEnabled
    FROM resources.ResourceVersion rv
    WHERE rv.Deleted = 0
      AND rv.Id = (
            SELECT MAX(rv2.Id)
            FROM resources.ResourceVersion rv2
            WHERE rv2.ResourceId = rv.ResourceId
              AND rv2.Deleted = 0
        )
)
    INSERT INTO #MyActivity (ResourceId, ResourceActivityId)
    SELECT
        ra.ResourceId,
        MAX(ra.Id) AS ResourceActivityId
    FROM activity.ResourceActivity ra
    JOIN resources.Resource r
        ON ra.ResourceId = r.Id
    JOIN LatestResourceVersion lrv
        ON lrv.ResourceId = ra.ResourceId
    WHERE ra.UserId = @UserId
      AND lrv.CertificateEnabled = 1
      AND (
            (r.ResourceTypeId IN (2, 7) AND ra.ActivityStatusId = 3)
         OR (ra.ActivityStart < '2020-09-07T00:00:00+00:00')
         OR EXISTS (
                SELECT 1 
                FROM activity.MediaResourceActivity mar
                WHERE mar.ResourceActivityId = ra.Id 
                  AND mar.PercentComplete = 100
            )
         OR (r.ResourceTypeId = 6 AND (
                EXISTS (
                    SELECT 1 
                    FROM activity.ScormActivity sa
                    WHERE sa.ResourceActivityId = ra.Id
                      AND sa.CmiCoreLesson_status IN (3,5)
                )
                OR ra.ActivityStatusId IN (3,5)
            ))
        OR (
            r.ResourceTypeId = 11
            AND (
                -- Must be passed by score
                EXISTS (
                    SELECT 1
                    FROM activity.AssessmentResourceActivity ara
                    JOIN resources.AssessmentResourceVersion arv
                      ON arv.ResourceVersionId = ra.ResourceVersionId
                    WHERE ara.ResourceActivityId = ra.Id
                      AND ara.Score is not null AND ara.Score >= arv.PassMark -- formal AND informal assessment
                )
                -- Or explicitly marked as passed
                OR ra.ActivityStatusId = 5
            )
        )
         OR (r.ResourceTypeId IN (1, 5, 8, 9, 10, 12) 
             AND ra.ActivityStatusId = 3)
          )
    GROUP BY ra.ResourceId;

    -- Full result set without paging
    SELECT
	    rv.Id,
        rv.Title,
        r.ResourceTypeId,
        COALESCE((
			SELECT TOP (1) rr.OriginalResourceReferenceId
			FROM resources.ResourceReference rr
			JOIN hierarchy.NodePath np
			  ON np.Id = rr.NodePathId
			 AND np.NodeId = n.Id
			 AND np.Deleted = 0
			WHERE rr.ResourceId = rv.ResourceId
			  AND rr.Deleted = 0
		), '') AS ResourceReferenceID,
        rv.MajorVersion,
        rv.MinorVersion,
        COALESCE(ra.ActivityEnd, ra.ActivityStart) AS AwardedDate,
		NULL AS CertificateDownloadUrl,
		NULL AS CertificatePreviewUrl,
        rv.Id AS ResourceVersionId,
		rpAgg.ProvidersJson
    FROM #MyActivity ma
    JOIN activity.ResourceActivity ra 
        ON ra.Id = ma.ResourceActivityId
    JOIN resources.ResourceVersion rv 
        ON rv.Id = ra.ResourceVersionId 
       AND rv.Deleted = 0
    JOIN resources.Resource r 
        ON r.Id = rv.ResourceId
	LEFT JOIN (
				SELECT 
					rp.ResourceVersionId,
					JSON_QUERY('[' + STRING_AGG(
						'{"Id":' + CAST(p.Id AS NVARCHAR) +
						',"Name":"' + p.Name + '"' +
						',"Description":"' + p.Description + '"' +
						',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
					',') + ']') AS ProvidersJson
				FROM resources.ResourceVersionProvider rp
				JOIN hub.Provider p ON p.Id = rp.ProviderId
				WHERE p.Deleted = 0 and rp.Deleted = 0
				GROUP BY rp.ResourceVersionId
				) rpAgg ON rpAgg.ResourceVersionId = r.CurrentResourceVersionId
    JOIN hierarchy.Publication p 
        ON rv.PublicationId = p.Id 
       AND p.Deleted = 0
    JOIN hierarchy.NodeResource nr 
        ON r.Id = nr.ResourceId 
       AND nr.Deleted = 0
    JOIN hierarchy.Node n 
        ON n.Id = nr.NodeId 
       AND n.Hidden = 0 
       AND n.Deleted = 0
    JOIN hierarchy.NodePath np 
        ON np.NodeId = n.Id 
       AND np.Deleted = 0 
       AND np.IsActive = 1
    JOIN hierarchy.NodeVersion nv 
        ON nv.NodeId = np.CatalogueNodeId 
       AND nv.VersionStatusId = 2 
       AND nv.Deleted = 0
    JOIN hierarchy.CatalogueNodeVersion cnv 
        ON cnv.NodeVersionId = nv.Id 
       AND cnv.Deleted = 0
    WHERE (
        @FilterText = N'' 
        OR rv.Title LIKE @FilterText + N'%'
    )
    ORDER BY ma.ResourceActivityId DESC, rv.Title
    OPTION (RECOMPILE);
END;