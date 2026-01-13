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

  MERGE [elfh].[userAdminLocationTBL] AS target
    USING @UserAdminLocationList AS source
        ON target.[userId] = source.[userId]
       AND target.[adminLocationId] = source.[adminLocationId]  -- composite key match

    WHEN MATCHED THEN
        UPDATE SET
            target.[deleted]       = source.[deleted],
            target.[amendUserId]   = source.[amendUserId],
            target.[amendDate]     = source.[amendDate],
            target.[createdUserId] = source.[createdUserId],
            target.[createdDate]   = source.[createdDate]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [userId],
            [adminLocationId],
            [deleted],
            [amendUserId],
            [amendDate],
            [createdUserId],
            [createdDate]
        )
        VALUES (
            source.[userId],
            source.[adminLocationId],
            source.[deleted],
            source.[amendUserId],
            source.[amendDate],
            source.[createdUserId],
            source.[createdDate]
        );

END
GO
