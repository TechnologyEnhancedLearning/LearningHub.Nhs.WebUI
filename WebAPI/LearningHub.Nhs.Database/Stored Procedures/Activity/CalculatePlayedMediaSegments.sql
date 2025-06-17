
-------------------------------------------------------------------------------
-- Author       RobS (original by Jeremy and later modified by RobS)
-- Created      09-11-2020
-- Purpose      Analyse MediaActivityInteractions to produce a list of played segments and percentage complete.
--
-- Modification History
--
-- 09-11-2020  RobS	Initial Revision
-- 30-10-2023  RobS Updates for changes to activity status. Ending ResourceActivity status is now 'Incomplete' if
--             user hasn't played whole media file. Used to be 'Completed'.
-- 23-04-2025  SA   TD-4382 - added the condition to exclude the deleted resource versions 
-------------------------------------------------------------------------------

CREATE PROCEDURE [activity].[CalculatePlayedMediaSegments]
(
	@ResourceId int,
	@MajorVersion int,
	@MediaResourceActivityId int,
	@UserId int,
	@AuditUserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

/*
Reference:
https://www.red-gate.com/simple-talk/sql/t-sql-programming/calculating-gaps-between-overlapping-time-intervals-in-sql/

*/

-- Get the total duration.
declare @DurationInSeconds decimal(11,3) 
select @DurationInSeconds = CAST(ISNULL(vrv.[DurationInMilliseconds], arv.[DurationInMilliseconds]) AS DECIMAL) / 1000
from [resources].[ResourceVersion] (nolock) rv
	left join [resources].[VideoResourceVersion] (nolock) vrv on vrv.ResourceVersionId = rv.Id
	left join [resources].[AudioResourceVersion] (nolock) arv on arv.ResourceVersionId = rv.Id
where rv.ResourceId = @ResourceId
	and rv.MajorVersion = @MajorVersion
	and rv.VersionStatusId > 1 and rv.Deleted = 0
--select @DurationInSeconds

-- Start the analysis.
drop table if exists #MediaResourceActivityInteractionPivot1

select 
	ra.UserId
	, ra.ResourceId
	, ra.MajorVersion
	, ra.Id ResourceActivityId
	, MRA.id MRAid 
	, mrai.id mraiid
	, DisplayTimeSeconds DisplayTime
	, name [eventname]
	, mrai.ClientDateTime
	, LEAD(ra.Id ,1) OVER (PARTITION BY ra.UserId, ra.ResourceId, ra.MajorVersion, MRA.id ORDER BY mrai.ClientDateTime) nextResourceActivityId
	, LEAD(MRAI.DisplayTimeSeconds,1) OVER (PARTITION BY ra.UserId, ra.ResourceId, ra.MajorVersion, MRA.id ORDER BY mrai.ClientDateTime) nextDisplaytime
	, cast(0 as decimal(11,3)) calc_nextDisplaytime -- holding for future use
	, LEAD(name,1) OVER (PARTITION BY ra.UserId, ra.ResourceId, ra.MajorVersion ORDER BY mrai.ClientDateTime) nextEventName
	, LEAD(mrai.ClientDateTime,1) OVER (PARTITION BY ra.UserId, ra.ResourceId, ra.MajorVersion, MRA.id ORDER BY mrai.ClientDateTime) nextClientDateTime
	, cast(null as float) displaytimediff -- holding for future use 
	, cast(null as float) actionduration -- holding for future use
	, cast('' as nvarchar(30)) event_Type -- holding for future use
	, cast(0 as decimal(11,3)) viewtime -- holding for future use
into #MediaResourceActivityInteractionPivot1
from [activity].[MediaResourceActivity] (nolock) MRA
	inner join (select * 
					, (datepart(hour,DisplayTime) * 360) + (datepart(minute,DisplayTime) * 60) + datepart(second,DisplayTime) +  cast(datepart(millisecond,DisplayTime) as float)/1000 DisplayTimeSeconds
				from [activity].[MediaResourceActivityInteraction] nolock) MRAI
		on MRAI.MediaResourceActivityId = MRA.Id
	inner join [activity].[MediaResourceActivityType] (nolock) MRAT
		on MRAT.Id = MRAI.MediaResourceActivityTypeId
	inner join [activity].[ResourceActivity] (nolock) ra
		on ra.Id = mra.ResourceActivityId
	inner join [activity].[ResourceActivity] (nolock) ra2
		on ra.Id = ra2.LaunchResourceActivityId
where ra.userId = @userid
	and ra.ResourceId = @ResourceId
	and ra.MajorVersion = @MajorVersion
	and MRAI.MediaResourceActivityTypeId <> 4 -- Exclude 'Playing' events.
	and ra.ActivityStart <= (select ra.ActivityStart from [activity].[ResourceActivity] ra inner join [activity].[MediaResourceActivity] mra on ra.Id = mra.ResourceActivityId where mra.Id = @MediaResourceActivityId and ra.Deleted = 0) -- Exclude interactions from activities that started after the one in question.
	and ra.Deleted = 0
order by ra.Id, mrai.ClientDateTime

update #MediaResourceActivityInteractionPivot1
set displaytimediff = nextdisplaytime - DisplayTime 
	, actionduration = cast(datediff(MILLISECOND, ClientDateTime, nextClientDateTime) as float) /1000
where ResourceActivityId = nextResourceActivityId

-- Play ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Update #MediaResourceActivityInteractionPivot1
set event_Type = 'Play'
	, calc_nextDisplaytime = nextDisplaytime
	, viewtime = calc_nextDisplaytime - Displaytime
where eventName = 'Play'
	and actionduration > 1
	and ResourceActivityId = nextResourceActivityId

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

-- Rewind ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Update #MediaResourceActivityInteractionPivot1
set event_Type = 'Play + Rewind'
	--, calc_nextDisplaytime = nextDisplaytime--  + actionduration
	--, calc_nextDisplaytime = Displaytime  + actionduration
	, calc_nextDisplaytime = (SELECT MIN(x) FROM (VALUES (Displaytime  + actionduration), (@DurationInSeconds)) AS n(x)) -- Make sure segment end time is not longer than the total duration!
	, viewtime  = actionduration
where eventName in ('Play')
	and displaytimediff < actionduration
	-- and actionduration - displaytimediff > 1
	and ResourceActivityId = nextResourceActivityId

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

-- Fast Forward ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Update #MediaResourceActivityInteractionPivot1
set event_Type = 'Play + Fast Forward'
	--, calc_nextDisplaytime = Displaytime + actionduration
	, calc_nextDisplaytime = (SELECT MIN(x) FROM (VALUES (Displaytime  + actionduration), (@DurationInSeconds)) AS n(x)) -- Make sure segment end time is not longer than the total duration!
	, viewtime = actionduration
where eventName in ('Play')
	and displaytimediff > actionduration
	and ResourceActivityId = nextResourceActivityId

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

-- Buffering ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Update #MediaResourceActivityInteractionPivot1
set event_Type = 'Buffer'
where eventName in  ('Pause')
	and ResourceActivityId = nextResourceActivityId

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

-- select * from #MediaResourceActivityInteractionPivot1 

drop table if exists #MediaResourceActivityInteractionPivot2

select
	UserId
	, ResourceId
	, MajorVersion
	, mraip.ResourceActivityId
	, MRAid 
	, mraip.DisplayTime
	, mraip.eventname
	, ClientDateTime
	, nextResourceActivityId
	, nextDisplaytime
	, mraip.calc_nextDisplaytime
	, mraip.nextEventName
	, nextClientDateTime
	, mraip.displaytimediff
	, mraip.actionduration
	, mraip.event_Type
	, mraiid
	, viewtime
	, ( select top 1 max(mraip2.calc_nextDisplaytime) over (order by mraip2.calc_nextDisplaytime desc) 
		from #MediaResourceActivityInteractionPivot1 (nolock) mraip2
		where mraip2.DisplayTime < mraip.DisplayTime 
		) lastpriorevent
	, cast(0 as decimal(11,3)) skipped_time -- holding for future use
into #MediaResourceActivityInteractionPivot2
from #MediaResourceActivityInteractionPivot1 (nolock) mraip
where event_type like 'Play%' or nextEventName = 'End'
order by DisplayTime

-- Update SkippedTime
update #MediaResourceActivityInteractionPivot2
set skipped_time = iif(lastpriorevent - Displaytime > 0, null, Displaytime - lastpriorevent)  

-- select * from #MediaResourceActivityInteractionPivot2 order by displayTime

-- set segment start and end dates 
drop table if exists #MediaResourceActivityInteractionPivot3

-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
-- ~~~ Calculate played segments (across all activities) ~~~
-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

;WITH GroupedEvnts AS
(
    -- Essentially the same as Sample Query #3 except a different filter
    SELECT 
        displayTime StartTime
        , calc_nextDisplaytime EndTime
        , skipped_time
    FROM #MediaResourceActivityInteractionPivot2
),
C1 AS 
(
    -- Since the CTE above produces rows with a start and end date, we'll first unpivot
    -- those using CROSS APPLY VALUES and assign a type to each.  The columns e and s will
    -- either be NULL (s NULL for an end date, e NULL for a start date) or row numbers
    -- sequenced by the time.  UserID has been eliminated because we're looking for overlapping
    -- intervals across all users.
    SELECT ROUND(ts, 0) ts, [Type]
        ,e=CASE [Type] 
            WHEN 1 THEN NULL 
            ELSE ROW_NUMBER() OVER (PARTITION BY [Type] ORDER BY EndTime) END
        ,s=CASE [Type] 
            WHEN -1 THEN NULL 
            ELSE ROW_NUMBER() OVER (PARTITION BY [Type] ORDER BY StartTime) END
    FROM GroupedEvnts
    CROSS APPLY (VALUES (1, StartTime), (-1, EndTime)) a([Type], ts)
),
C2 AS
(
	-- Add a row number ordered as shown
	SELECT C1.*, se=ROW_NUMBER() OVER (ORDER BY ts, coalesce(e,s), [Type] DESC)
	FROM C1
),
C3 AS 
(
    -- Create a grpnm that pairs the rows
    SELECT ts, grpnm=(ROW_NUMBER() OVER (ORDER BY ts)-1) / 2 + 1
    FROM C2
    -- This filter is the magic that eliminates the overlaps
    WHERE COALESCE(s-(se-s)-1, (se-e)-e) = 0
),
C4 AS
(
    -- Grouping by grpnm restores the records to only non-overlapped intervals
    SELECT 
		SegmentStartTime=MIN(ts)
		, SegmentEndTime=MAX(ts)
    FROM C3
    GROUP BY grpnm
),
-- C5 to C7 merge contiguous segments into one segment.
C5 AS
(
	select SegmentStartTime, SegmentEndTime,
	case
		when SegmentStartTime = lag(SegmentEndTime) over(order by SegmentStartTime, SegmentEndTime)
		then 0 else 1
	end grp_start
	from C4
)
, 
C6 AS
(
	select SegmentStartTime, SegmentEndTime,
	sum(grp_start) over(order by SegmentStartTime, SegmentEndTime) grp
	from C5
),
C7 AS
(
	select min(SegmentStartTime) SegmentStartTime,
	max(SegmentEndTime) SegmentEndTime
	from C6
	group by grp
)
SELECT 
	@ResourceId		ResourceId
	, @userid		UserId
	, @MajorVersion MajorVersion
	, SegmentStartTime
	, SegmentEndTime
	, SegmentEndTime - SegmentStartTime as Duration
	, 0				Deleted
	, @AuditUserId		CreateUserId
	, @AmendDate		CreateDate
	, @AuditUserId		AmendUserId
	, @AmendDate		AmendDate
into #MediaResourceActivityInteractionPivot3
FROM C7
where SegmentStartTime <> SegmentEndTime
ORDER BY SegmentStartTime;

-- Delete records from MediaResourcePlayedSegment
Delete From activity.MediaResourcePlayedSegment
where 
	ResourceId = @ResourceId
	and UserId = @userid
	and MajorVersion = @MajorVersion 

-- Insert records into MediaResourcePlayedSegment
insert into activity.MediaResourcePlayedSegment
select * from #MediaResourceActivityInteractionPivot3

-- Calculate the Percentage complete
declare @PercentComplete decimal(7,3)
select 
	--ResourceId
	--, UserId
	--, MajorVersion
	--, @DurationInSeconds DurationInSeconds
	--, sum(Duration) TotalDuration
	@PercentComplete = iif(@DurationInSeconds <> 0, sum(Duration)/round(@DurationInSeconds, 0) * 100, null)
from #MediaResourceActivityInteractionPivot3 (nolock)
group by 
	ResourceId
	, UserId
	, MajorVersion


-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
-- ~~~ Calculate seconds played for this MediaResourceActivity ~~~
-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

-- These CTEs are identical to the previous set, but we start off with only the media interactions for this particular MediaResourceActivity.
declare @SecondsPlayed int
;WITH GroupedEvnts AS
(
    -- Essentially the same as Sample Query #3 except a different filter
    SELECT 
        displayTime StartTime
        , calc_nextDisplaytime EndTime
        , skipped_time
    FROM #MediaResourceActivityInteractionPivot2
	WHERE MRAid = @MediaResourceActivityId
),
C1 AS 
(
    -- Since the CTE above produces rows with a start and end date, we'll first unpivot
    -- those using CROSS APPLY VALUES and assign a type to each.  The columns e and s will
    -- either be NULL (s NULL for an end date, e NULL for a start date) or row numbers
    -- sequenced by the time.  UserID has been eliminated because we're looking for overlapping
    -- intervals across all users.
    SELECT ROUND(ts, 0) ts, [Type]
        ,e=CASE [Type] 
            WHEN 1 THEN NULL 
            ELSE ROW_NUMBER() OVER (PARTITION BY [Type] ORDER BY EndTime) END
        ,s=CASE [Type] 
            WHEN -1 THEN NULL 
            ELSE ROW_NUMBER() OVER (PARTITION BY [Type] ORDER BY StartTime) END
    FROM GroupedEvnts
    CROSS APPLY (VALUES (1, StartTime), (-1, EndTime)) a([Type], ts)
),
C2 AS
(
	-- Add a row number ordered as shown
	SELECT C1.*, se=ROW_NUMBER() OVER (ORDER BY ts, coalesce(e,s), [Type] DESC)
	FROM C1
),
C3 AS 
(
    -- Create a grpnm that pairs the rows
    SELECT ts, grpnm=(ROW_NUMBER() OVER (ORDER BY ts)-1) / 2 + 1
    FROM C2
    -- This filter is the magic that eliminates the overlaps
    WHERE COALESCE(s-(se-s)-1, (se-e)-e) = 0
),
C4 AS
(
    -- Grouping by grpnm restores the records to only non-overlapped intervals
    SELECT 
		SegmentStartTime=MIN(ts)
		, SegmentEndTime=MAX(ts)
    FROM C3
    GROUP BY grpnm
),
-- C5 to C7 merge contiguous segments into one segment.
C5 AS
(
	select SegmentStartTime, SegmentEndTime,
	case
		when SegmentStartTime = lag(SegmentEndTime) over(order by SegmentStartTime, SegmentEndTime)
		then 0 else 1
	end grp_start
	from C4
)
, 
C6 AS
(
	select SegmentStartTime, SegmentEndTime,
	sum(grp_start) over(order by SegmentStartTime, SegmentEndTime) grp
	from C5
),
C7 AS
(
	select min(SegmentStartTime) SegmentStartTime,
	max(SegmentEndTime) SegmentEndTime
	from C6
	group by grp
)

select @SecondsPlayed = sum(SegmentEndTime - SegmentStartTime)
from C7 


-- Update the MediaResourceActivity with the two calculated values. Set seconds to zero if still null.

Update mra
set mra.SecondsPlayed = Coalesce(@SecondsPlayed, 0),
	mra.PercentComplete = @PercentComplete,
	mra.AmendUserID = @AuditUserId,
	mra.AmendDate = @AmendDate
from activity.MediaResourceActivity mra 
where mra.Id = @MediaResourceActivityId


-- If the MediaResourceActivity.PercentComplete value is now 100%, update the end ResourceActivity record to 'Completed' status.

IF @PercentComplete = 100
	UPDATE raEnd 
	SET ActivityStatusId = 3, /* Completed */
		AmendUserID = @AuditUserId,
		AmendDate = @AmendDate
	FROM [activity].[MediaResourceActivity] mra 
		LEFT JOIN [activity].[ResourceActivity] raStart ON raStart.Id = mra.ResourceActivityId
		LEFT JOIN [activity].[ResourceActivity] raEnd ON raStart.Id = raEnd.LaunchResourceActivityId
	WHERE mra.Id = @MediaResourceActivityId

END