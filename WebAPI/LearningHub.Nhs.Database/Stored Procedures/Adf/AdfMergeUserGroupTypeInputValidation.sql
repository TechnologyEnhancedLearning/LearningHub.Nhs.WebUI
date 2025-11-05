-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserGroupTypeInputValidation]
    @userGroupTypeInputValidationList dbo.UserGroupTypeInputValidation READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if userGroupTypeInputValidationId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[userGroupTypeInputValidationTBL] ON;

    MERGE [elfh].[userGroupTypeInputValidationTBL] AS target
    USING @userGroupTypeInputValidationList AS source
    ON target.userGroupTypeInputValidationId = source.userGroupTypeInputValidationId

    WHEN MATCHED THEN
        UPDATE SET 
              userGroupId       = source.userGroupId,
              userGroupTypePrefix = source.userGroupTypePrefix,
              userGroupTypeId   = source.userGroupTypeId,
              validationTextValue = source.validationTextValue,
              validationMethod  = source.validationMethod,
              deleted           = source.deleted,
              amendUserId       = source.amendUserId,
              amendDate         = source.amendDate,
              createdUserId     = source.createdUserId,
              createdDate       = source.createdDate

    WHEN NOT MATCHED THEN
        INSERT (
              userGroupTypeInputValidationId,
              userGroupId,
              userGroupTypePrefix,
              userGroupTypeId,
              validationTextValue,
              validationMethod,
              deleted,
              amendUserId,
              amendDate,
              createdUserId,
              createdDate
        )
        VALUES (
              source.userGroupTypeInputValidationId,
              source.userGroupId,
              source.userGroupTypePrefix,
              source.userGroupTypeId,
              source.validationTextValue,
              source.validationMethod,
              source.deleted,
              source.amendUserId,
              source.amendDate,
              source.createdUserId,
              source.createdDate
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userGroupTypeInputValidationTBL] OFF;
END
GO
