-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Creates a new Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 05-02-2021  KD	Revision for IT2 - inclusion of Resources.
--					Addition of hierarchy.HierarchyEditNodeResourceLookup to provide
--					details of Resource placement within a HierarchyEdit (for all Resource statuses)
-- 12-12-2022  DB	Addition of call to hierarchy.HierarchyEditHouseKeeping to remove old temporary data.
-- 07-05-2024  DB	Change input parameter to NodePathId to enable specific instances of node to be edited.
--					Also add Child NodePathId NodeLink type detail entries.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditCreate]
(
	@RootNodePathId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@HierarchyEditId int OUTPUT
)

AS

BEGIN

	BEGIN TRY

		-- Tidy up previous temporary hierarchy data for this root node.
		Exec hierarchy.HierarchyEditHouseKeeping @RootNodePathId

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		-- Check if there is already an active hierarchy edit for this root node path.
		IF EXISTS (SELECT Id FROM hierarchy.HierarchyEdit WHERE RootNodePathId = @RootNodePathId AND HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */) AND Deleted = 0)
		BEGIN
			RAISERROR ('Cannot create hierarchy edit as there is already one currently active (draft/submitted/publishing)',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Create new header record for new Hierarchy Edit
		INSERT INTO hierarchy.HierarchyEdit (RootNodePathId, HierarchyEditStatusId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		VALUES (@RootNodePathId, 1, 0, @UserId, @AmendDate, @UserId, @AmendDate) -- HierarchyEditStatusId = 1 (Draft)

		SELECT @HierarchyEditId = SCOPE_IDENTITY()
	
		-- Create starting point of Hierarchy Edit as the current published branch underneath the Root Node Path.
		;WITH
		  cteEditBranch(NodeId, NodeVersionId, ParentNodeId, NodeLinkId, DisplayOrder, InitialNodePath)
		  AS
		  (
			SELECT 
				n.Id AS NodeId,
				n.CurrentNodeVersionId AS NodeVersionId,
				ParentNodeId = NULL,
				NodeLinkId = NULL,
				DisplayOrder = 1,
				CAST(n.Id AS nvarchar(128)) AS InitialNodePath
			 FROM 
                 hierarchy.NodePath np
             INNER JOIN
			 	hierarchy.[Node] n ON np.NodeId = n.Id
			 WHERE 
			 	np.Id = @RootNodePathId AND np.Deleted = 0 AND n.Deleted = 0
	
			UNION ALL

			SELECT 
				ChildNodeId AS NodeId,
				n.CurrentNodeVersionId AS NodeVersionId,
				nl.ParentNodeId,
				nl.Id AS NodeLinkId,
				nl.DisplayOrder,
				CAST(cte.InitialNodePath + '\' + CAST(ChildNodeId AS nvarchar(8)) AS nvarchar(128)) AS InitialNodePath
			FROM 
				hierarchy.NodeLink nl
			INNER JOIN
				hierarchy.[Node] n ON nl.ChildNodeId = n.Id
			INNER JOIN	
				cteEditBranch cte ON nl.ParentNodeId = cte.NodeId
			WHERE 
				n.CurrentNodeVersionId IS NOT NULL AND n.Deleted = 0 AND nl.Deleted = 0
			)
		INSERT INTO [hierarchy].[HierarchyEditDetail] (HierarchyEditId,
													   HierarchyEditDetailTypeId,
													   HierarchyEditDetailOperationId,
													   NodePathId,
													   NodeId,
													   NodeVersionId,
													   InitialParentNodeId,
													   ParentNodeId,
													   ParentNodePathId,
													   NodeLinkId,
													   ResourceId,
													   ResourceVersionId,
													   NodeResourceId,
													   DisplayOrder,
													   InitialNodePath,
													   Deleted,
													   CreateUserId,
													   CreateDate,
													   AmendUserId,
													   AmendDate)
		SELECT 
			@HierarchyEditId, 
			4 AS HierarchyEditDetailTypeId, -- NodeLink
			NULL AS HierarchyEditDetailOperationId,
			np.Id AS NodePathId,
			cte.NodeId,
			cte.NodeVersionId,
			cte.ParentNodeId AS InitialParentNodeId,
			cte.ParentNodeId,
			pnp.Id AS ParentNodePathId,
			cte.NodeLinkId,
			NULL AS ResourceId,
			NULL AS ResourceVersionId,
			NULL AS NodeResourceId,
			cte.DisplayOrder,
			cte.InitialNodePath,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM
			cteEditBranch cte
        LEFT OUTER JOIN
            hierarchy.NodePath np ON cte.NodeId = np.NodeId AND cte.InitialNodePath = np.NodePath
        LEFT OUTER JOIN
            hierarchy.NodePath pnp ON cte.ParentNodeId = pnp.NodeId AND LEFT(cte.InitialNodePath, LEN(cte.InitialNodePath) - CHARINDEX('\', REVERSE(cte.InitialNodePath))) = pnp.NodePath

		-- Add all Resources to the HierarchyEdit structure
		INSERT INTO [hierarchy].[HierarchyEditDetail] (HierarchyEditId,
														HierarchyEditDetailTypeId,
														HierarchyEditDetailOperationId,
														NodePathId,
														NodeId,
														NodeVersionId,
														ParentNodeId,
														ParentNodePathId,
														NodeLinkId,
														ResourceId,
														ResourceVersionId,
														ResourceReferenceId,
														NodeResourceId,
														DisplayOrder,
														InitialNodePath,
														Deleted,
														CreateUserId,
														CreateDate,
														AmendUserId,
														AmendDate)
		SELECT 
			@HierarchyEditId, 
			5 AS HierarchyEditDetailTypeId, -- NodeResource
			NULL AS HierarchyEditDetailOperationId,													
			hed.NodePathId,
			hed.NodeId,
			hed.NodeVersionId,
			NULL AS ParentNodeId,
			NULL AS ParentNodePathId,
			NULL AS NodeLinkId,
			r.Id AS ResourceId,
			CASE WHEN r.CurrentResourceVersionId IS NOT NULL THEN r.CurrentResourceVersionId ELSE rv.Id END AS ResourceVersionId,
			rr.Id AS ResourceReferenceId,
			nr.Id AS NodeResourceId,
			nr.DisplayOrder AS DisplayOrder,
			hed.InitialNodePath,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM
			[hierarchy].[HierarchyEditDetail] hed
		INNER JOIN
			hierarchy.NodeResource nr ON hed.NodeId = nr.NodeId
		INNER JOIN	
			resources.[Resource] r ON nr.ResourceId = r.Id
		INNER JOIN
			resources.ResourceReference rr ON rr.ResourceId = r.Id AND rr.NodePathId = hed.NodePathId AND rr.Deleted = 0
		LEFT JOIN
			resources.ResourceVersion rv ON nr.ResourceId = rv.ResourceId AND rv.VersionStatusId = 1 AND rv.Deleted = 0
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND nr.Deleted = 0
			AND r.Deleted = 0
	
		------------------------------------------------------------ 
		-- Populate HierarchyEditNodeResourceLookup
		----------------------------------------------------------
		EXEC hierarchy.HierarchyEditRefreshNodeResourceLookup @HierarchyEditId

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