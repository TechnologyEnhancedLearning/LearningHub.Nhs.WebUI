-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Update a Folder within a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 22-04-2024  DB	Updated so that a new draft NodeVersion is created if the existing NodeVersion is not in Draft status.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditFolderUpdate]
(
	@HierarchyEditDetailId int,
	@Name nvarchar(255), 
	@Description nvarchar(4000), 
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	DECLARE @NodeId INT
	DECLARE @NodeVersionId INT
	DECLARE @CreatedNodeVersionId INT
	DECLARE @HierarchyEditId INT
	DECLARE @ParentNodeId INT

	SELECT	@NodeId = NodeId, 
			@NodeVersionId = NodeVersionId,
			@HierarchyEditId = HierarchyEditId,
			@ParentNodeId = ParentNodeId
	FROM [hierarchy].[HierarchyEditDetail]
	WHERE Id = @HierarchyEditDetailId

	BEGIN TRY

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		-- If NodeVersion is in Draft status update it, otherwise create a new NodeVersion in Draft status.
		IF EXISTS (SELECT 1 
					FROM hierarchy.NodeVersion nv
					INNER JOIN	[hierarchy].[HierarchyEditDetail] hed ON hed.NodeVersionId = nv.Id
					WHERE	
						hed.Id = @HierarchyEditDetailId
						AND hed.Deleted = 0
						AND nv.Deleted = 0
						AND nv.VersionStatusId = 1) -- Draft
		BEGIN
			UPDATE
				fnv
			SET
				[Name] = @Name,
				[Description] = @Description,
				AmendUserId = @UserId,
				AmendDate = @AmendDate
			FROM
				hierarchy.FolderNodeVersion fnv
			INNER JOIN
				 [hierarchy].[HierarchyEditDetail] hed ON hed.NodeVersionId = fnv.NodeVersionId
			WHERE	
				hed.Id = @HierarchyEditDetailId
				AND hed.Deleted = 0
				AND fnv.Deleted = 0
		END
		ELSE
		BEGIN
			EXECUTE [hierarchy].[FolderNodeVersionCreate] @NodeId, @Name, @Description, @CreatedNodeVersionId OUTPUT
		END


		UPDATE 
			hed
		SET
			[HierarchyEditDetailOperationId] = ISNULL(hed.HierarchyEditDetailOperationId, 2),
			NodeVersionId = ISNULL(@CreatedNodeVersionId, @NodeVersionId),
			AmendUserId = @UserId,
			AmendDate = @AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN
			hierarchy.FolderNodeVersion fnv ON hed.NodeVersionId = fnv.NodeVersionId
		WHERE	
			hed.Id = @HierarchyEditDetailId
			AND hed.Deleted = 0
			AND fnv.Deleted = 0

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