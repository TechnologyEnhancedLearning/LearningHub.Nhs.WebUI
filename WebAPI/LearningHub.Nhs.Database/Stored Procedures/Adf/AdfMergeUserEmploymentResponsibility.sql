-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [AdfMergeUserEmploymentResponsibility]
    @UserEmploymentResponsibilityList dbo.UserEmploymentResponsibilityType READONLY
AS
BEGIN
    SET NOCOUNT ON;

	SET IDENTITY_INSERT [elfh].[userEmploymentResponsibilityTBL] ON;
    MERGE [elfh].[userEmploymentResponsibilityTBL] AS target
    USING @UserEmploymentResponsibilityList AS source
        ON target.[userEmploymentResponsibilityId] = source.[userEmploymentResponsibilityId]

    WHEN MATCHED THEN
        UPDATE SET
            target.[userEmploymentId]          = source.[userEmploymentId],
            target.[additionalResponsibilityId] = source.[additionalResponsibilityId],
            target.[deleted]                   = source.[deleted],
            target.[amendUserId]               = source.[amendUserId]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [userEmploymentResponsibilityId],
            [userEmploymentId],
            [additionalResponsibilityId],
            [deleted],
            [amendUserId]
        )
        VALUES (
            source.[userEmploymentResponsibilityId],
            source.[userEmploymentId],
            source.[additionalResponsibilityId],
            source.[deleted],
            source.[amendUserId]
        );
		SET IDENTITY_INSERT [elfh].[userEmploymentResponsibilityTBL] OFF;
END
GO
