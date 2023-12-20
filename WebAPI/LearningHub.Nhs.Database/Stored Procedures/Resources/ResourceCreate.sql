-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Create a new Resource. Create an initial "draft" version for the Resource.
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 25-02-2020  Dave Brown		Additional ResourceVersion columns added (HasCost, Cost) & ResourceVersionId retuned
-- 28-09-2020  Dave Brown		Creation of [resources].[ResourceReference] moved to publishing process
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-- 26-01-2023  RobS             Increased @Title param from 128 to 255 chars to match table
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceCreate]
(
	@ResourceTypeId int,
	@Title nvarchar(255),
	@Description nvarchar(1024),
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@ResourceVersionId int output
)

AS

BEGIN

	DECLARE @CommunityCatalogueNodeId int
	SET		@CommunityCatalogueNodeId = 1

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN
		
		DECLARE @ResourceId int

		INSERT INTO [resources].[Resource]([ResourceTypeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT 
			@ResourceTypeId, 
			Deleted = 0, 
			CreateUserId = @UserId, 
			CreateDate = @AmendDate, 
			AmendUserId = @UserId, 
			AmendDate = @AmendDate

		SELECT @ResourceId = SCOPE_IDENTITY()

		INSERT INTO [resources].[ResourceVersion]
			   ([ResourceId],[VersionStatusId],[PublicationId],[MajorVersion],[MinorVersion],[Title],[Description],[AdditionalInformation],[ReviewDate],[HasCost],[Cost],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT 
			ResourceId = @ResourceId, 
			VersionStatusId = 1, 
			PublicationId = NULL, 
			MajorVersion = NULL, 
			MinorVersion = NULL, 
			Title = @Title, 
			[Description] = @Description, 
			AdditionalInformation = '', 
			ReviewDate = NULL, 
			HasCost = 0, 
			Cost = 0, 
			Deleted = 0, 
			CreateUserId = @UserId, 
			CreateDate = @AmendDate, 
			AmendUserId = @UserId, 
			AmendDate = @AmendDate

		SELECT @ResourceVersionId = SCOPE_IDENTITY()
		
		EXECUTE [resources].[ResourceVersionDraftEventCreate] @ResourceVersionId, @UserId, @UserTimezoneOffset

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
GO