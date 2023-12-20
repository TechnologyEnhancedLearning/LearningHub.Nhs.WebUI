CREATE PROCEDURE [messaging].[MessageSendFailed]
	@MessageSendIds NVARCHAR(max),
	@UserId int
AS
	BEGIN TRY
		BEGIN TRAN
			-- Any sends which have SendAttempts < 2, need to be reset to Pending and increment attempts
			-- If SendAttempts >= 2, set to Failed and increment attempts 
			

			UPDATE [messaging].[MessageSend]
			SET 
				-- Send attempts is incremented, if we have already made two failed attempts then
				-- this means the third has failed.
				Status = case
					WHEN ms.SendAttempts < 2 THEN 0 /* Pending */
					WHEN ms.SendAttempts >= 2 THEN 3 /* Failed */
				END,
				AmendUserId = @UserId,
				SendAttempts = ms.SendAttempts + 1
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
