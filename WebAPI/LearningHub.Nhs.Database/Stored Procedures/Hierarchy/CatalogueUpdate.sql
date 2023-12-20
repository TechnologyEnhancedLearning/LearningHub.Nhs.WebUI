-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-02-2021
-- Purpose      Updates a Catalogue.
--
-- Modification History
--
-- 01-02-2021  Killian Davies	Initial Revision
-- 01-08-2023  Swapnamol Abraham Provided BY
-- 11-08-2023  RS               Added CardImageUrl parameter
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueUpdate]
(
	@NodeVersionStatusId int,
	@Name nvarchar(255),
	@BadgeUrl nvarchar(128),
	@CardImageUrl nvarchar(128),
	@BannerUrl nvarchar(128),
	@CertificateUrl nvarchar(128),
	@Order int,
	@Description nvarchar(4000),
	@UserId int,
	@Keywords nvarchar(max),
	@CatalogueNodeVersionId int,
	@RestrictedAccess bit,
	@UserTimezoneOffset int = NULL,
	@ProviderId INT
)

AS

BEGIN
	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
		DECLARE @CatalogueNodeVersionProviderId INT;

		BEGIN TRAN
			UPDATE [hierarchy].[CatalogueNodeVersion]
			SET
				[BadgeUrl] = @BadgeUrl,
				[CardImageUrl] = @CardImageUrl,
				[BannerUrl] = @BannerUrl,
				[CertificateUrl] = @CertificateUrl,
				[Description] = @Description,
				[Name] = @Name,
				[Order] = @Order,
				[RestrictedAccess] = @RestrictedAccess,
				[AmendUserId] = @UserId,
				[AmendDate] = @AmendDate
			WHERE Id = @CatalogueNodeVersionId

			-- Add new keywords
			INSERT INTO [hierarchy].[CatalogueNodeVersionKeyword]
			SELECT
				@CatalogueNodeVersionId as CatalogueNodeVersionId,
				nkw.keyword as Keyword,
				0 as Deleted,
				@UserId as AmendUserId,
				@AmendDate as AmendDate,
				@UserId as CreateUserId,
				@AmendDate as CreateDate
			FROM 
			(
				SELECT value as Keyword FROM STRING_SPLIT(@Keywords, ',')
				WHERE value NOT IN (
					SELECT 
						Keyword as Keyword
					FROM [hierarchy].[CatalogueNodeVersionKeyword] 
					WHERE CatalogueNodeVersionId = @CatalogueNodeVersionId
				)
			) as nkw

			-- Remove unused keywords
			UPDATE [hierarchy].[CatalogueNodeVersionKeyword]
			SET Deleted = 1
			WHERE Id in (
				SELECT kw.id 
				FROM [hierarchy].[CatalogueNodeVersionKeyword] kw 
				WHERE kw.Deleted = 0
				AND kw.CatalogueNodeVersionId = @CatalogueNodeVersionId
				AND kw.Keyword not in (SELECT value as keyword FROM STRING_SPLIT(@Keywords, ',')))

			-- Restore previously deleted keywords
			UPDATE [hierarchy].[CatalogueNodeVersionKeyword]
			SET Deleted = 0
			WHERE Id in (
				SELECT kw.id 
				FROM (SELECT value as keyword FROM STRING_SPLIT(@Keywords, ',')) ak 
				JOIN [hierarchy].[CatalogueNodeVersionKeyword] kw on kw.Keyword = ak.keyword
				WHERE kw.Deleted = 1
				AND kw.CatalogueNodeVersionId = @CatalogueNodeVersionId)

			DECLARE @NodeId int
			SELECT @NodeId = nv.NodeId
			FROM [hierarchy].[CatalogueNodeVersion] cnv
			INNER JOIN [hierarchy].[NodeVersion] nv ON nv.Id = cnv.NodeVersionId
			WHERE cnv.Id = @CatalogueNodeVersionId

			SELECT @CatalogueNodeVersionProviderId = Id FROM hierarchy.CatalogueNodeVersionProvider CNP WHERE CNP.CatalogueNodeVersionId = @CatalogueNodeVersionId AND Deleted = 0

			IF (@CatalogueNodeVersionProviderId > 0 )		    	
				BEGIN
				UPDATE hierarchy.CatalogueNodeVersionProvider 
					SET Deleted = 1,RemovalDate = SYSDATETIMEOFFSET(), AmendUserId = @UserId, AmendDate = @AmendDate 
					WHERE CatalogueNodeVersionId = @CatalogueNodeVersionId AND Id = @CatalogueNodeVersionProviderId ;
				END
				IF (@ProviderId >0)
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