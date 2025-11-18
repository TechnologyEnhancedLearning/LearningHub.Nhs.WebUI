-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeschool]
    @schoolList dbo.School READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[schoolTBL] ON;
    MERGE [elfh].[schoolTBL] AS target
    USING @schoolList AS source
    ON target.schoolId = source.schoolId

    WHEN MATCHED THEN
        UPDATE SET 
              deaneryId   = source.deaneryId,
              specialtyId = source.specialtyId,
              schoolName  = source.schoolName,
              displayOrder = source.displayOrder,
              deleted     = source.deleted,
              amendUserID = source.amendUserID,
              amendDate   = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              schoolId,
              deaneryId,
              specialtyId,
              schoolName,
              displayOrder,
              deleted,
              amendUserID,
              amendDate
        )
        VALUES (
              source.schoolId,
              source.deaneryId,
              source.specialtyId,
              source.schoolName,
              source.displayOrder,
              source.deleted,
              source.amendUserID,
              source.amendDate
        );
		SET IDENTITY_INSERT [elfh].[schoolTBL] OFF;
END
GO
