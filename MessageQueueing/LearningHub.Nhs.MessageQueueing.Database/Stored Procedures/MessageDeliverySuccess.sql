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