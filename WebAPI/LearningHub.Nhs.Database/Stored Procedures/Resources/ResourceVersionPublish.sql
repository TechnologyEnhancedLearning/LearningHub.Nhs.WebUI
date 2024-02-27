----------------------------------------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Publishes a ResourceVersion that is currently in "Draft"
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 11-05-2020  Dave brown		PublicationId returned as Output parameter
-- 09-06-2020  Robert Smith     Added creation of ResourceVersionRatingSummary record
-- 03-08-2020  Robert Smith     Added @PublicationDate parameter
-- 03-09-2020  Killian Davies	Moved event creation to Azure Function
-- 23-10-2020  Dave Brown		Draft NodeResource record updated to published
--								ResourceReference record added if not existing
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-- 29-01-2021  HV				Amended to add ScormResourceReference record
-- 01-04-2021  RobS				Added call to UpdateMigrationStatus stored proc for migration tool improvements.
-- 08-10-2021  Killian Davies	Modified to use hierarchy.NodeResourcePublish (IT1)
-- 21-12-2021  RobS             Fix to NodeResource update when republishing unpublished resource.
-- 10-10-2023  Dave Brown		Allow Generic File resource types to have ESR links.
-- 13-10-2023  Dharmendra Verma	Allow Html resource types to have ESR links.
----------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionPublish]
(
	@ResourceVersionId int,
	@MajorRevisionInd bit,
	@Notes nvarchar(4000),
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@PublicationId int OUTPUT,
	@PublicationDate DateTimeOffset = NULL
)

AS

BEGIN

	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
		SELECT @PublicationDate = COALESCE(@PublicationDate, @AmendDate)
		
		DECLARE @ResourceId int
		DECLARE @ExistingMajorRevisionInd int
		DECLARE @CatalogueNodeId int
		DECLARE @NodePathId int

		SELECT @ResourceId = ResourceId, @PublicationId = PublicationId, @ExistingMajorRevisionInd = MajorVersion
		FROM resources.ResourceVersion 
		WHERE Id = @ResourceVersionId

		IF @PublicationId IS NOT NULL
		BEGIN
			RAISERROR ('Error - ResourceVersion has already been published', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		IF @ExistingMajorRevisionInd IS NULL
		BEGIN
			SET @MajorRevisionInd = 1
		END

		BEGIN TRAN

		INSERT INTO [hierarchy].[Publication] ([ResourceVersionId],[NodeVersionId],[Notes],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			 VALUES (@ResourceVersionId, NULL, @Notes, 0, @UserId, @PublicationDate, @UserId, @PublicationDate)
		SELECT @PublicationId = SCOPE_IDENTITY();

		DECLARE @MajorVersion int
		DECLARE @MinorVersion int
		SELECT 
			@MajorVersion = MajorVersion,
			@MinorVersion = MinorVersion
		FROM 
			resources.ResourceVersion
		WHERE
			[Id] = (SELECT MAX(Id) FROM resources.ResourceVersion WHERE ResourceId=@ResourceId AND MajorVersion IS NOT NULL)
	
		IF @MajorRevisionInd = 1
		BEGIN
		
			UPDATE 
				resources.ResourceVersion
			SET
				VersionStatusId = 2,
				PublicationId = @PublicationId,
				MajorVersion=ISNULL(@MajorVersion, 0) + 1,
				MinorVersion = 0
			WHERE
				Id=@ResourceVersionId

		    -- If a new major version, ratings restart at zero.
			INSERT INTO [resources].[ResourceVersionRatingSummary] ([ResourceVersionId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
				VALUES (@ResourceVersionId, 0, @UserId, @AmendDate, @UserId, @AmendDate)

		END
		ELSE
		BEGIN

			UPDATE 
				resources.ResourceVersion
			SET
				VersionStatusId = 2,
				PublicationId = @PublicationId,
				MajorVersion=@MajorVersion,
				MinorVersion=ISNULL(@MinorVersion, 0) + 1
			WHERE
				Id=@ResourceVersionId

			-- If a new minor version, ratings continue from previous published minor version.
			DECLARE @PreviousPublishedResourceVersionId int
			SELECT @PreviousPublishedResourceVersionId = CurrentResourceVersionId FROM resources.[Resource] WHERE Id = @ResourceId

			INSERT INTO [resources].[ResourceVersionRatingSummary] ([ResourceVersionId],[AverageRating],[RatingCount],[Rating1StarCount],[Rating2StarCount],[Rating3StarCount],[Rating4StarCount],[Rating5StarCount],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
				SELECT @ResourceVersionId, AverageRating, RatingCount, Rating1StarCount, Rating2StarCount, Rating3StarCount, Rating4StarCount, Rating5StarCount, 0, @UserId, @AmendDate, @UserId, @AmendDate
				FROM [resources].[ResourceVersionRatingSummary] WHERE ResourceVersionId = @PreviousPublishedResourceVersionId

		END
	
		-- IT1 - Only 1 NodeResource in draft is possible. (May also be the case in IT2)
		-- When republishing an unpublished resource, the unpublished NodeResource will be updated back to published unless there is also 
		-- a draft NodeResource (the resource has moved catalogues since being unpublished), in which case the draft NodeResoruce takes precedence.
		DECLARE @NodeId int
		SELECT TOP 1 @NodeId = NodeId
		FROM	hierarchy.NodeResource nr
		INNER JOIN	hierarchy.Node n ON nr.NodeId = n.Id
		WHERE	nr.ResourceId = @ResourceId
			AND nr.VersionStatusId IN (1, 3) /* Draft or Unpublished */
			AND nr.Deleted = 0
		ORDER BY nr.VersionStatusId ASC

		IF @NodeId IS NOT NULL
		BEGIN
			EXECUTE hierarchy.NodeResourcePublish @NodeId, @ResourceId, @PublicationId, @UserId
		END

		DECLARE @ResourceTypeId INT
		SELECT	@ResourceTypeId = ResourceTypeId
		FROM	resources.Resource
		WHERE	Id = @ResourceId

		IF (@ResourceTypeId = 6 /* SCORM */ OR @ResourceTypeId = 9 /* GenericFile */ OR @ResourceTypeId = 12 /* HTML Resource */)
		BEGIN
			DECLARE @ExternalNodeId INT
			DECLARE @ResourceReferenceId INT

			SELECT	@ExternalNodeId = NodeId 
			FROM	[hub].[ExternalOrganisation] 
			WHERE	Id = 1  --ESR
				AND Deleted = 0

			SELECT	@NodePathId = Id
			FROM	hierarchy.NodePath
			WHERE	NodeId = @ExternalNodeId 
				AND Deleted = 0

			SELECT	@ResourceReferenceId = Id
			FROM	resources.ResourceReference
			WHERE	ResourceId = @ResourceId
				AND NodePathId = @NodePathId

			DECLARE @EsrLinkTypeId INT
			IF (@ResourceTypeId = 6 /* SCORM */)
			BEGIN
				SELECT	@EsrLinkTypeId = EsrLinkTypeId
				FROM	resources.ScormResourceVersion
				WHERE	ResourceVersionId = @ResourceVersionId
					AND Deleted = 0
			END
			ELSE IF (@ResourceTypeId = 12 /* HTML */)
			BEGIN -- HTML Resource
				SELECT	@EsrLinkTypeId = EsrLinkTypeId
				FROM	resources.HtmlResourceVersion
				WHERE	ResourceVersionId = @ResourceVersionId
					AND Deleted = 0
			END
			ELSE
			BEGIN -- GenericFile
				SELECT	@EsrLinkTypeId = EsrLinkTypeId
				FROM	resources.GenericFileResourceVersion
				WHERE	ResourceVersionId = @ResourceVersionId
					AND Deleted = 0
			END

			IF (@EsrLinkTypeId IN (2,3,4)) -- HAS ESR Link
			BEGIN

				IF (@ResourceReferenceId IS NULL)
				BEGIN
					INSERT INTO	resources.ResourceReference (ResourceId, NodePathId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
					VALUES (@ResourceId, @NodePathId, 0, @UserId, @AmendDate, @UserId, @AmendDate)
					SET @ResourceReferenceId = @@IDENTITY

					UPDATE resources.ResourceReference SET OriginalResourceReferenceId = Id WHERE Id = @ResourceReferenceId
				END

				IF NOT EXISTS (SELECT 1 FROM resources.ExternalReference WHERE ResourceReferenceId = @ResourceReferenceId AND Deleted = 0)
				BEGIN
					INSERT INTO	resources.ExternalReference(ResourceReferenceId, ExternalReference, Active, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
					VALUES (@ResourceReferenceId, NEWID(), 1, 0, @UserId, @AmendDate, @UserId, @AmendDate)
				END
				ELSE
				BEGIN
					UPDATE	resources.ExternalReference
					SET		Active = 1,
							AmendDate = @AmendDate,
							AmendUserId = @UserId
					WHERE	ResourceReferenceId = @ResourceReferenceId
						AND	Deleted = 0
						AND Active != 1
				END
			END
			Else
			BEGIN -- NO ESR Link
				UPDATE	resources.ExternalReference
				SET		Active = 0,
						AmendDate = @AmendDate,
						AmendUserId = @UserId
				WHERE	ResourceReferenceId = @ResourceReferenceId
					AND	Deleted = 0
					AND Active = 1
			END
		END

		UPDATE resources.[Resource] SET [CurrentResourceVersionId]=@ResourceVersionId WHERE Id=@ResourceId

		-- If this resource version was created via the Migration Tool, we need to update the status of the corresponding MigrationInputRecord.
		EXEC migrations.UpdateMigrationStatus @ResourceVersionId, 1
		EXEC migrations.PopulateScormEsrLinkUrl @ResourceVersionId, @UserId, @UserTimezoneOffset

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