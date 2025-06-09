-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Update message request status as failed.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[MessageDeliveryFailed]
	@Id int,
	@ErrorMessage nvarchar(max)
AS
BEGIN		
			UPDATE [dbo].[QueueRequests]
			SET 
				Status = 3,
				RetryCount = RetryCount + 1,
				ErrorMessage = @ErrorMessage,
				LastAttemptAt = SYSDATETIMEOFFSET()
			where Id = @Id;
END
GO