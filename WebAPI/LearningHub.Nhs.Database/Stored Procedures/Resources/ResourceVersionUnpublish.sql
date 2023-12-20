-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Unpublish a Resource Version
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-- 16-11-2021  Killian Davies	Update NodeResource and NodeResourceLookup
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionUnpublish]
(
	@ResourceVersionId int,
	@Details nvarchar(1204)=null,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY
			
		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @ResourceId int
		DECLARE @CurrentVersionStatusId int

		SELECT @CurrentVersionStatusId = VersionStatusId, @ResourceId = ResourceId
		FROM resources.ResourceVersion 
		WHERE Id = @ResourceVersionId

		IF @CurrentVersionStatusId = 1
		BEGIN
			RAISERROR ('Error - ResourceVersion is in "Draft" and cannot be unpublished', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END
		
		IF @CurrentVersionStatusId = 3
		BEGIN
			RAISERROR ('Error - ResourceVersion already has a status of "Unpublished"', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		BEGIN TRAN
		
			UPDATE 
				resources.ResourceVersion
			SET
				VersionStatusId = 3,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			WHERE
				Id=@ResourceVersionId

			-- DeActivate External Reference Link
			IF EXISTS (SELECT 1 FROM resources.Resource r WHERE r.Id = @ResourceId AND r.ResourceTypeId = 6) --SCORM
			BEGIN
				DECLARE @NodePathId int
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

				UPDATE	resources.ExternalReference
				SET		Active = 0,
						AmendDate = @AmendDate,
						AmendUserId = @UserId
				WHERE	ResourceReferenceId = @ResourceReferenceId
			END

			-- Unpublish NodeResource - note IT1: NodeResource may only be published under a single Node.
			UPDATE 
				[hierarchy].[NodeResource]
			SET 
				[VersionStatusId] = 3,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			WHERE 
				ResourceId  = @ResourceId
				AND VersionStatusId = 2
				AND Deleted = 0

			-- Update NodeResourceLookup data - note IT1: NodeResourceLookup caters for Published only.
			UPDATE 
				[hierarchy].[NodeResourceLookup]
			SET 
				Deleted = 1,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			WHERE 
				ResourceId  = @ResourceId
				AND Deleted = 0

			EXECUTE [resources].[ResourceVersionUnpublishEventCreate] @ResourceVersionId, @Details, @UserId, @UserTimezoneOffset

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