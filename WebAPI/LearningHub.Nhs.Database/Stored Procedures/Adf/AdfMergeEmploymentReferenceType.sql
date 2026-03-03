-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeEmploymentReferenceType]
    @EmploymentReferenceType dbo.EmploymentReferenceType READONLY
AS
BEGIN
    SET NOCOUNT ON;


    MERGE [ELFH].[employmentReferenceTypeTBL] AS target
    USING @EmploymentReferenceType AS source
        ON target.[EmploymentReferenceTypeId] = source.[EmploymentReferenceTypeId]

    WHEN MATCHED THEN
        UPDATE SET 
            target.[Title]     = source.[Title],
            target.[RefAccess] = source.[RefAccess]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [EmploymentReferenceTypeId],
            [Title],
            [RefAccess]
        )
        VALUES (
            source.[EmploymentReferenceTypeId],
            source.[Title],
            source.[RefAccess]
        );

END
GO
