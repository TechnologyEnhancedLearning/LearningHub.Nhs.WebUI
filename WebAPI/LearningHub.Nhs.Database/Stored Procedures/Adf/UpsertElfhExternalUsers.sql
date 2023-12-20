-------------------------------------------------------------------------------
-- Author       HV
-- Created      24-12-2021
-- Purpose      Syncs External System Users from Elfh with LH
-------------------------------------------------------------------------------
CREATE PROCEDURE [adf].[UpsertElfhExternalUsers]	
(
	@JobRunId int
)
AS
BEGIN

	BEGIN TRY

		DECLARE @createdCount int = 0
		DECLARE @updatedCount int = 0

		UPDATE
			esu
		SET
			esu.Deleted = elesu.active ^ 1,
			AmendUserId = 4,
			AmendDate = SYSDATETIMEOFFSET()
		FROM
			 [adf].[Staging_ElfhExternalSystemUser] elesu
		  JOIN [hub].[User] u on u.userName = elesu.UserName 
		  JOIN [external].[ExternalSystem] es on es.Code = elesu.Code
		  LEFT JOIN [external].[ExternalSystemUser] esu ON esu.UserId = u.id AND esu.ExternalSystemId = es.Id
		WHERE
		JobRunId = @JobRunId AND esu.UserId = u.Id AND esu.ExternalSystemId = es.Id AND esu.Deleted <> (elesu.active ^ 1)		
		
		SELECT @updatedCount = @@ROWCOUNT

		---- Create new External System User/s
		INSERT INTO  [external].[ExternalSystemUser]([UserId],[ExternalSystemId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT 
			u.Id AS UserId, 
			es.Id AS ExternalSystemId, 
			elesu.active ^ 1 AS Deleted,
			4 AS CreateUserId,
			SYSDATETIMEOFFSET() AS CreateDate,
			4 AS AmendUserId,
			SYSDATETIMEOFFSET() AS AmendDate		
		FROM [adf].[Staging_ElfhExternalSystemUser] elesu
		  JOIN [hub].[User] u on u.userName = elesu.UserName 
		  JOIN [external].[ExternalSystem] es on es.Code = elesu.Code
		  LEFT JOIN [external].[ExternalSystemUser] esu ON esu.UserId = u.id AND esu.ExternalSystemId = es.Id
		WHERE
		JobRunId = @JobRunId
		AND esu.UserId IS NULL AND esu.ExternalSystemId IS NULL
			
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
