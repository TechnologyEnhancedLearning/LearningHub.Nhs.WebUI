-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserHistoryType]
    @userHistoryTypeList dbo.UserHistoryType READONLY
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [ELFH].[userHistoryTypeTBL] AS target
    USING @userHistoryTypeList AS source
    ON target.UserHistoryTypeId = source.UserHistoryTypeId

    WHEN MATCHED THEN
        UPDATE SET
            [Description] = source.[Description]

    WHEN NOT MATCHED THEN
        INSERT (
            [UserHistoryTypeId],
            [Description]
        )
        VALUES (
            source.[UserHistoryTypeId],
            source.[Description]
        );

END
GO
