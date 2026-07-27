-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeregion]
    @regionList dbo.Region READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [elfh].[regionTBL] AS target
    USING @regionList AS source
    ON target.regionId = source.regionId

    WHEN MATCHED THEN
        UPDATE SET 
              regionName   = source.regionName,
              displayOrder = source.displayOrder,
              deleted      = source.deleted,
              amendUserID  = source.amendUserID,
              amendDate    = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              regionId,
              regionName,
              displayOrder,
              deleted,
              amendUserID,
              amendDate
        )
        VALUES (
              source.regionId,
              source.regionName,
              source.displayOrder,
              source.deleted,
              source.amendUserID,
              source.amendDate
        );

END
GO
