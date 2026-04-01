-------------------------------------------------------------------------------
-- Author       OA
-- Created      24 JUN 2024 Nov 2020
-- Purpose      Break down the GetDashboardResources SP to smaller SP for a specific data type
--
-- Modification History
--
-- 24 Jun 2024	OA	Initial Revision
-- 27 Jun 2024  SA  Removed unused temp tables
-- 29 Sep 2025  SA  Integrated the provider dertails 
-- 31 Mar 2026  OA  TD-7057 Script Optimization
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetRecentDashboardResources]
    @UserId         INT,
    @PageNumber     INT = 1,
    @TotalRecords   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxPageNumber INT = 4;
    DECLARE @FetchRows INT = 3;

    IF @PageNumber > @MaxPageNumber
        SET @PageNumber = @MaxPageNumber;

    DECLARE @MaxRows INT = @MaxPageNumber * @FetchRows;
    DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows;

    -------------------------------------------------------------------------
    -- Providers (pre-aggregated)
    -------------------------------------------------------------------------
    SELECT 
        rp.ResourceVersionId,
        JSON_QUERY('[' + STRING_AGG(
            '{"Id":' + CAST(p.Id AS NVARCHAR) +
            ',"Name":"' + p.Name + '"' +
            ',"Description":"' + p.Description + '"' +
            ',"Logo":"' + ISNULL(p.Logo,'') + '"}',
        ',') + ']') AS ProvidersJson
    INTO #Providers
    FROM resources.ResourceVersionProvider rp
    JOIN hub.Provider p ON p.Id = rp.ProviderId
    WHERE rp.Deleted = 0 AND p.Deleted = 0
    GROUP BY rp.ResourceVersionId;

    -------------------------------------------------------------------------
    -- User Authorization (precomputed)
    -------------------------------------------------------------------------
    SELECT DISTINCT CatalogueNodeId
    INTO #Auth
    FROM hub.RoleUserGroupView rug
    JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
    WHERE rug.ScopeTypeId = 1
      AND rug.RoleId IN (1,2,3)
      AND uug.Deleted = 0
      AND uug.UserId = @UserId;

    -------------------------------------------------------------------------
    -- ResourceReferenceId (precomputed)
    -------------------------------------------------------------------------
    SELECT 
        rr.ResourceId,
        rr.NodePathId,
        rr.OriginalResourceReferenceId
    INTO #Ref
    FROM resources.ResourceReference rr
    JOIN hierarchy.NodePath np ON np.Id = rr.NodePathId 
                               AND np.Deleted = 0
    WHERE rr.Deleted = 0;

    -------------------------------------------------------------------------
    -- Main dataset (TOP @MaxRows)
    -------------------------------------------------------------------------
    SELECT TOP (@MaxRows)
        r.Id AS ResourceId,
        rv.Id AS ResourceVersionId,
        r.ResourceTypeId,
        rv.Title,
        rv.Description,
        rvrs.AverageRating,
        rvrs.RatingCount,
        p.CreateDate,
        cnv.Name AS CatalogueName,
        cnv.Url,
        cnv.BadgeUrl,
        cnv.RestrictedAccess,
        prov.ProvidersJson,

        ---------------------------------------------------------------------
        -- Duration (Video/Audio)
        ---------------------------------------------------------------------
        CASE 
            WHEN r.ResourceTypeId = 7 THEN 
                (SELECT DurationInMilliseconds 
                 FROM resources.VideoResourceVersion 
                 WHERE ResourceVersionId = rv.Id)
            WHEN r.ResourceTypeId = 2 THEN 
                (SELECT DurationInMilliseconds 
                 FROM resources.AudioResourceVersion 
                 WHERE ResourceVersionId = rv.Id)
            ELSE NULL
        END AS DurationInMilliseconds,

        ---------------------------------------------------------------------
        -- ResourceReferenceId (TOP 1 per original logic)
        ---------------------------------------------------------------------
        (SELECT TOP 1 rf.OriginalResourceReferenceId
         FROM #Ref rf
         JOIN hierarchy.NodePath np2 ON np2.Id = rf.NodePathId 
                                    AND np2.NodeId = n.Id 
                                    AND np2.Deleted = 0
         WHERE rf.ResourceId = r.Id) AS ResourceReferenceId,

        ---------------------------------------------------------------------
        -- Bookmark
        ---------------------------------------------------------------------
        ub.Id AS BookMarkId,
        CAST(ISNULL(ub.Deleted, 1) ^ 1 AS BIT) AS IsBookmarked,

        ---------------------------------------------------------------------
        -- Access
        ---------------------------------------------------------------------
        CAST(
            CASE 
                WHEN cnv.RestrictedAccess = 1 
                     AND auth.CatalogueNodeId IS NULL 
                THEN 0 ELSE 1 
            END AS BIT
        ) AS HasAccess

    INTO #Recent
    FROM resources.Resource r
    JOIN resources.ResourceVersion rv 
        ON rv.Id = r.CurrentResourceVersionId 
       AND rv.Deleted = 0
       AND rv.VersionStatusId = 2
    JOIN hierarchy.Publication p 
        ON rv.PublicationId = p.Id 
       AND p.Deleted = 0
    JOIN resources.ResourceVersionRatingSummary rvrs 
        ON rv.Id = rvrs.ResourceVersionId 
       AND rvrs.Deleted = 0
    JOIN hierarchy.NodeResource nr 
        ON nr.ResourceId = r.Id 
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
    LEFT JOIN #Providers prov 
        ON prov.ResourceVersionId = rv.Id
    LEFT JOIN #Auth auth 
        ON auth.CatalogueNodeId = n.Id
    LEFT JOIN hub.UserBookmark ub 
        ON ub.UserId = @UserId
       AND ub.ResourceReferenceId = (
            SELECT TOP 1 rf.OriginalResourceReferenceId
            FROM #Ref rf
            JOIN hierarchy.NodePath np2 ON np2.Id = rf.NodePathId 
                                       AND np2.NodeId = n.Id 
                                       AND np2.Deleted = 0
            WHERE rf.ResourceId = r.Id
       )
    ORDER BY p.CreateDate DESC;

    -------------------------------------------------------------------------
    -- Total Records
    -------------------------------------------------------------------------
    SELECT @TotalRecords = CASE WHEN COUNT(*) > @MaxRows THEN @MaxRows ELSE COUNT(*) END
    FROM #Recent;

    -------------------------------------------------------------------------
    -- Final Paged Output
    -------------------------------------------------------------------------
    SELECT *
    FROM #Recent
    ORDER BY CreateDate DESC
    OFFSET @OffsetRows ROWS FETCH NEXT @FetchRows ROWS ONLY;

END

