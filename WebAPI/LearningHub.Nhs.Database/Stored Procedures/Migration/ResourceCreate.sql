-------------------------------------------------------------------------------
-- Author       RobS
-- Created      07-04-2020
-- Purpose      Create a new Resource. Create an initial "draft" version for the Resource.
--
-- Modification History
--
-- 07-04-2020  RobS	- Initial Revision.
-- 08-01-2020  RobS - Modifications for migration tool.
-- 03-03-2021  RobS - New fields added for new Staging Tables migration type.
-- 07-04-2021  RobS - Changes for SCORM support.
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[ResourceCreate]
(
	@DestinationNodeId int,
	@MigrationInputRecordId int,
	@ResourceTypeId int,
	@Title nvarchar(255),
	@Description nvarchar(1024),
	@UserId int,
	@SensitiveContentFlag bit,
	@ResourceVersionId int output,
	@FileId int = NULL,
	@ResourceLicenceId int = NULL,
	@AdditionalInformation nvarchar(250) = '',
	@ArticleBody nvarchar(max) = NULL,
	@ArticleResourceVersionId int = NULL output,
	@ESRLinkTypeId int = 1, -- 1 = Don't display
	@WebLinkURL nvarchar(1024) = NULL,
	@WebLinkDisplayText nvarchar(1024) = NULL,
	@YearAuthored int = NULL,
	@MonthAuthored int = NULL,
	@DayAuthored int = NULL,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN
		
		DECLARE @ResourceId int

		-- resources.Resource
		INSERT INTO [resources].[Resource] ([ResourceTypeId],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		SELECT @ResourceTypeId, 0, @UserId, @AmendDate, @UserId, @AmendDate
		SELECT @ResourceId = SCOPE_IDENTITY()

		-- Note: We add a line feed character (char(10)) to the description and article content to workaround a problem with editing the resource after migration.
		-- Without the line feed, the text editor in LH seems to add it in automatically, which confuses the logic in the page and stops the publish button from working.
		
		-- resources.ResourceVersion
		INSERT INTO [resources].[ResourceVersion]
			   ([ResourceId] ,[VersionStatusId],[Title],[Description],[AdditionalInformation],[HasCost],[Cost],[ResourceLicenceId],[SensitiveContent],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		SELECT @ResourceId, 1, @Title, @Description+char(10), @AdditionalInformation, 0, 0, @ResourceLicenceId, @SensitiveContentFlag, 0, @UserId, @AmendDate, @UserId, @AmendDate
		SELECT @ResourceVersionId = SCOPE_IDENTITY()

		-- Insert a record into NodeResource table to add the resource into the correct catalogue.
		INSERT INTO [hierarchy].[NodeResource] ([NodeId],[ResourceId],[VersionStatusId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					VALUES (@DestinationNodeId, @ResourceId, 1, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		
		EXECUTE [resources].[ResourceVersionDraftEventCreate] @ResourceVersionId, @UserId


		-- Article
		IF @ResourceTypeId = 1 
		BEGIN
			INSERT INTO [resources].[ArticleResourceVersion] ([ResourceVersionId],[Content],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, @ArticleBody+char(10), 0, @UserId, @AmendDate, @UserId, @AmendDate)
			SELECT @ArticleResourceVersionId = SCOPE_IDENTITY()
		END

		-- Audio
		IF @ResourceTypeId = 2
		BEGIN
			INSERT INTO [resources].[AudioResourceVersion] ([ResourceVersionId],[AudioFileId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, @FileId, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		END

		-- Embedded
		--IF @ResourceTypeId = 3
		--BEGIN
		--	-- Not yet implemented!
		--END

		-- Equipment
		--IF @ResourceTypeId = 4
		--BEGIN
		--	-- Not yet implemented!
		--END

		-- Image
		IF @ResourceTypeId = 5
		BEGIN
			INSERT INTO [resources].[ImageResourceVersion] ([ResourceVersionId],[ImageFileId],[AltTag],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, @FileId, 'Unnamed image for ' + @Title, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		END

		-- Scorm
		IF @ResourceTypeId = 6 
		BEGIN
			INSERT INTO [resources].[ScormResourceVersion] ([ResourceVersionId],[FileId],[EsrLinkTypeId],[CanDownload],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					VALUES (@ResourceVersionId, @FileId, @EsrLinkTypeId, 0, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		END

		-- Video
		IF @ResourceTypeId = 7 
		BEGIN
			INSERT INTO [resources].[VideoResourceVersion] ([ResourceVersionId],[VideoFileId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, @FileId, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		END

		-- Web Link
		IF @ResourceTypeId = 8
		BEGIN
			INSERT INTO [resources].[WebLinkResourceVersion] ([ResourceVersionId],[WebLinkURL],[DisplayText],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, @WebLinkURL, @WebLinkDisplayText, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		END

		-- Generic File
		IF @ResourceTypeId = 9 
		BEGIN
			INSERT INTO [resources].[GenericFileResourceVersion] ([ResourceVersionId],[FileId],[ScormAiccContent],[AuthoredYear],[AuthoredMonth],[AuthoredDayOfMonth],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, @FileId, 0, @YearAuthored, @MonthAuthored, @DayAuthored, 0, @UserId, @AmendDate, @UserId, @AmendDate)
		END

		-- Add a resource attribute to record the migrationInputRecordId against the resource.
		DECLARE @AttributeId int
		SELECT @AttributeId = Id FROM [hub].[Attribute] WHERE Name = 'Resource MigrationInputRecordId'

		INSERT INTO [resources].[ResourceAttribute] ([ResourceId],[AttributeId],[Value],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			 VALUES (@ResourceId, @AttributeId, @MigrationInputRecordId, 0, @UserId, @AmendDate, @UserId, @AmendDate)

		-- Set status of migration input record to created.
		UPDATE [migrations].[MigrationInputRecord] SET [MigrationInputRecordStatusId] = 4, [ExceptionDetails] = NULL, [ResourceVersionId] = @ResourceVersionId WHERE Id = @MigrationInputRecordId -- StatusId 4 = LHMetadataCreationComplete

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