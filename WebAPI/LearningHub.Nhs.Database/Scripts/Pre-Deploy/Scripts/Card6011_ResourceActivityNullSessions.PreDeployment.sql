/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Killian Davies - 11 Jun 2020
	Card 6011 - Clear any resource activity associated with a null session (userId=0)
				Prior to application of FK_ResourceActivity_CreateUser
--------------------------------------------------------------------------------------
*/

IF EXISTS(SELECT 1 FROM activity.ResourceActivity ra LEFT JOIN hub.[User] u ON ra.CreateUserID=u.Id WHERE u.Id IS NULL)
BEGIN

	DELETE ra
	FROM activity.ResourceActivity ra
	LEFT JOIN hub.[User] u ON ra.CreateUserID=u.Id
	WHERE u.Id IS NULL

END

GO