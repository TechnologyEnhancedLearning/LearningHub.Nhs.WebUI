CREATE PROCEDURE [messaging].[SetSendStatus]
	@MessageSendIds NVARCHAR(max),
	@UserId int,
	@SendStatus int
AS
	BEGIN TRY
		BEGIN TRAN
			UPDATE [messaging].[MessageSend]
			SET Status = @SendStatus, AmendUserId = @UserId
			FROM [messaging].[MessageSend] ms
			JOIN (select CONVERT(INT, value) as Id from STRING_SPLIT(@MessageSendIds, ',')) as SendIds
				ON SendIds.Id = ms.Id;
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
RETURN 0
