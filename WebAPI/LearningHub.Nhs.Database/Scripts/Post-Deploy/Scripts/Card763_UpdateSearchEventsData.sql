-- Change Event Type to 'SearchCatalogue' Event where eventType was logged as 1 and parent id is 0
UPDATE [analytics].[Event] SET EventTypeId = 14 WHERE EventTypeId = 1 AND createDate >= '2021-05-06 14:23:22.1487137 +00:00' AND GroupId IS NULL
GO

-- Change Event Type to 'Basic Search' where eventType was logged as 'Search Filter which value equals 3' and parent id is 0
UPDATE 
	[analytics].[Event] 
SET 
	EventTypeId = 1
FROM 
	[analytics].[Event] AS EventsData
INNER JOIN
 (
	SELECT 
		Id
	FROM  
		[analytics].[Event]
	WHERE 
		EventTypeId = 3 AND ParentId = 0 AND GroupId IS NULL
	
 ) FilteredData

 ON	
	EventsData.Id = FilteredData.Id
GO
	
-- assign group id where group id is null and event type is basic search or searchwithincatalogue
UPDATE 
	[analytics].[Event] 
SET 
	GroupId = FilteredData.GroupId 
FROM 
	[analytics].[Event] AS EventsData
INNER JOIN
 (
	SELECT 
		Id, NEWID() as GroupId 
	FROM  
		[analytics].[Event]
	WHERE 
		(EventTypeId = 1 OR EventTypeId = 12) AND ParentId = 0 AND GroupId IS NULL
	
 ) FilteredData

 ON	
	EventsData.Id = FilteredData.Id		
GO

-- assign same group Id to childrens events generated from basic search (sorting, filtering, load more, submit feedback, launch resource), where parent is not null  

UPDATE 
	[analytics].[Event] 
SET 
	GroupId = BasicSearchParentData.GroupId 
FROM
	[analytics].[Event] AS EventsData
INNER JOIN 
 (
	SELECT 
		Id, GroupId 
	FROM 
		[analytics].[Event]
	WHERE 
		(EventTypeId = 1 OR EventTypeId = 12) AND ParentId = 0 AND GroupId IS NOT NULL
 ) BasicSearchParentData

ON
	EventsData.ParentId = BasicSearchParentData.Id
AND 
	EventsData.GroupId IS NULL

GO
	
-- update group Id on catalogue search data to link with the basic search data, logic search text and create user id is same and event logging happened within 400 milliseconds
UPDATE 
	[analytics].[Event] 
SET 
	GroupId = BasicSearchData.GroupId 
FROM
	[analytics].[Event] AS EventsData
INNER JOIN 
 (
	SELECT 
		*
	FROM 
		[analytics].[Event]
	WHERE 
		EventTypeId = 1 AND ParentId = 0	
 ) BasicSearchData
 ON
	EventsData.CreateUserId = BasicSearchData.CreateUserId
 AND
	JSON_VALUE(EventsData.JsonData,'$.SearchText') = JSON_VALUE(BasicSearchData.JsonData,'$.SearchText') 
 AND
	DateDiff_big(ms, EventsData.createDate, BasicSearchData.CreateDate) BETWEEN -400 AND 400
 WHERE
	EventsData.EventTypeId = 14 and EventsData.ParentId = 0 AND EventsData.GroupId IS NULL

GO