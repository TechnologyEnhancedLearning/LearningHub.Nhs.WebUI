-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserRoleUpgrade]
    @userRoleUpgradeList dbo.UserRoleUpgrade READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if userRoleUpgradeId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[userRoleUpgradeTBL] ON;

    MERGE [elfh].[userRoleUpgradeTBL] AS target
    USING @userRoleUpgradeList AS source
    ON target.[userRoleUpgradeId] = source.[userRoleUpgradeId]

    WHEN MATCHED THEN
        UPDATE SET
            [userId] = source.[userId],
            [emailAddress] = source.[emailAddress],
            [upgradeDate] = source.[upgradeDate],
            [deleted] = source.[deleted],
            [createUserId] = source.[createUserId],
            [createDate] = source.[createDate],
            [amendUserId] = source.[amendUserId],
            [amendDate] = source.[amendDate],
            [userHistoryTypeId] = source.[userHistoryTypeId]

    WHEN NOT MATCHED THEN
        INSERT (
            [userRoleUpgradeId],
            [userId],
            [emailAddress],
            [upgradeDate],
            [deleted],
            [createUserId],
            [createDate],
            [amendUserId],
            [amendDate],
            [userHistoryTypeId]
        )
        VALUES (
            source.[userRoleUpgradeId],
            source.[userId],
            source.[emailAddress],
            source.[upgradeDate],
            source.[deleted],
            source.[createUserId],
            source.[createDate],
            source.[amendUserId],
            source.[amendDate],
            source.[userHistoryTypeId]
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userRoleUpgradeTBL] OFF;
END
GO
