-------------------------------------------------------------------------------
-- Author       Robert Smith
-- Created      19-06-2020
-- Purpose      Create a new resources.File.
--
-- Modification History
--
-- 19-06-2020  Robert Smith	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[ResourceFileCreate]
(
	@FileTypeId int,
	@FileName nvarchar(128),
	@FilePath nvarchar(1024),
	@FileSizeKb int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@FileId int OUTPUT
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		INSERT INTO [resources].[File]
			   ([FileTypeId]
			   ,[FileName]
			   ,[FilePath]
			   ,[FileSizeKb]
			   ,[Deleted]
			   ,[CreateUserId]
			   ,[CreateDate]
			   ,[AmendUserId]
			   ,[AmendDate])
		 VALUES
			   (@FileTypeId
			   ,@FileName
			   ,@FilePath
			   ,@FileSizeKb
			   ,0
			   ,@UserId
			   ,@AmendDate
			   ,@UserId
			   ,@AmendDate)

		SELECT @FileId = SCOPE_IDENTITY()

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