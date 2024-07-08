
-------------------------------------------------------------------------------
-- Author       Phil T
-- Created      04-07-24
-- Purpose      Return resource activity for each major version for user


-- Description
/*
		This procedure returns a single entry per resource Id, selecting the most important one for that major version.
		This is so users can still have a resourceActivity history following a majorVersion change

		UserIds is nullable so that general resource activity can be searched for
		ResourceIds is nullable so that all a users history can be searched for

		When determining the resourceActivity statusDescription in the front end resourceTypeId is also required for changing completed statuses to resourceType specific ones

		Currently if multiple rows meet the case criteria we retrieve the one with the highest Id which is also expected to be the ActivityEnd part of the activityStatus pair.

*/
-- Future Considerations
/*
		Because the activityResource should come in pairs one with ActivityStart populated and one with ActivityEnd populated 
		it could be desireable to join via LaunchResourceActivityId and coalesce the data in future.
		Or/And coalesce where the case returns multiple rows.

*/
-- Notes
   --  resourceId is used not originalResourceId

	
-------------------------------------------------------------------------------

-- Create the new stored procedure
CREATE PROCEDURE [activity].[GetResourceActivityPerResourceMajorVersion]
    @ResourceIds VARCHAR(MAX) = NULL,
    @UserIds VARCHAR(MAX) = NULL
AS
BEGIN

  -- Split the comma-separated list into a table of integers
    DECLARE @ResourceIdTable TABLE (ResourceId INT);

    IF @ResourceIds IS NOT NULL AND @ResourceIds <> ''
    BEGIN
        INSERT INTO @ResourceIdTable (ResourceId)
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@ResourceIds, ',');
    END;

    -- Split the comma-separated list of UserIds into a table
    DECLARE @UserIdTable TABLE (UserId INT);

    IF @UserIds IS NOT NULL AND @UserIds <> ''
    BEGIN
        INSERT INTO @UserIdTable (UserId)
        SELECT CAST(value AS INT)
        FROM STRING_SPLIT(@UserIds, ',');
    END;

    WITH FilteredResourceActivities AS (
        SELECT 
            ars.[Id],
            ars.[UserId],
            ars.[LaunchResourceActivityId],
            ars.[ResourceId],
            ars.[ResourceVersionId],
            ars.[MajorVersion],
            ars.[MinorVersion],
            ars.[NodePathId],
            ars.[ActivityStatusId],
            ars.[ActivityStart],
            ars.[ActivityEnd],
            ars.[DurationSeconds],
            ars.[Score],
            ars.[Deleted],
            ars.[CreateUserID],
            ars.[CreateDate],
            ars.[AmendUserID],
            ars.[AmendDate]
        FROM 
            [activity].[resourceactivity] ars
        WHERE 
            (@UserIds IS NULL OR ars.userId IN (SELECT UserId FROM @UserIdTable) OR NOT EXISTS (SELECT 1 FROM @UserIdTable))
            AND (@ResourceIds IS NULL OR @ResourceIds = '' OR ars.resourceId IN (SELECT ResourceId FROM @ResourceIdTable) OR NOT EXISTS (SELECT 1 FROM @ResourceIdTable))
            AND ars.Deleted = 0
            AND ars.ActivityStatusId NOT IN (1, 6, 2) -- These Ids are not in use - Launched, Downloaded, In Progress (stored as completed and incomplete then renamed in the application)
    ),
    RankedActivities AS (
        SELECT 
            ra.[Id],
            ra.[UserId],
            ra.[LaunchResourceActivityId],
            ra.[ResourceId],
            ra.[ResourceVersionId],
            ra.[MajorVersion],
            ra.[MinorVersion],
            ra.[NodePathId],
            ra.[ActivityStatusId],
            ra.[ActivityStart],
            ra.[ActivityEnd],
            ra.[DurationSeconds],
            ra.[Score],
            ra.[Deleted],
            ra.[CreateUserID],
            ra.[CreateDate],
            ra.[AmendUserID],
            ra.[AmendDate],
            ROW_NUMBER() OVER (
                PARTITION BY resourceId, userId, MajorVersion 
                ORDER BY 
                    CASE 
                        WHEN ActivityStatusId = 5 THEN 1    -- Passed
                        WHEN ActivityStatusId = 3 THEN 2    -- Completed
                        WHEN ActivityStatusId = 4 THEN 3    -- Failed
                        WHEN ActivityStatusId = 7 THEN 4    -- Incomplete
                        ELSE 5 -- shouldn't be any
                    END,
					Id DESC -- we have two entries per interacting with a resource the start and the end, we are just returning the last entry made
					-- there is the option of instead coalescing LaunchResourceActivityId, ActivityStart,ActivityEnd potentially via joining LaunchResourceActivityId and UserId
            ) AS RowNum
        FROM 
            FilteredResourceActivities ra
    )
    SELECT 
        ra.[Id],
        ra.[UserId],
        ra.[LaunchResourceActivityId],
        ra.[ResourceId],
        ra.[ResourceVersionId],
        ra.[MajorVersion],
        ra.[MinorVersion],
        ra.[NodePathId],
        ra.[ActivityStatusId],
        ra.[ActivityStart],
        ra.[ActivityEnd],
        ra.[DurationSeconds],
        ra.[Score],
        ra.[Deleted],
        ra.[CreateUserID],
        ra.[CreateDate],
        ra.[AmendUserID],
        ra.[AmendDate]
    FROM 
        RankedActivities ra
    WHERE 
        RowNum = 1
	order by MajorVersion desc;
END;
GO


