-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserHistory]
    @userHistoryList dbo.UserHistory READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if userHistoryId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[userHistoryTBL] ON;

    MERGE [elfh].[userHistoryTBL] AS target
    USING @userHistoryList AS source
    ON target.[userHistoryId] = source.[userHistoryId]

    WHEN MATCHED THEN
        UPDATE SET
            [userHistoryTypeId] = source.[userHistoryTypeId],
            [userId] = source.[userId],
            [createdDate] = source.[createdDate],
            [tenantId] = source.[tenantId]

    WHEN NOT MATCHED THEN
        INSERT (
            [userHistoryId],
            [userHistoryTypeId],
            [userId],
            [createdDate],
            [tenantId]
        )
        VALUES (
            source.[userHistoryId],
            source.[userHistoryTypeId],
            source.[userId],
            source.[createdDate],
            source.[tenantId]
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userHistoryTBL] OFF;
END
GO
