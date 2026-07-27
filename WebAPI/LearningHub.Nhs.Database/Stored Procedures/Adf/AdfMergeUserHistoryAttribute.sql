-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [AdfMergeUserHistoryAttribute]
    @UserHistoryAttributeList dbo.UserHistoryAttributeType READONLY
AS
BEGIN
    SET NOCOUNT ON;

     ALTER TABLE [elfh].[userHistoryAttributeTBL] NOCHECK CONSTRAINT ALL;
	SET IDENTITY_INSERT [elfh].[userHistoryAttributeTBL] ON;
    MERGE [elfh].[userHistoryAttributeTBL] AS target
    USING @UserHistoryAttributeList AS source
        ON target.[userHistoryAttributeId] = source.[userHistoryAttributeId]

    WHEN MATCHED THEN
        UPDATE SET
            target.[userHistoryId] = source.[userHistoryId],
            target.[attributeId] = source.[attributeId],
            target.[intValue] = source.[intValue],
            target.[textValue] = source.[textValue],
            target.[booleanValue] = source.[booleanValue],
            target.[dateValue] = source.[dateValue],
            target.[deleted] = source.[deleted],
            target.[amendUserId] = source.[amendUserId],
            target.[amendDate] = source.[amendDate]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [userHistoryAttributeId],
            [userHistoryId],
            [attributeId],
            [intValue],
            [textValue],
            [booleanValue],
            [dateValue],
            [deleted],
            [amendUserId],
            [amendDate]
        )
        VALUES (
            source.[userHistoryAttributeId],
            source.[userHistoryId],
            source.[attributeId],
            source.[intValue],
            source.[textValue],
            source.[booleanValue],
            source.[dateValue],
            source.[deleted],
            source.[amendUserId],
            source.[amendDate]
        );
			SET IDENTITY_INSERT [elfh].[userHistoryAttributeTBL] OFF;
     ALTER TABLE [elfh].[userHistoryAttributeTBL] CHECK CONSTRAINT ALL;
END
GO
