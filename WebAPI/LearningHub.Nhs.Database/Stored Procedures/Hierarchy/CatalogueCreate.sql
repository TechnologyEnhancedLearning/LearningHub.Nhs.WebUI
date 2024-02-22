-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-02-2021
-- Purpose      Creates a Catalogue.
--
-- Modification History
--
-- 01-02-2021  Killian Davies	Initial Revision
-- 11-08-2023  RS               Added CardImageUrl parameter
-- 01-08-2023  SA               Provided BY
-- 08-02-2024  SA               Added Previewer user group 
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueCreate]
(
	@NodeVersionStatusId int,
	@Name nvarchar(255),
	@Url nvarchar(1000),
	@BadgeUrl nvarchar(128) null,
	@CertificateUrl nvarchar(128) null,
	@CardImageUrl nvarchar(128),
	@BannerUrl nvarchar(128) null,
	@Order int,
	@Description nvarchar(4000),
	@UserId int,
	@Hidden bit,
	@Keywords nvarchar(max),
	@RestrictedAccess bit,
	@UserTimezoneOffset int = NULL,
	@ProviderId INT,
	@CatalogueNodeVersionId int output
)

AS

BEGIN
	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN

			DECLARE @NodeId int

			INSERT INTO [hierarchy].[Node] 
			([Description], [Name], [NodeTypeId], [Hidden], [Deleted], [AmendUserId], [AmendDate], [CreateUserId], [CreateDate])
			SELECT
				[Description] = 'Catalogue',
				[Name] = 'Catalogue',
				[NodeTypeId] = (SELECT Id FROM [hierarchy].[NodeType] WHERE [Name] = 'Catalogue'),
				[Hidden] = @Hidden,
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate,
				[CreateUserId] = @UserId,
				[CreateDate] = @AmendDate
			
			SELECT @NodeId = SCOPE_IDENTITY()
			
			DECLARE @NodePathId int

			INSERT INTO [hierarchy].[NodePath]
			([NodeId], [CatalogueNodeId], [NodePath], [Deleted], [AmendUserId], [AmendDate], [CreateUserId], [CreateDate], [IsActive])
			SELECT
				[NodeId] = @NodeId,
				[CatalogueNodeId] = @NodeId,
				[NodePath] = CONVERT(varchar(10), @NodeId),
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate,
				[CreateUserId] = @UserId,
				[CreateDate] = @AmendDate,
				[IsActive] = 1

			SELECT @NodePathId = SCOPE_IDENTITY()

			INSERT INTO [hierarchy].[NodePathNode]
			([NodePathId],[NodeId],[Depth],[Deleted],[CreateUserID],[CreateDate],[AmendUserID],[AmendDate])
			SELECT
				NodePathId = @NodePathId,
				NodeId = @NodeId,
				[Depth] = 0,
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = SYSDATETIMEOFFSET(),
				[CreateUserId] = @UserId,
				[CreateDate] = SYSDATETIMEOFFSET()

			DECLARE @NodeVersionId int

			INSERT INTO [hierarchy].[NodeVersion] 
			([NodeId], [MajorVersion], [MinorVersion], [Deleted], [VersionStatusId], [AmendUserId], [AmendDate], [CreateUserId], [CreateDate])
			SELECT
				[NodeId] = @NodeId,
				[MajorVersion] = 1,
				[MinorVersion] = 0,
				[Deleted] = 0,
				[VersionStatusId] = @NodeVersionStatusId,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate,
				[CreateUserId] = @UserId,
				[CreateDate] = @AmendDate

			SELECT @NodeVersionId = SCOPE_IDENTITY()

			UPDATE [hierarchy].[Node]
			SET CurrentNodeVersionId = @NodeVersionId
			WHERE Id = @NodeId

			INSERT INTO [hierarchy].[CatalogueNodeVersion]
			([NodeVersionId], [Name], [Description], [BadgeUrl],[CertificateUrl], [CardImageUrl], [BannerUrl], [Order], [Url], [RestrictedAccess], [Deleted], [AmendUserId], [AmendDate], [CreateUserId], [CreateDate])
			SELECT 
				[NodeVersionId] = @NodeVersionId,
				[Name] = @Name,
				[Description] = @Description,
				[BadgeUrl] = @BadgeUrl,
				[CertificateUrl] = @CertificateUrl,
				[CardImageUrl] = @CardImageUrl,
				[BannerUrl] = @BannerUrl,
				[Order] = @Order,
				[Url] = @Url,
				[RestrictedAccess] = @RestrictedAccess,
				[Deleted] = 0,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate,
				[CreateUserId] = @UserId,
				[CreateDate] = @AmendDate

			-- Assigning the output variable
			SELECT @CatalogueNodeVersionId = SCOPE_IDENTITY()
			IF @Keywords IS NOT NULL AND @Keywords != ''
				INSERT INTO [hierarchy].[CatalogueNodeVersionKeyword]
				SELECT 
					@CatalogueNodeVersionId as CatalogueNodeVersionId,
					[value] as Keyword,
					0 as Deleted,
					@UserId as CreateUserId,
					@AmendDate as CreateDate,
					@UserId as AmendUserId,
					@AmendDate as AmendDate
				FROM STRING_SPLIT(@Keywords, ',')
				
			-- Create Scope for the Catalogue
			DECLARE @ScopeId int
			INSERT INTO [hub].[Scope] ([ScopeTypeId],[CatalogueNodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			 SELECT 
				1 AS ScopeTypeId, -- Catalogue Scope Type
				@NodeId AS CatalogueNodeId,
				0 as Deleted,
				@UserId as CreateUserId,
				@AmendDate as CreateDate,
				@UserId as AmendUserId,
				@AmendDate as AmendDate
			SELECT @ScopeId = SCOPE_IDENTITY()

			IF 	@ProviderId >0
			BEGIN

			INSERT INTO [hierarchy].[CatalogueNodeVersionProvider]
			   ([CatalogueNodeVersionId]
			   ,[ProviderId]
			   ,[RemovalDate]
			   ,[Deleted]
			   ,[CreateUserId]
			   ,[CreateDate]
			   ,[AmendUserId]
			   ,[AmendDate])
				VALUES
					   (@CatalogueNodeVersionId
					   ,@ProviderId
					   ,NULL
					   ,0
					   ,@UserId
					   ,SYSDATETIMEOFFSET()
					   ,@UserId
					   ,SYSDATETIMEOFFSET())

			END

			IF @RestrictedAccess = 1
			BEGIN
				EXEC  [hierarchy].[UserGroupRestrictedAccessEnsure] @NodeId, @UserId, @UserTimezoneOffset
			END

			EXEC [hierarchy].[UserGroupPreviewerEnsure] @NodeId, @UserId, @UserTimezoneOffset

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