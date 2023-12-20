-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      17-08-2020
-- Purpose      Submit a ResourceVersion to Publishing Queue
--
-- Modification History
--
-- 17-08-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionSubmitForPublishing]
(
	@ResourceVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
			
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @ResourceId int	
		DECLARE @CurrentVersionStatusId int
		DECLARE @ResourceVersionEventTypeId int = 4
		DECLARE @Details nvarchar(1024) = 'Submitted for Publishing'

		SELECT @CurrentVersionStatusId = VersionStatusId, @ResourceId = ResourceId
		FROM resources.ResourceVersion 
		WHERE Id = @ResourceVersionId

		-- Resource Version must be in Draft status to be set to Publishing
		IF @CurrentVersionStatusId <> 1
		BEGIN
			RAISERROR ('Error - ResourceVersion should be in "Draft" to be set to publishing status', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		-- Check if the catalogue is currently locked for editing by admin.
		IF EXISTS (SELECT 1 FROM resources.ResourceVersion rv 
					INNER JOIN hierarchy.NodeResource nr ON nr.ResourceId = rv.ResourceId
					INNER JOIN hierarchy.NodePath np ON np.NodeId = nr.NodeId
					INNER JOIN hierarchy.HierarchyEdit he ON he.RootNodeId = np.CatalogueNodeId
					WHERE rv.Id = @ResourceVersionId AND nr.Deleted = 0 AND np.Deleted = 0 AND np.IsActive = 1 AND he.Deleted = 0 AND
					HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */))
		BEGIN
			RAISERROR ('Error - Cannot publish the ResourceVersion because the catalogue is currently locked for editing',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		BEGIN TRAN
		
			UPDATE 
				resources.ResourceVersion
			SET
				VersionStatusId = 5, -- SubmittedToPublishingQueue
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			WHERE
				Id=@ResourceVersionId

			UPDATE 
				[hierarchy].[NodeResource]
			SET 
				[VersionStatusId] =  1,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			WHERE 
				ResourceId  = @ResourceId
				AND VersionStatusId = 3
				AND Deleted = 0
			
			-- create a resource version entry
			EXECUTE [resources].[ResourceVersionEventCreate]  @ResourceVersionId, @ResourceVersionEventTypeId, @Details, @UserId, @UserTimezoneOffset
			
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