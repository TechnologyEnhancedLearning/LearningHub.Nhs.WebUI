-------------------------------------------------------------------------------
-- Author       RS
-- Created      15-11-2021
-- Purpose      Creates/updates the NodeResource record for a draft resource.
--              New drafts start off at the top of the list in folders.
--              DisplayOrder values of other resources in same source/destination nodes are updated to suit.
--
-- Modification History
--
-- 15-11-2021  RS	Initial Revision. 
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[NodeResourceCreateOrUpdate]
(
	@NodeId int,
	@ResourceId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)
AS

BEGIN

	BEGIN TRY

		-- Check if the destination catalogue is currently locked for editing by admin. Can't crete/move a resource into it when a hierarchy edit is taking place.
		IF EXISTS (SELECT 1 FROM hierarchy.NodePath np
					INNER JOIN hierarchy.HierarchyEdit he ON he.RootNodeId = np.CatalogueNodeId
					WHERE np.NodeId = @NodeId AND np.Deleted = 0 AND np.IsActive = 1 AND he.Deleted = 0 AND
					HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */))
		BEGIN
			RAISERROR ('Error - Cannot create or update the NodeResource because the destination catalogue is currently locked for editing',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @DraftNodeResourceCount INT
		SELECT @DraftNodeResourceCount = Count(*) FROM [hierarchy].[NodeResource] WHERE ResourceId = @ResourceId AND VersionStatusId = 1 /* Draft */ AND Deleted = 0

		IF @DraftNodeResourceCount > 1
		BEGIN
			RAISERROR ('Error in NodeResourceSetDraft. Resource must belong to a single node.',
						16,	-- Severity.  
						1	-- State.  
						); 
		END

		DECLARE @DraftNodeResourceNodeId INT, @DraftNodeResourceDisplayOrder INT, @PublishedNodeResourceNodeId INT
		SELECT @DraftNodeResourceNodeId = NodeId, @DraftNodeResourceDisplayOrder = DisplayOrder FROM [hierarchy].[NodeResource] WHERE ResourceId = @ResourceId AND VersionStatusId = 1  /* Draft */ AND Deleted = 0
		SELECT @PublishedNodeResourceNodeId = NodeId FROM [hierarchy].[NodeResource] WHERE ResourceId = @ResourceId AND VersionStatusId IN (2, 3)  /* Published or Unpublished */ AND Deleted = 0

		IF @DraftNodeResourceCount = 0
		BEGIN

			IF @PublishedNodeResourceNodeId IS NULL OR @PublishedNodeResourceNodeId <> @NodeId
			BEGIN
				-- No draft NodeResource exists and any published NodeResource is for a different Node. Need to create a new draft NodeResource.

				IF @NodeId > 1
				BEGIN
					-- Update DisplayOrder of NodeResources in destination node which will appear after this one - increment by 1.
					UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate WHERE NodeId = @NodeId AND Deleted = 0
					UPDATE [hierarchy].NodeLink SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE ParentNodeId = @NodeId AND Deleted = 0 
				END
			
				-- Create new draft NodeResource
				INSERT INTO [hierarchy].[NodeResource]
					   ([NodeId]
					   ,[ResourceId]
					   ,[DisplayOrder]
					   ,[VersionStatusId]
					   ,[Deleted]
					   ,[CreateUserId]
					   ,[CreateDate]
					   ,[AmendUserId]
					   ,[AmendDate])
				 VALUES
					   (@NodeId
					   ,@ResourceId
					   ,CASE @NodeId
							WHEN 1 THEN NULL
							ELSE 1
						END
					   ,1
					   ,0
					   ,@UserId
					   ,@AmendDate
					   ,@UserId
					   ,@AmendDate)

			END

		END
		ELSE IF @DraftNodeResourceNodeId <> @NodeId
		BEGIN
			-- There is a draft NodeResource but it's for a different Node.

			-- Check if the source catalogue is currently locked for editing by admin. Can't move resource out of it when a hierarchy edit is taking place.
			IF EXISTS (SELECT 1 FROM hierarchy.NodePath np
						INNER JOIN hierarchy.HierarchyEdit he ON he.RootNodeId = np.CatalogueNodeId
						WHERE np.NodeId = @DraftNodeResourceNodeId AND np.Deleted = 0 AND np.IsActive = 1 AND he.Deleted = 0 AND
						HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */))
			BEGIN
				RAISERROR ('Error - Cannot create or update the NodeResource because the source catalogue is currently locked for editing',
						16,	-- Severity.  
						1	-- State.  
						); 
			END

			IF @PublishedNodeResourceNodeId IS NOT NULL AND @PublishedNodeResourceNodeId = @NodeId
			BEGIN
				-- There's already a published NodeResource for the correct Node. Mark the draft one as deleted. Goes back to using the published NodeResource.
			
				UPDATE [hierarchy].[NodeResource] SET Deleted = 1, AmendDate = @AmendDate, AmendUserId = @UserId 
				WHERE ResourceId = @ResourceId AND VersionStatusId = 1  /* Draft */ AND Deleted = 0
			END
			ELSE
			BEGIN
				-- There's no published NodeResource for the correct Node either. So update the existing draft NodeResource with the correct NodeId.

				IF @NodeId > 1
				BEGIN
					-- Update DisplayOrder of NodeResources in destination node which will appear after this one - increment by 1.
					UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @UserId 
					WHERE NodeId = @NodeId AND Deleted = 0
					UPDATE [hierarchy].NodeLink SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @UserId WHERE ParentNodeId = @NodeId AND Deleted = 0 
				END
			
				-- Update the existing draft NodeResource to point to the new node.
				UPDATE [hierarchy].[NodeResource] 
				SET NodeId = @NodeId, 
					DisplayOrder = 
						CASE @NodeId
							WHEN 1 THEN NULL
							ELSE 1
						END, 
					AmendDate = @AmendDate 
				WHERE ResourceId = @ResourceId AND VersionStatusId = 1  /* Draft */ AND Deleted = 0
			END

			IF @DraftNodeResourceNodeId > 1
			BEGIN
				-- Update DisplayOrder of NodeResources in source node which appeared after this one - decrement by 1.
				UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder - 1, AmendDate = @AmendDate, AmendUserId = @UserId 
				WHERE NodeId = @DraftNodeResourceNodeId AND Deleted = 0 AND DisplayOrder > @DraftNodeResourceDisplayOrder
			END
		END
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