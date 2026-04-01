-------------------------------------------------------------------------------
-- Author       OA
-- Created      24 JUN 2024 Nov 2020
-- Purpose      Break down the GetDashboardResources SP to smaller SP for a specific data type
--
-- Modification History
--
-- 24 Jun 2024  OA  Initial Revision
-- 27 Jun 2024  SA  Removed unused temp tables
-- 29 Sep 2025  SA  Integrated the provider details 
-- 31 Mar 2026  OA  TD-7057 Script Optimization
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetMyRecentCompletedDashboardResources]
(
    @UserId        INT,
    @PageNumber    INT = 1,
    @TotalRecords  INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxPageNumber INT = 4;
    DECLARE @FetchRows INT = 3;

    IF @PageNumber > @MaxPageNumber
        SET @PageNumber = @MaxPageNumber;

    DECLARE @MaxRows INT = @MaxPageNumber * @FetchRows;
    DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows;

    -- Temp table for activity
    CREATE TABLE #MyActivity (
        ResourceId INT PRIMARY KEY,
        ResourceActivityId INT
    );


    -- Latest activity per resource per user

    WITH LatestActivity AS (
        SELECT 
            a.Id,
            a.ResourceId,
            a.ResourceVersionId,
            a.LaunchResourceActivityId,
            a.ActivityStatusId,
            a.ActivityStart,
            ROW_NUMBER() OVER (
                PARTITION BY a.ResourceId 
                ORDER BY a.Id DESC
            ) AS rn
        FROM activity.ResourceActivity a
        WHERE a.UserId = @UserId
    )
    INSERT INTO #MyActivity
    SELECT TOP (@MaxRows)
        la.ResourceId,
        la.Id
    FROM LatestActivity la
    JOIN resources.Resource r ON r.Id = la.ResourceId
    JOIN resources.ResourceVersion rv ON rv.Id = la.ResourceVersionId
    LEFT JOIN resources.AssessmentResourceVersion arv 
        ON arv.ResourceVersionId = la.ResourceVersionId
    LEFT JOIN activity.AssessmentResourceActivity ara 
        ON ara.ResourceActivityId = COALESCE(la.LaunchResourceActivityId, la.Id)
    LEFT JOIN activity.MediaResourceActivity mar 
        ON mar.ResourceActivityId = COALESCE(la.LaunchResourceActivityId, la.Id)
    LEFT JOIN activity.ScormActivity sa 
        ON sa.ResourceActivityId = la.Id
    WHERE 
        la.rn = 1
        AND (
            (r.ResourceTypeId IN (2,7) 
                AND la.ActivityStatusId = 3 
                AND (
                    (mar.Id IS NOT NULL AND mar.PercentComplete = 100)
                    OR la.ActivityStart < '2020-09-07'
                )
            )
            OR (
                r.ResourceTypeId = 6 
                AND (
                    sa.CmiCoreLesson_status IN (3,5) 
                    OR la.ActivityStatusId IN (3,5)
                )
            )
            OR (
                r.ResourceTypeId = 11 
                AND (
                    ara.Score >= arv.PassMark 
                    OR la.ActivityStatusId IN (3,5)
                )
            )
            OR (
                r.ResourceTypeId IN (1,5,8,9,10,12) 
                AND la.ActivityStatusId = 3
            )
        )
    ORDER BY la.Id DESC;


    -- Total count

    SELECT @TotalRecords = COUNT(*) FROM #MyActivity;


    -- Precompute ResourceReference lookup (replaces OUTER APPLY)

    CREATE TABLE #ResourceRefLookup (
        ResourceId INT NOT NULL,
        NodeId INT NOT NULL,
        OriginalResourceReferenceId INT NOT NULL,
        PRIMARY KEY (ResourceId, NodeId)
    );

    INSERT INTO #ResourceRefLookup (ResourceId, NodeId, OriginalResourceReferenceId)
	SELECT 
		x.ResourceId,
		x.NodeId,
		x.OriginalResourceReferenceId
	FROM (
		SELECT 
			rr.ResourceId,
			np.NodeId,
			rr.OriginalResourceReferenceId,
			ROW_NUMBER() OVER (
				PARTITION BY rr.ResourceId, np.NodeId
				ORDER BY rr.Id DESC
			) AS rn
		FROM resources.ResourceReference rr
		JOIN hierarchy.NodePath np 
			ON np.Id = rr.NodePathId
		WHERE rr.Deleted = 0
			AND np.Deleted = 0
	) x
	WHERE x.rn = 1;



    -- Final SELECT

    SELECT 
        r.Id AS ResourceId,
        rrRef.OriginalResourceReferenceId AS ResourceReferenceID,
        r.CurrentResourceVersionId AS ResourceVersionId,
        r.ResourceTypeId,
        rv.Title,
        rv.Description,
        CASE 
            WHEN r.ResourceTypeId = 7 THEN vrv.DurationInMilliseconds
            WHEN r.ResourceTypeId = 2 THEN arv2.DurationInMilliseconds
            ELSE NULL
        END AS DurationInMilliseconds,
        CASE WHEN n.Id = 1 THEN NULL ELSE cnv.Name END AS CatalogueName,
        cnv.Url,
        CASE WHEN n.Id = 1 THEN NULL ELSE cnv.BadgeUrl END AS BadgeUrl,
        cnv.RestrictedAccess,
        CAST(
            CASE 
                WHEN cnv.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL 
                THEN 0 ELSE 1 
            END AS BIT
        ) AS HasAccess,
        ub.Id AS BookMarkId,
        CAST(ISNULL(ub.Deleted,1) ^ 1 AS BIT) AS IsBookmarked,
        rvrs.AverageRating,
        rvrs.RatingCount,
        rpAgg.ProvidersJson
    FROM #MyActivity ma
    JOIN activity.ResourceActivity ra ON ra.Id = ma.ResourceActivityId
    JOIN resources.ResourceVersion rv ON rv.Id = ra.ResourceVersionId AND rv.Deleted = 0
    JOIN resources.Resource r ON r.Id = rv.ResourceId
    JOIN hierarchy.NodeResource nr ON r.Id = nr.ResourceId AND nr.Deleted = 0

    -- Replaced OUTER APPLY
    LEFT JOIN #ResourceRefLookup rrRef
        ON rrRef.ResourceId = r.Id
        AND rrRef.NodeId = nr.NodeId

    LEFT JOIN (
    SELECT 
        rp.ResourceVersionId,
        (
            SELECT 
                p.Id,
                p.Name,
                p.Description,
                p.Logo
            FROM resources.ResourceVersionProvider rp2
            JOIN hub.Provider p ON p.Id = rp2.ProviderId
            WHERE rp2.ResourceVersionId = rp.ResourceVersionId
              AND rp2.Deleted = 0
              AND p.Deleted = 0
            FOR JSON PATH
        ) AS ProvidersJson
    FROM resources.ResourceVersionProvider rp
    GROUP BY rp.ResourceVersionId
) rpAgg ON rpAgg.ResourceVersionId = r.CurrentResourceVersionId

    JOIN hierarchy.Publication p ON rv.PublicationId = p.Id AND p.Deleted = 0
    JOIN resources.ResourceVersionRatingSummary rvrs ON rv.Id = rvrs.ResourceVersionId AND rvrs.Deleted = 0
    JOIN hierarchy.Node n ON n.Id = nr.NodeId AND n.Hidden = 0 AND n.Deleted = 0
    JOIN hierarchy.NodePath np ON np.NodeId = n.Id AND np.Deleted = 0 AND np.IsActive = 1
    JOIN hierarchy.NodeVersion nv ON nv.NodeId = np.CatalogueNodeId AND nv.VersionStatusId = 2 AND nv.Deleted = 0
    JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id AND cnv.Deleted = 0
    LEFT JOIN hub.UserBookmark ub 
        ON ub.UserId = @UserId 
        AND ub.ResourceReferenceId = rrRef.OriginalResourceReferenceId
    LEFT JOIN (
        SELECT DISTINCT CatalogueNodeId 
        FROM hub.RoleUserGroupView rug
        JOIN hub.UserUserGroup uug 
            ON rug.UserGroupId = uug.UserGroupId
        WHERE rug.ScopeTypeId = 1 
            AND rug.RoleId IN (1,2,3) 
            AND uug.Deleted = 0 
            AND uug.UserId = @UserId
    ) auth ON n.Id = auth.CatalogueNodeId
    LEFT JOIN resources.VideoResourceVersion vrv 
        ON vrv.ResourceVersionId = r.CurrentResourceVersionId
    LEFT JOIN resources.AudioResourceVersion arv2 
        ON arv2.ResourceVersionId = r.CurrentResourceVersionId
    ORDER BY ma.ResourceActivityId DESC, rv.Title
    OFFSET @OffsetRows ROWS
    FETCH NEXT @FetchRows ROWS ONLY;

END
