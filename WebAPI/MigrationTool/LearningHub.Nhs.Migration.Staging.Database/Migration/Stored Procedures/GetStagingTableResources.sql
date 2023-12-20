--------------------------------------------------------------------------
--	13.01.21	RobS		Initial Revision
--------------------------------------------------------------------------
CREATE PROCEDURE [Migration].[GetStagingTableResources]

AS
BEGIN

SET NOCOUNT ON
	DECLARE @Err int

	DROP TABLE IF EXISTS  #StagingTableResources

	CREATE TABLE #StagingTableResources(
		[Id] int null,
		[ResourceUniqueRef] [nvarchar](50) NULL,
		[IAmTheAuthorFlag] [nvarchar](10) NULL,
		[Title] [nvarchar](255) NULL,
		[Description] [nvarchar](1024) NULL,
		[ResourceType] [nvarchar](50) NULL,
		[SensitiveContentFlag] [nvarchar](10) NULL,
		[Keywords] [nvarchar](2048) NULL,
		[CatalogueName] [nvarchar](255) NULL,
		[Licence] [nvarchar](100) NULL,
		[PublishedDate] [date] NULL,

		-- Article
		[ArticleContentFilename] [nvarchar](1024) NULL,
		[ArticleFile1] [nvarchar](1024) NULL,
		[ArticleFile2] [nvarchar](1024) NULL,
		[ArticleFile3] [nvarchar](1024) NULL,
		[ArticleFile4] [nvarchar](1024) NULL,
		[ArticleFile5] [nvarchar](1024) NULL,
		[ArticleFile6] [nvarchar](1024) NULL,
		[ArticleFile7] [nvarchar](1024) NULL,
		[ArticleFile8] [nvarchar](1024) NULL,
		[ArticleFile9] [nvarchar](1024) NULL,
		[ArticleFile10] [nvarchar](1024) NULL,

		-- File
		[ServerFileName] [nvarchar](1024) NULL,
		[YearAuthored] [int] NULL,
		[MonthAuthored] [int] NULL,
		[DayAuthored] [int] NULL,

		-- Weblink
		[WeblinkUrl] [nvarchar](1024) NULL,
		[WeblinkText] [nvarchar](50) NULL,

		-- SCORM
		[LMSLink] [nvarchar](1024) NULL, 
		[LMSLinkVisibility] [nvarchar](100) NULL, 

		[AdditionalInformation] [nvarchar](100) NULL,
		[AuthorName1] [nvarchar](100) NULL,
		[AuthorRole1] [nvarchar](100) NULL,
		[AuthorOrganisation1] [nvarchar](100) NULL,
		[AuthorName2] [nvarchar](100) NULL,
		[AuthorRole2] [nvarchar](100) NULL,
		[AuthorOrganisation2] [nvarchar](100) NULL,
		[AuthorName3] [nvarchar](100) NULL,
		[AuthorRole3] [nvarchar](100) NULL,
		[AuthorOrganisation3] [nvarchar](100) NULL,

		-- Contributor
		[ContributorOrgName] [nvarchar](100) NULL,
		[ContributorDate] [nvarchar](100) NULL,
		[ContributorLearningHubUserName] [nvarchar](100) NULL,
		[ContributorFirstName] [nvarchar](100) NULL,
		[ContributorLastName] [nvarchar](100) NULL,
		[ContributorJobRole] [nvarchar](100) NULL,
		[ContributorProfessionalBody] [nvarchar](100) NULL,
		[ContributorRegistrationNumber] [nvarchar](100) NULL,
	)

	-- Articles
	INSERT INTO #StagingTableResources(
		ResourceUniqueRef,
		IAmTheAuthorFlag,
		Title,
		Description,
		ResourceType,
		SensitiveContentFlag,
		Keywords,
		CatalogueName,
		Licence,
		PublishedDate,
		ArticleContentFilename,
		ArticleFile1,
		ArticleFile2,
		ArticleFile3,
		ArticleFile4,
		ArticleFile5,
		ArticleFile6,
		ArticleFile7,
		ArticleFile8,
		ArticleFile9,
		ArticleFile10,
		AuthorName1,
		AuthorRole1,
		AuthorOrganisation1,
		AuthorName2,
		AuthorRole2,
		AuthorOrganisation2,
		AuthorName3,
		AuthorRole3,
		AuthorOrganisation3
	)
	SELECT
		Resource_Unique_Ref,
		I_Am_The_Author,
		Resource_Title,
		Description,
		'Article',
		Sensitive_content_flag,
		Keywords,
		Catalogue,
		Licence,
		Published_Date,
		Article,
		Article_file_1,
		Article_file_2,
		Article_file_3,
		Article_file_4,
		Article_file_5,
		Article_file_6,
		Article_file_7,
		Article_file_8,
		Article_file_9,
		Article_file_10,
		Author_Name_1,
		Role_1,
		Organisation_1,
		Author_Name_2,
		Role_2,
		Organisation_2,
		Author_Name_3,
		Role_3,
		Organisation_3
	FROM [Migration].[ArticleStaging]
	WHERE Resource_Title IS NOT NULL

	-- Files
	INSERT INTO #StagingTableResources(
		ResourceUniqueRef,
		IAmTheAuthorFlag,
		Title,
		Description,
		ResourceType,
		SensitiveContentFlag,
		Keywords,
		CatalogueName,
		Licence,
		PublishedDate,

		ServerFileName,
		YearAuthored,
		MonthAuthored,
		DayAuthored,

		AdditionalInformation,
		AuthorName1,
		AuthorRole1,
		AuthorOrganisation1,
		AuthorName2,
		AuthorRole2,
		AuthorOrganisation2,
		AuthorName3,
		AuthorRole3,
		AuthorOrganisation3
	)
	SELECT
		Resource_Unique_Ref,
		I_Am_The_Author,
		Resource_Title,
		Description,
		'File',
		Sensitive_content_flag,
		Keywords,
		Catalogue,
		Licence,
		Published_Date,

		[File_name],
		Year_Authored,
		Month_Authored,
		Day_Authored,

		Additional_information,
		Author_Name_1,
		Role_1,
		Organisation_1,
		Author_Name_2,
		Role_2,
		Organisation_2,
		Author_Name_3,
		Role_3,
		Organisation_3
	FROM [Migration].[FileStaging]
	WHERE Resource_Title IS NOT NULL

	-- Web Links
	INSERT INTO #StagingTableResources(
		ResourceUniqueRef,
		IAmTheAuthorFlag,
		Title,
		Description,
		ResourceType,
		SensitiveContentFlag,
		Keywords,
		CatalogueName,
		PublishedDate,

		[WeblinkURL],
		[WeblinkText],

		AdditionalInformation,
		AuthorName1,
		AuthorRole1,
		AuthorOrganisation1,
		AuthorName2,
		AuthorRole2,
		AuthorOrganisation2,
		AuthorName3,
		AuthorRole3,
		AuthorOrganisation3
	)
	SELECT
		Resource_Unique_Ref,
		I_Am_The_Author,
		Resource_Title,
		Description,
		'WebLink',
		Sensitive_content_flag,
		Keywords,
		Catalogue,
		Published_Date,

		[URL],
		[Text],

		Additional_information,
		Author_Name_1,
		Role_1,
		Organisation_1,
		Author_Name_2,
		Role_2,
		Organisation_2,
		Author_Name_3,
		Role_3,
		Organisation_3
	FROM [Migration].[WebLinkStaging]
	WHERE Resource_Title IS NOT NULL

	-- SCORM
	INSERT INTO #StagingTableResources(
		ResourceUniqueRef,
		IAmTheAuthorFlag,
		Title,
		Description,
		ResourceType,
		SensitiveContentFlag,
		Keywords,
		CatalogueName,
		Licence,
		PublishedDate,

		ServerFileName,
		LMSLink,
		LMSLinkVisibility,

		AdditionalInformation,
		AuthorName1,
		AuthorRole1,
		AuthorOrganisation1,
		AuthorName2,
		AuthorRole2,
		AuthorOrganisation2,
		AuthorName3,
		AuthorRole3,
		AuthorOrganisation3
	)
	SELECT
		Resource_Unique_Ref,
		I_Am_The_Author,
		Resource_Title,
		Description,
		'SCORM',
		Sensitive_content_flag,
		Keywords,
		Catalogue,
		Licence,
		Published_Date,

		[File_name],
		LMS_Link,
		LMS_Link_Visibility,

		Additional_information,
		Author_Name_1,
		Role_1,
		Organisation_1,
		Author_Name_2,
		Role_2,
		Organisation_2,
		Author_Name_3,
		Role_3,
		Organisation_3
	FROM [Migration].[SCORMStaging]
	WHERE Resource_Title IS NOT NULL

	-- Set Contributor on all rows
	DECLARE @ContributorOrgName [nvarchar](100)
	DECLARE @ContributorDate [nvarchar](100)
	DECLARE @ContributorLearningHubUserName [nvarchar](100)
	DECLARE @ContributorFirstName [nvarchar](100)
	DECLARE @ContributorLastName [nvarchar](100)
	DECLARE @ContributorJobRole [nvarchar](100)
	DECLARE @ContributorProfessionalBody [nvarchar](100)
	DECLARE @ContributorRegistrationNumber [nvarchar](100)

	SELECT @ContributorOrgName = VALUE FROM Migration.ContributorStaging WHERE NAME = 'Organisation name'
	SELECT @ContributorDate = VALUE FROM Migration.ContributorStaging WHERE NAME = 'Date'
	SELECT @ContributorLearningHubUserName = VALUE FROM Migration.ContributorStaging WHERE NAME = 'e-LfH User Name'
	SELECT @ContributorFirstName = VALUE FROM Migration.ContributorStaging WHERE NAME = 'First  name'
	SELECT @ContributorLastName = VALUE FROM Migration.ContributorStaging WHERE NAME = 'Last name'
	SELECT @ContributorJobRole = VALUE FROM Migration.ContributorStaging WHERE NAME = 'Job role'
	SELECT @ContributorProfessionalBody = VALUE FROM Migration.ContributorStaging WHERE NAME = 'Professional Body'
	SELECT @ContributorRegistrationNumber = VALUE FROM Migration.ContributorStaging WHERE NAME = 'Registration Number'

	UPDATE #StagingTableResources SET
		[ContributorOrgName] = @ContributorOrgName,
		[ContributorDate] = @ContributorDate,
		[ContributorLearningHubUserName] = @ContributorLearningHubUserName,
		[ContributorFirstName] = @ContributorFirstName,
		[ContributorLastName] = @ContributorLastName,
		[ContributorJobRole] = @ContributorJobRole,
		[ContributorProfessionalBody] = @ContributorProfessionalBody,
		[ContributorRegistrationNumber] = @ContributorRegistrationNumber

	DECLARE @id INT 
	SET @id = 0 
	UPDATE #StagingTableResources
	SET @id = Id = @id + 1 

	-- Return the data.
	SELECT * FROM #StagingTableResources

	DROP TABLE #StagingTableResources

	SET @Err = @@Error

	RETURN @Err

END