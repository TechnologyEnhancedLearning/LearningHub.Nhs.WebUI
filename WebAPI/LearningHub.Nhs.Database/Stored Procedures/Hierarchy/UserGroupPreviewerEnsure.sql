-------------------------------------------------------------------------------
-- Author       Swapna Abraham
-- Created      21-01-2021
-- Purpose      Enusres that Previewer Default User Group Exists for supplied
--				Catalogue Node Id.
--
-- Modification History
--
-- 08-02-2024  Swapna Abraham	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[UserGroupPreviewerEnsure]
(
	@NodeId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	BEGIN TRY
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN
			IF NOT EXISTS
				(SELECT 'X' 
				FROM hub.UserGroup ug
				INNER JOIN hub.UserGroupAttribute uga ON ug.Id = uga.UserGroupId
				INNER JOIN hub.Scope s ON uga.ScopeId = s.Id
										AND s.ScopeTypeId = 1 -- Catalogue ScopeType
										AND s.CatalogueNodeId = @NodeId
				WHERE ug.Deleted = 0
				AND uga.Deleted = 0
				AND s.Deleted = 0
				AND uga.AttributeId = 8
				)

			BEGIN

				DECLARE @CatalogueName nvarchar(255)
				DECLARE @PreviewerUserGroupId int
				DECLARE @PreviewerAttributeId int = 8

				SELECT 
					@CatalogueName = cnv.[Name]
				FROM
					[hierarchy].[Node] n
					INNER JOIN [hierarchy].[NodeVersion] nv ON n.CurrentNodeVersionId = nv.Id
					INNER JOIN [hierarchy].[CatalogueNodeVersion] cnv ON nv.Id = cnv.NodeVersionId
				WHERE 
					n.Id = @NodeId

				INSERT INTO [hub].[UserGroup]([Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
				SELECT 
					@CatalogueName + ' - Preview' AS [Name],
					'Previewer access for ' + @CatalogueName +' Catalogue' AS [Description],
					0 as Deleted,
					@UserId as CreateUserId,
					@AmendDate as CreateDate,
					@UserId as AmendUserId,
					@AmendDate as AmendDate
				SELECT @PreviewerUserGroupId = SCOPE_IDENTITY()

				DECLARE @ScopeId int
				SELECT @ScopeId = [Id]
				FROM hub.Scope 
				WHERE ScopeTypeId = 1 -- Catalogue ScopeType
				AND CatalogueNodeId = @NodeId

				INSERT INTO [hub].[UserGroupAttribute] ([UserGroupId],[AttributeId],[ScopeId],[IntValue],[TextValue],[BooleanValue],[DateValue],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
				SELECT 
					@PreviewerUserGroupId AS [UserGroupId], 
					@PreviewerAttributeId AS [AttributeId],
					@ScopeId AS [ScopeId],
					@NodeId AS [IntValue],
					NULL AS [TextValue],
					NULL AS [BooleanValue],
					NULL AS [DateValue],
					0 as Deleted,
					@UserId as CreateUserId,
					@AmendDate as CreateDate,
					@UserId as AmendUserId,
					@AmendDate as AmendDate

				-- Assign previewer role to the user group - scope
				INSERT INTO [hub].[RoleUserGroup] ([RoleId],[UserGroupId],[ScopeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
				SELECT 
					8 AS [RoleId], -- Previewer
					@PreviewerUserGroupId AS [UserGroupId], 
					@ScopeId AS [ScopeId],
					0 as Deleted,
					@UserId as CreateUserId,
					@AmendDate as CreateDate,
					@UserId as AmendUserId,
					@AmendDate as AmendDate

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