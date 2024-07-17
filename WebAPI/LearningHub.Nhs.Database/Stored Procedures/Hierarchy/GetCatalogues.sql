CREATE PROCEDURE [hierarchy].[GetCatalogues] (
	 @userId INT	
	,@filterChar nvarchar(10)
	,@OffsetRows int
	,@fetchRows int
	)
AS
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
			FROM [hierarchy].[Node] n
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @userId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2 
			and cnv.Name like @filterChar+'%'
			ORDER BY cnv.Name
			OFFSET @OffsetRows ROWS
			FETCH NEXT @FetchRows ROWS ONLY
	

	
		
END
