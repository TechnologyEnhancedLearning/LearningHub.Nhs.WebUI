
--------------------------------------------------------------------------
--	Chris Bain	   08 Aug 2014 - Initial Build
--  Killian Davies 12 Aug 2015 - Added userAgent info (for Login history event)
--  Chris Bain     18 Nov 2016 - Added tenant Id
--  Jignesh Jethwani     02 Mar 2018 - Save details to User History Attribute, add url referer
--------------------------------------------------------------------------
CREATE PROCEDURE  [elfh].[proc_UserHistoryInsert]
	@userId				int = 0,
	@historyTypeId		int,
	@detailedInfo		NVARCHAR(1000) = NULL,
	@userAgent			NVARCHAR(1000) = NULL,
	@browserName		NVARCHAR(1000) = NULL,
	@browserVersion		NVARCHAR(1000) = NULL,
	@urlReferer         NVARCHAR(1000) = NULL,	
	@loginIP			NVARCHAR(50)   = NULL,
	@loginSuccessFul	bit   = NULL,
	@tenantId			INT,
	@amendUserId		INT,
	@amendDate			DATETIMEOFFSET  = NULL
AS
BEGIN

	DECLARE @UserHistoryId int		
	DECLARE @detailInfoAttributeId int, @userAgentAttributeId int, @browserNameAttributeId int, @browserVersionAttributeId int, @urlRefererAttributeId int, @loginSuccessFulAttributeId int
	SET @amendDate = CoalEsce(@amendDate,SysDateTimeOffset())

	INSERT INTO userHistoryTBL (userId, userHistoryTypeId, tenantId,createdDate)
	VALUES (@userId, @historyTypeId, @tenantId, @amendDate)


	SET @UserHistoryId = CAST(SCOPE_IDENTITY() AS int)

	SELECT @detailInfoAttributeId = attributeId FROM [elfh].[attributeTBL] WHERE [attributeName] = 'UserHistory_DetailedInfo' AND deleted = 0
	SELECT @userAgentAttributeId = attributeId FROM [elfh].[attributeTBL] WHERE [attributeName] = 'UserHistory_UserAgent' AND deleted = 0
	SELECT @browserNameAttributeId = attributeId FROM [elfh].[attributeTBL] WHERE [attributeName] = 'UserHistory_BrowserName' AND deleted = 0
	SELECT @browserVersionAttributeId = attributeId FROM [elfh].[attributeTBL] WHERE [attributeName] = 'UserHistory_BrowserVersion' AND deleted = 0
	SELECT @urlRefererAttributeId = attributeId FROM [elfh].[attributeTBL] WHERE [attributeName] = 'UserHistory_UrlReferer' AND deleted = 0	
	SELECT @loginSuccessFulAttributeId = attributeId FROM [elfh].[attributeTBL] WHERE [attributeName] = 'UserHistory_LoginSuccessful' AND deleted = 0

	-- DetailedInfo
	IF @detailInfoAttributeId > 0 AND @detailedInfo IS NOT NULL
	BEGIN
		EXECUTE		[elfh].[proc_UserHistoryAttributeSave] null,
														  @UserHistoryId,
														  @detailInfoAttributeId,	
														  NULL,
														  @detailedInfo, -- textValue,
														  NULL, -- booleanValue,
														  NULL, -- dateValue,
														  0,	-- deleted
														  @amendUserId,
														  @amendDate
	END

	-- User Agent
	IF @userAgentAttributeId > 0  AND @userAgent IS NOT NULL
	BEGIN
		
		EXECUTE		[elfh].[proc_UserHistoryAttributeSave] null,
														@UserHistoryId,
														@userAgentAttributeId,	
														NULL,	-- intValue
														@userAgent, -- textValue,
														NULL, -- booleanValue,
														NULL, -- dateValue,
														0,	-- deleted
														@amendUserId,
														@amendDate
	END

	-- Browser Name
	IF @browserNameAttributeId > 0 AND @browserName IS NOT NULL
	BEGIN
		
		EXECUTE [elfh].[proc_UserHistoryAttributeSave] null,
													  @UserHistoryId,
													  @browserNameAttributeId,
													  NULL,	-- intValue
													  @browserName, -- textValue,
													  NULL, -- booleanValue,
													  NULL, -- dateValue,
													  0,	-- deleted
													  @amendUserId,
													  @amendDate

	END

	-- Browser Version
	IF @browserVersionAttributeId > 0 AND @browserVersion IS NOT NULL
	BEGIN

		
		EXECUTE  [elfh].[proc_UserHistoryAttributeSave] null,
													  @UserHistoryId,
													  @browserVersionAttributeId,
													  NULL,	-- intValue													 
													  @browserVersion,
													  NULL, -- booleanValue,
													  NULL, -- dateValue,
													  0,	-- deleted
													  @amendUserId,
													  @amendDate
	END

	
	-- Url Referer
	IF @urlRefererAttributeId > 0 AND @urlReferer IS NOT NULL
	BEGIN
		
		EXECUTE [elfh].[proc_UserHistoryAttributeSave] null,
													  @UserHistoryId,
													  @urlRefererAttributeId,
													  NULL,	-- intValue
													  @urlReferer, -- textValue,													  
													  NULL, -- booleanValue,													 
													  NULL, -- dateValue,
													  0,	-- deleted
													  @amendUserId,
													  @amendDate
	
	END	

	
	-- Login SuccessFul
	IF @loginSuccessFulAttributeId  > 0 AND @loginSuccessFul IS NOT NULL
	BEGIN
		
		EXECUTE [elfh].[proc_UserHistoryAttributeSave] null,
													  @UserHistoryId,
													  @loginSuccessFulAttributeId,
													  NULL,	-- intValue
													  @loginIP, -- textValue,													  
													  @loginSuccessFul, -- booleanValue,													 
													  NULL, -- dateValue,
													  0,	-- deleted
													  @amendUserId,
													  @amendDate
	
	END
	

END