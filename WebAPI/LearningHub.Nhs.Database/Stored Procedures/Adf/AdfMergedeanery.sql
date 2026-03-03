-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergedeanery]
    @deaneryList dbo.Deanery READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;
		SET IDENTITY_INSERT [elfh].[deaneryTBL] ON;
    MERGE [elfh].[deaneryTBL] AS target
    USING @deaneryList AS source
    ON target.deaneryId = source.deaneryId

    WHEN MATCHED THEN
        UPDATE SET 
              deaneryName  = source.deaneryName,
              displayOrder = source.displayOrder,
              deleted      = source.deleted,
              amendUserID  = source.amendUserID,
              amendDate    = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              deaneryId,
              deaneryName,
              displayOrder,
              deleted,
              amendUserID,
              amendDate
        )
        VALUES (
              source.deaneryId,
              source.deaneryName,
              source.displayOrder,
              source.deleted,
              source.amendUserID,
              source.amendDate
        );
		SET IDENTITY_INSERT [elfh].[deaneryTBL] OFF;
END
GO
