-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      01-09-2024
-- Purpose      Remove node reference details within a Hierarchy Edit.
--
-- Modification History
--
-- 01-09-2024  DB	Initial Version.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditDeleteNodeReferenceDetails] 
(
	@HierarchyEditDetailId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		UPDATE	hierarchy.HierarchyEditDetail
		SET		NodePathDisplayVersionId = NULL,
				HierarchyEditDetailOperationId = CASE WHEN HierarchyEditDetailOperationId = 1 THEN HierarchyEditDetailOperationId ELSE 2 END, -- Set to Edit if existing Node
				AmendUserId = @UserId,
				AmendDate = @AmendDate
		WHERE	Id = @HierarchyEditDetailId

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
Go