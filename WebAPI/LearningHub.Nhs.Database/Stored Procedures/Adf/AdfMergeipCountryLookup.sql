-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeipCountryLookup]
    @ipCountryLookupList dbo.IPCountryLookup READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [elfh].[ipCountryLookupTBL] AS target
    USING @ipCountryLookupList AS source
    ON target.fromInt = source.fromInt AND target.toInt = source.toInt

    WHEN MATCHED THEN
        UPDATE SET 
              fromIP  = source.fromIP,
              toIP    = source.toIP,
              country = source.country

    WHEN NOT MATCHED THEN
        INSERT (
              fromIP,
              toIP,
              country,
              fromInt,
              toInt
        )
        VALUES (
              source.fromIP,
              source.toIP,
              source.country,
              source.fromInt,
              source.toInt
        );

END
GO
