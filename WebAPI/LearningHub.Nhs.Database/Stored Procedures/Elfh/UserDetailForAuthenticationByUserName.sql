-------------------------------------------------------------------------
--	Jignesh Jethwani 27 Sept 2023  - Initial version
--  Tobi Awe		 24 Sept 2025  - swapped elfh user table to hub user table
--------------------------------------------------------------------------
CREATE PROCEDURE [elfh].[proc_UserDetailForAuthenticationByUserName]
(
	@userName varchar(100)
)
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	SET NOCOUNT ON
	DECLARE @Err int
	DECLARE @false bit
	SET @false = 0
	SELECT	elfhuser.Id as Id,
			elfhuser.userName,			
			ISNULL(elfhuser.passwordHash, '') AS 'passwordHash',				
			elfhuser.RestrictToSSO,
			up.Active,
			elfhuser.activeFromDate,
			elfhuser.activeToDate,			
			elfhuser.passwordLifeCounter,
			userAttribData.userAttributeId as OpenAthensUserAttributeId
	  FROM	
			[hub].[User] elfhuser
	  INNER JOIN hub.UserProfile up 
        ON elfhuser.Id = up.Id
	  OUTER APPLY
	   (
		SELECT 
			TOP 1 userAttrib.userAttributeId
		FROM 
			elfh.userAttributeTBL userAttrib 
		 INNER Join 
			elfh.attributeTBL attrib ON userAttrib.attributeId = attrib.attributeId AND lower(attrib.attributeName) = 'openathens_userid' AND userAttrib.deleted = 0 	  
		WHERE
			userAttrib.userId = elfhuser.Id
		) userAttribData
	  WHERE 
			elfhuser.userName = @userName
	  AND	
			elfhuser.deleted = 0
	SET @Err = @@Error
	RETURN @Err
END