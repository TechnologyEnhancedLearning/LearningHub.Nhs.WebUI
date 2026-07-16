-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Update message request status as success.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-- 08-06-2026  SA   TD-7354
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[MessageDeliverySuccess]
	@Id int,
	@NotificationId nvarchar(100),
	@UserTimezoneOffset int = NULL

AS
BEGIN	
DECLARE @SentDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
			UPDATE [dbo].[QueueRequests]
			SET 
				Status = 2,
				NotificationId = @NotificationId,
				RetryCount = RetryCount + 1,
				SentAt = @SentDate,
				LastAttemptAt = @SentDate
			where Id = @Id;
END
GO