-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [AdfMergeUserEmploymentReference]
    @UserEmploymentReferenceList dbo.UserEmploymentReferenceType READONLY
AS
BEGIN
    SET NOCOUNT ON;

	SET IDENTITY_INSERT [elfh].[userEmploymentReferenceTBL] ON;
    MERGE [elfh].[userEmploymentReferenceTBL] AS target
    USING @UserEmploymentReferenceList AS source
        ON target.[userEmploymentReferenceId] = source.[userEmploymentReferenceId]

    WHEN MATCHED THEN
        UPDATE SET
            target.[employmentReferenceTypeId] = source.[employmentReferenceTypeId],
            target.[userEmploymentId]          = source.[userEmploymentId],
            target.[referenceValue]            = source.[referenceValue],
            target.[deleted]                   = source.[deleted],
            target.[amendUserId]               = source.[amendUserId],
            target.[amendDate]                 = source.[amendDate]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [userEmploymentReferenceId],
            [employmentReferenceTypeId],
            [userEmploymentId],
            [referenceValue],
            [deleted],
            [amendUserId],
            [amendDate]
        )
        VALUES (
            source.[userEmploymentReferenceId],
            source.[employmentReferenceTypeId],
            source.[userEmploymentId],
            source.[referenceValue],
            source.[deleted],
            source.[amendUserId],
            source.[amendDate]
        );
		SET IDENTITY_INSERT [elfh].[userEmploymentReferenceTBL] OFF;
END
GO
