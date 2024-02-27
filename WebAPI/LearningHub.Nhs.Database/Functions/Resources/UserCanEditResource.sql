-----------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      11 Oct 2023
-- Purpose      Indicates whether a User has edit permissions on a resource version
--
-- Modification History
--
-----------------------------------------------------------------------------------
CREATE FUNCTION [resources].[UserCanEditResource]
(
	@ResourceVersionId		INT,
	@UserId					INT
)
RETURNS BIT
AS
BEGIN

	DECLARE @IsEditor BIT
	SET		@IsEditor = 0

	SELECT @IsEditor = CASE WHEN nr.NodeId IS NULL THEN CAST (0 AS BIT) ELSE CAST(1 AS BIT) END 
	FROM resources.Resource r 
	INNER JOIN hierarchy.NodeResource nr ON nr.ResourceId = r.Id AND nr.VersionStatusId = 2 -- Published
	INNER JOIN hub.Scope s ON nr.NodeId = s.CatalogueNodeId
	INNER JOIN hub.RoleUserGroup rug ON rug.ScopeId = s.Id AND rug.RoleId = 1 -- Editor
	INNER JOIN hub.userUserGroup uug ON uug.UserGroupId = rug.UserGroupId AND uug.UserId = @userId
	WHERE	r.CurrentResourceVersionId = @ResourceVersionId
		AND nr.Deleted = 0 
		AND s.Deleted = 0
		AND rug.Deleted = 0
		AND uug.Deleted = 0

   IF @IsEditor = 0 
		BEGIN

		SELECT @IsEditor = CASE WHEN nr.NodeId IS NULL THEN CAST (0 AS BIT) ELSE CAST(1 AS BIT) END 
		FROM resources.Resource r 
		INNER JOIN hierarchy.NodeResource nr ON nr.ResourceId = r.Id AND nr.VersionStatusId = 2 -- Published 
		INNER JOIN hierarchy.NodeLink NL on NL.ChildNodeId = nr.NodeId		
		INNER JOIN hub.Scope s ON NL.ParentNodeId = s.CatalogueNodeId
		INNER JOIN hub.RoleUserGroup rug ON rug.ScopeId = s.Id AND rug.RoleId = 1 -- Editor
		INNER JOIN hub.userUserGroup uug ON uug.UserGroupId = rug.UserGroupId AND uug.UserId = @userId
		WHERE	r.CurrentResourceVersionId = @resourceVersionId
			AND nr.Deleted = 0 
			AND s.Deleted = 0
			AND rug.Deleted = 0
			AND uug.Deleted = 0

	END

	RETURN @IsEditor

END
GO