-------------------------------------------------------------------------------
-- Author       Robert Smith
-- Created      19-06-2020
-- Purpose      Create a new ArticleResourceVersionFile. Called once for each article attachment.
--
-- Modification History
--
-- 19-06-2020  Robert Smith	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[ArticleResourceVersionFileCreate]
(
	@ArticleResourceVersionId int,
	@FileId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		INSERT INTO [resources].[ArticleResourceVersionFile]
				   ([ArticleResourceVersionId]
				   ,[FileId]
				   ,[Deleted]
				   ,[CreateUserId]
				   ,[CreateDate]
				   ,[AmendUserId]
				   ,[AmendDate])
			 VALUES
				   (@ArticleResourceVersionId
				   ,@FileId
				   ,0
				   ,@UserId
				   ,@AmendDate
				   ,@UserId
				   ,@AmendDate)

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