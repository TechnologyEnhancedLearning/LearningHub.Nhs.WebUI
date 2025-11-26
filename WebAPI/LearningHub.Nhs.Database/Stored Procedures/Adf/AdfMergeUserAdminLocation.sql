-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserAdminLocation]
    @UserAdminLocationList dbo.UserAdminLocationType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Deduplicate source first
    ;WITH DedupedSource AS (
        SELECT *,
            ROW_NUMBER() OVER (
                PARTITION BY userId, adminLocationId, deleted
                ORDER BY amendDate DESC, createdDate DESC
            ) AS rn
        FROM @UserAdminLocationList
    ),
    CleanSource AS (
        SELECT * FROM DedupedSource WHERE rn = 1
    )

    MERGE elfh.userAdminLocationTBL AS target
    USING CleanSource AS source
        ON  target.userId          = source.userId
        AND target.adminLocationId = source.adminLocationId
        AND target.deleted         = source.deleted     -- IMPORTANT!

    WHEN MATCHED THEN
        UPDATE SET
            target.amendUserId   = source.amendUserId,
            target.amendDate     = source.amendDate,
            target.createdUserId = source.createdUserId,
            target.createdDate   = source.createdDate

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            userId,
            adminLocationId,
            deleted,
            amendUserId,
            amendDate,
            createdUserId,
            createdDate
        )
        VALUES (
            source.userId,
            source.adminLocationId,
            source.deleted,
            source.amendUserId,
            source.amendDate,
            source.createdUserId,
            source.createdDate
        );
END
GO
