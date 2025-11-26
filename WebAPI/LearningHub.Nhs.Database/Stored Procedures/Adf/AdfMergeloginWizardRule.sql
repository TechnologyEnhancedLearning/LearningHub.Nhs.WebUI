-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeloginWizardRule]
    @loginWizardRuleList dbo.LoginWizardRule READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;
    MERGE [elfh].[loginWizardRuleTBL] AS target
    USING @loginWizardRuleList AS source
    ON target.loginWizardRuleId = source.loginWizardRuleId

    WHEN MATCHED THEN
        UPDATE SET 
              loginWizardStageId        = source.loginWizardStageId,
              loginWizardRuleCategoryId = source.loginWizardRuleCategoryId,
              description               = source.description,
              reasonDisplayText         = source.reasonDisplayText,
              activationPeriod          = source.activationPeriod,
              required                  = source.required,
              active                    = source.active,
              deleted                   = source.deleted,
              amendUserId               = source.amendUserId,
              amendDate                 = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              loginWizardRuleId,
              loginWizardStageId,
              loginWizardRuleCategoryId,
              description,
              reasonDisplayText,
              activationPeriod,
              required,
              active,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.loginWizardRuleId,
              source.loginWizardStageId,
              source.loginWizardRuleCategoryId,
              source.description,
              source.reasonDisplayText,
              source.activationPeriod,
              source.required,
              source.active,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );
END
GO
