-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      25-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 25-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserGroupReporter]
    @UserGroupReporterList [dbo].[UserGroupReporterType] READONLY
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[userGroupReporterTBL] ON;
	ALTER TABLE elfh.userGroupReporterTBL NOCHECK CONSTRAINT FK_userGroupReporterTBL_userGroupTBL;
    MERGE [elfh].[userGroupReporterTBL] AS target
    USING @UserGroupReporterList AS source
        ON target.[userGroupReporterId] = source.[userGroupReporterId]

    WHEN MATCHED THEN
        UPDATE SET 
            target.[userId]        = source.[userId],
            target.[userGroupId]   = source.[userGroupId],
            target.[deleted]       = source.[deleted],
            target.[amendUserId]   = source.[amendUserId],
            target.[amendDate]     = source.[amendDate]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [userGroupReporterId],
            [userId],
            [userGroupId],
            [deleted],
            [amendUserId],
            [amendDate]
        )
        VALUES (
            source.[userGroupReporterId],
            source.[userId],
            source.[userGroupId],
            source.[deleted],
            source.[amendUserId],
            source.[amendDate]
        );
		SET IDENTITY_INSERT [elfh].[userGroupReporterTBL] OFF;
	   ALTER TABLE elfh.userGroupReporterTBL CHECK CONSTRAINT FK_userGroupReporterTBL_userGroupTBL;
END
GO

