-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Return user group - role - scope info for supplied UserId.
--
-- Modification History
--
-- 21-01-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[RoleUserGroupGetByUserId]
(
@userId int
)

AS

BEGIN

	SELECT
		CAST([Sequence] AS int) AS [Key],
		RoleUserGroupId,
		rug.UserGroupId,
		UserGroupName,
		RoleId,
		RoleId AS RoleEnum,
		RoleName,
		ScopeId,
		ScopeTypeId AS ScopeType,
		CatalogueNodeId,
		CatalogueNodeVersionId,
		CatalogueName
	FROM
		hub.RoleUserGroupView rug
	INNER JOIN
		hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
	WHERE
		uug.UserId = @userId
		AND uug.Deleted = 0

END
GO