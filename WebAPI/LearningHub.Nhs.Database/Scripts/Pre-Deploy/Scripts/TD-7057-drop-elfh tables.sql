--DROP TABLE IF EXISTS elfh.userTermsAndConditionsTBL;
--DROP TABLE IF EXISTS elfh.userRoleUpgradeTBL;
--DROP TABLE IF EXISTS elfh.userReportingUserTBL;
--DROP TABLE IF EXISTS elfh.userPasswordValidationTokenTBL;

--IF OBJECT_ID('FK_userHistoryAttributeTBL_attributeId', 'F') IS NOT NULL
--BEGIN
--    ALTER TABLE [elfh].[userHistoryAttributeTBL]
--    DROP CONSTRAINT [FK_userHistoryAttributeTBL_attributeId];
--END;

--IF OBJECT_ID('FK_userHistoryAttributeTBL_userHistoryId', 'F') IS NOT NULL
--BEGIN
--    ALTER TABLE [elfh].[userHistoryAttributeTBL]
--    DROP CONSTRAINT [FK_userHistoryAttributeTBL_userHistoryId];
--END;

--DROP TABLE IF EXISTS elfh.userHistoryTBL;
--DROP TABLE IF EXISTS elfh.userHistoryTypeTBL;

--DROP TABLE IF EXISTS elfh.userGroupTypeInputValidationTBL;
--DROP TABLE IF EXISTS elfh.userGroupReporterTBL;
--DROP TABLE IF EXISTS elfh.userEmploymentResponsibilityTBL;
--DROP TABLE IF EXISTS elfh.userEmploymentReferenceTBL;
--DROP TABLE IF EXISTS elfh.userAttributeTBL;
--DROP TABLE IF EXISTS elfh.userAdminLocationTBLCopy;
--DROP TABLE IF EXISTS elfh.userAdminLocationTBL;
--DROP TABLE IF EXISTS elfh.termsAndConditionsTBL;
--DROP TABLE IF EXISTS elfh.tenantUrlTBL;
--DROP TABLE IF EXISTS elfh.tenantTBL;
--DROP TABLE IF EXISTS elfh.tenantSmtpTBL;
--DROP TABLE IF EXISTS elfh.systemSettingTBL;
--DROP TABLE IF EXISTS elfh.mergeUserTBL;
--DROP TABLE IF EXISTS elfh.loginWizardStageActivityTBL;
--DROP TABLE IF EXISTS elfh.ipCountryLookupTBL;
--DROP TABLE IF EXISTS elfh.gmclrmpTBL;
--DROP TABLE IF EXISTS elfh.gdcRegisterTBL;
--DROP TABLE IF EXISTS elfh.employmentReferenceTypeTBL;
--DROP TABLE IF EXISTS elfh.emailTemplateTypeTBL;
--DROP TABLE IF EXISTS elfh.emailTemplateTBL;
--DROP TABLE IF EXISTS elfh.attributeTBL;
--DROP TABLE IF EXISTS elfh.attributeTypeTBL;
--DROP TABLE IF EXISTS elfh.loginWizardRuleTBL;
--DROP TABLE IF EXISTS elfh.loginWizardStageTBL;


---- Foreign Keys
--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_gradeTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_gradeTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_jobRoleTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_jobRoleTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_locationTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_locationTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_medicalCouncilTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_medicalCouncilTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_schoolTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_schoolTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_specialtyTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_specialtyTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_userTBL]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_userTBL];

--IF OBJECT_ID('[elfh].[FK_userEmploymentTBL_userTBL_AmendUser]', 'F') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [FK_userEmploymentTBL_userTBL_AmendUser];

---- Default Constraints
--IF OBJECT_ID('[elfh].[DF_userEmploymentTBL_deleted]', 'D') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [DF_userEmploymentTBL_deleted];

--IF OBJECT_ID('[elfh].[DF_userEmploymentTBL_archived]', 'D') IS NOT NULL
--    ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [DF_userEmploymentTBL_archived];

---- Primary Key
----IF OBJECT_ID('[elfh].[PK_userEmploymentTBL]', 'PK') IS NOT NULL
--  --  ALTER TABLE [elfh].[userEmploymentTBL] DROP CONSTRAINT [PK_userEmploymentTBL];


--DROP TABLE IF EXISTS elfh.medicalCouncilTBL;
--DROP TABLE IF EXISTS elfh.gradeTBL;
--DROP TABLE IF EXISTS elfh.deaneryTBL;
--DROP TABLE IF EXISTS elfh.locationTBL;
--DROP TABLE IF EXISTS elfh.locationTypeTBL;
--DROP TABLE IF EXISTS elfh.jobRoleTBL;
--DROP TABLE IF EXISTS elfh.schoolTBL;
--DROP TABLE IF EXISTS elfh.staffGroupTBL;
--DROP TABLE IF EXISTS elfh.specialtyTBL;


--IF OBJECT_ID('[hub].[FK_userTBL_countryTBL]', 'F') IS NOT NULL
--BEGIN
--    ALTER TABLE hub.[User]
--    DROP CONSTRAINT FK_userTBL_countryTBL;
--END;

--IF OBJECT_ID('[hub].[FK_userTBL_regionTBL]', 'F') IS NOT NULL
--BEGIN
--    ALTER TABLE hub.[User]
--    DROP CONSTRAINT FK_userTBL_regionTBL;
--END;

--IF OBJECT_ID('[hub].[FK_userTBL_userEmploymentTBL]', 'F') IS NOT NULL
--BEGIN
--    ALTER TABLE hub.[User]
--    DROP CONSTRAINT FK_userTBL_userEmploymentTBL;
--END;

--DROP TABLE IF EXISTS elfh.userEmploymentTBL;
--DROP TABLE IF EXISTS elfh.regionTBL;
--DROP TABLE IF EXISTS elfh.countryTBL;
--DROP TABLE IF EXISTS elfh.userEmploymentTBL;
--DROP TABLE IF EXISTS elfh.regionTBL;
--DROP TABLE IF EXISTS elfh.countryTBL;
--DROP TABLE IF EXISTS elfh.userHistoryAttributeTBL




DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql = @sql + '
ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(fk.schema_id)) + '.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) +
' DROP CONSTRAINT ' + QUOTENAME(fk.name) + ';'
FROM sys.foreign_keys fk
WHERE fk.referenced_object_id IN (
    SELECT object_id FROM sys.tables WHERE schema_id = SCHEMA_ID('elfh')
);

EXEC sp_executesql @sql;


/* ============================================================
   2. DROP SPECIFIC FOREIGN KEYS OUTSIDE elfh (hub.User)
   ============================================================ */
IF OBJECT_ID('[hub].[FK_userTBL_countryTBL]', 'F') IS NOT NULL
    ALTER TABLE hub.[User] DROP CONSTRAINT [FK_userTBL_countryTBL];

IF OBJECT_ID('[hub].[FK_userTBL_regionTBL]', 'F') IS NOT NULL
    ALTER TABLE hub.[User] DROP CONSTRAINT [FK_userTBL_regionTBL];

IF OBJECT_ID('[hub].[FK_userTBL_userEmploymentTBL]', 'F') IS NOT NULL
    ALTER TABLE hub.[User] DROP CONSTRAINT [FK_userTBL_userEmploymentTBL];


/* ============================================================
   3. DROP ALL DEFAULT CONSTRAINTS IN elfh SCHEMA
   ============================================================ */
SET @sql = N'';

SELECT @sql = @sql + '
ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(t.schema_id)) + '.' + QUOTENAME(t.name) +
' DROP CONSTRAINT ' + QUOTENAME(dc.name) + ';'
FROM sys.default_constraints dc
JOIN sys.tables t ON dc.parent_object_id = t.object_id
WHERE t.schema_id = SCHEMA_ID('elfh');

EXEC sp_executesql @sql;


/* ============================================================
   4. DROP ALL TABLES IN elfh SCHEMA
   ============================================================ */
SET @sql = N'';

SELECT @sql = @sql + '
DROP TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ';'
FROM sys.tables
WHERE schema_id = SCHEMA_ID('elfh')
ORDER BY name;

EXEC sp_executesql @sql;


