-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-02-2021
-- Purpose      Loads e-LfH users from staging
--
-- Modification History
--
-- 23-02-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [adf].[LoadElfhUsers]
(
	@JobRunId int
)

AS
BEGIN

	BEGIN TRY

		DECLARE @createdCount int = 0
		DECLARE @updatedCount int = 0

		-- Update existing User/s
		UPDATE
			u
		SET
			UserName = elfh_user.userName,
			Deleted = elfh_user.deleted,
			AmendUserId = 4,
			AmendDate = SYSDATETIMEOFFSET()
		FROM
			hub.[User] u
		INNER JOIN 
			adf.Staging_Elfh_User elfh_user 
			ON u.Id = elfh_user.userId 
			AND (u.UserName != elfh_user.userName
				OR (u.Deleted != elfh_user.deleted))
		WHERE
			elfh_user.JobRunId = @JobRunId
		
		SELECT @updatedCount = @@ROWCOUNT

		-- Create new User/s
		INSERT INTO [hub].[User]([Id],[UserName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT 
			elfh_user.userId AS Id, 
			elfh_user.userName AS UserName, 
			elfh_user.deleted AS Deleted,
			4 AS CreateUserId,
			SYSDATETIMEOFFSET() AS CreateDate,
			4 AS AmendUserId,
			SYSDATETIMEOFFSET() AS AmendDate
		FROM
			adf.Staging_Elfh_User elfh_user
		LEFT JOIN
			hub.[User] u ON elfh_user.userId = u.Id
		WHERE
			JobRunId = @JobRunId
			AND u.Id IS NULL
			
		SELECT @createdCount = @@ROWCOUNT

		SELECT 
			CreatedCount = @createdCount, 
			UpdatedCount = @updatedCount

	END TRY
	BEGIN CATCH
	    DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  
  
		SELECT   
			@ErrorMessage = ERROR_MESSAGE(),  
			@ErrorSeverity = ERROR_SEVERITY(),  
			@ErrorState = ERROR_STATE();  

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  

	END CATCH

END
GO