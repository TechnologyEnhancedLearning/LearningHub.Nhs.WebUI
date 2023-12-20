-------------------------------------------------------------------------------
-- Author       Robert Smith
-- Created      07-04-2020
-- Purpose      Create a new ResourceVersionAuthor.
--
-- Modification History
--
-- 07-04-2020  RobS - Initial Revision
-- 03-03-2021  RobS - IAmTheAuthor parameter added for new Staging Tables migration type.
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[ResourceVersionAuthorCreate]
(
	@ResourceVersionId int,
	@IAmTheAuthor bit,
	@AuthorName nvarchar(100),
	@Organisation nvarchar(100),
	@Role nvarchar(100),
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

	INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId]
           ,[AuthorUserId]
           ,[AuthorName]
		   ,[Organisation]
		   ,[Role]
		   ,[IsContributor]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (@ResourceVersionId
           , CASE WHEN @IAmTheAuthor = 1 THEN @UserId
		     ELSE NULL
			 END
           ,@AuthorName
		   ,@Organisation
		   ,@Role
		   ,@IAmTheAuthor
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