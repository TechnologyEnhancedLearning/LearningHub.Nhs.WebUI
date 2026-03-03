-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergespecialty]
    @specialtyList dbo.Specialty READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[specialtyTBL] ON;
    MERGE [elfh].[specialtyTBL] AS target
    USING @specialtyList AS source
    ON target.specialtyId = source.specialtyId

    WHEN MATCHED THEN
        UPDATE SET 
              specialtyName = source.specialtyName
            , displayOrder  = source.displayOrder
            , deleted       = source.deleted
            , amendUserID   = source.amendUserID
            , amendDate     = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              specialtyId
            , specialtyName
            , displayOrder
            , deleted
            , amendUserID
            , amendDate
        )
        VALUES (
              source.specialtyId
            , source.specialtyName
            , source.displayOrder
            , source.deleted
            , source.amendUserID
            , source.amendDate
        );
		SET IDENTITY_INSERT [elfh].[specialtyTBL] OFF;
END
GO
