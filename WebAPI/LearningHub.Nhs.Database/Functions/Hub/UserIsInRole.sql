-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      10-03-2021
-- Purpose      Indicates whether a User has the supplied Role / Scope via UserGroup membership
--
-- Modification History
--
-- 10-03-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE FUNCTION hub.UserIsInRole
(
	@UserId	int,
	@RoleId	int,
	@ScopeId int
)
RETURNS bit
AS
BEGIN

	DECLARE @isInRole bit = 0

	IF EXISTS (	SELECT	'X'
				FROM	hub.UserUserGroup uug
				INNER JOIN hub.RoleUserGroup rug 
					ON uug.UserGroupId = rug.UserGroupId AND rug.RoleId = @RoleId AND rug.ScopeId = @ScopeId
				WHERE uug.UserId = @UserId
					AND uug.Deleted = 0
					AND rug.Deleted = 0
				)
	BEGIN
		SET @isInRole = 1
	END

	RETURN @isInRole

END
GO