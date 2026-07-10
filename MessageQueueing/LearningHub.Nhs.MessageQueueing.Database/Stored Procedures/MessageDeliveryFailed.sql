-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Update message request status as failed.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-- 08-06-2026  SA   TD-7354
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[MessageDeliveryFailed]
	@Id int,
	@ErrorMessage nvarchar(max),
	@UserTimezoneOffset int = NULL
AS
BEGIN		
       
	   DECLARE @LastAttemptDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

			UPDATE [dbo].[QueueRequests]
			SET 
				Status = 3,
				RetryCount = RetryCount + 1,
				ErrorMessage = @ErrorMessage,
				LastAttemptAt = @LastAttemptDate
			where Id = @Id;
END
GO