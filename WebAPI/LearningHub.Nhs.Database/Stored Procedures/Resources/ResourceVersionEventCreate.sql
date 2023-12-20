-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Create a Resource Version Event.
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionEventCreate]
(
	@ResourceVersionId int,
	@ResourceVersionEventTypeId int,
	@Details nvarchar(1024),
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
		
	INSERT INTO [resources].[ResourceVersionEvent] ([ResourceVersionId],[ResourceVersionEventTypeId],[Details],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	SELECT @ResourceVersionId, @ResourceVersionEventTypeId, @Details, 0, @UserId, @AmendDate, @UserId, @AmendDate

END

GO