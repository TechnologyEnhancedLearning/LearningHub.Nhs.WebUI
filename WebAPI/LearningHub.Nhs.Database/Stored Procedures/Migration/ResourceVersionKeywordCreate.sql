-------------------------------------------------------------------------------
-- Author       Robert Smith
-- Created      07-04-2020
-- Purpose      Create a new ResourceVersionKeyword.
--
-- Modification History
--
-- 07-04-2020  Robert Smith	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[ResourceVersionKeywordCreate]
(
	@ResourceVersionId int,
	@Keyword nvarchar(100),
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

	INSERT INTO [resources].[ResourceVersionKeyword]
           ([ResourceVersionId]
           ,[Keyword]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (@ResourceVersionId
           ,@Keyword
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