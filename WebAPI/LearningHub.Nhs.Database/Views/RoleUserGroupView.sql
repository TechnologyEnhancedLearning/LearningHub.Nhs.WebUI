-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      11-01-2021
-- Purpose      View of User Group - Role -Scope details
--				Used by Admin UI to display related info.
--
-- Modification History
--
-- 11-01-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE VIEW [hub].[RoleUserGroupView]

AS

SELECT 
	ROW_NUMBER() OVER(ORDER BY rug.Id) AS [Sequence],
	rug.Id AS RoleUserGroupId,
	ug.Id AS UserGroupId,
	ug.[Name] AS UserGroupName,
	r.Id AS RoleId,
	r.[Name] AS RoleName,
	s.Id AS ScopeId,
	st.Id AS ScopeTypeId,
	st.[Name] AS ScopeTypeName,
	cn.Id AS CatalogueNodeId,
	cnv.Id AS CatalogueNodeVersionId,
	cnv.[Name] AS CatalogueName
FROM 
	hub.UserGroup ug 
	INNER JOIN hub.RoleUserGroup rug ON ug.Id = rug.UserGroupId
	INNER JOIN hub.[Role] r ON rug.RoleId = r.Id
	LEFT JOIN hub.Scope s  ON rug.ScopeId = s.Id AND s.Deleted = 0
	LEFT JOIN hub.ScopeType st ON s.ScopeTypeId = st.Id AND st.Deleted = 0
	LEFT JOIN hierarchy.[Node] cn ON (s.CatalogueNodeId = cn.Id OR (s.CatalogueNodeId IS NULL AND s.ScopeTypeId = 1)) AND cn.Deleted = 0
	LEFT JOIN hierarchy.CatalogueNodeVersion cnv ON (cn.CurrentNodeVersionId = cnv.NodeVersionId) AND cnv.Deleted = 0 -- Catalogue
WHERE
	ug.Deleted = 0
	AND rug.Deleted = 0
	AND r.Deleted = 0

GO