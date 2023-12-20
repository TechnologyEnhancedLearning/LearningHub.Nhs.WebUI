CREATE PROCEDURE [messaging].[CreateNotificationForUser]
(
	@Subject nvarchar(512),
	@Body nvarchar(max),
	@RecipientUserId int,
	@UserId int,
	@NotificationStartDate datetimeoffset,
	@NotificationEndDate datetimeoffset,
	@NotificationPriority int,
	@NotificationType int,
	@UserTimezoneOffset int = NULL
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRAN
			DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
			DECLARE @MessageId int;

			INSERT INTO [messaging].[Message] (Subject, Body, CreateDate, CreateUserId, AmendDate, AmendUserId, Deleted)
			VALUES (@Subject, @Body, @AmendDate, @UserId, @AmendDate, @UserId, 0);

			SELECT @MessageId = SCOPE_IDENTITY();

			DECLARE @MessageSendId int;

			INSERT INTO [messaging].[MessageSend] (MessageId, MessageTypeId, CreateDate, CreateUserId, AmendDate, AmendUserId, Deleted)
			VALUES (@MessageId, 2, @AmendDate, @UserId, @AmendDate, @UserId, 0);

			SELECT @MessageSendId = SCOPE_IDENTITY();

			INSERT INTO [messaging].[MessageSendRecipient] (UserId, MessageSendId, CreateDate, CreateUserId, AmendDate, AmendUserId, Deleted)
			VALUES (@RecipientUserId, @MessageSendId, @AmendDate, @UserId, @AmendDate, @UserId, 0);

			INSERT INTO [messaging].[MessageMetaData] (MessageId, NotificationEndDate, NotificationStartDate, NotificationPriority, NotificationType, CreateDate, CreateUserId, AmendDate, AmendUserId, Deleted)
			VALUES (@MessageId, @NotificationEndDate, @NotificationStartDate, @NotificationPriority, @NotificationType, @AmendDate, @UserId, @AmendDate, @UserId, 0)

		COMMIT
	END TRY
	BEGIN CATCH
	    DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  
  
		SELECT   
			@ErrorMessage = ERROR_MESSAGE(),  
			@ErrorSeverity = ERROR_SEVERITY(),  
			@ErrorState = ERROR_STATE();  
  
		IF @@TRANCOUNT > 0  
			ROLLBACK TRAN;  

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  

	END CATCH
END
RETURN 0
