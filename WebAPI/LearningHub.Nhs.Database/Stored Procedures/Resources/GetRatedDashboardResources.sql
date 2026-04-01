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

CREATE PROCEDURE [resources].[GetRatedDashboardResources]
    @UserId                 INT,    
    @PageNumber             INT = 1,
    @TotalRecords           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MaxPageNumber INT = 4;
    DECLARE @FetchRows INT = 3;

    IF @PageNumber > @MaxPageNumber
        SET @PageNumber = @MaxPageNumber;

    DECLARE @MaxRows INT = @MaxPageNumber * @FetchRows;
    DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows;


    CREATE TABLE #RatedResources (
        ResourceId INT,
        ResourceReferenceID INT,
        ResourceVersionId INT,
        ResourceTypeId INT,
        Title NVARCHAR(500),
        Description NVARCHAR(MAX),
        DurationInMilliseconds INT NULL,
        CatalogueName NVARCHAR(255),
        Url NVARCHAR(500),
        BadgeUrl NVARCHAR(500),
        RestrictedAccess BIT,
        HasAccess BIT,
        BookMarkId INT NULL,
        IsBookmarked BIT,
        AverageRating FLOAT,
        RatingCount INT,
        ProvidersJson NVARCHAR(MAX)
    );

    -- Insert TOP(@MaxRows) rated resources
    INSERT INTO #RatedResources
    SELECT TOP (@MaxRows)
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

    FROM Resources.Resource r
    JOIN resources.ResourceVersion rv 
        ON rv.ResourceId = r.Id
        AND rv.Id = r.CurrentResourceVersionId
        AND rv.Deleted = 0

    JOIN resources.ResourceVersionRatingSummary rvrs 
        ON rvrs.ResourceVersionId = r.CurrentResourceVersionId
        AND rvrs.RatingCount > 0

    -- Deterministic ResourceReference lookup
    OUTER APPLY (
        SELECT TOP 1 rr.OriginalResourceReferenceId
        FROM resources.ResourceReference rr
        JOIN hierarchy.NodePath np2 
            ON np2.Id = rr.NodePathId
            AND np2.NodeId = nr.NodeId
            AND np2.Deleted = 0
        WHERE rr.ResourceId = rv.ResourceId
            AND rr.Deleted = 0
        ORDER BY rr.Id DESC
    ) rrRef

    -- Provider JSON aggregation

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

    -- Hierarchy joins
    JOIN hierarchy.NodeResource nr ON r.Id = nr.ResourceId AND nr.Deleted = 0
    JOIN hierarchy.Node n ON n.Id = nr.NodeId AND n.Hidden = 0 AND n.Deleted = 0
    JOIN hierarchy.NodePath np ON np.NodeId = n.Id AND np.Deleted = 0 AND np.IsActive = 1
    JOIN hierarchy.NodeVersion nv ON nv.NodeId = np.CatalogueNodeId AND nv.VersionStatusId = 2 AND nv.Deleted = 0
    JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id AND cnv.Deleted = 0

    -- Bookmark lookup
    LEFT JOIN hub.UserBookmark ub 
        ON ub.UserId = @UserId
        AND ub.ResourceReferenceId = rrRef.OriginalResourceReferenceId

    -- Access check
    LEFT JOIN (
        SELECT DISTINCT CatalogueNodeId 
        FROM hub.RoleUserGroupView rug
        JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
        WHERE rug.ScopeTypeId = 1
            AND rug.RoleId IN (1,2,3)
            AND uug.Deleted = 0
            AND uug.UserId = @UserId
    ) auth ON n.Id = auth.CatalogueNodeId

    LEFT JOIN resources.VideoResourceVersion vrv 
        ON vrv.ResourceVersionId = r.CurrentResourceVersionId

    LEFT JOIN resources.AudioResourceVersion arv2 
        ON arv2.ResourceVersionId = r.CurrentResourceVersionId

    WHERE rv.VersionStatusId = 2
    ORDER BY rvrs.AverageRating DESC, rvrs.RatingCount DESC, rv.Title;


    SELECT *
    FROM #RatedResources
    ORDER BY AverageRating DESC, RatingCount DESC, Title
    OFFSET @OffsetRows ROWS
    FETCH NEXT @FetchRows ROWS ONLY;

    SELECT @TotalRecords = COUNT(*)  FROM #RatedResources;
   

END

