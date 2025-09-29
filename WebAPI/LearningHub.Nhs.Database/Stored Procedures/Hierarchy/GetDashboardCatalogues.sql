-------------------------------------------------------------------------------
-- Author       HV
-- Created      18 Nov 2020
-- Purpose      Gets the catalogues to be displayed on dashboard
--
-- Modification History
--
-- 19 Nov 2020	HV	Initial Revision
-- 19 Feb 2021	DB	Total number of records returned
-- 28 Apr 2021	KD	Added restricted access indication
-- 17 Nov 2022  RS  Added offset-fetch functionality for paging
-- 28 Mar 2023  RS  Removed BadgeUrl column as no longer used
-- 15 Jun 2023  RS  Re-added BadgeUrl column following design change
-- 11 Aug 2023  RS  Added CardImageUrl column
-- 27 Sep 2023  HV  Included Paging and user accessed catalogues
-- 13 Nov 2023  SA  Included Node VersionId in also.
-- 29 Sep 2025  SA  Integrated the provider dertails 
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetDashboardCatalogues]
	@DashboardType			nvarchar(30),
	@UserId					INT,
	@PageNumber				INT = 1,
	@TotalRecords			INT OUTPUT
AS
BEGIN
	DECLARE @MaxPageNumber INT = 4
	DECLARE @FetchRows INT = 3
	DECLARE @MaxRows INT = @MaxPageNumber * @FetchRows
	DECLARE @OffsetRows INT = (@PageNumber - 1) * @FetchRows

	IF @PageNumber > 4 AND  @DashboardType <> 'all-catalogues'
	BEGIN
		SET @PageNumber = @MaxPageNumber		
	END

	DECLARE @Catalogues TABLE (NodeId [int] NOT NULL PRIMARY KEY, NodeCount [int] NOT NULL)

	IF @DashboardType = 'popular-catalogues'
	BEGIN
			INSERT INTO @Catalogues
				SELECT na.NodeId, Count( na.CatalogueNodeVersionId) NodeCount
				FROM [hierarchy].[Node] n
				JOIN [activity].[NodeActivity] na ON na.NodeId = n.Id AND na.CatalogueNodeVersionId = n.CurrentNodeVersionId
				JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
				JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
				WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2	AND na.NodeId !=113 /* ORCHA - temporary removal */							
				GROUP BY  na.NodeId				
			SELECT
				nv.NodeId
				,cnv.Id AS NodeVersionId
				,cnv.Name
				,cnv.Description
				,cnv.BannerUrl
				,cnv.BadgeUrl
				,cnv.CardImageUrl
				,cnv.Url
				,cnv.RestrictedAccess
				,CAST(CASE WHEN cnv.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0 ELSE 1 END AS bit) AS HasAccess
				,ub.Id AS BookMarkId
			    ,CAST(ISNULL(ub.[Deleted], 1) ^ 1 AS BIT) AS IsBookmarked 
				,cpAgg.ProvidersJson
			FROM @Catalogues tc
			JOIN [hierarchy].[Node] n ON tc.NodeId = n.Id
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN (
			SELECT 
				cnp.CatalogueNodeVersionId,
				JSON_QUERY('[' + STRING_AGG(
					'{"Id":' + CAST(p.Id AS NVARCHAR) +
					',"Name":"' + p.Name + '"' +
					',"Description":"' + p.Description + '"' +
					',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
				',') + ']') AS ProvidersJson
			FROM hierarchy.CatalogueNodeVersionProvider cnp
			JOIN hub.Provider p ON p.Id = cnp.ProviderId
			WHERE p.Deleted = 0 and cnp.Deleted = 0
			GROUP BY cnp.CatalogueNodeVersionId
			) cpAgg ON cpAgg.CatalogueNodeVersionId = cnv.Id
			JOIN hub.Scope s ON n.Id = s.CatalogueNodeId
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2 AND s.Deleted = 0
			ORDER BY NodeCount DESC, cnv.Name
				OFFSET @OffsetRows ROWS
				FETCH NEXT @FetchRows ROWS ONLY

			SELECT @TotalRecords = CASE WHEN COUNT(*)  > 12 THEN @MaxRows ELSE COUNT(*) END FROM @Catalogues
	END
	ELSE IF  @DashboardType = 'recent-catalogues'	
	BEGIN
			SELECT
				nv.NodeId
				,cnv.Id AS NodeVersionId
				,cnv.Name
				,cnv.Description
				,cnv.BannerUrl
				,cnv.BadgeUrl
				,cnv.CardImageUrl
				,cnv.Url
				,cnv.RestrictedAccess
				,CAST(CASE WHEN cnv.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0 ELSE 1 END AS bit) AS HasAccess
				,ub.Id AS BookMarkId
			    ,CAST(ISNULL(ub.[Deleted], 1) ^ 1 AS BIT) AS IsBookmarked 
				,cpAgg.ProvidersJson
			INTO #recentcatalogues
			FROM [hierarchy].[Node] n
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN (
			SELECT 
				cnp.CatalogueNodeVersionId,
				JSON_QUERY('[' + STRING_AGG(
					'{"Id":' + CAST(p.Id AS NVARCHAR) +
					',"Name":"' + p.Name + '"' +
					',"Description":"' + p.Description + '"' +
					',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
				',') + ']') AS ProvidersJson
			FROM hierarchy.CatalogueNodeVersionProvider cnp
			JOIN hub.Provider p ON p.Id = cnp.ProviderId
			WHERE p.Deleted = 0 and cnp.Deleted = 0
			GROUP BY cnp.CatalogueNodeVersionId
			) cpAgg ON cpAgg.CatalogueNodeVersionId = cnv.Id
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2 AND cnv.LastShownDate IS NOT NULL
			ORDER BY cnv.LastShownDate DESC
			
		SELECT rc.* FROM #recentcatalogues rc		
		ORDER BY rc.NodeId DESC
		OFFSET @OffsetRows ROWS
		FETCH NEXT @FetchRows ROWS ONLY

		SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM #recentcatalogues
	END
	ELSE IF  @DashboardType = 'highly-contributed-catalogues'
	BEGIN
			INSERT INTO @Catalogues
				select nr.NodeId, count(*) AS NodeCount 
				from [hierarchy].[Node] n
				JOIN hierarchy.NodeResource nr ON nr.NodeId = n.Id
				JOIN resources.Resource r ON r.Id = nr.ResourceId
				JOIN resources.ResourceVersion rv ON rv.Id = r.CurrentResourceVersionId AND rv.VersionStatusId = 2 and rv.Deleted = 0
				JOIN hierarchy.Publication p ON p.Id = nr.PublicationId AND p.Deleted = 0
				where n.Id <> 1	AND nr.Deleted = 0 AND n.Deleted = 0 AND n.Hidden = 0
				GROUP BY nr.NodeId
				
			SELECT
				nv.NodeId
				,cnv.Id AS NodeVersionId
				,cnv.Name
				,cnv.Description
				,cnv.BannerUrl
				,cnv.BadgeUrl
				,cnv.CardImageUrl
				,cnv.Url
				,cnv.RestrictedAccess	
				,CAST(CASE WHEN cnv.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0 ELSE 1 END AS bit) AS HasAccess		
				,(SELECT Sum(rvrs.AverageRating) 
				  FROM [resources].[ResourceVersionRatingSummary] rvrs
				  WHERE rvrs.resourceversionid IN 
					(SELECT r.currentresourceversionid 
						FROM [resources].[Resource] r 
					    WHERE r.deleted = 0  AND r.id IN 
					(SELECT nr.resourceid 
						 FROM   [hierarchy].[NodeResource] nr 
						 WHERE  nr.nodeid = n.id))
					) AS AverageRating
					,ub.Id AS BookMarkId
			    ,CAST(ISNULL(ub.[Deleted], 1) ^ 1 AS BIT) AS IsBookmarked 
				,cpAgg.ProvidersJson
			FROM @Catalogues tc
			JOIN [hierarchy].[Node] n ON tc.NodeId = n.Id
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN (
			SELECT 
				cnp.CatalogueNodeVersionId,
				JSON_QUERY('[' + STRING_AGG(
					'{"Id":' + CAST(p.Id AS NVARCHAR) +
					',"Name":"' + p.Name + '"' +
					',"Description":"' + p.Description + '"' +
					',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
				',') + ']') AS ProvidersJson
			FROM hierarchy.CatalogueNodeVersionProvider cnp
			JOIN hub.Provider p ON p.Id = cnp.ProviderId
			WHERE p.Deleted = 0 and cnp.Deleted = 0
			GROUP BY cnp.CatalogueNodeVersionId
			) cpAgg ON cpAgg.CatalogueNodeVersionId = cnv.Id
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2
			ORDER BY tc.NodeCount DESC, AverageRating DESC, cnv.Name
				OFFSET @OffsetRows ROWS
				FETCH NEXT @FetchRows ROWS ONLY
		
			SELECT @TotalRecords = CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END FROM @Catalogues
	END
	ELSE IF  @DashboardType = 'all-catalogues'
	BEGIN
		SET @FetchRows = 9
		SET @OffsetRows = (@PageNumber - 1) * @FetchRows
		IF @PageNumber = -1
		BEGIN
			SET @FetchRows = 100000
			SET @OffsetRows = 0
		END
			SELECT
				nv.NodeId
				,cnv.Id AS NodeVersionId
				,cnv.Name
				,cnv.Description
				,cnv.BannerUrl
				,cnv.BadgeUrl
				,cnv.CardImageUrl
				,cnv.Url
				,cnv.RestrictedAccess
				,CAST(CASE WHEN cnv.RestrictedAccess = 1 AND auth.CatalogueNodeId IS NULL THEN 0 ELSE 1 END AS bit) AS HasAccess
				,ub.Id AS BookMarkId
			    ,CAST(ISNULL(ub.[Deleted], 1) ^ 1 AS BIT) AS IsBookmarked 
				,cpAgg.ProvidersJson
			FROM [hierarchy].[Node] n
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN (
			SELECT 
				cnp.CatalogueNodeVersionId,
				JSON_QUERY('[' + STRING_AGG(
					'{"Id":' + CAST(p.Id AS NVARCHAR) +
					',"Name":"' + p.Name + '"' +
					',"Description":"' + p.Description + '"' +
					',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
				',') + ']') AS ProvidersJson
			FROM hierarchy.CatalogueNodeVersionProvider cnp
			JOIN hub.Provider p ON p.Id = cnp.ProviderId
			WHERE p.Deleted = 0 and cnp.Deleted = 0
			GROUP BY cnp.CatalogueNodeVersionId
			) cpAgg ON cpAgg.CatalogueNodeVersionId = cnv.Id
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2
			ORDER BY cnv.Name
				OFFSET @OffsetRows ROWS
				FETCH NEXT @FetchRows ROWS ONLY

			SELECT @TotalRecords = Count(1)
			FROM [hierarchy].[Node] n
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			WHERE  n.Id <> 1	AND n.Hidden = 0 AND  n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2
	END
	ELSE IF  @DashboardType = 'my-catalogues'	
	BEGIN
			DECLARE @MyActivity TABLE (NodeId [int] NOT NULL PRIMARY KEY, ResourceActivityId [int] NOT NULL, CatalogueNodeId [INT] NOT NULL)

			INSERT INTO @MyActivity
				SELECT np.NodeId, MAX(ra.Id) ResourceActivityId, CatalogueNodeId
				FROM
				activity.ResourceActivity ra				
				JOIN [hierarchy].[NodePath] np ON  np.id = ra.NodePathId				
				WHERE ra.UserId = @UserId AND  np.NodeId <> 1 
				GROUP BY np.NodeId , CatalogueNodeId			
				ORDER BY ResourceActivityId DESC

			SELECT DISTINCT
				nv.NodeId
				,cnv.Id AS NodeVersionId
				,cnv.Name
				,cnv.Description
				,cnv.BannerUrl
				,cnv.BadgeUrl
				,cnv.CardImageUrl
				,cnv.Url
				,cnv.RestrictedAccess
				,CAST(CASE WHEN cnv.RestrictedAccess = 1 THEN 0 ELSE 1 END AS bit) AS HasAccess
				,ub.Id AS BookMarkId
			    ,CAST(ISNULL(ub.[Deleted], 1) ^ 1 AS BIT) AS IsBookmarked 
				,cpAgg.ProvidersJson
			FROM @MyActivity ma
			JOIN [hierarchy].[Node] n ON ma.NodeId = n.Id
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = ma.CatalogueNodeId
			LEFT JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN (
			SELECT 
				cnp.CatalogueNodeVersionId,
				JSON_QUERY('[' + STRING_AGG(
					'{"Id":' + CAST(p.Id AS NVARCHAR) +
					',"Name":"' + p.Name + '"' +
					',"Description":"' + p.Description + '"' +
					',"Logo":"' + ISNULL(p.Logo, '') + '"}', 
				',') + ']') AS ProvidersJson
			FROM hierarchy.CatalogueNodeVersionProvider cnp
			JOIN hub.Provider p ON p.Id = cnp.ProviderId
			WHERE p.Deleted = 0 and cnp.Deleted = 0
			GROUP BY cnp.CatalogueNodeVersionId
			) cpAgg ON cpAgg.CatalogueNodeVersionId = cnv.Id
			INNER JOIN hub.Scope s ON ma.CatalogueNodeId = s.CatalogueNodeId
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE 
			n.Id <> 1
			AND	n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2 AND s.Deleted = 0
			ORDER BY nv.NodeId DESC, cnv.Name
			OFFSET @OffsetRows ROWS
			FETCH NEXT @FetchRows ROWS ONLY	

			;WITH CTETEMP AS (SELECT DISTINCT NV.NodeId 
			FROM @MyActivity ma
				JOIN [hierarchy].[Node] n ON ma.NodeId = n.Id
				JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = ma.CatalogueNodeId
				LEFT JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
				INNER JOIN hub.Scope s ON ma.CatalogueNodeId = s.CatalogueNodeId
				LEFT JOIN hub.UserBookmark ub ON ub.UserId = @UserId AND ub.NodeId = nv.NodeId
				LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
							  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
							  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
				WHERE 
				n.Id <> 1
				AND	n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2 AND s.Deleted = 0)

				SELECT @TotalRecords =  CASE WHEN COUNT(*) > 12 THEN @MaxRows ELSE COUNT(*) END
				FROM CTETEMP
	END
END