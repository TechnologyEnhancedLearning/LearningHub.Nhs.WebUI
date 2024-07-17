-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      12-06-2024
-- Purpose      Creates or updates a ResourceReferenceDisplayVersionUpdate within a Hierarchy Edit.
--
-- Modification History
--
-- 12-06-2024  DB	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditResourceReferenceDisplayVersionUpdate]
(
	@HierarchyEditDetailId int,
	@Name nvarchar(255), 
	@ResourceReferenceId int,
	@ResourceReferenceDisplayVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@NewResourceReferenceDisplayVersionId int OUTPUT
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		IF EXISTS (SELECT 1 FROM resources.ResourceReferenceDisplayVersion WHERE Id = @ResourceReferenceDisplayVersionId AND VersionStatusId = 1) -- Does a draft version already exist
		BEGIN
			UPDATE	hierarchy.ResourceReferenceDisplayVersion
			SET		DisplayName = @Name,
					AmendUserId = @UserId,
					AmendDate = @AmendDate
			WHERE	Id = @ResourceReferenceDisplayVersionId
			SET @NewResourceReferenceDisplayVersionId = @ResourceReferenceDisplayVersionId
		END
		ELSE
		BEGIN
			INSERT INTO resources.ResourceReferenceDisplayVersion (ResourceReferenceId, DisplayName, VersionStatusId, PublicationId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
			VALUES (@ResourceReferenceId, @Name, 1, NULL, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET()) -- Draft VersionStatusId=1
			SELECT @NewResourceReferenceDisplayVersionId = SCOPE_IDENTITY()

			UPDATE	hierarchy.HierarchyEditDetail
			SET		ResourceReferenceDisplayVersionId = @NewResourceReferenceDisplayVersionId,
					HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
					AmendUserId = @UserId,
					AmendDate = @AmendDate
			WHERE	Id = @HierarchyEditDetailId
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