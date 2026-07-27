-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeloginWizardStage]
    @loginWizardStageList dbo.LoginWizardStage READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;
    MERGE [elfh].[loginWizardStageTBL] AS target
    USING @loginWizardStageList AS source
    ON target.loginWizardStageId = source.loginWizardStageId

    WHEN MATCHED THEN
        UPDATE SET 
              description        = source.description,
              reasonDisplayText  = source.reasonDisplayText,
              deleted            = source.deleted,
              amendUserId        = source.amendUserId,
              amendDate          = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              loginWizardStageId,
              description,
              reasonDisplayText,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.loginWizardStageId,
              source.description,
              source.reasonDisplayText,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

END
GO
