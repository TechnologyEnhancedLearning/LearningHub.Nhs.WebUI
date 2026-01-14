-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-- 14-01-2026  Swapna           TD-6760: To handle duplicate records in the Sync table
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserAdminLocation]
    @UserAdminLocationList dbo.UserAdminLocationType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    --------------------------------------------------------------------
    -- 1. Remove deleted duplicates when an active row exists
    --------------------------------------------------------------------
    DELETE tgt
    FROM elfh.userAdminLocationTBL tgt
    INNER JOIN @UserAdminLocationList src
        ON  tgt.userId = src.userId
        AND tgt.adminLocationId = src.adminLocationId
    WHERE tgt.deleted = 1
      AND EXISTS (
            SELECT 1
            FROM elfh.userAdminLocationTBL a
            WHERE a.userId = src.userId
              AND a.adminLocationId = src.adminLocationId
              AND a.deleted = 0
      );

    --------------------------------------------------------------------
    -- 2. Update existing ACTIVE rows only
    --    (do NOT overwrite deleted rows)
    --------------------------------------------------------------------
    UPDATE tgt
    SET
        tgt.amendUserId   = src.amendUserId,
        tgt.amendDate     = src.amendDate,
        tgt.createdUserId = src.createdUserId,
        tgt.createdDate   = src.createdDate,
        tgt.deleted       = src.deleted
    FROM elfh.userAdminLocationTBL tgt
    INNER JOIN @UserAdminLocationList src
        ON  tgt.userId = src.userId
        AND tgt.adminLocationId = src.adminLocationId
    WHERE tgt.deleted = 0;

    --------------------------------------------------------------------
    -- 3. Insert rows that do not exist
    --    (both deleted = 0 and deleted = 1 allowed)
    --------------------------------------------------------------------
    INSERT INTO elfh.userAdminLocationTBL (
        userId,
        adminLocationId,
        deleted,
        amendUserId,
        amendDate,
        createdUserId,
        createdDate
    )
    SELECT
        src.userId,
        src.adminLocationId,
        src.deleted,
        src.amendUserId,
        src.amendDate,
        src.createdUserId,
        src.createdDate
    FROM @UserAdminLocationList src
    WHERE NOT EXISTS (
        SELECT 1
        FROM elfh.userAdminLocationTBL tgt WITH (UPDLOCK, HOLDLOCK)
        WHERE tgt.userId = src.userId
          AND tgt.adminLocationId = src.adminLocationId
          AND tgt.deleted = src.deleted
    );
END;
GO