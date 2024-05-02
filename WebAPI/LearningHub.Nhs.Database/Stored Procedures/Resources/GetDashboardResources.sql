-------------------------------------------------------------------------------
-- Author       HV
-- Created      18 Nov 2020
-- Purpose      Gets the resources to be displayed on dashboard
--
-- Modification History
--
-- 18 Nov 2020	HV	Initial Revision
-- 28 Apr 2021	KD	Added restricted access indication
-- 28 Jun 2021	DB	Fix to the join between Node & NodeVersion
-- 14 Nov 2022  RS  Fix to include resources located in catalogue subfolders
-- 16 Nov 2022  RS  FetchRows changes for progressive enhancement
-- 16 Mar 2023  RS  Added rating data
-- 28 Mar 2023  RS  Removed columns no longer used
-- 15 Jun 2023  RS  Re-added BadgeUrl column following design change
-- 27 Sep 2023  HV  Included Paging and user resource activity
-- 08 Nov 2023  OA  Fixed latest resource activity entry selection(with updated logic for media activities) and  status check for incomplete assessment.
-- 17 Jan 2024  SA  Changes to accomadate activity status changes
-- 27 Feb 2024  SS  Fixed missing In progress resources in the My Accessed Learning tray issue
-- 2 May 2024   SA  Fixed the issue on showing statuses on 'My accessed Learning' for resource type file
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetDashboardResources]
	@dashboardType			nvarchar(30),
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

	IF @dashboardType = 'popular-resources'
	BEGIN
		INSERT INTO @Resources	
				SELECT TOP (@MaxRows) ra.ResourceId
				,Count(ra.ResourceVersionId) ResourceActivityCount 
				FROM resources.Resource r					
				JOIN resources.ResourceVersion rv On rv.id = r.CurrentResourceVersionId AND rv.VersionStatusId = 2
				JOIN activity.ResourceActivity ra ON ra.ResourceId = r.Id	
				JOIN hierarchy.NodeResource nr ON r.Id = nr.ResourceId AND nr.Deleted = 0
				JOIN hierarchy.Node n ON n.Id = nr.NodeId AND n.Hidden = 0 AND n.Deleted = 0
				JOIN hierarchy.NodePath np ON np.NodeId = n.Id AND np.Deleted = 0 AND np.IsActive = 1
				JOIN hierarchy.NodeVersion nv ON nv.NodeId = np.CatalogueNodeId	AND nv.VersionStatusId = 2 AND nv.Deleted = 0
				JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id AND cnv.Deleted = 0
				GROUP BY  ra.ResourceId
				ORDER BY ResourceActivityCount DESC					
		SELECT 
			tr.ResourceId
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
			,rs.AverageRating
			,rs.RatingCount
		FROM @Resources tr
		JOIN resources.Resource r ON r.id = tr.ResourceId
		JOIN resources.resourceversion rv ON rv.ResourceId = r.Id	AND rv.id = r.CurrentResourceVersionId AND rv.Deleted = 0
		JOIN resources.ResourceVersionRatingSummary rvrs ON r.CurrentResourceVersionId = rvrs.ResourceVersionId AND rvrs.Deleted = 0
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
		LEFT JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id
		WHERE rv.VersionStatusId = 2	
		ORDER BY tr.ResourceActivityCount DESC, rv.Title
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY	

		SELECT @TotalRecords = CASE WHEN COUNT(*)  > 12 THEN @MaxRows ELSE COUNT(*) END FROM @Resources
	END
	ELSE IF  @dashboardType = 'rated-resources'	
	BEGIN
			SELECT TOP(@MaxRows) r.Id AS ResourceId
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
			,rs.AverageRating
			,rs.RatingCount
		INTO #ratedresources
		FROM Resources.Resource r
		JOIN resources.resourceversion rv ON rv.ResourceId = r.Id	AND rv.id = r.CurrentResourceVersionId	AND rv.Deleted = 0
		JOIN resources.ResourceVersionRatingSummary rvrs ON r.CurrentResourceVersionId = rvrs.ResourceVersionId AND rvrs.RatingCount > 0	
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
		INNER JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id
		WHERE rv.VersionStatusId = 2	
		ORDER BY rvrs.AverageRating DESC, rvrs.RatingCount DESC, rv.Title

		SELECT rr.* FROM #ratedresources rr
		ORDER BY rr.AverageRating DESC, rr.RatingCount DESC, rr.Title
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY	

		SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM #ratedresources
	END
	ELSE IF  @dashboardType = 'recent-resources'
	BEGIN
			SELECT TOP(@MaxRows) r.Id AS ResourceId
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
			,rs.AverageRating
			,rs.RatingCount
			INTO #recentresources
		FROM Resources.Resource r
		JOIN resources.resourceversion rv ON rv.ResourceId = r.Id	AND rv.id = r.CurrentResourceVersionId AND rv.Deleted = 0
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
		INNER JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id
		WHERE rv.VersionStatusId = 2
		ORDER BY p.CreateDate DESC
		
		SELECT rr.* FROM #recentresources rr		
		ORDER BY rr.ResourceVersionId DESC
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY

		SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM #recentresources
	END
	ELSE IF  @dashboardType = 'my-in-progress'
	BEGIN
	INSERT INTO @MyActivity					
			SELECT TOP (@MaxRows) ra.ResourceId, MAX(ra.Id) ResourceActivityId
				FROM 
				(SELECT a.* FROM activity.ResourceActivity a INNER JOIN (SELECT ResourceId, MAX(Id) as id FROM activity.ResourceActivity GROUP BY ResourceId,ActivityStatusId ) AS b ON a.ResourceId = b.ResourceId AND a.id = b.id  order by a.Id desc OFFSET 0 ROWS) ra	
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
					OR (r.ResourceTypeId IN (9) AND ra.ActivityStatusId NOT IN (3))
					OR (r.ResourceTypeId = 11 AND ((ara.Id IS NOT NULL AND ara.score < arv.PassMark) OR ra.ActivityStatusId IN (1))) 
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
			,rs.AverageRating
			,rs.RatingCount
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
		LEFT JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id
		ORDER BY ma.ResourceActivityId DESC
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY	

		SELECT @TotalRecords = CASE WHEN COUNT(*)  > 12 THEN @MaxRows ELSE COUNT(*) END FROM @MyActivity
	END
	ELSE IF  @dashboardType IN ('my-recent-completed')
	BEGIN	
	INSERT INTO @MyActivity		
			SELECT TOP (@MaxRows) ra.ResourceId, MAX(ra.Id) ResourceActivityId
				FROM
				(SELECT a.* FROM activity.ResourceActivity a INNER JOIN (SELECT ResourceId, MAX(Id) as id FROM activity.ResourceActivity GROUP BY ResourceId ) AS b ON a.ResourceId = b.ResourceId AND a.id = b.id  order by a.Id desc OFFSET 0 ROWS) ra					
				JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
				JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId
				LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
				LEFT JOIN [activity].[AssessmentResourceActivity] ara ON ara.ResourceActivityId = COALESCE(ra.LaunchResourceActivityId, ra.Id)
				LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = COALESCE(ra.LaunchResourceActivityId, ra.Id)
				LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id
				WHERE ra.UserId = @UserId
				AND (					
					 (r.ResourceTypeId IN (2, 7) AND ra.ActivityStatusId IN (3) AND ((mar.Id IS NOT NULL AND mar.PercentComplete = 100) OR ra.ActivityStart < '2020-09-07 00:00:00 +00:00'))
					OR (r.ResourceTypeId = 6 AND (sa.CmiCoreLesson_status IN(3,5) OR (ra.ActivityStatusId IN(3, 5))))
					OR (r.ResourceTypeId = 11 AND ara.Score >= arv.PassMark OR ra.ActivityStatusId IN( 3, 5))
					OR (r.ResourceTypeId IN (1, 5, 8, 9, 10, 12) AND ra.ActivityStatusId IN (3)))		
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
			,rs.AverageRating
			,rs.RatingCount
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
		LEFT JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id
		ORDER BY ma.ResourceActivityId DESC, rv.Title
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY	

		SELECT @TotalRecords = CASE WHEN COUNT(*)  > 12 THEN @MaxRows ELSE COUNT(*) END FROM @MyActivity
	END
	ELSE IF  @dashboardType IN ('my-certificates')
	BEGIN
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
					 (r.ResourceTypeId IN (2, 7) AND ra.ActivityStatusId IN (3) OR ra.ActivityStart < '2020-09-07 00:00:00 +00:00' OR mar.Id IS NOT NULL AND mar.PercentComplete = 100)
					OR (r.ResourceTypeId = 6 AND (sa.CmiCoreLesson_status IN(3,5) OR (ra.ActivityStatusId IN(3, 5))))
					OR (r.ResourceTypeId = 11 AND ara.Score >= arv.PassMark OR ra.ActivityStatusId IN(3, 5))
					OR (r.ResourceTypeId IN (1, 5, 8, 9, 10, 12) AND ra.ActivityStatusId IN (3)))		
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
			,rs.AverageRating
			,rs.RatingCount
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
		LEFT JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id			
		ORDER BY ma.ResourceActivityId DESC, rv.Title
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY	

		SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM @MyActivity
	END	
END