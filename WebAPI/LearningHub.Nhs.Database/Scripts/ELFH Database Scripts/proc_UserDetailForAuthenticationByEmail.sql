-------------------------------------------------------------------------
--	23-Apr-2026 SA Initial Version
--------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[proc_UserDetailForAuthenticationByEmail]
(
	@emailaddress varchar(100)
)
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	SET NOCOUNT ON
	DECLARE @Err int
	DECLARE @false bit
	SET @false = 0
	SELECT	elfhuser.userId as Id,
			elfhuser.userName,			
			ISNULL(elfhuser.passwordHash, '') AS 'passwordHash',				
			elfhuser.RestrictToSSO,
			elfhuser.active,
			elfhuser.activeFromDate,
			elfhuser.activeToDate,			
			elfhuser.passwordLifeCounter,
			userAttribData.userAttributeId as OpenAthensUserAttributeId
	  FROM	
			dbo.userTBL elfhuser
	  OUTER APPLY
	   (
		SELECT 
			TOP 1 userAttrib.userAttributeId
		FROM 
			dbo.userAttributeTBL userAttrib 
		 INNER Join 
			dbo.attributeTBL attrib ON userAttrib.attributeId = attrib.attributeId AND lower(attrib.attributeName) = 'openathens_userid' AND userAttrib.deleted = 0 	  
		WHERE
			userAttrib.userId = elfhuser.userId
		) userAttribData
	  WHERE 
			elfhuser.emailAddress = @emailaddress
			 AND active = 1
             AND deleted = 0
	  AND	
			elfhuser.deleted = 0
	SET @Err = @@Error
	RETURN @Err
END
