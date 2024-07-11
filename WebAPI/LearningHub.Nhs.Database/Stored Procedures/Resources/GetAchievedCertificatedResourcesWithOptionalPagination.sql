
-------------------------------------------------------------------------------
-- Author       PT
-- Created      11 July 2024
-- Purpose      Get achieved certificated resources with optional pagination
-- Description  Extracted from the GetDashboardResources sproc to enable one source of truth for determining achieved certificated resources
--              To support the GetDashboardResources it has pagination and to support other requests the default values disable pagination effects
-------------------------------------------------------------------------------


CREATE PROCEDURE [resources].[GetAchievedCertficatedResourcesWithOptionalPagination]
    @UserId INT,

	-- Default values disable pagination
    @MaxRows INT = 2147483647,  -- Warning! Magic number. To disable pagination by default.
    @OffsetRows INT = 0,
    @FetchRows INT = 2147483647, -- Warning! Magic number. To disable pagination by default.

    @TotalRecords INT OUTPUT
AS
BEGIN

    -- Step 1: Create a table variable to store intermediate results
    DECLARE @MyActivity TABLE (
        ResourceId INT,
        ResourceActivityId INT
    );

		INSERT INTO @MyActivity					
			SELECT TOP (@MaxRows) ra.ResourceId, MAX(ra.Id) ResourceActivityId
				FROM
				/* resources with resource activity, resource activity determines if certificated*/
				activity.ResourceActivity ra				
				JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
				JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId

				/* Determining if certificated scorm, assessment mark, media*/
				LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
				LEFT JOIN [activity].[AssessmentResourceActivity] ara ON ara.ResourceActivityId = ra.Id
				LEFT JOIN [activity].[MediaResourceActivity] mar ON mar.ResourceActivityId = ra.Id
				LEFT JOIN [activity].[ScormActivity] sa ON sa.ResourceActivityId = ra.Id

				WHERE ra.UserId = @UserId AND rv.CertificateEnabled = 1  -- detemining if certificated	
				AND (					
					 (r.ResourceTypeId IN (2, 7) AND ra.ActivityStatusId IN (3) /* resourceType 2 Audio and 7 is video activityStatusId 3 is completed */
						OR ra.ActivityStart < '2020-09-07 00:00:00 +00:00' /* old activity assumed to be valid*/
						OR mar.Id IS NOT NULL AND mar.PercentComplete = 100  /* media activity 100% complete*/
					)
					/* type 6 scorm elearning,*/
					OR (r.ResourceTypeId = 6 AND (sa.CmiCoreLesson_status IN(3,5) OR (ra.ActivityStatusId IN(3, 5))))  /* activityStatus 3 and 5 are completed and passed */
					/* 11 is assessment */
					OR (r.ResourceTypeId = 11 AND ara.Score >= arv.PassMark OR ra.ActivityStatusId IN(3, 5)) /*assessment mark and activity status passed completed */
					/* 1 Article, 5 Image, 8 Weblink 9 file, 10 case, 12 html */
					OR (r.ResourceTypeId IN (1, 5, 8, 9, 10, 12) AND ra.ActivityStatusId IN (3))) 	/* Completed */			
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
		
		/* Catalogue logic */
		JOIN hierarchy.NodeResource nr ON r.Id = nr.ResourceId AND nr.Deleted = 0
		JOIN hierarchy.Node n ON n.Id = nr.NodeId AND n.Hidden = 0 AND n.Deleted = 0
		JOIN hierarchy.NodePath np ON np.NodeId = n.Id AND np.Deleted = 0 AND np.IsActive = 1
		JOIN hierarchy.NodeVersion nv ON nv.NodeId = np.CatalogueNodeId	AND nv.VersionStatusId = 2 AND nv.Deleted = 0
		JOIN hierarchy.CatalogueNodeVersion cnv ON cnv.NodeVersionId = nv.Id AND cnv.Deleted = 0

		/* Book marks */
		LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.ResourceReferenceId = (SELECT TOP 1 rr.OriginalResourceReferenceId
				FROM [resources].[ResourceReference] rr
				JOIN hierarchy.NodePath np on np.id = rr.NodePathId and np.NodeId = n.Id and np.Deleted = 0
				WHERE rr.ResourceId = rv.ResourceId AND rr.Deleted = 0)
		LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
		LEFT JOIN resources.ResourceVersionRatingSummary rs ON rs.ResourceVersionId = rv.Id			
		ORDER BY ma.ResourceActivityId DESC, rv.Title

		/* pagination logic */
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY	
		SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM @MyActivity
		
	END


