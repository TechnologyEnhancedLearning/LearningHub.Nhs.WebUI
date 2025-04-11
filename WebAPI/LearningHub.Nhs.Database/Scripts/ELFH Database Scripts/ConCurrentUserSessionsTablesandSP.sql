ALTER TABLE [dbo].[userHistoryTBL]
ADD sessionId NVARCHAR(50) NULL,
    isActive BIT NULL;

GO

--------------------------------------------------------------------------
--	Chris Bain	   08 Aug 2014 - Initial Build
--  Killian Davies 12 Aug 2015 - Added userAgent info (for Login history event)
--  Chris Bain     18 Nov 2016 - Added tenant Id
--  Jignesh Jethwani     02 Mar 2018 - Save details to User History Attribute, add url referer
--  Swapnamol Abraham    01 Apr 2025  - Save session and isactive details
--------------------------------------------------------------------------
ALTER PROCEDURE  [dbo].[proc_UserHistoryInsert]
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
	@sessionId          NVARCHAR(50) = NULL,
	@isActive           bit  = NULL,
	@amendUserId		INT,
	@amendDate			DATETIMEOFFSET  = NULL
AS
BEGIN

	DECLARE @UserHistoryId int		
	DECLARE @detailInfoAttributeId int, @userAgentAttributeId int, @browserNameAttributeId int, @browserVersionAttributeId int, @urlRefererAttributeId int, @loginSuccessFulAttributeId int
	SET @amendDate = CoalEsce(@amendDate,SysDateTimeOffset())

	INSERT INTO userHistoryTBL (userId, userHistoryTypeId, tenantId,sessionId,isActive, createdDate)
	VALUES (@userId, @historyTypeId, @tenantId, @sessionId,@isActive, @amendDate)

	If(@historyTypeId = 10)
	BEGIN
	UPDATE userHistoryTBL SET isActive = 0 Where UserId = @userId AND isActive =1

	END

	SET @UserHistoryId = CAST(SCOPE_IDENTITY() AS int)

	SELECT @detailInfoAttributeId = attributeId FROM [dbo].[attributeTBL] WHERE [attributeName] = 'UserHistory_DetailedInfo' AND deleted = 0
	SELECT @userAgentAttributeId = attributeId FROM [dbo].[attributeTBL] WHERE [attributeName] = 'UserHistory_UserAgent' AND deleted = 0
	SELECT @browserNameAttributeId = attributeId FROM [dbo].[attributeTBL] WHERE [attributeName] = 'UserHistory_BrowserName' AND deleted = 0
	SELECT @browserVersionAttributeId = attributeId FROM [dbo].[attributeTBL] WHERE [attributeName] = 'UserHistory_BrowserVersion' AND deleted = 0
	SELECT @urlRefererAttributeId = attributeId FROM [dbo].[attributeTBL] WHERE [attributeName] = 'UserHistory_UrlReferer' AND deleted = 0	
	SELECT @loginSuccessFulAttributeId = attributeId FROM [dbo].[attributeTBL] WHERE [attributeName] = 'UserHistory_LoginSuccessful' AND deleted = 0

	-- DetailedInfo
	IF @detailInfoAttributeId > 0 AND @detailedInfo IS NOT NULL
	BEGIN
		EXECUTE		[dbo].[proc_UserHistoryAttributeSave] null,
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
		
		EXECUTE		[dbo].[proc_UserHistoryAttributeSave] null,
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
		
		EXECUTE [dbo].[proc_UserHistoryAttributeSave] null,
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

		
		EXECUTE  [dbo].[proc_UserHistoryAttributeSave] null,
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
		
		EXECUTE [dbo].[proc_UserHistoryAttributeSave] null,
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
		
		EXECUTE [dbo].[proc_UserHistoryAttributeSave] null,
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
GO


--------------------------------------------------------------------------------
-- Swapnamol Abraham	01 Apr 2024	Initial Revision - services LH active user list
--------------------------------------------------------------------------------

CREATE PROCEDURE  [dbo].[proc_ActiveLearningHubUserbyId]
	@userId			int
AS
	DECLARE @History Table(userHistoryId int)

	DECLARE @ItemsReturned INT
	DECLARE @StartRow INT
	DECLARE @EndRow INT

	INSERT INTO @History
	SELECT
		userHistoryId
	FROM
		userHistoryTBL
	WHERE
		userId = @userId
		and tenantId = 10
		AND isActive = 1
	ORDER BY 
		createdDate DESC
	

	SELECT 
		uh.*,
		uht.[Description],
		ISNULL(t.tenantName,'Unknown') as tenantName
	FROM userHistoryVW uh
		INNER JOIN @History h ON h.userHistoryId = uh.userHistoryId
		INNER JOIN userHistoryTypeTBL uht ON uht.UserHistoryTypeId = uh.userHistoryTypeId
		LEFT JOIN tenantTBL t ON t.tenantId = uh.tenantId
