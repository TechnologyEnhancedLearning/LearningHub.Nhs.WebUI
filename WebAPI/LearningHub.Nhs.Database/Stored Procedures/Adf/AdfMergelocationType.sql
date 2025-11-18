-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergelocationType]
    @locationTypeList dbo.LocationType READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable explicit identity insert (if locationTypeID is an IDENTITY column)
    SET IDENTITY_INSERT [elfh].[locationTypeTBL] ON;

    MERGE [elfh].[locationTypeTBL] AS target
    USING @locationTypeList AS source
    ON target.locationTypeID = source.locationTypeID

    WHEN MATCHED THEN
        UPDATE SET 
              locationType  = source.locationType,
              countryId     = source.countryId,
              healthService = source.healthService,
              healthBoard   = source.healthBoard,
              primaryTrust  = source.primaryTrust,
              secondaryTrust = source.secondaryTrust

    WHEN NOT MATCHED THEN
        INSERT (
              locationTypeID,
              locationType,
              countryId,
              healthService,
              healthBoard,
              primaryTrust,
              secondaryTrust
        )
        VALUES (
              source.locationTypeID,
              source.locationType,
              source.countryId,
              source.healthService,
              source.healthBoard,
              source.primaryTrust,
              source.secondaryTrust
        );

    -- Disable identity insert after operation
    SET IDENTITY_INSERT [elfh].[locationTypeTBL] OFF;
END
GO
