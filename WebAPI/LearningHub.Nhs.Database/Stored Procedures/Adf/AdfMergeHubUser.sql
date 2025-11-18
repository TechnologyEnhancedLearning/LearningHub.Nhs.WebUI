-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeHubUser]
    @UserList [dbo].[UserType_Hub] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    ALTER TABLE [hub].[user] NOCHECK CONSTRAINT FK_userTBL_userEmploymentTBL;
	ALTER TABLE [elfh].[userEmploymentTBL] NOCHECK CONSTRAINT FK_userEmploymentTBL_userTBL;
	ALTER TABLE [hub].[User] NOCHECK CONSTRAINT ALL;
  
    MERGE [hub].[User] AS target
    USING @UserList AS source
    ON target.[Id] = source.[Id]
    WHEN MATCHED THEN
        UPDATE SET 
            target.[UserName] = source.[UserName],
            target.[countryId] = source.[countryId],
            target.[registrationCode] = source.[registrationCode],
            target.[activeFromDate] = source.[activeFromDate],
            target.[activeToDate] = source.[activeToDate],
            target.[passwordHash] = source.[passwordHash],
            target.[mustChangeNextLogin] = source.[mustChangeNextLogin],
            target.[passwordLifeCounter] = source.[passwordLifeCounter],
            target.[securityLifeCounter] = source.[securityLifeCounter],
            target.[RemoteLoginKey] = source.[RemoteLoginKey],
            target.[RemoteLoginGuid] = source.[RemoteLoginGuid],
            target.[RemoteLoginStart] = source.[RemoteLoginStart],
            target.[RestrictToSSO] = source.[RestrictToSSO],
            target.[loginTimes] = source.[loginTimes],
            target.[loginWizardInProgress] = source.[loginWizardInProgress],
            target.[lastLoginWizardCompleted] = source.[lastLoginWizardCompleted],
            target.[primaryUserEmploymentId] = source.[primaryUserEmploymentId],
            target.[regionId] = source.[regionId],
            target.[preferredTenantId] = source.[preferredTenantId],
            target.[AmendUserId] = source.[AmendUserId],
            target.[AmendDate] = source.[AmendDate],
            target.[Deleted] = source.[Deleted]
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [Id], [UserName], [countryId], [registrationCode],
            [activeFromDate], [activeToDate], [passwordHash],
            [mustChangeNextLogin], [passwordLifeCounter], [securityLifeCounter],
            [RemoteLoginKey], [RemoteLoginGuid], [RemoteLoginStart],
            [RestrictToSSO], [loginTimes], [loginWizardInProgress],
            [lastLoginWizardCompleted], [primaryUserEmploymentId],
            [regionId], [preferredTenantId], [CreateUserId],
            [CreateDate], [AmendUserId], [AmendDate], [Deleted]
        )
        VALUES (
            source.[Id], source.[UserName], source.[countryId], source.[registrationCode],
            source.[activeFromDate], source.[activeToDate], source.[passwordHash],
            source.[mustChangeNextLogin], source.[passwordLifeCounter], source.[securityLifeCounter],
            source.[RemoteLoginKey], source.[RemoteLoginGuid], source.[RemoteLoginStart],
            source.[RestrictToSSO], source.[loginTimes], source.[loginWizardInProgress],
            source.[lastLoginWizardCompleted], source.[primaryUserEmploymentId],
            source.[regionId], source.[preferredTenantId],4,
            source.[CreateDate], source.[AmendUserId], source.[AmendDate], source.[Deleted]
        );
		ALTER TABLE [hub].[user] NOCHECK CONSTRAINT FK_userTBL_userEmploymentTBL;
	ALTER TABLE [elfh].[userEmploymentTBL] NOCHECK CONSTRAINT FK_userEmploymentTBL_userTBL;
	ALTER TABLE [hub].[User] CHECK CONSTRAINT ALL;
END
GO
