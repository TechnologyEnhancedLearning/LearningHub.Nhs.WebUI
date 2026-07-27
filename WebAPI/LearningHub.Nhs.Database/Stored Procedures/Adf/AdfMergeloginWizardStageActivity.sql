-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeloginWizardStageActivity]
    @loginWizardStageActivityList dbo.LoginWizardStageActivity READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    SET IDENTITY_INSERT [elfh].[loginWizardStageActivityTBL] ON;
    MERGE [elfh].[loginWizardStageActivityTBL] AS target
    USING @loginWizardStageActivityList AS source
    ON target.loginWizardStageActivityId = source.loginWizardStageActivityId

    WHEN MATCHED THEN
        UPDATE SET 
              loginWizardStageId = source.loginWizardStageId,
              userId             = source.userId,
              activityDatetime   = source.activityDatetime

    WHEN NOT MATCHED THEN
        INSERT (
              loginWizardStageActivityId,
              loginWizardStageId,
              userId,
              activityDatetime
        )
        VALUES (
              source.loginWizardStageActivityId,
              source.loginWizardStageId,
              source.userId,
              source.activityDatetime
        );

    SET IDENTITY_INSERT [elfh].[loginWizardStageActivityTBL] OFF;
END
GO
