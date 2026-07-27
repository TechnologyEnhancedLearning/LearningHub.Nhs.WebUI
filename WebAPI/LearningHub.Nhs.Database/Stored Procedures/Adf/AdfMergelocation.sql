-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergelocation]
    @locationList dbo.Location READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;
    MERGE [elfh].[locationTBL] AS target
    USING @locationList AS source
    ON target.locationId = source.locationId

    WHEN MATCHED THEN
        UPDATE SET 
              locationCode            = source.locationCode,
              locationName            = source.locationName,
              locationSubName         = source.locationSubName,
              locationTypeId          = source.locationTypeId,
              address1                = source.address1,
              address2                = source.address2,
              address3                = source.address3,
              address4                = source.address4,
              town                    = source.town,
              county                  = source.county,
              postCode                = source.postCode,
              telephone               = source.telephone,
              acute                   = source.acute,
              ambulance               = source.ambulance,
              mental                  = source.mental,
              care                    = source.care,
              mainHosp                = source.mainHosp,
              nhsCode                 = source.nhsCode,
              parentId                = source.parentId,
              dataSource              = source.dataSource,
              active                  = source.active,
              importExclusion         = source.importExclusion,
              depth                   = source.depth,
              lineage                 = source.lineage,
              created                 = source.created,
              updated                 = source.updated,
              archivedDate            = source.archivedDate,
              countryId               = source.countryId,
              iguId                   = source.iguId,
              letbId                  = source.letbId,
              ccgId                   = source.ccgId,
              healthServiceId         = source.healthServiceId,
              healthBoardId           = source.healthBoardId,
              primaryTrustId          = source.primaryTrustId,
              secondaryTrustId        = source.secondaryTrustId,
              islandId                = source.islandId,
              otherNHSOrganisationId  = source.otherNHSOrganisationId

    WHEN NOT MATCHED THEN
        INSERT (
              locationId,
              locationCode,
              locationName,
              locationSubName,
              locationTypeId,
              address1,
              address2,
              address3,
              address4,
              town,
              county,
              postCode,
              telephone,
              acute,
              ambulance,
              mental,
              care,
              mainHosp,
              nhsCode,
              parentId,
              dataSource,
              active,
              importExclusion,
              depth,
              lineage,
              created,
              updated,
              archivedDate,
              countryId,
              iguId,
              letbId,
              ccgId,
              healthServiceId,
              healthBoardId,
              primaryTrustId,
              secondaryTrustId,
              islandId,
              otherNHSOrganisationId
        )
        VALUES (
              source.locationId,
              source.locationCode,
              source.locationName,
              source.locationSubName,
              source.locationTypeId,
              source.address1,
              source.address2,
              source.address3,
              source.address4,
              source.town,
              source.county,
              source.postCode,
              source.telephone,
              source.acute,
              source.ambulance,
              source.mental,
              source.care,
              source.mainHosp,
              source.nhsCode,
              source.parentId,
              source.dataSource,
              source.active,
              source.importExclusion,
              source.depth,
              source.lineage,
              source.created,
              source.updated,
              source.archivedDate,
              source.countryId,
              source.iguId,
              source.letbId,
              source.ccgId,
              source.healthServiceId,
              source.healthBoardId,
              source.primaryTrustId,
              source.secondaryTrustId,
              source.islandId,
              source.otherNHSOrganisationId
        );

END
GO
