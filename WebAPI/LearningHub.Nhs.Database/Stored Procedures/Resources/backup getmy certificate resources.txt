-------------------------------------------------------------------------------
-- Author       OA
-- Created      24 JUN 2024 Nov 2020
-- Purpose      Break down the GetDashboardResources SP to smaller SP for a specific data type
--
-- Modification History
--
-- 24 Jun 2024	OA	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetMyCertificatesDashboardResources]
	@UserId					INT,	
	@PageNumber				INT = 1,
	@TotalRecords			INT OUTPUT
AS
BEGIN
	DECLARE @MaxPageNumber INT = 4
	
	IF @PageNumber > 4
	BEGIN
		SET @PageNumber = @MaxPageNumber
	END
		
	DECLARE @FetchRows INT = 3
	DECLARE @MaxRows INT = @MaxPageNUmber * @FetchRows
	DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows

	DECLARE @MyActivity TABLE (ResourceId [int] NOT NULL PRIMARY KEY, ResourceActivityId [int] NOT NULL);
	DECLARE @Resources TABLE (ResourceId [int] NOT NULL PRIMARY KEY, ResourceActivityCount [int] NOT NULL);

    INSERT INTO @MyActivity					
        SELECT TOP (@MaxRows) ra.ResourceId, MAX(ra.Id) ResourceActivityId
            FROM
            activity.ResourceActivity ra				
            JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
            JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId
            LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
            LEFT JOIN [activity].[AssessmentResourceActivity] ara ON ara.ResourceActivityId = ra.Id
            LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = ra.Id
            LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id
            WHERE ra.UserId = @UserId AND rv.CertificateEnabled = 1
            AND (					
                    (r.ResourceTypeId IN (2, 7) AND ra.ActivityStatusId = 3 OR ra.ActivityStart < '2020-09-07 00:00:00 +00:00' OR mar.Id IS NOT NULL AND mar.PercentComplete = 100)
                OR (r.ResourceTypeId = 6 AND (sa.CmiCoreLesson_status IN(3,5) OR (ra.ActivityStatusId IN(3, 5))))
                OR ((r.ResourceTypeId = 11 AND arv.AssessmentType = 2) AND (ara.Score >= arv.PassMark OR ra.ActivityStatusId IN(3, 5)))
                OR ((r.ResourceTypeId = 11 AND arv.AssessmentType =1) AND (ara.Score >= arv.PassMark AND ra.ActivityStatusId IN(3, 5,7)))
                OR (r.ResourceTypeId IN (1, 5, 8, 9, 10, 12) AND ra.ActivityStatusId = 3))		
            GROUP BY ra.ResourceId
            ORDER BY ResourceActivityId DESC

    SELECT r.Id AS ResourceId
    ,(	SELECT TOP 1 rr.OriginalResourceReferenceId
        FROM [resources].[ResourceReference] rr
        JOIN hierarchy.NodePath np on np.id = rr.NodePathId and np.NodeId = n.Id and np.Deleted = 0
        WHERE rr.ResourceId = rv.ResourceId AND rr.Deleted = 0
        ) AS ResourceReferenceID
    ,r.CurrentResourceVersionId AS ResourceVersionId
    ,r.ResourceTypeId AS ResourceTypeId
    ,rv.Title
    ,rv.Description
    ,CASE 
        WHEN r.ResourceTypeId = 7	THEN				 
            (SELECT vrv.DurationInMilliseconds from  [resources].[VideoResourceVersion] vrv WHERE vrv.[ResourceVersionId] = r.CurrentResourceVersionId)
        WHEN r.ResourceTypeId = 2	THEN				 
        (SELECT vrv.DurationInMilliseconds from  [resources].[AudioResourceVersion] vrv WHERE vrv.[ResourceVersionId] = r.CurrentResourceVersionId)
        ELSE 
        NULL
        END  AS DurationInMilliseconds
    ,CASE WHEN n.id = 1 THEN NULL ELSE cnv.Name END AS CatalogueName
    ,cnv.Url AS Url
    ,CASE WHEN n.id = 1 THEN NULL ELSE cnv.BadgeUrl END AS BadgeUrl
    ,cnv.RestrictedAccess
    ,CAST(CASE WHEN cnv.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0 ELSE 1 END AS bit) AS HasAccess
    ,ub.Id AS BookMarkId
    ,CAST(ISNULL(ub.[Deleted], 1) ^ 1 AS BIT) AS IsBookmarked
    ,rvrs.AverageRating
    ,rvrs.RatingCount
FROM @MyActivity ma 		
JOIN activity.ResourceActivity ra ON ra.id = ma.ResourceActivityId
JOIN resources.resourceversion rv ON rv.id = ra.ResourceVersionId AND rv.Deleted = 0	
JOIN Resources.Resource r ON r.Id = rv.ResourceId
JOIN hierarchy.Publication p ON rv.PublicationId = p.Id AND p.Deleted = 0
JOIN resources.ResourceVersionRatingSummary rvrs ON rv.Id = rvrs.ResourceVersionId	AND rvrs.Deleted = 0
JOIN hierarchy.NodeResource nr ON r.Id = nr.ResourceId AND nr.Deleted = 0
JOIN hierarchy.Node n ON n.Id = nr.NodeId AND n.Hidden = 0 AND n.Deleted = 0
JOIN hierarchy.NodePath np ON np.NodeId = n.Id AND np.Deleted = 0 AND np.IsActive = 1
JOIN hierarchy.NodeVersion nv ON nv.NodeId = np.CatalogueNodeId	AND nv.VersionStatusId = 2 AND nv.Deleted = 0
JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id AND cnv.Deleted = 0
LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.ResourceReferenceId = (SELECT TOP 1 rr.OriginalResourceReferenceId
        FROM [resources].[ResourceReference] rr
        JOIN hierarchy.NodePath np on np.id = rr.NodePathId and np.NodeId = n.Id and np.Deleted = 0
        WHERE rr.ResourceId = rv.ResourceId AND rr.Deleted = 0)
LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
                FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
                WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId		
ORDER BY ma.ResourceActivityId DESC, rv.Title
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY	

    SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM @MyActivity
END