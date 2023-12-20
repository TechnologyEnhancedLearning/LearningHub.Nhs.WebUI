/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Killian davies 13.07.21
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(SELECT 'X' FROM [activity].[ActivityStatus] WHERE Id = 6)
BEGIN
	INSERT INTO [activity].[ActivityStatus]([Id],[ActivityStatus],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(6, 'Downloaded', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO

-- Modify existing ResourceActivity events for Generic File from "Launched" to "Downloaded"
UPDATE 
	ra
SET
	ActivityStatusId=6
FROM
	activity.ResourceActivity ra
INNER JOIN
	resources.[Resource] r on ra.ResourceId = r.Id
WHERE 
	r.ResourceTypeId = 9 AND ActivityStatusId=1
GO
