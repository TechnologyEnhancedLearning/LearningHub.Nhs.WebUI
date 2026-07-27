-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
create PROCEDURE [dbo].[AdfMergecountry]
    @countryList dbo.Country READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;

    SET IDENTITY_INSERT [elfh].[countryTBL] ON;

    MERGE [elfh].[countryTBL] AS target
    USING @countryList AS source
    ON target.countryId = source.countryId

    WHEN MATCHED THEN
        UPDATE SET 
              countryName  = source.countryName,
              alpha2       = source.alpha2,
              alpha3       = source.alpha3,
              numeric      = source.numeric,
              EUVatRate    = source.EUVatRate,
              displayOrder = source.displayOrder,
              deleted      = source.deleted,
              amendUserId  = source.amendUserId,
              amendDate    = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              countryId,
              countryName,
              alpha2,
              alpha3,
              numeric,
              EUVatRate,
              displayOrder,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.countryId,
              source.countryName,
              source.alpha2,
              source.alpha3,
              source.numeric,
              source.EUVatRate,
              source.displayOrder,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

    SET IDENTITY_INSERT [elfh].[countryTBL] OFF;
END
GO
