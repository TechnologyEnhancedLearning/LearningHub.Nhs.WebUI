-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      25-03-2021
-- Purpose      Retrieves access requests for "Manage restricted catalogues"
--
-- Modification History
--
-- 25-03-2021  Killian Davies	Initial Revision
-- 08-02-2024  SA				Included Role details.
-- 31-05-2024  SS				Considering only active users
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[RestrictedCatalogueGetAccessRequests] 
(
	@catalogueNodeId int,
	@includeNew bit,
	@includeApproved bit,
	@includeDenied bit
)

AS

BEGIN

	SELECT CatalogueAccessRequestId, UserName, EmailAddress, FullName, CatalogueAccessRequestStatus, RequestedDatetime, RoleId
	FROM
	(
		SELECT 
			car.Id AS CatalogueAccessRequestId,
			up.UserName,
			car.EmailAddress,
			LTRIM(RTRIM(ISNULL(up.FirstName,'') + ' ' + ISNULL(up.LastName,''))) AS FullName,
			car.[Status] AS CatalogueAccessRequestStatus,
			car.CreateDate AS RequestedDatetime,
			[Sequence] = ROW_NUMBER() OVER (PARTITION BY car.UserId ORDER BY car.CreateDate DESC),
			car.RoleId
		FROM
			[hierarchy].[CatalogueAccessRequest] car
		INNER JOIN
			hub.UserProfile up ON car.UserId = up.Id
		WHERE
			car.CatalogueNodeId = @catalogueNodeId
			AND car.Deleted = 0 and up.Deleted=0
	) AS T1
	WHERE T1.[Sequence] = 1
	AND ((@includeNew = 1 AND CatalogueAccessRequestStatus = 0)
		OR (@includeApproved = 1 AND CatalogueAccessRequestStatus = 1)
		OR (@includeDenied = 1 AND CatalogueAccessRequestStatus = 2))
	ORDER BY
		RequestedDatetime DESC

END

GO