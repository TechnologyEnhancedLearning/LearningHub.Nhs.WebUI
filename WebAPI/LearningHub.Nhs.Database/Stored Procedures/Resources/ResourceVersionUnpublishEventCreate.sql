-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Create an "Unpublish" Resource Version Event.
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionUnpublishEventCreate]
(
	@ResourceVersionId int,
	@Details nvarchar(1024),
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
		
	DECLARE @ResourceVersionEventTypeId int=3

	SELECT @Details = CASE WHEN ISNULL(@Details,'') = '' THEN 'Unpublished by ' + [UserName]
						   ELSE 'Unpublished by ' + [UserName] + ': ' + ISNULL(@Details,'') 
					  END
	FROM hub.[User]
	WHERE Id = @UserId

	EXECUTE [resources].[ResourceVersionEventCreate]  @ResourceVersionId, @ResourceVersionEventTypeId, @Details, @UserId, @UserTimezoneOffset

END

GO