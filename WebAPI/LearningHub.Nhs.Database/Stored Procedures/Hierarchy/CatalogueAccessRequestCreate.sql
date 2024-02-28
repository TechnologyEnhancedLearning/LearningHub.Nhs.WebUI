-------------------------------------------------------------------------------
-- Author     <>  
-- Created      28-01-2022
-- Purpose      To handle Catalogue Access requests.
--
-- Modification History
--
-- 01-02-2021 <> Initial Revision
-- 12-01-2024  Swapnamol Abraham Added Role ID referernce
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueAccessRequestCreate]
(
	@currentUserId int,
	@reference nvarchar(max),
	@message nvarchar(max),
	@catalogueManageAccessUrl nvarchar(max),
	@UserTimezoneOffset int = NULL,
	@accessType nvarchar(max),
	@roleId int
)
AS
BEGIN
	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN
			DECLARE @localAdminRoleId int = 3;

			DECLARE @catalogueNodeId int;
			DECLARE @catalogueName nvarchar(max);
			DECLARE @currentUserEmailAddress nvarchar(max);
			DECLARE @currentUserFullName nvarchar(max);
			SELECT 
				@catalogueNodeId = nv.NodeId,
				@catalogueName = cnv.Name
			FROM 
				[hierarchy].[CatalogueNodeVersion] cnv
			JOIN [hierarchy].[NodeVersion] nv on cnv.NodeVersionId = nv.Id
			WHERE Url = @reference AND cnv.Deleted = 0 AND nv.Deleted = 0;

			SELECT
				@currentUserEmailAddress = EmailAddress,
				@CurrentUserFullName = FirstName + ' ' + LastName
			FROM [hub].[UserProfile] where Id = @currentUserId;

			DECLARE @emailTemplateBody nvarchar(max);
			DECLARE @emailTemplateSubject nvarchar(max);
			IF @accessType = 'access'
			BEGIN
			SELECT 
				@emailTemplateBody = REPLACE(etl.Body, '[Content]', et.Body),
				@emailTemplateSubject = et.Subject
			FROM 
				[messaging].[EmailTemplate] et
			JOIN [messaging].[EmailTemplateLayout] etl on et.LayoutId = etl.Id
			WHERE et.Title = 'CatalogueAccessRequest';
			END
			ELSE
			BEGIN
			SELECT 
				@emailTemplateBody = REPLACE(etl.Body, '[Content]', et.Body),
				@emailTemplateSubject = et.Subject
			FROM 
				[messaging].[EmailTemplate] et
			JOIN [messaging].[EmailTemplateLayout] etl on et.LayoutId = etl.Id
			WHERE et.Title = 'CataloguePermissionRequest';
			END

			DECLARE @AdminUserCursor CURSOR 
			SET @AdminUserCursor = CURSOR FAST_FORWARD 
			FOR
			SELECT DISTINCT
				up.FirstName as AdminFirstName,
				u.Id as AdminUserId
			FROM 
				[hub].[UserUserGroup] uug
			JOIN 
				(
					SELECT UserGroupId FROM [hub].[RoleUserGroup] rug
					JOIN [hub].[Scope] s on rug.ScopeId = s.Id
					WHERE s.CatalogueNodeId = @catalogueNodeId
					AND rug.RoleId = @localAdminRoleId
					AND s.Deleted = 0 AND rug.Deleted = 0
					AND s.Deleted = 0
				) arug on uug.UserGroupId = arug.UserGroupId
			JOIN [hub].[User] u on u.Id = uug.UserId
			JOIN [hub].[UserProfile] up on up.Id = u.Id
			where uug.Deleted = 0 and u.Deleted = 0 and up.Deleted = 0;

			DECLARE @adminFirstName nvarchar(max);
			DECLARE @adminUserId nvarchar(max); 

			OPEN @AdminUserCursor;
			FETCH NEXT FROM @AdminUserCursor INTO @adminFirstName, @adminUserId;

			WHILE @@FETCH_STATUS = 0
			BEGIN
				DECLARE @fullSubject nvarchar(max);
				DECLARE @fullBody nvarchar(max);

				SELECT 
				@fullSubject = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@emailTemplateSubject, '[AdminFirstName]', @adminFirstName), '[UserEmailAddress]', @currentUserEmailAddress), '[CatalogueName]', @catalogueName), '[UserMessage]', @message), '[UserFullName]', @currentUserFullName), '[ManageAccessUrl]', @catalogueManageAccessUrl),
				@fullBody = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@emailTemplateBody, '[AdminFirstName]', @adminFirstName), '[UserEmailAddress]', @currentUserEmailAddress), '[CatalogueName]', @catalogueName), '[UserMessage]', @message), '[UserFullName]', @currentUserFullName), '[ManageAccessUrl]', @catalogueManageAccessUrl);

				exec [messaging].[CreateEmailForUser] 
					@fullSubject,
					@fullBody,
					@adminUserId,
					@currentUserId, 
					@UserTimezoneOffset;

				FETCH NEXT FROM @AdminUserCursor INTO @adminFirstName, @adminUserId;
			END;
			CLOSE @AdminUserCursor;
			DEALLOCATE @AdminUserCursor;

			INSERT INTO [hierarchy].[CatalogueAccessRequest]
			(UserId, CatalogueNodeId, EmailAddress, Message, Status, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate, RoleId)
			VALUES (@currentUserId, @catalogueNodeId, @currentUserEmailAddress, @message, 0, 0, @currentUserId, @AmendDate, @currentUserId, @AmendDate, @roleId);
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
END;