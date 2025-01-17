-------------------------------------------------------------------------------
-- Author       OA
-- Created      24 JUN 2024 Nov 2020
-- Purpose      Break down the GetDashboardResources SP to smaller SP for a specific data type
--
-- Modification History
--
-- 24 Jun 2024	OA	Initial Revision
-- 27 Jun 2024	SA My Learning Dashboard Tray showing Wrong Counts
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetMyInProgressDashboardResources]
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

	INSERT INTO @MyActivity					
			SELECT TOP (@MaxRows) ra.ResourceId, MAX(ra.Id) ResourceActivityId
				FROM 
				(SELECT a.Id,a.ResourceId,a.ResourceVersionId,a.LaunchResourceActivityId,a.UserId,a.ActivityStatusId,a.ActivityStart FROM activity.ResourceActivity a INNER JOIN (SELECT ResourceId, MAX(Id) as id FROM activity.ResourceActivity WHERE UserId = @UserId GROUP BY ResourceId) AS b ON a.ResourceId = b.ResourceId AND a.id = b.id  order by a.Id desc OFFSET 0 ROWS) ra	
				JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
				JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId
				LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
				LEFT JOIN [activity].[AssessmentResourceActivity] ara ON ara.ResourceActivityId = COALESCE(ra.LaunchResourceActivityId, ra.Id)
				LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = COALESCE(ra.LaunchResourceActivityId, ra.Id)
				LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id
				WHERE ra.UserId = @UserId
				AND (
					 (r.ResourceTypeId IN (1, 5, 8, 9,10, 12) AND ra.ActivityStatusId <> 3)
				    OR (r.ResourceTypeId IN (2, 7) AND (mar.Id IS NULL OR (mar.Id IS NOT NULL AND mar.PercentComplete < 100) OR ra.ActivityStart < '2020-09-07 00:00:00 +00:00'))
					OR  (r.ResourceTypeId = 6 AND (sa.CmiCoreLesson_status NOT IN (3, 5) AND (ra.ActivityStatusId NOT IN(3, 5))))
					OR ((r.ResourceTypeId = 11 AND arv.AssessmentType = 2) AND ((ara.Id IS NOT NULL AND ara.score < arv.PassMark) OR ra.ActivityStatusId = 7)) 

					OR ((r.ResourceTypeId = 11 AND arv.AssessmentType = 1) AND ra.ActivityStatusId = 7) 
					)		
				GROUP BY ra.ResourceId	
				ORDER BY ResourceActivityId DESC
				
	SELECT ma.ResourceActivityId, r.Id AS ResourceId
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
	ORDER BY ma.ResourceActivityId DESC
	OFFSET @OffsetRows ROWS
	FETCH NEXT @FetchRows ROWS ONLY	

	SELECT @TotalRecords = CASE WHEN COUNT(ma.ResourceActivityId)  > 12 THEN @MaxRows ELSE COUNT(*) END 
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
	
END