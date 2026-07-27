-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserAttribute]
    @userAttributeList dbo.UserAttribute READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;
    SET IDENTITY_INSERT [elfh].[userAttributeTBL] ON;

    MERGE [elfh].[userAttributeTBL] AS target
    USING @userAttributeList AS source
    ON target.userAttributeId = source.userAttributeId

    WHEN MATCHED THEN
        UPDATE SET 
              userId       = source.userId,
              attributeId  = source.attributeId,
              intValue     = source.intValue,
              textValue    = source.textValue,
              booleanValue = source.booleanValue,
              dateValue    = source.dateValue,
              deleted      = source.deleted,
              amendUserId  = source.amendUserId,
              amendDate    = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              userAttributeId,
              userId,
              attributeId,
              intValue,
              textValue,
              booleanValue,
              dateValue,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.userAttributeId,
              source.userId,
              source.attributeId,
              source.intValue,
              source.textValue,
              source.booleanValue,
              source.dateValue,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userAttributeTBL] OFF;
END
GO
