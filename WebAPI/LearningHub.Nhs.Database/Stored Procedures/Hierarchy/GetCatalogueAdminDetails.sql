-------------------------------------------------------------------------------
-- Author     <Swapnamol Abraham>  
-- Created      28-01-2022
-- Purpose      To get the details of catalogue admin users of a catalogue.
--
-- Modification History
--
-- 04-07-2025  Swapnamol Abraham Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[GetCatalogueAdminDetails]
(
	@currentUserId int,
	@reference nvarchar(max),
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

			SELECT DISTINCT
				up.FirstName as FirstName,
				up.LastName As LastName,
				up.EmailAddress AS EmailAddress,
				u.Id as UserId
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
