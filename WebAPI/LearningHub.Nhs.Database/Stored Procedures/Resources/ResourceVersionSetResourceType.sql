-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Sets the ResourceType for the supplied ResourceVersionId
--				Creates blank entry for the relevant sub-type
--				Only permitted when a ResourceVersion is in "draft"
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionSetResourceType]
(
	@ResourceVersionId int,
	@ResourceTypeId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
			
		DECLARE @CurrentResourceTypeId int
		DECLARE @ResourceId int
		DECLARE @PublicationId int

		SELECT @ResourceId = r.Id, @CurrentResourceTypeId = r.ResourceTypeId, @PublicationId = rv.PublicationId 
		FROM resources.ResourceVersion rv
		INNER JOIN resources.[Resource] r ON rv.ResourceId = r.Id
		WHERE rv.Id = @ResourceVersionId

		IF @PublicationId IS NOT NULL
		BEGIN
			RAISERROR ('Error - Cannot set the ResourceType on a ResourceVersion that has been published', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN

		IF @CurrentResourceTypeId = 1
		BEGIN
			UPDATE avf
			SET	Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			FROM [resources].[ArticleResourceVersionFile] avf
			INNER JOIN resources.ArticleResourceVersion av ON avf.ArticleResourceVersionId = av.Id
		    WHERE av.ResourceVersionId = @ResourceVersionId
			
			UPDATE resources.ArticleResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 2 
		BEGIN
			UPDATE resources.AudioResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 3 
		BEGIN
			UPDATE resources.EmbeddedResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END		
		IF @CurrentResourceTypeId = 4 
		BEGIN
			UPDATE resources.EquipmentResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 5 
		BEGIN
			UPDATE resources.ImageResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 6 
		BEGIN
			UPDATE resources.ScormResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 7 
		BEGIN
			UPDATE resources.VideoResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 8 
		BEGIN
			UPDATE resources.WebLinkResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 9 
		BEGIN
			UPDATE resources.GenericFileResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END

		IF @ResourceTypeId = 1 -- Article Resource Type
		BEGIN
			UPDATE [resources].[ArticleResourceVersion]
			SET	[Content] = '',
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate
			WHERE [ResourceVersionId] = @ResourceVersionId
			IF @@ROWCOUNT = 0
			BEGIN
				INSERT INTO [resources].[ArticleResourceVersion] ([ResourceVersionId],[Content],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					 VALUES (@ResourceVersionId, '', 0, @UserId, @AmendDate, @UserId, @AmendDate)
			END
		END

		IF @ResourceTypeId = 6 --Scorm Resource Type
		BEGIN
			UPDATE [resources].[ScormResourceVersion]
			SET	FileId = NULL,
				[ContentFilePath] = NULL,
				[DevelopmentId] = NULL, 
				EsrLinkTypeId = 1, 
				CanDownload = 0,
				PopupWidth = NULL,
				PopupHeight = NULL,
				[ClearSuspendData] = 0,
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate
			WHERE [ResourceVersionId] = @ResourceVersionId
			IF @@ROWCOUNT = 0
			BEGIN
				INSERT INTO [resources].[ScormResourceVersion] ([ResourceVersionId],[FileId],[ContentFilePath],[DevelopmentId],[EsrLinkTypeId],[CanDownload],[PopupWidth],[PopupHeight],[ClearSuspendData],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					VALUES (@ResourceVersionId, NULL, NULL, NULL, 1, 0 ,NULL, NULL, 0, 0, @UserId, @AmendDate, @UserId, @AmendDate)
			END
		END

		IF @ResourceTypeId = 8 -- Web Link Resource Type
		BEGIN
			UPDATE [resources].[WebLinkResourceVersion]
			SET	[WebLinkURL] = '',
				[DisplayText] = '',
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate
			WHERE [ResourceVersionId] = @ResourceVersionId
			IF @@ROWCOUNT = 0
			BEGIN
				INSERT INTO [resources].[WebLinkResourceVersion] ([ResourceVersionId],[WebLinkURL],[DisplayText],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
					VALUES (@ResourceVersionId, '', '', 0, @UserId, @AmendDate, @UserId, @AmendDate)
			END
		END

		UPDATE resources.[Resource]
		SET ResourceTypeId = @ResourceTypeId, AmendUserId = @UserId, AmendDate=@AmendDate 
		WHERE Id = @ResourceId

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

