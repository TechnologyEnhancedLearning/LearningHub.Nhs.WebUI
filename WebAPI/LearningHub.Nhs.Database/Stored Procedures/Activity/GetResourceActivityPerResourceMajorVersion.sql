
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
    @ResourceIds dbo.IntIdsTableList READONLY,
    @UserId INT
AS
BEGIN

   SET NOCOUNT ON;

   WITH RankedActivities AS
    (
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
            ars.[AmendDate],

            ROW_NUMBER() OVER
            (
                PARTITION BY
                    ars.ResourceId,
                    ars.UserId,
                    ars.MajorVersion

                ORDER BY
                    CASE ars.ActivityStatusId
                        WHEN 5 THEN 1 -- Passed
                        WHEN 3 THEN 2 -- Completed
                        WHEN 4 THEN 3 -- Failed
                        WHEN 7 THEN 4 -- Incomplete
                        ELSE 5
                    END,
                    ars.Id DESC
            ) AS RowNum

        FROM [activity].[resourceactivity] ars

        INNER JOIN @ResourceIds r
            ON r.Id = ars.ResourceId

        WHERE ars.UserId = @UserId
          AND ars.Deleted = 0
          AND ars.ActivityStatusId NOT IN (1, 6, 2)
    )

    SELECT
        [Id],
        [UserId],
        [LaunchResourceActivityId],
        [ResourceId],
        [ResourceVersionId],
        [MajorVersion],
        [MinorVersion],
        [NodePathId],
        [ActivityStatusId],
        [ActivityStart],
        [ActivityEnd],
        [DurationSeconds],
        [Score],
        [Deleted],
        [CreateUserID],
        [CreateDate],
        [AmendUserID],
        [AmendDate]
    FROM RankedActivities
    WHERE RowNum = 1
    ORDER BY MajorVersion DESC;
END;


