-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergetermsAndConditions]
    @termsAndConditionsList dbo.TermsAndConditions READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if termsAndConditionsId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[termsAndConditionsTBL] ON;

    MERGE [elfh].[termsAndConditionsTBL] AS target
    USING @termsAndConditionsList AS source
    ON target.termsAndConditionsId = source.termsAndConditionsId

    WHEN MATCHED THEN
        UPDATE SET 
              createdDate   = source.createdDate,
              description   = source.description,
              details       = source.details,
              tenantId      = source.tenantId,
              active        = source.active,
              reportable    = source.reportable,
              deleted       = source.deleted,
              amendUserID   = source.amendUserID,
              amendDate     = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              termsAndConditionsId,
              createdDate,
              description,
              details,
              tenantId,
              active,
              reportable,
              deleted,
              amendUserID,
              amendDate
        )
        VALUES (
              source.termsAndConditionsId,
              source.createdDate,
              source.description,
              source.details,
              source.tenantId,
              source.active,
              source.reportable,
              source.deleted,
              source.amendUserID,
              source.amendDate
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[termsAndConditionsTBL] OFF;
END
GO
