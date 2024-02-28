-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      01-05-2020
-- Purpose      Creates the next "Draft" resource version
--
-- Modification History
--
-- 01-05-2020  Dave Brown	Initial Revision
-- 20 Jul 2020 Dave Brown	Addition of Sensitive Content Flag
-- 29 Sep 2020 Dave Brown	Addition of NodeResource records
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-- 17-08-2023  Jignesh Jetghwani Card-1318, Addition of ResourceNodeVersion Provider
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionCreateNext]
(
	@ResourceId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@ResourceVersionId int OUTPUT
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
			
		DECLARE @CurrentResourceTypeId int
		DECLARE @CurrentResourceVersionId int
		DECLARE @CurrentVersionStatusId int
		DECLARE @PublicationId int
		DECLARE @CurrentArticleResourceVersionId int
		DECLARE @NewArticleResourceVersionId int
			
		SELECT @CurrentResourceVersionId = r.CurrentResourceVersionId, @CurrentResourceTypeId = r.ResourceTypeId, @CurrentVersionStatusId = rv.VersionStatusId
		FROM resources.Resource r
		INNER JOIN resources.ResourceVersion rv ON r.CurrentResourceVersionId = rv.Id
		WHERE r.Id = @ResourceId

		Declare @ErrorMessage NVARCHAR(4000);

		IF @CurrentVersionStatusId != 2 /* Published */ AND @CurrentVersionStatusId != 3 /* Unpublished */
		BEGIN
			Set @ErrorMessage = 'Error - Cannot create a new version of a resource with a current status of ' + Convert(NVARCHAR(50), @CurrentVersionStatusId)
			RAISERROR (@ErrorMessage, -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN

		INSERT INTO resources.ResourceVersion (ResourceId, VersionStatusId,ResourceAccessibilityId, PublicationId, MajorVersion, MinorVersion, Title, Description, AdditionalInformation, ReviewDate, HasCost, Cost, ResourceLicenceId, SensitiveContent, CertificateEnabled, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT	ResourceId, 1 /* Draft */,ResourceAccessibilityId, null, MajorVersion, MinorVersion, Title, Description, AdditionalInformation, ReviewDate, HasCost, Cost, ResourceLicenceId, SensitiveContent, CertificateEnabled, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	resources.ResourceVersion
		WHERE	id = @CurrentResourceVersionId
		SELECT @ResourceVersionId = SCOPE_IDENTITY()

		INSERT INTO resources.ResourceVersionAuthor (ResourceVersionId, AuthorUserId, AuthorName, Organisation, Role, IsContributor, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT	@ResourceVersionId, AuthorUserId, AuthorName, Organisation, Role, IsContributor, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	resources.ResourceVersionAuthor
		WHERE	ResourceVersionId = @CurrentResourceVersionId
			AND	Deleted = 0

		INSERT INTO resources.ResourceVersionKeyword (ResourceVersionId, Keyword, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT	@ResourceVersionId, Keyword, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	resources.ResourceVersionKeyword
		WHERE	ResourceVersionId = @CurrentResourceVersionId
			AND	Deleted = 0

		INSERT INTO resources.ResourceVersionProvider (ResourceVersionId, ProviderId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT	@ResourceVersionId, ProviderId, 0, @UserId, @AmendDate, @UserId, @AmendDate
		FROM	resources.ResourceVersionProvider
		WHERE	ResourceVersionId = @CurrentResourceVersionId
			AND	Deleted = 0

		IF @CurrentResourceTypeId = 1
		BEGIN

			INSERT INTO resources.ArticleResourceVersion (ResourceVersionId, Content, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, Content, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.ArticleResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
			SELECT	@NewArticleResourceVersionId = SCOPE_IDENTITY()

			SELECT	@CurrentArticleResourceVersionId = Id
			FROM	resources.ArticleResourceVersion 
			WHERE	ResourceVersionId = @CurrentResourceVersionId

			INSERT INTO [resources].[ArticleResourceVersionFile] (ArticleResourceVersionId, FileId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@NewArticleResourceVersionId, FileId, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	[resources].[ArticleResourceVersionFile]
			WHERE	ArticleResourceVersionId = @CurrentArticleResourceVersionId AND Deleted = 0

		END
		IF @CurrentResourceTypeId = 2 
		BEGIN
			INSERT INTO resources.AudioResourceVersion (ResourceVersionId, AudioFileId, TranscriptFileId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, AudioFileId, TranscriptFileId, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.AudioResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 3 
		BEGIN
			INSERT INTO resources.EmbeddedResourceVersion (ResourceVersionId, EmbedCode, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, EmbedCode, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.EmbeddedResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END		
		IF @CurrentResourceTypeId = 4 
		BEGIN
			INSERT INTO resources.EquipmentResourceVersion (ResourceVersionId, ContactName, ContactTelephone, ContactEmail, AddressId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, ContactName, ContactTelephone, ContactEmail, AddressId, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.EquipmentResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 5 
		BEGIN
			INSERT INTO resources.ImageResourceVersion (ResourceVersionId, ImageFileId, AltTag, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, ImageFileId, AltTag, Description, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.ImageResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 6 
		BEGIN
			INSERT INTO resources.ScormResourceVersion(ResourceVersionId,FileId,ContentFilePath,Deleted,CreateUserId,CreateDate,AmendUserId,AmendDate,DevelopmentId,EsrLinkTypeId, CanDownload, ClearSuspendData, PopupWidth, PopupHeight)
			SELECT	@ResourceVersionId, FileId,ContentFilePath, 0, @UserId, @AmendDate, @UserId, @AmendDate, DevelopmentId, EsrLinkTypeId, CanDownload, ClearSuspendData, PopupWidth, PopupHeight		
			FROM	resources.ScormResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 7 
		BEGIN
			INSERT INTO resources.VideoResourceVersion (ResourceVersionId, VideoFileId, TranscriptFileId, ClosedCaptionsFileId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, VideoFileId, TranscriptFileId, ClosedCaptionsFileId, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.VideoResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 8 
		BEGIN
			INSERT INTO resources.WebLinkResourceVersion (ResourceVersionId, WebLinkURL, DisplayText, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, WebLinkURL, DisplayText, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.WebLinkResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 9 
		BEGIN
			INSERT INTO resources.GenericFileResourceVersion (ResourceVersionId, FileId, ScormAiccContent, AuthoredYear, AuthoredMonth, AuthoredDayOfMonth, EsrLinkTypeId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, FileId, ScormAiccContent, AuthoredYear, AuthoredMonth, AuthoredDayOfMonth, EsrLinkTypeId, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.GenericFileResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END
		IF @CurrentResourceTypeId = 12 
		BEGIN
			INSERT INTO resources.HtmlResourceVersion (ResourceVersionId, FileId, PopupWidth, PopupHeight, EsrLinkTypeId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			SELECT	@ResourceVersionId, FileId, PopupWidth, PopupHeight, EsrLinkTypeId, 0, @UserId, @AmendDate, @UserId, @AmendDate
			FROM	resources.HtmlResourceVersion
			WHERE	ResourceVersionId = @CurrentResourceVersionId
		END

		EXECUTE [resources].[ResourceVersionDraftEventCreate] @ResourceVersionId, @UserId, @UserTimezoneOffset

		COMMIT

	END TRY
	BEGIN CATCH
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