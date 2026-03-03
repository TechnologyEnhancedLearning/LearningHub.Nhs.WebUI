-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergesystemSetting]
    @systemSettingList dbo.SystemSetting READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [ELFH].[systemSettingTBL] AS target
    USING @systemSettingList AS source
    ON target.systemSettingId = source.systemSettingId

    WHEN MATCHED THEN
        UPDATE SET 
              systemSettingName = source.systemSettingName,
              intValue          = source.intValue,
              textValue         = source.textValue,
              booleanValue      = source.booleanValue,
              dateValue         = source.dateValue,
              deleted           = source.deleted,
              amendUserId       = source.amendUserId,
              amendDate         = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              systemSettingId,
              systemSettingName,
              intValue,
              textValue,
              booleanValue,
              dateValue,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.systemSettingId,
              source.systemSettingName,
              source.intValue,
              source.textValue,
              source.booleanValue,
              source.dateValue,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

END
GO
