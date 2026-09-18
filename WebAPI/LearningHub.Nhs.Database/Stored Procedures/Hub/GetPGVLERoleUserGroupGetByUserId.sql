-- Author      Swapnamol Abraham
-- Created      01-01-2020
-- Purpose      Return user group info for supplied PGVLE UserId.
--
-- Modification History
--
-- 17-09-2026  SA	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[GetPGVLERoleUserGroupGetByUserId]
(
@userId int
)
AS

BEGIN

	SELECT ug.Id,
	    uug.Id As UserGroupId,
		[Name] AS UserGroupName,
		@userId As UserId,
		'' AS UserName
	FROM
		hub.UserGroup ug
	INNER JOIN
		hub.UserUserGroup uug ON ug.Id = uug.UserGroupId
	WHERE
		uug.UserId = @userId
	    AND ug.Id = 10690
		AND uug.Deleted = 0 AND ug.Deleted = 0

END