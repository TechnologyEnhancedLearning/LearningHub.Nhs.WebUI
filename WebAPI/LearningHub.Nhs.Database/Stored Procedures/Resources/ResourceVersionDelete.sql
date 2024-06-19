-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      08 Apr 2020
-- Purpose      Marks a resource version and it's childrent as deleted
--
-- Modification History
--
-- 08 Apr 2020  Dave Brown	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionDelete]
(
	@ResourceVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY

		-- Check if the catalogue is currently locked for editing by admin.
		IF EXISTS (SELECT 1 FROM resources.ResourceVersion rv 
					INNER JOIN hierarchy.NodeResource nr ON nr.ResourceId = rv.ResourceId
					INNER JOIN hierarchy.NodePath np ON np.NodeId = nr.NodeId
					INNER JOIN hierarchy.NodePath rnp ON np.CatalogueNodeId = rnp.NodeId
					INNER JOIN hierarchy.HierarchyEdit he ON he.RootNodePathId = rnp.Id
					WHERE rv.Id = @ResourceVersionId AND nr.Deleted = 0 AND np.Deleted = 0 AND np.IsActive = 1 AND he.Deleted = 0 AND
					HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */))
		BEGIN
			RAISERROR ('Error - Cannot delete the ResourceVersion because the catalogue is currently locked for editing',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
			
		DECLARE @CurrentResourceTypeId int, @ResourceId int

		SELECT @CurrentResourceTypeId = r.ResourceTypeId, @ResourceId = ResourceId
		FROM resources.ResourceVersion rv
		INNER JOIN resources.[Resource] r ON rv.ResourceId = r.Id
		WHERE rv.Id = @ResourceVersionId

		BEGIN TRAN

		IF @CurrentResourceTypeId = 1
		BEGIN
			UPDATE avf
			SET	Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			FROM [resources].[ArticleResourceVersionFile] avf
			INNER JOIN resources.ArticleResourceVersion av ON avf.ArticleResourceVersionId = av.Id
		    WHERE av.ResourceVersionId = @ResourceVersionId
			
			UPDATE resources.ArticleResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 2 
		BEGIN
			UPDATE resources.AudioResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 3 
		BEGIN
			UPDATE resources.EmbeddedResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END		
		IF @CurrentResourceTypeId = 4 
		BEGIN
			UPDATE resources.EquipmentResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 5 
		BEGIN
			UPDATE resources.ImageResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 6 
		BEGIN
			UPDATE resources.ScormResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 7 
		BEGIN
			UPDATE resources.VideoResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 8 
		BEGIN
			UPDATE resources.WebLinkResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 9 
		BEGIN
			UPDATE resources.GenericFileResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 10
		BEGIN
			UPDATE resources.CaseResourceVersion
			SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
			WHERE ResourceVersionId = @ResourceVersionId
		END
		IF @CurrentResourceTypeId = 11
        BEGIN
            UPDATE resources.AssessmentResourceVersion
            SET Deleted=1, AmendDate=SYSDATETIMEOFFSET(), AmendUserId=@UserId
            WHERE ResourceVersionId = @ResourceVersionId
        END
		IF @CurrentResourceTypeId = 12
        BEGIN
            UPDATE resources.HtmlResourceVersion
            SET Deleted=1, AmendDate=SYSDATETIMEOFFSET(), AmendUserId=@UserId
            WHERE ResourceVersionId = @ResourceVersionId
        END

		UPDATE resources.ResourceVersion
		SET Deleted=1, AmendDate=@AmendDate, AmendUserId=@UserId
		WHERE Id = @ResourceVersionId


		-- Mark the NodeResource as deleted. Update the NodeResource.DisplayOrder for all other resources that are in the same hierarchy node (excluding Community catalogue).
		DECLARE @NodeId INT, @NodeResourceDisplayOrder INT
		SELECT @NodeId = NodeId, @NodeResourceDisplayOrder = DisplayOrder FROM [hierarchy].[NodeResource] WHERE ResourceId = @ResourceId AND VersionStatusId = 1 /* Draft */ AND Deleted = 0

		UPDATE [hierarchy].[NodeResource] SET Deleted = 1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE ResourceId = @ResourceId AND NodeId = @NodeId AND VersionStatusId = 1 /* Draft */ AND Deleted = 0

		IF @NodeId > 1
		BEGIN
			-- Update NodeResources in source node which appeared after this one - decrement by 1.
			UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder - 1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE NodeId = @NodeId AND Deleted = 0 AND DisplayOrder > @NodeResourceDisplayOrder
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