-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Return user group - role - scope info for supplied UserGroupId.
--
-- Modification History
--
-- 21-01-2021  Killian Davies	Initial Revision
-- 05-06-2023  SA   Modified the sp to fix the sql timeout issues.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[RoleUserGroupGetByUserGroupId]
(
@userGroupId int
)

AS

BEGIN

	SELECT
	    CAST([Sequence] AS int) AS [Key],
		RoleUserGroupId,
		UserGroupId,
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
		hub.RoleUserGroupView
	WHERE
		UserGroupId = @userGroupId

END
GO