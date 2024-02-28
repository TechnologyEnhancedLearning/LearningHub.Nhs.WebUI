-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      25-03-2021
-- Purpose      Retrieves users for "Manage restricted catalogues"
--
-- Modification History
--
-- 25-03-2021  Killian Davies	Initial Revision
-- 09-02-2024  SA				Included RoleId
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[RestrictedCatalogueGetUsers] 
(
	@catalogueNodeId int,
	@emailAddressFilter nvarchar(100),
	@includeCatalogueAdmins bit,
	@includePlatformAdmins bit,
	@skip int,
	@take int = 10,
	@userCount int output
)

AS

BEGIN

	SELECT @userCount = COUNT(UserId)
	FROM
	(
		SELECT 
			uug.UserId,
			[AttributeSequence] = ROW_NUMBER() OVER (PARTITION BY uug.UserId ORDER BY ISNULL(uga.Id, 0))
		FROM
			hub.UserUserGroup uug
			INNER JOIN  hub.RoleUserGroup rug ON uug.UserGroupId = rug.UserGroupId
			INNER JOIN hub.Scope s ON rug.ScopeId = s.Id
			INNER JOIN hub.[UserProfile] up ON uug.UserId = up.Id
			INNER JOIN hub.[User] u_addedBy ON uug.CreateUserId = u_addedBy.Id
			LEFT JOIN hub.UserGroupAttribute uga 
				ON uug.UserGroupId = uga.UserGroupId AND uga.AttributeId IN (2,8) AND uga.Deleted = 0 -- Restricted Access & Preview User Group
			LEFT JOIN hub.UserUserGroup uug_exclude 
				ON uug.UserId = uug_exclude.UserId 
				AND uug_exclude.UserGroupId IN (2, 1083) -- System Admin, Internal
				AND uug_exclude.Deleted = 0
			LEFT JOIN 
			(  SELECT DISTINCT
					uug.UserId
				FROM
					hub.UserUserGroup uug
					INNER JOIN  hub.RoleUserGroup rug ON uug.UserGroupId = rug.UserGroupId
					INNER JOIN hub.Scope s ON rug.ScopeId = s.Id
					LEFT JOIN hub.UserGroupAttribute uga 
						ON uug.UserGroupId = uga.UserGroupId AND uga.AttributeId IN (2,8) AND uga.Deleted = 0 -- Restricted Access & preview User Group
				WHERE
					s.CatalogueNodeId = @catalogueNodeId
					AND rug.RoleId IN (1, 2, 3, 8)
					AND uga.Id IS NULL
					AND uug.Deleted = 0
					AND rug.Deleted = 0
					AND s.Deleted = 0) PlatformAdminAdded ON uug.UserId = PlatformAdminAdded.UserId
		WHERE
			s.CatalogueNodeId = @catalogueNodeId
			AND (ISNULL(@emailAddressFilter, '') = '' OR up.EmailAddress = @emailAddressFilter)
			AND ((@includeCatalogueAdmins = 1 AND PlatformAdminAdded.UserId IS NULL)
				 OR 
				 (@includePlatformAdmins = 1 AND PlatformAdminAdded.UserId IS NOT NULL))
			AND rug.RoleId IN (1, 2, 3, 8)
			AND uug_exclude.Id IS NULL
			AND uug.Deleted = 0
			AND rug.Deleted = 0
			AND s.Deleted = 0
	) AS T1

	SELECT TOP (@take) 
		UserUserGroupId,
		UserName,
		EmailAddress,
		LTRIM(RTRIM(ISNULL(FirstName, '') + ' ' + ISNULL(LastName, ''))) AS FullName,
		AddedByUsername,
		AddedDatetime,
		CAST(CanRemove AS bit) CanRemove,
		RoleId
	FROM
	(
		SELECT 
			uug.Id AS UserUserGroupId,
			up.UserName,
			up.EmailAddress,
			up.FirstName,
			up.LastName,
			[AttributeSequence] = ROW_NUMBER() OVER (PARTITION BY uug.UserId ORDER BY ISNULL(uga.Id, 0)),
			[UserIdDenseRank] = DENSE_RANK() OVER (ORDER BY up.LastName, up.FirstName, up.Id),
			AddedByPlatformAdmin = CASE WHEN PlatformAdminAdded.UserId IS NULL THEN 0 ELSE 1 END,
			AddedByUsername = CASE WHEN uga.Id IS NULL THEN 'Platform admin' ELSE u_addedBy.UserName END,
			AddedDatetime =  CASE WHEN uga.Id IS NULL THEN NULL ELSE uug.CreateDate END,
			CanRemove = CASE WHEN PlatformAdminAdded.UserId  IS NULL THEN 1 ELSE 0 END,
			rug.RoleId 
		FROM
			hub.UserUserGroup uug
			INNER JOIN  hub.RoleUserGroup rug ON uug.UserGroupId = rug.UserGroupId
			INNER JOIN hub.Scope s ON rug.ScopeId = s.Id
			INNER JOIN hub.[UserProfile] up ON uug.UserId = up.Id
			INNER JOIN hub.[User] u_addedBy ON uug.CreateUserId = u_addedBy.Id
			LEFT JOIN hub.UserGroupAttribute uga 
				ON uug.UserGroupId = uga.UserGroupId AND uga.AttributeId IN (2,8) AND uga.Deleted = 0 -- Restricted Access & Preview User Group
			LEFT JOIN hub.UserUserGroup uug_exclude 
				ON uug.UserId = uug_exclude.UserId 
				AND uug_exclude.UserGroupId IN (2, 1083) -- System Admin, Internal
				AND uug_exclude.Deleted = 0
			LEFT JOIN 
			(  SELECT DISTINCT
					uug.UserId
				FROM
					hub.UserUserGroup uug
					INNER JOIN  hub.RoleUserGroup rug ON uug.UserGroupId = rug.UserGroupId
					INNER JOIN hub.Scope s ON rug.ScopeId = s.Id
					LEFT JOIN hub.UserGroupAttribute uga 
						ON uug.UserGroupId = uga.UserGroupId AND uga.AttributeId IN( 2,8) AND uga.Deleted = 0 -- Restricted Access & preview User Group
				WHERE
					s.CatalogueNodeId = @catalogueNodeId
					AND rug.RoleId IN (1, 2, 3, 8)
					AND uga.Id IS NULL
					AND uug.Deleted = 0
					AND rug.Deleted = 0
					AND s.Deleted = 0) PlatformAdminAdded ON uug.UserId = PlatformAdminAdded.UserId
		WHERE
			s.CatalogueNodeId = @catalogueNodeId
			AND (ISNULL(@emailAddressFilter, '') = '' OR up.EmailAddress = @emailAddressFilter)
			AND ((@includeCatalogueAdmins = 1 AND PlatformAdminAdded.UserId IS NULL)
				 OR 
				 (@includePlatformAdmins = 1 AND PlatformAdminAdded.UserId IS NOT NULL))
			AND rug.RoleId IN (1, 2, 3, 8)
			AND uug_exclude.Id IS NULL
			AND uug.Deleted = 0
			AND rug.Deleted = 0
			AND s.Deleted = 0
	) AS T1
	WHERE 
		 T1.UserIdDenseRank > @skip
END
GO