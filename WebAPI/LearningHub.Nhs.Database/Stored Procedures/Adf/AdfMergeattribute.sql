-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE TYPE [dbo].[EmailTemplate] AS TABLE
(
    [emailTemplateId] INT,
    [emailTemplateTypeId] INT,
    [programmeComponentId] INT,
    [title] NVARCHAR(256),
    [subject] NVARCHAR(256),
    [body] ntext,
    [deleted] BIT,
    [amendUserID] INT,
    [amendDate] DATETIMEOFFSET,
    [tenantId] INT
);
GO
CREATE TYPE dbo.UserReportingUserType AS TABLE
(
    [userReportingUserId] INT,
    [userId] INT,
    [reportingUserId] INT,
    [reportable] BIT,
    [Deleted] BIT,
    [AmendUserID] INT,
    [AmendDate] DATETIMEOFFSET
);
GO
CREATE TYPE UserHistoryAttributeType AS TABLE
(
    [userHistoryAttributeId] INT,
    [userHistoryId] INT,
    [attributeId] INT,
    [intValue] INT NULL,
    [textValue] NVARCHAR(1000) NULL,
    [booleanValue] BIT NULL,
    [dateValue] DATETIMEOFFSET NULL,
    [deleted] BIT,
    [amendUserId] INT,
    [amendDate] DATETIMEOFFSET
);
GO
CREATE TYPE UserEmploymentResponsibilityType AS TABLE
(
    [userEmploymentResponsibilityId] INT,
    [userEmploymentId] INT,
    [additionalResponsibilityId] INT,
    [deleted] BIT,
    [amendUserId] INT
);
GO
CREATE TYPE UserEmploymentReferenceType AS TABLE
(
    [userEmploymentReferenceId] INT,
    [employmentReferenceTypeId] INT,
    [userEmploymentId] INT,
    [referenceValue] NVARCHAR(100),
    [deleted] BIT,
    [amendUserId] INT,
    [amendDate] DATETIMEOFFSET
);
GO
CREATE TYPE dbo.UserAdminLocationType AS TABLE
(
    [userId] INT,
    [adminLocationId] INT,
    [deleted] BIT,
    [amendUserId] INT,
    [amendDate] DATETIMEOFFSET,
    [createdUserId] INT,
    [createdDate] DATETIMEOFFSET
);
GO
CREATE TYPE dbo.MergeUser AS TABLE
(
    [mergeUserId] INT,
    [fromUserId] INT,
    [intoUserId] INT,
    [amendUserId] INT,
    [createdDatetime] DATETIMEOFFSET
);
GO
CREATE TYPE dbo.EmploymentReferenceType AS TABLE
(
    [EmploymentReferenceTypeId] INT,
    [Title] NVARCHAR(255),
    [RefAccess] NVARCHAR(255)
);
GO
CREATE TYPE [dbo].[UserProfileType] AS TABLE
(
    [Id]               INT             ,
    [UserName]         NVARCHAR(255)  ,
    [EmailAddress]     NVARCHAR(255) ,
    [AltEmailAddress]  NVARCHAR(255) ,
    [FirstName]        NVARCHAR(255) ,
    [LastName]         NVARCHAR(255) ,
    [PreferredName]    NVARCHAR(255) ,
    [Active]           BIT           ,
    [CreateDate]       DATETIMEOFFSET,
    [AmendUserId]      INT           ,
    [AmendDate]        DATETIMEOFFSET,
    [Deleted]          BIT           
);
GO
CREATE TYPE dbo.UserType_Hub AS TABLE
(
    [Id] INT,
    [UserName] NVARCHAR(250),
    [countryId] INT,
    [registrationCode] NVARCHAR(100),
    [activeFromDate] DATETIMEOFFSET,
    [activeToDate] DATETIMEOFFSET,
    [passwordHash] NVARCHAR(500),
    [mustChangeNextLogin] BIT,
    [passwordLifeCounter] INT,
    [securityLifeCounter] INT,
    [RemoteLoginKey] NVARCHAR(250),
    [RemoteLoginGuid] UNIQUEIDENTIFIER,
    [RemoteLoginStart] DATETIMEOFFSET,
    [RestrictToSSO] BIT,
    [loginTimes] INT,
    [loginWizardInProgress] BIT,
    [lastLoginWizardCompleted] DATETIMEOFFSET,
    [primaryUserEmploymentId] INT,
    [regionId] INT,
    [preferredTenantId] INT,
    [CreateDate] DATETIMEOFFSET,
    [AmendUserId] INT,
    [AmendDate] DATETIMEOFFSET,
    [Deleted] BIT
);
GO
CREATE TYPE dbo.UserTermsAndConditions AS TABLE
(
    [userTermsAndConditionsId] INT,
    [termsAndConditionsId] INT,
    [userId] INT,
    [acceptanceDate] DATETIMEOFFSET,
    [deleted] BIT,
    [amendUserID] INT,
    [amendDate] DATETIMEOFFSET
);
GO
CREATE TYPE dbo.UserRoleUpgrade AS TABLE
(
    [userRoleUpgradeId] INT,
    [userId] INT,
    [emailAddress] NVARCHAR(100),
    [upgradeDate] DATETIMEOFFSET,
    [deleted] BIT,
    [createUserId] INT,
    [createDate] DATETIMEOFFSET,
    [amendUserId] INT,
    [amendDate] DATETIMEOFFSET,
    [userHistoryTypeId] INT
);
GO
CREATE TYPE dbo.UserHistory AS TABLE
(
    [userHistoryId] INT,
    [userHistoryTypeId] INT,
    [userId] INT,
    [createdDate] DATETIMEOFFSET,
    [tenantId] INT
);
GO
CREATE TYPE dbo.UserHistoryType AS TABLE
(
    UserHistoryTypeId INT,
    [Description] NVARCHAR(100)
);
GO
CREATE TYPE dbo.UserPasswordValidationToken AS TABLE
(
    userPasswordValidationTokenId INT,
    hashedToken NVARCHAR(128),
    salt NVARCHAR(128),
    [lookup] NVARCHAR(128),
    expiry DATETIMEOFFSET(7),
    tenantId INT,
    userId INT,
    createdUserId INT,
    createdDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.UserGroupTypeInputValidation AS TABLE
(
    userGroupTypeInputValidationId INT,
    userGroupId INT,
    userGroupTypePrefix NVARCHAR(10),
    userGroupTypeId INT,
    validationTextValue NVARCHAR(1000),
    validationMethod INT,
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7),
    createdUserId INT,
    createdDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.UserAttribute AS TABLE
(
    userAttributeId INT,
    userId INT,
    attributeId INT,
    intValue INT,
    textValue NVARCHAR(255),
    booleanValue BIT,
    dateValue DATETIMEOFFSET(7),
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.TermsAndConditions AS TABLE
(
    termsAndConditionsId INT,
    createdDate DATETIMEOFFSET(7),
    description NVARCHAR(512),
    details ntext,
    tenantId INT,
    active BIT,
    reportable BIT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.TenantUrl AS TABLE
(
    tenantUrlId INT,
    tenantId INT,
    urlHostName NVARCHAR(128),
    useHostForAuth BIT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.Tenant AS TABLE
(
    tenantId INT,
    tenantCode NVARCHAR(100),
    tenantName NVARCHAR(250),
    tenantDescription NVARCHAR(1024),
    showFullCatalogInfoMessageInd BIT,
    catalogUrl NVARCHAR(500),
    quickStartGuideUrl NVARCHAR(1024),
    supportFormUrl NVARCHAR(500),
    liveChatStatus NVARCHAR(50),
    liveChatSnippet NVARCHAR(2048),
    myElearningDefaultView NVARCHAR(100),
    preLoginCatalogueDefaultView NVARCHAR(100),
    postLoginCatalogueDefaultView NVARCHAR(100),
    authSignInUrlRelative NVARCHAR(1024),
    authSignOutUrlRelative NVARCHAR(500),
    authSecret UNIQUEIDENTIFIER,
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.TenantSmtp AS TABLE
(
    tenantId INT,
    deliveryMethod NVARCHAR(128),
    pickupDirectoryLocation NVARCHAR(256),
    [from] NVARCHAR(256),
    userName NVARCHAR(256),
    [password] NVARCHAR(256),
    enableSsl BIT,
    host NVARCHAR(256),
    port INT,
    active BIT,
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.SystemSetting AS TABLE
(
    systemSettingId INT,
    systemSettingName NVARCHAR(50),
    intValue INT,
    textValue NVARCHAR(255),
    booleanValue BIT,
    dateValue DATETIMEOFFSET(7),
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.LoginWizardStageActivity AS TABLE
(
    loginWizardStageActivityId INT,
    loginWizardStageId INT,
    userId INT,
    activityDatetime DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.LoginWizardRule AS TABLE
(
    loginWizardRuleId INT,
    loginWizardStageId INT,
    loginWizardRuleCategoryId INT,
    description NVARCHAR(128),
    reasonDisplayText NVARCHAR(1024),
    activationPeriod INT,
    required BIT,
    active BIT,
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.LoginWizardStage AS TABLE
(
    loginWizardStageId INT,
    description NVARCHAR(128),
    reasonDisplayText NVARCHAR(1024),
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.IPCountryLookup AS TABLE
(
    fromIP NVARCHAR(50),
    toIP NVARCHAR(50),
    country NVARCHAR(100),
    fromInt BIGINT,
    toInt BIGINT
);
GO
CREATE TYPE dbo.EmailTemplateType AS TABLE
(
    emailTemplateTypeId INT,
    emailTemplateTypeName NVARCHAR(250),
    availableTags NVARCHAR(MAX),
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.Attribute AS TABLE
(
    attributeId INT,
    attributeTypeId INT,
    attributeName NVARCHAR(250),
    attributeAccess NVARCHAR(100),
    attributeDescription NVARCHAR(500),
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.AttributeType AS TABLE
(
    attributeTypeId INT,
    attributeTypeName NVARCHAR(250),
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.GDCRegister AS TABLE
(
    reg_number NVARCHAR(50),
    Dentist bit,
    Title NVARCHAR(50),
    Surname NVARCHAR(200),
    Forenames NVARCHAR(200),
    honorifics NVARCHAR(200),
    house_name NVARCHAR(200),
    address_line1 NVARCHAR(200),
    address_line2 NVARCHAR(200),
    address_line3 NVARCHAR(200),
    address_line4 NVARCHAR(200),
    Town NVARCHAR(200),
    County NVARCHAR(200),
    PostCode NVARCHAR(50),
    Country NVARCHAR(100),
    regdate NVARCHAR(50),
    qualifications NVARCHAR(1000),
    dcp_titles NVARCHAR(500),
    specialties NVARCHAR(500),
    [condition] NVARCHAR(500),
    suspension NVARCHAR(500),
    dateProcessed DATETIMEOFFSET(7),
    action NVARCHAR(50)
);
GO

CREATE TYPE dbo.GMCLRMP AS TABLE
(
    GMC_Ref_No NVARCHAR(50),
    Surname NVARCHAR(200),
    Given_Name NVARCHAR(200),
    Year_Of_Qualification FLOAT,
    GP_Register_Date NVARCHAR(100),
    Registration_Status NVARCHAR(100),
    Other_Names NVARCHAR(200),
    dateProcessed DATETIME,
    action NVARCHAR(50)
);
GO
CREATE TYPE dbo.Region AS TABLE
(
    regionId INT,
    regionName NVARCHAR(250),
    displayOrder INT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.UserEmployment AS TABLE
(
    userEmploymentId INT,
    userId INT,
    jobRoleId INT,
    specialtyId INT,
    gradeId INT,
    schoolId INT,
    locationId INT,
    medicalCouncilId INT,
    medicalCouncilNo NVARCHAR(100),
    startDate DATETIMEOFFSET(7),
    endDate DATETIMEOFFSET(7),
    deleted BIT,
    archived BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.Location AS TABLE
(
    locationId INT,
    locationCode NVARCHAR(50),
    locationName NVARCHAR(250),
    locationSubName NVARCHAR(250),
    locationTypeId INT,
    address1 NVARCHAR(250),
    address2 NVARCHAR(250),
    address3 NVARCHAR(250),
    address4 NVARCHAR(250),
    town NVARCHAR(250),
    county NVARCHAR(250),
    postCode NVARCHAR(50),
    telephone NVARCHAR(50),
    acute BIT,
    ambulance BIT,
    mental BIT,
    care BIT,
    mainHosp BIT,
    nhsCode NVARCHAR(50),
    parentId INT,
    dataSource NVARCHAR(250),
    active BIT,
    importExclusion BIT,
    depth INT,
    lineage NVARCHAR(MAX),
    created DATETIMEOFFSET(7),
    updated DATETIMEOFFSET(7),
    archivedDate DATETIMEOFFSET(7),
    countryId INT,
    iguId INT,
    letbId INT,
    ccgId INT,
    healthServiceId INT,
    healthBoardId INT,
    primaryTrustId INT,
    secondaryTrustId INT,
    islandId INT,
    otherNHSOrganisationId INT
);
GO
CREATE TYPE dbo.LocationType AS TABLE
(
    locationTypeID INT,
    locationType NVARCHAR(250),
    countryId INT,
    healthService NVARCHAR(250),
    healthBoard NVARCHAR(250),
    primaryTrust NVARCHAR(250),
    secondaryTrust NVARCHAR(250)
);
GO
CREATE TYPE dbo.Country AS TABLE
(
    countryId INT,
    countryName NVARCHAR(250),
    alpha2 NVARCHAR(10),
    alpha3 NVARCHAR(10),
    numeric NVARCHAR(10),
    EUVatRate DECIMAL(10,4),
    displayOrder INT,
    deleted BIT,
    amendUserId INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.School AS TABLE
(
    schoolId INT,
    deaneryId INT,
    specialtyId INT,
    schoolName NVARCHAR(250),
    displayOrder INT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.Deanery AS TABLE
(
    deaneryId INT,
    deaneryName NVARCHAR(250),
    displayOrder INT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.Grade AS TABLE
(
    gradeId INT,
    gradeName NVARCHAR(250),
    displayOrder INT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.Specialty AS TABLE
(
    specialtyId INT,
    specialtyName NVARCHAR(250),
    displayOrder INT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.JobRole AS TABLE
(
    jobRoleId INT,
    staffGroupId INT,
    jobRoleName NVARCHAR(250),
    medicalCouncilId INT,
    displayOrder INT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.StaffGroup AS TABLE
(
    staffGroupId INT,
    staffGroupName NVARCHAR(250),
    displayOrder INT,
    internalUsersOnly BIT,
    deleted BIT,
    amendUserID INT,
    amendDate DATETIMEOFFSET(7)
);
GO
CREATE TYPE dbo.MedicalCouncil AS TABLE
(
	medicalCouncilId int, 
	medicalCouncilName nvarchar(50), 
	medicalCouncilCode nvarchar(50),
	uploadPrefix nvarchar(3),
	includeOnCerts bit ,
	deleted bit,
	amendUserID int ,
	amendDate datetimeoffset(7)
);
GO
CREATE PROCEDURE [dbo].[AdfMergeattribute]
    @attributeList dbo.Attribute READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    SET IDENTITY_INSERT [elfh].[attributeTBL] ON;

    MERGE [elfh].[attributeTBL] AS target
    USING @attributeList AS source
    ON target.attributeId = source.attributeId

    WHEN MATCHED THEN
        UPDATE SET 
              attributeTypeId      = source.attributeTypeId,
              attributeName        = source.attributeName,
              attributeAccess      = source.attributeAccess,
              attributeDescription = source.attributeDescription,
              deleted              = source.deleted,
              amendUserId          = source.amendUserId,
              amendDate            = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              attributeId,
              attributeTypeId,
              attributeName,
              attributeAccess,
              attributeDescription,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.attributeId,
              source.attributeTypeId,
              source.attributeName,
              source.attributeAccess,
              source.attributeDescription,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

    SET IDENTITY_INSERT [elfh].[attributeTBL] OFF;
END
GO
