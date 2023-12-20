/*
Script for adding the Learning Hub tenant
*/
--Required Information - UPDATE BEFORE RUNNING
DECLARE @tenantId INT = 10
DECLARE @tenantCode NVARCHAR(20) = 'LearningHub'
DECLARE @tenantName NVARCHAR(64) = 'Learning Hub'
DECLARE @tenantDescription NVARCHAR(1024) = 'Learning Hub'
DECLARE @showFullCatalog BIT = 0
DECLARE @catalogUrl NVARCHAR(256) = ''
DECLARE @quickStartGuideUrl NVARCHAR(1024) = 'http://support.e-lfh.org.uk/media/161597/e-lfh_quick_start_guide.pdf'
DECLARE @supportFormUrl NVARCHAR(1024) = 'http://millennium.kayako.com/ES/Tickets/Submit'
DECLARE @tenantUrl NVARCHAR(128) = 'localhost'
--LIVE URL DECLARE @tenantUrl NVARCHAR(128) = 'populationwellbeingportal.e-lfh.org.uk'
DECLARE @liveChatSnippet NVARCHAR(MAX) = '<!-- BEGIN TAG CODE - DO NOT EDIT! --><div><div id="proactivechatcontainer6yrq7csnnp"></div><table border="0" cellspacing="2" cellpadding="2"><tr><td align="center" id="swifttagcontainer6yrq7csnnp"><div style="display: inline;" id="swifttagdatacontainer6yrq7csnnp"></div><a target="_blank" href="https://millennium.kayako.com/visitor/index.php?/ES/LiveChat/Chat/Request/_sessionID=/_promptType=chat/_proactive=0/_filterDepartmentID=82/_randomNumber=6c67135wkhpfa3636mynl367tmhpa36a/_fullName=/_email=/" class="livechatlink"><img src="https://millennium.kayako.com/visitor/index.php?/ES/LiveChat/HTML/NoJSImage/cHJvbXB0dHlwZT1jaGF0JnVuaXF1ZWlkPTZ5cnE3Y3NubnAmdmVyc2lvbj00LjkzLjAxJnByb2R1Y3Q9ZnVzaW9uJmZpbHRlcmRlcGFydG1lbnRpZD04MiZjdXN0b21vbmxpbmU9aHR0cHMlM0ElMkYlMkZtaWxsZW5uaXVtLmtheWFrby5jb20lMkZfX3N3aWZ0JTJGZmlsZXMlMkZmaWxlX2Q3cHEyZDQ2ejhjMnVkNC5naWYmY3VzdG9tb2ZmbGluZT1odHRwcyUzQSUyRiUyRm1pbGxlbm5pdW0ua2F5YWtvLmNvbSUyRl9fc3dpZnQlMkZmaWxlcyUyRmZpbGVfcjA1MHZvMXNkdHplNm8xLmdpZiZjdXN0b21hd2F5PWh0dHBzJTNBJTJGJTJGbWlsbGVubml1bS5rYXlha28uY29tJTJGX19zd2lmdCUyRmZpbGVzJTJGZmlsZV81cmYyZ3NtemRwMnphc24uZ2lmJmN1c3RvbWJhY2tzaG9ydGx5PWh0dHBzJTNBJTJGJTJGbWlsbGVubml1bS5rYXlha28uY29tJTJGX19zd2lmdCUyRmZpbGVzJTJGZmlsZV94bDJ1anhvcWN6YjF2eG8uZ2lmCjRkY2VhOGJjY2RlZmJlYjk5ZmFjZmEzMWM3Y2JiODZkNjlmYjc4Y2I=" align="absmiddle" border="0" /></a></td> </tr></table></div><!-- END TAG CODE - DO NOT EDIT! -->'
--Specify where to copy the smtp settings for emailing from (e-LfH is 1)
DECLARE @copyEmailConfigFromTenantId int = 1
DECLARE @copyTermsandConditionsFromTenantId int = 1

--Copy bulk upload email templates from an existing template for editing later
DECLARE @copyBulkUploadEmailTemplatesFromTenantId int = 1

--Must duplicate terms and conditions for this tenant

--Resource Strings
DECLARE @emailFooter NVARCHAR(1000) = N'Regards,
	The e-LfH Support Team
	www.e-lfh.org.uk'

DECLARE @accessDenied NVARCHAR(1000) = N'You are not entitled to access the ' + @tenantDescription


/* DO NOT EDIT BELOW THIS LINE =========================================================================================*/
BEGIN TRAN

SET NOCOUNT ON

PRINT 'Core Tenant Record'
IF NOT EXISTS(SELECT 1 FROM tenantTBL WHERE tenantId = @tenantId)
BEGIN
	INSERT INTO tenantTBL (tenantId, tenantCode, tenantName, tenantDescription,
							showFullCatalogInfoMessageInd, catalogUrl,quickStartGuideUrl,supportFormUrl,deleted, amendUserId, amendDate, liveChatSnippet)
	 VALUES (@tenantId,@tenantCode,@tenantName,@tenantDescription,@showFullCatalog,@catalogUrl,@quickStartGuideUrl,@supportFormUrl,0,4,SYSDATETIMEOFFSET(),@liveChatSnippet)
	 PRINT ' -Tenant created'
END
ELSE
BEGIN 
	PRINT ' -Tenant NOT Created - Tenant already exists'
END

PRINT 'URL Setup'
IF NOT EXISTS(SELECT 1 FROM tenantUrlTBL WHERE tenantId = @tenantId)
BEGIN
	INSERT INTO tenantUrlTBL (tenantId, urlHostName, deleted, amendUserID, amendDate)
	 VALUES (@tenantId, @tenantUrl, 0, 4, SYSDATETIMEOFFSET())
	 PRINT ' -' + @tenantUrl + ' assigned to tenant'
END
ELSE
BEGIN 
	PRINT ' -URL not assigned - One already exists for this tenant'
END

PRINT 'SMTP Settings'
IF NOT EXISTS(SELECT 1 FROM tenantSmtpTBL WHERE tenantId = @tenantId)
BEGIN 
	INSERT INTO tenantSmtpTBL (
	  [tenantId]
      ,[deliveryMethod]
      ,[pickupDirectoryLocation]
      ,[from]
      ,[userName]
      ,[password]
      ,[enableSsl]
      ,[host]
      ,[port]
      ,[active]
      ,[deleted]
      ,[amendUserId]
      ,[amendDate]
	  )
	  SELECT @tenantId
      ,[deliveryMethod]
      ,[pickupDirectoryLocation]
      ,[from]
      ,[userName]
      ,[password]
      ,[enableSsl]
      ,[host]
      ,[port]
      ,[active]
      ,0
      ,4
      ,SYSDATETIMEOFFSET()
	  FROM tenantSmtpTBL
	  WHERE tenantId = @copyEmailConfigFromTenantId
	  PRINT ' -SMTP settings copied'
END
ELSE
BEGIN 
	PRINT ' -SMTP settings not created - settings already exist'
END

PRINT 'Bulk Upload Email Templates'
PRINT ' -New account'
IF NOT EXISTS(SELECT 1 FROM emailTemplateTBL WHERE tenantId = @tenantId AND emailTemplateTypeId=3 AND programmeComponentId = 11)
BEGIN 
	INSERT INTO emailTemplateTBL (
       [emailTemplateTypeId]
      ,[programmeComponentId]
      ,[title]
      ,[subject]
      ,[body]
      ,[deleted]
      ,[amendUserID]
      ,[amendDate]
      ,[tenantId]
	)
	SELECT
       [emailTemplateTypeId]
      ,[programmeComponentId]
      ,@tenantName + ' New User'
      ,[subject]
      ,[body]
      ,0
      ,4
      ,SYSDATETIMEOFFSET()
      ,@tenantId
	  FROM emailTemplateTBL
	  WHERE tenantId = @copyBulkUploadEmailTemplatesFromTenantId
		AND emailTemplateTypeId=3
		AND programmeComponentId = 11
		AND deleted = 0
	PRINT '   -Created'
END 
ELSE
BEGIN
PRINT '   -Skipped (Exists)'
END
PRINT ' -Updated account'
IF NOT EXISTS(SELECT 1 FROM emailTemplateTBL WHERE tenantId = @tenantId AND emailTemplateTypeId=4 AND programmeComponentId = 11)
BEGIN 
	INSERT INTO emailTemplateTBL (
       [emailTemplateTypeId]
      ,[programmeComponentId]
      ,[title]
      ,[subject]
      ,[body]
      ,[deleted]
      ,[amendUserID]
      ,[amendDate]
      ,[tenantId]
	)
	SELECT
       [emailTemplateTypeId]
      ,[programmeComponentId]
      ,@tenantName + ' Existing User'
      ,[subject]
      ,[body]
      ,0
      ,4
      ,SYSDATETIMEOFFSET()
      ,@tenantId
	  FROM emailTemplateTBL
	  WHERE tenantId = @copyBulkUploadEmailTemplatesFromTenantId
		AND emailTemplateTypeId=4
		AND programmeComponentId = 11
		AND deleted = 0
	PRINT '   -Created'
END 
ELSE
BEGIN
PRINT '   -Skipped (Exists)'
END

PRINT 'Tenant Resource Strings'

INSERT INTO [tenantResourceTBL]
           ([tenantId],[resourceId],[resourceValue],[deleted],[amendUserId],[amendDate])
     VALUES
           (@tenantId,20,@emailFooter,0,4,sysdatetimeoffset())
          ,(@tenantId,21,@tenantDescription,0,4,sysdatetimeoffset())
          ,(@tenantId,22,@accessDenied,0,4,sysdatetimeoffset())		   
	PRINT '   -Created'


-- Create Terms and Conditions
IF NOT EXISTS (SELECT 1 FROM termsAndConditionsTBL Where tenantId = 10)
BEGIN
	INSERT INTO termsAndConditionsTBL (createdDate, description, details, tenantId, active, reportable, deleted, amendUserId, amendDate)
	SELECT TOP 1 SYSDATETIMEOFFSET(), 'Learning Hub Terms', details, @tenantId, 1, 1, 0, 0, SYSDATETIMEOFFSET()FROM termsAndConditionsTBL 
	WHERE tenantId = @copyTermsandConditionsFromTenantId
	ORDER BY termsAndConditionsId DESC
END

COMMIT
