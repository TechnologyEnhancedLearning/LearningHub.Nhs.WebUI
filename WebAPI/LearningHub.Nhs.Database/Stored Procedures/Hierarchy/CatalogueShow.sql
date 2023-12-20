-------------------------------------------------------------------------------S
-- Author       Corey Grant
-- Created      17-11-2021
-- Purpose      Shows a catalogue.
--
-- Modification History
--
-- 17-11-2021  Corey Grant	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[CatalogueShow]
(
	@NodeId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	BEGIN TRY
		DECLARE @LastShownDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		BEGIN TRAN
			UPDATE hierarchy.Node
			SET Hidden = 0, AmendDate = @LastShownDate, AmendUserId = @UserId
			WHERE Id = @NodeId;
		
			UPDATE cnv
			SET cnv.LastShownDate = @LastShownDate, cnv.AmendDate = @LastShownDate, AmendUserId = @UserId
			FROM 
				hierarchy.CatalogueNodeVersion cnv
				JOIN hierarchy.NodeVersion nv on cnv.NodeVersionId = nv.Id
				JOIN hierarchy.Node n on nv.NodeId = n.Id
			WHERE cnv.Deleted = 0 AND nv.Deleted = 0 AND n.Deleted = 0 AND n.Id = @NodeId;
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