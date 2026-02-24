-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Update message request status as success.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[MessageDeliverySuccess]
	@Id int,
	@NotificationId nvarchar(100)

AS
BEGIN	
			UPDATE [dbo].[QueueRequests]
			SET 
				Status = 2,
				NotificationId = @NotificationId,
				RetryCount = RetryCount + 1,
				SentAt = SYSDATETIMEOFFSET(),
				LastAttemptAt = SYSDATETIMEOFFSET()
			where Id = @Id;
END
GO