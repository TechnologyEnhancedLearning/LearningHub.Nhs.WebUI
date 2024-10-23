-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      25-03-2021
-- Purpose      Retrieves summary counts for "Manage restricted catalogues"
--
-- Modification History
--
-- 25-03-2021  Killian Davies	Initial Revision
-- 12-02-2023  SA				Included preview access
-- 06-08-2023  SS				Considering only active users
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[RestrictedCatalogueGetSummary]
(
	@catalogueNodeId int
)

AS

BEGIN
	
	DECLARE @userCount int
	DECLARE @accessRequestCount int

	SELECT @userCount = COUNT(uug.UserId)
	FROM
		hub.UserUserGroup uug
		INNER JOIN  hub.RoleUserGroup rug ON uug.UserGroupId = rug.UserGroupId
		INNER JOIN hub.Scope s ON rug.ScopeId = s.Id
		LEFT JOIN hub.UserUserGroup uug_exclude 
			ON uug.UserId = uug_exclude.UserId 
			AND uug_exclude.UserGroupId IN (2, 1083) -- System Admin, Internal
			AND uug_exclude.Deleted = 0
	WHERE
		s.CatalogueNodeId = @catalogueNodeId
		AND rug.RoleId IN (1, 2, 3, 8)
		AND uug.Deleted = 0
		AND rug.Deleted = 0
		AND s.Deleted = 0
		AND uug_exclude.Id IS NULL

	SELECT @accessRequestCount = COUNT(Id)
	FROM
		hierarchy.CatalogueAccessRequest 
	WHERE
		CatalogueNodeId = @catalogueNodeId
		AND [Status] = 2
		AND Deleted = 0

	SELECT @accessRequestCount = COUNT(CatalogueAccessRequestId)
	FROM
	(
		SELECT 
			car.Id AS CatalogueAccessRequestId,
			up.UserName,
			car.EmailAddress,
			LTRIM(RTRIM(ISNULL(up.FirstName,'') + ' ' + ISNULL(up.LastName,''))) AS FullName,
			car.[Status] AS CatalogueAccessRequestStatus,
			car.CreateDate AS RequestedDatetime,
			[Sequence] = ROW_NUMBER() OVER (PARTITION BY car.UserId ORDER BY car.CreateDate DESC)
		FROM
			[hierarchy].[CatalogueAccessRequest] car
		INNER JOIN
			hub.UserProfile up ON car.UserId = up.Id
		WHERE
			car.CatalogueNodeId = @catalogueNodeId
			AND car.Deleted = 0
			AND up.Deleted = 0
			AND car.Status = 0
			AND car.EmailAddress NOT IN (SELECT DISTINCT EmailAddress FROM [hierarchy].[CatalogueAccessRequest] WHERE CatalogueNodeId = @catalogueNodeId
										AND Deleted = 0
										AND Status IN(1) )
	) AS T1
	WHERE T1.[Sequence] = 1

	SELECT 
		CatalogueNodeId = @catalogueNodeId,
		UserCount = @userCount,
		AccessRequestCount = @accessRequestCount

END

GO