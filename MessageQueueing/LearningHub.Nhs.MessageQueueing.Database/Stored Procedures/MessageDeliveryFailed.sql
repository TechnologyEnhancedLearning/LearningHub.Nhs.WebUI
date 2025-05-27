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