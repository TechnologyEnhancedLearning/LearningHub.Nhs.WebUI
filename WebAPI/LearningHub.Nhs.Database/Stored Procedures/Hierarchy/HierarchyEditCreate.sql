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
-- 01-09-2023  SA	Changes for the Catalogue structure - folders always displayed at the top.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditCreate]
(
	@RootNodeId int,
	@UserId int,
	@UserTimezoneOffset int = NULL,
	@HierarchyEditId int OUTPUT
)

AS

BEGIN

	BEGIN TRY

	    -- Tidy up previous temporary hierarchy data for this root node.
		Exec hierarchy.HierarchyEditHouseKeeping @RootNodeId

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		-- Check if there is already an active hierarchy edit for this root node.
		IF EXISTS (SELECT Id FROM hierarchy.HierarchyEdit WHERE RootNodeId = @RootNodeId AND HierarchyEditStatusId IN (1 /* Draft */, 4 /* Publishing */, 5 /* Submitted */) AND Deleted = 0)
		BEGIN
			RAISERROR ('Cannot create hierarchy edit as there is already one currently active (draft/submitted/publishing)',
					16,	-- Severity.  
					1	-- State.  
					); 
		END

		-- Create new header record for new Hierarchy Edit
		INSERT INTO hierarchy.HierarchyEdit (RootNodeId, HierarchyEditStatusId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		VALUES (@RootNodeId, 1, 0, @UserId, @AmendDate, @UserId, @AmendDate) -- HierarchyEditStatusId = 1 (Draft)

		SELECT @HierarchyEditId = SCOPE_IDENTITY()
	
		-- Create starting point of Hierarchy Edit as the current published branch underneath the Root Node.
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
				hierarchy.[Node] n
			WHERE 
				Id = @RootNodeId AND n.Deleted = 0
	
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
													   NodeId,
													   NodeVersionId,
													   ParentNodeId,
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
			cteEditBranch.NodeId,
			cteEditBranch.NodeVersionId,
			cteEditBranch.ParentNodeId,
			cteEditBranch.NodeLinkId,
			NULL AS ResourceId,
			NULL AS ResourceVersionId,
			NULL AS NodeResourceId,
			cteEditBranch.DisplayOrder,
			cteEditBranch.InitialNodePath,
			0 AS Deleted,
			@UserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@UserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM
			cteEditBranch

		-- Add all Resources to the HierarchyEdit structure
		INSERT INTO [hierarchy].[HierarchyEditDetail] (HierarchyEditId,
														HierarchyEditDetailTypeId,
														HierarchyEditDetailOperationId,
														NodeId,
														NodeVersionId,
														ParentNodeId,
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
			5 AS HierarchyEditDetailTypeId, -- NodeResource
			NULL AS HierarchyEditDetailOperationId,													
			hed.NodeId,
			hed.NodeVersionId,
			hed.NodeId AS ParentNodeId,
			NULL AS NodeLinkId,
			r.Id AS ResourceId,
			CASE WHEN r.CurrentResourceVersionId IS NOT NULL THEN r.CurrentResourceVersionId ELSE rv.Id END AS ResourceVersionId,
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
		LEFT JOIN
			resources.ResourceVersion rv ON nr.ResourceId = rv.ResourceId AND rv.VersionStatusId = 1 AND rv.Deleted = 0
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND nr.Deleted = 0
			AND r.Deleted = 0

		;WITH CTEDisplayOrder AS
        (
		  select Id as HieracyEditDetailID, 
		  ROW_NUMBER ()  over (partition by  (ParentNodeId) order by DisplayOrder) DisplayOrder,
		  ParentNodeId FROM [hierarchy].[HierarchyEditDetail]
          WHERE HierarchyEditId = @HierarchyEditId
        )
		 -- Update the DisplayOrder in the HierarchyEditDetail table based on the parent node id
        UPDATE hed
        SET DisplayOrder = CD.DisplayOrder 
        FROM [hierarchy].[HierarchyEditDetail] hed
        INNER JOIN CTEDisplayOrder CD ON hed.ParentNodeId = CD.ParentNodeId and hed.Id = CD.HieracyEditDetailID
        WHERE hed.HierarchyEditId = @HierarchyEditId	     

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