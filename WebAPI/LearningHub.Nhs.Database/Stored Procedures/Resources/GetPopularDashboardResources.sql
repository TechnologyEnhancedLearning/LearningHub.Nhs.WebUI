-------------------------------------------------------------------------------
-- Author       OA
-- Created      24 JUN 2024 Nov 2020
-- Purpose      Break down the GetDashboardResources SP to smaller SP for a specific data type
--
-- Modification History
--
-- 24 Jun 2024	OA	Initial Revision
-- 27 Jun 2024  SA  Removed unused temp tables
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetPopularDashboardResources]
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

	DECLARE @Resources TABLE (ResourceId [int] NOT NULL PRIMARY KEY, ResourceActivityCount [int] NOT NULL);


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
		,rvrs.AverageRating
		,rvrs.RatingCount
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
	WHERE rv.VersionStatusId = 2	
	ORDER BY tr.ResourceActivityCount DESC, rv.Title
	OFFSET @OffsetRows ROWS
	FETCH NEXT @FetchRows ROWS ONLY	

	SELECT @TotalRecords = CASE WHEN COUNT(*)  > 12 THEN @MaxRows ELSE COUNT(*) END FROM @Resources

END