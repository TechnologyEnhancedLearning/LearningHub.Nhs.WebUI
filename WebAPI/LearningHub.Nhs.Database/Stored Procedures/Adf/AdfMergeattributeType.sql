-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeattributeType]
    @attributeTypeList dbo.AttributeType READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;


    MERGE [elfh].[attributeTypeTBL] AS target
    USING @attributeTypeList AS source
    ON target.attributeTypeId = source.attributeTypeId

    WHEN MATCHED THEN
        UPDATE SET 
              attributeTypeName = source.attributeTypeName,
              deleted           = source.deleted,
              amendUserId       = source.amendUserId,
              amendDate         = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              attributeTypeId,
              attributeTypeName,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.attributeTypeId,
              source.attributeTypeName,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

END
GO
