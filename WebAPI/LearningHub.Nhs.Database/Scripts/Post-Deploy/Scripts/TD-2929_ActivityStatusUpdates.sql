/* 
	TD-2929
	Add new ActivityStatusId for Incomplete and update existing ResourceActivity records as per the Confluence table
	linked to in the ticket.
*/

IF NOT EXISTS (SELECT 1 FROM activity.ActivityStatus WHERE Id = 7)
BEGIN
	-- Add new ActivityStatus for Incomplete
	INSERT INTO activity.ActivityStatus(Id, ActivityStatus, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (7, 'Incomplete', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())


	-- Set the ActivityStatusId of all ResourceActivity records to Completed for Articles, Images, Web links, Files and Cases and HTML.
	UPDATE ra
	SET ActivityStatusId = 3 /* Completed */
	FROM activity.ResourceActivity ra
		LEFT JOIN resources.Resource r ON ra.ResourceId = r.Id
	WHERE r.ResourceTypeId IN (1,5,8,9,10, 12)


	-- Set Video & Audio ResourceActivity records to Completed (3) for those activities recorded BEFORE detailed media activity recording was introduced (Sept 2020).
	-- These activities only had one ResourceActivity record with ActivityStart and ActivityEnd dates both set.
	UPDATE ra
	SET ActivityStatusId = 3 /* Completed */
	FROM activity.ResourceActivity ra
		LEFT JOIN resources.Resource r ON ra.ResourceId = r.Id
	WHERE r.ResourceTypeId IN (2,7) AND ra.LaunchResourceActivityId IS NULL AND ActivityEnd IS NOT NULL

	-- Set Video & Audio ResourceActivity start records to Incomplete (7) for those activities recorded AFTER detailed media activity recording was introduced (Sept 2020).
	UPDATE ra
	SET ActivityStatusId = 7 /* Incomplete */
	FROM activity.ResourceActivity ra
		LEFT JOIN resources.Resource r ON ra.ResourceId = r.Id
	WHERE r.ResourceTypeId IN (2,7) AND ra.LaunchResourceActivityId is null AND ActivityEnd is null

	-- Set Video & Audio ResourceActivity end records to Incomplete (7) where corresponding MediaResourceActivity is less than 100% complete.
	UPDATE raEnd 
	SET ActivityStatusId = 7 /* Incomplete */
	FROM [activity].[MediaResourceActivity] mra 
		LEFT JOIN [activity].[ResourceActivity] raStart ON raStart.Id = mra.ResourceActivityId
		LEFT JOIN [activity].[ResourceActivity] raEnd ON raStart.Id = raEnd.LaunchResourceActivityId
	WHERE mra.PercentComplete < 100

	-- Set assessment resource activity to Incomplete (7) where launch resource activity is null

	UPDATE ra SET ra.ActivityStatusId =  7
	FROM activity.ResourceActivity ra
		LEFT JOIN resources.Resource r ON ra.ResourceId = r.Id
	WHERE r.ResourceTypeId IN (11) AND ra.LaunchResourceActivityId IS NULL AND ActivityEnd IS NULL

	-- Set assessment resouce activity to completed for the informal assessment type (not required - already status marked as completed.
	-- Formal assessment type , if the score is greater than or equal to pass mark - status is passed

	UPDATE ra SET ActivityStatusId = 5
				FROM
				(SELECT a.* FROM activity.ResourceActivity a INNER JOIN (SELECT ResourceId, MAX(Id) as id FROM activity.ResourceActivity GROUP BY ResourceId ) AS b ON a.ResourceId = b.ResourceId AND a.id = b.id  ) ra					
				INNER JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
				INNER JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId
				LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
				LEFT JOIN [activity].[AssessmentResourceActivity] ara ON ara.ResourceActivityId = COALESCE(ra.LaunchResourceActivityId, ra.Id)
				WHERE 													
					 r.ResourceTypeId = 11 AND ara.Score >= arv.PassMark  AND AssessmentType = 2

	-- Formal assessment type , if the score is greater than or equal to pass mark - status is failed
	UPDATE ra SET ActivityStatusId = 4
				FROM
				(SELECT a.* FROM activity.ResourceActivity a INNER JOIN (SELECT ResourceId, MAX(Id) as id FROM activity.ResourceActivity GROUP BY ResourceId ) AS b ON a.ResourceId = b.ResourceId AND a.id = b.id  ) ra					
				INNER JOIN [resources].[Resource] r ON  ra.ResourceId = r.Id
				INNER JOIN [resources].[ResourceVersion] rv ON  rv.Id = ra.ResourceVersionId
				LEFT JOIN [resources].[AssessmentResourceVersion] arv ON arv.ResourceVersionId = ra.ResourceVersionId
				LEFT JOIN [activity].[AssessmentResourceActivity] ara ON ara.ResourceActivityId = COALESCE(ra.LaunchResourceActivityId, ra.Id)
				WHERE 													
					 r.ResourceTypeId = 11 AND ara.Score < arv.PassMark  AND AssessmentType = 2 
	-- Set SCORM resource activity to Incomplete (7) where launch resource activity is null , it will update all the existing records with status 1 and 6 to 7

	UPDATE RA SET ra.ActivityStatusId =  7
	FROM activity.ResourceActivity RA
		LEFT JOIN resources.Resource R ON RA.ResourceId = R.Id
	WHERE R.ResourceTypeId IN (6) AND RA.LaunchResourceActivityId IS NULL AND ActivityEnd IS NULL

END