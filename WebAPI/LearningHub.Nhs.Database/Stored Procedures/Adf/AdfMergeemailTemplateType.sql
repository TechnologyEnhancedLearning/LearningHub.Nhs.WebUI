-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE  PROCEDURE [dbo].[AdfMergeemailTemplateType]
    @emailTemplateTypeList dbo.EmailTemplateType READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [elfh].[emailTemplateTypeTBL] AS target
    USING @emailTemplateTypeList AS source
    ON target.emailTemplateTypeId = source.emailTemplateTypeId

    WHEN MATCHED THEN
        UPDATE SET 
              emailTemplateTypeName = source.emailTemplateTypeName,
              availableTags          = source.availableTags,
              deleted                = source.deleted,
              amendUserID            = source.amendUserID,
              amendDate              = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              emailTemplateTypeId,
              emailTemplateTypeName,
              availableTags,
              deleted,
              amendUserID,
              amendDate
        )
        VALUES (
              source.emailTemplateTypeId,
              source.emailTemplateTypeName,
              source.availableTags,
              source.deleted,
              source.amendUserID,
              source.amendDate
        );

END
GO
