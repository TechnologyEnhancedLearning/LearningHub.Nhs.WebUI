-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [AdfMergeUserReportingUser]
    @UserReportingUserList dbo.UserReportingUserType READONLY
AS
BEGIN
    SET NOCOUNT ON;

		SET IDENTITY_INSERT [elfh].[userReportingUserTBL] ON;
    MERGE [elfh].[userReportingUserTBL] AS target
    USING @UserReportingUserList AS source
        ON target.[userReportingUserId] = source.[userReportingUserId]

    WHEN MATCHED THEN
        UPDATE SET
            target.[userId] = source.[userId],
            target.[reportingUserId] = source.[reportingUserId],
            target.[reportable] = source.[reportable],
            target.[Deleted] = source.[Deleted],
            target.[AmendUserID] = source.[AmendUserID],
            target.[AmendDate] = source.[AmendDate]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [userReportingUserId],
            [userId],
            [reportingUserId],
            [reportable],
            [Deleted],
            [AmendUserID],
            [AmendDate]
        )
        VALUES (
            source.[userReportingUserId],
            source.[userId],
            source.[reportingUserId],
            source.[reportable],
            source.[Deleted],
            source.[AmendUserID],
            source.[AmendDate]
        );
		SET IDENTITY_INSERT [elfh].[userReportingUserTBL] OFF;
    -- Optionally re-enable constraints
END
GO
