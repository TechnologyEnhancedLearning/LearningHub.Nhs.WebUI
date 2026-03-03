-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserTermsAndConditions]
    @userTermsAndConditionsList dbo.UserTermsAndConditions READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if userTermsAndConditionsId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[userTermsAndConditionsTBL] ON;
    MERGE [elfh].[userTermsAndConditionsTBL] AS target
    USING @userTermsAndConditionsList AS source
    ON target.[userTermsAndConditionsId] = source.[userTermsAndConditionsId]

    WHEN MATCHED THEN
        UPDATE SET
            [termsAndConditionsId] = source.[termsAndConditionsId],
            [userId] = source.[userId],
            [acceptanceDate] = source.[acceptanceDate],
            [deleted] = source.[deleted],
            [amendUserID] = source.[amendUserID],
            [amendDate] = source.[amendDate]

    WHEN NOT MATCHED THEN
        INSERT (
            [userTermsAndConditionsId],
            [termsAndConditionsId],
            [userId],
            [acceptanceDate],
            [deleted],
            [amendUserID],
            [amendDate]
        )
        VALUES (
            source.[userTermsAndConditionsId],
            source.[termsAndConditionsId],
            source.[userId],
            source.[acceptanceDate],
            source.[deleted],
            source.[amendUserID],
            source.[amendDate]
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userTermsAndConditionsTBL] OFF;
END
GO
