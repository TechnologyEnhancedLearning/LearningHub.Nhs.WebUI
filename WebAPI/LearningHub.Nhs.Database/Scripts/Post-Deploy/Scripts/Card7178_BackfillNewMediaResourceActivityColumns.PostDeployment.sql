/*
 Post-Deployment Script Template							
--------------------------------------------------------------------------------------
    RobS - 14 Dec 2020
	Card 7178 - 1. Populate the new SecondsPlayed and PercentComplete columns in the existing rows of the MediaResourceActivity table.

				2. Delete existing video/audio ResourceActivities that do not have a corresponding MediaResourceActivity. This has 
				happened because the resource screen has been creating ResourceActivity records immediately after loading the screen
				rather than when the video is played.
--------------------------------------------------------------------------------------
*/

-- Run this script once, when no records have a SecondsPlayed value. This signifies that this script has not yet been run.
IF NOT EXISTS(SELECT 1 FROM activity.MediaResourceActivity WHERE SecondsPlayed IS NOT NULL)
BEGIN

	-- 1. Complete any open activities. They are left open when the session times out before the resource screen is closed, or if using iphone/ipad. 
	-- For each MediaResourceActivity, if the launch ResourceActivity record doesn't have a corresponding completion record (ActivityStatusId = 3), create it now.
	INSERT INTO activity.ResourceActivity(
		[UserId]
		,[LaunchResourceActivityId]
		,[ResourceId]
		,[ResourceVersionId]
		,[MajorVersion]
		,[MinorVersion]
		,[NodePathId]
		,[ActivityStatusId]
		,[ActivityStart]
		,[ActivityEnd]
		,[DurationSeconds]
		,[Score]
		,[Deleted]
		,[CreateUserID]
		,[CreateDate]
		,[AmendUserID]
		,[AmendDate])
	SELECT 
		mra.CreateUserID,
		ra.Id,
		ra.ResourceId,
		ra.ResourceVersionId,
		ra.MajorVersion,
		ra.MinorVersion,
		ra.NodePathId,
		3, -- 3=completed
		NULL,
		COALESCE((SELECT MAX(ClientDateTime) FROM activity.MediaResourceActivityInteraction WHERE MediaResourceActivityId = mra.Id), ra.ActivityStart) as ActivityEnd, -- Use latest interaction datetime.
		0,
		NULL,
		0,
		mra.CreateUserID,
		SYSDATETIMEOFFSET(),
		mra.CreateUserID,
		SYSDATETIMEOFFSET()
	FROM activity.MediaResourceActivity mra 
	left join activity.ResourceActivity ra on mra.ResourceActivityId = ra.Id
	left join resources.ResourceVersion rv on ra.ResourceVersionId = rv.Id
	left join [activity].[ResourceActivity] ra2 on ra.Id = ra2.LaunchResourceActivityId and ra2.ActivityStatusId = 3
	where ra2.Id is null

	-- 2. Fix data issue with ResourceActivity records being created even when user didn't play the media. After the point where detailed media activity 
	-- recording started to take place, mark the ResourceActivity records as deleted where there is no corresponding MediaResourceActivity record. 
	-- These were caused by a bug which will have been fixed by the time this script is run.
	DECLARE @DetailedActivityStartDate DateTimeOffset(7)
	SELECT @DetailedActivityStartDate = MIN(ActivityStart) FROM activity.ResourceActivity ra
		INNER JOIN resources.Resource AS r ON ra.ResourceId = r.Id
	WHERE ra.ActivityEnd IS NULL AND (r.ResourceTypeId = 7 OR r.ResourceTypeId = 2) -- 7=video & 2=audio

	-- Mark the launch ResourceActivity records as deleted
	UPDATE ra
	  SET ra.Deleted = 1
	  FROM activity.ResourceActivity AS ra
	  INNER JOIN resources.Resource AS r ON ra.ResourceId = r.Id
	  LEFT JOIN activity.MediaResourceActivity AS mra ON mra.ResourceActivityId = ra.Id  
	  WHERE (r.ResourceTypeId = 7 OR r.ResourceTypeId = 2) AND -- 7=video & 2=audio
		ra.ActivityStart >= @DetailedActivityStartDate AND
		ra.ActivityStatusId = 1 AND
		mra.Id Is NULL

	-- Mark as deleted any others that reference a deleted record as their LaunchResourceActivityId.
	UPDATE ra
	  SET ra.Deleted = 1
	  FROM activity.ResourceActivity AS ra
		LEFT JOIN [activity].[ResourceActivity] ra2 on ra2.Id = ra.LaunchResourceActivityId
	  WHERE ra2.deleted = 1
    
	-- 3. Backfill all existing MediaResourceActivity records with SecondsPlayed and PercentComplete data. 
	-- Use cursor to call calculation SP once for each MediaResourceActivity record.
    DECLARE @ResourceId int,
		@MajorVersion int,
		@MediaResourceActivityId int,
		@UserId int,
		@AuditUserId int

	DECLARE cursorElement CURSOR FOR
			SELECT  
				rv.ResourceId, 
				rv.MajorVersion, 
				mra.Id as MediaResourceActivityId, 
				ra.UserId as UserId, 
				ra.UserId as AuditUserId
			FROM    
				activity.MediaResourceActivity mra
				left join activity.ResourceActivity ra on mra.ResourceActivityId = ra.Id
				left join resources.ResourceVersion rv on ra.ResourceVersionId = rv.Id

	OPEN cursorElement
	FETCH NEXT FROM cursorElement INTO @ResourceId, @MajorVersion, @MediaResourceActivityId, @UserId, @AuditUserId

	WHILE ( @@FETCH_STATUS = 0 )
	BEGIN
		exec activity.CalculatePlayedMediaSegments @ResourceId, @MajorVersion, @MediaResourceActivityId, @UserId, @AuditUserId

		FETCH NEXT FROM cursorElement INTO @ResourceId, @MajorVersion, @MediaResourceActivityId, @UserId, @AuditUserId
	END         
	CLOSE cursorElement
	DEALLOCATE cursorElement

END

GO

