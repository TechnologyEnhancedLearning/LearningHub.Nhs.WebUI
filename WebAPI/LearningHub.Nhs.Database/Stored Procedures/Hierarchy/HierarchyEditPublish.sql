-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-09-2021
-- Purpose      Publish a Hierarchy Edit.
--				Uses the hierarchy.HierarchyEditDetail entries to determine any
--				data updates that are required.  This includes CRUD operations for
--				content structure related tables - NodeLink, NodePath, NodePathNode etc.
--
-- Modification History
--
-- 23-09-2021  KD	Initial Revision.
-- 20-01-2022  KD	IT2 - inclusion of Resources in HierarchyEdit.
-- 23-04-2024  DB	Updated to manage publishing of draft NodeVersions during edits
-- 30-04-2024  DB	Handle referencing of folders
-- 02-05-2024  DB	Remove creation of NodePaths as this needs to be handled during the edit process.
-- 28-05-2024  DB	Moved nodes can not have their nodepaths updated in case a subsequent reference is made to the original position.
-- 30-05-2024  DB	Added the creation of a new ResourceReference records for the moved resources.
-- 03-06-2024  DB	Publish NodePathDisplayVersion records.
-- 13-06-2024  DB	Publish ResourceReferenceDisplayVersion records.
-- 26-07-2024  SA   Remove references to be implemented
-- 27-08-2024  SA   Moving a folder into a referenced folder should affect all instances of the referenced folder.[added
                    -- condition to avoid duplicate entries in to NodeLink table]
-- 02-09-2024  DB	Remove any deleted NodePathDisplayVersion anf ResourceReferenceDisplayVersion records.
-- 21-08-2024  SS	Publishing catalogues needs to update referencing catalogues
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditPublish] 
(
	@HierarchyEditId int,
	@MajorRevisionInd bit,
	@Notes nvarchar(4000),
	@AmendUserId int,
	@PublicationId int OUTPUT
)

AS

BEGIN
	BEGIN TRY

		BEGIN TRAN

		DECLARE @AmendDate datetimeoffset(7) = SYSDATETIMEOFFSET()

		----------------------------------------------------------
		-- Create Publication record
		----------------------------------------------------------
		DECLARE @RootNodeVersionId int
		SELECT @RootNodeVersionId = MAX(nv.Id)
		FROM
			hierarchy.NodeVersion nv
		INNER JOIN
			hierarchy.NodePath np ON nv.NodeId = np.NodeId
		INNER JOIN 
			hierarchy.HierarchyEdit he ON np.Id = he.RootNodePathId
		WHERE
			he.Id = @HierarchyEditId
			AND nv.Deleted = 0
			AND he.Deleted = 0

		INSERT INTO [hierarchy].[Publication] ([ResourceVersionId],[NodeVersionId],[Notes],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			VALUES (NULL, @RootNodeVersionId, 'Publication of Hierarchy Edit Id=' + CAST(@HierarchyEditId as nvarchar(8)), 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate)
		SELECT @PublicationId = SCOPE_IDENTITY();

		----------------------------------------------------------
		-- NodeLink
		----------------------------------------------------------
		-- Create new NodeLinks (arising from Create or Reference Node)
		INSERT INTO [hierarchy].[NodeLink] ([ParentNodeId],[ChildNodeId],[DisplayOrder],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT
			ParentNodeId,
			NodeId AS ChildNodeId, 
			DisplayOrder,
			0 AS Deleted,
			@AmendUserId AS CreateUserId,
			@AmendDate AS CreateDate,
			@AmendUserId AS AmendUserId,
			@AmendDate AS AmendDate
		FROM 
			hierarchy.HierarchyEditDetail 
		WHERE
			HierarchyEditId = @HierarchyEditId
            AND 
				(
				HierarchyEditDetailTypeId = 4 -- Node Link
				OR
				HierarchyEditDetailTypeId = 3 -- Folder Node
				)
			AND (
				HierarchyEditDetailOperationId = 1 -- Add
				OR
				HierarchyEditDetailOperationId = 2 -- Edit
				OR
				HierarchyEditDetailOperationId = 4 -- Add Reference
				)
			AND NodeLinkId IS NULL
			AND [Deleted] = 0

		-- Update NodeLink 'DisplayOrder' for Edits (may be required due to create, move, move up, move down)
		-- note: requires publication log entry for cache update when display order changes in a folder with published resource.
		INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT @PublicationId, hed.ParentNodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeLink nl ON hed.NodeLinkId = nl.Id
		LEFT JOIN
			(SELECT DISTINCT NodeId FROM hierarchy.NodeResource WHERE Deleted = 0 AND VersionStatusId = 2) nr ON hed.NodeId = nr.NodeId
		WHERE 
			HierarchyEditId = @HierarchyEditId
			AND hed.DisplayOrder != nl.DisplayOrder
			AND hed.ResourceId IS NULL
			AND nr.NodeId IS NOT NULL
			AND hed.Deleted = 0
			AND nl.Deleted = 0

		UPDATE 
			nl
		SET
			DisplayOrder = hed.DisplayOrder,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeLink nl ON hed.NodeLinkId = nl.Id
		WHERE 
			HierarchyEditId = @HierarchyEditId
            AND hed.HierarchyEditDetailTypeId = 4 -- Node Link
			AND hed.DisplayOrder != nl.DisplayOrder
			AND hed.Deleted = 0
			AND nl.Deleted = 0

		-- UPDATE  NodeLink 'deleted' for remove reference

		UPDATE 
			nl
		SET
			Deleted = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeLink nl ON hed.NodeLinkId = nl.Id
		WHERE 
			HierarchyEditId = @HierarchyEditId
            AND hed.HierarchyEditDetailTypeId = 4 -- Node Link
			AND hed.Deleted = 1

		-- For moved nodes, delete the original NodeLinks, providing they have not been used (by a reference to the original position).
		UPDATE 
			nl
		SET
			Deleted = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeLink nl ON hed.NodeId = nl.ChildNodeId
								 AND hed.InitialParentNodeId = nl.ParentNodeId
		LEFT OUTER JOIN 
			hierarchy.HierarchyEditDetail hed2 ON nl.Id = hed2.NodeLinkId AND hed2.HierarchyEditId = @HierarchyEditId AND hed2.Deleted = 0 -- Check NodeLink has not been re-used
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND hed.InitialParentNodeId != hed.ParentNodeId
			AND hed.HierarchyEditDetailTypeId = 4 -- Node Link
			AND hed.HierarchyEditDetailOperationId = 2 -- Edit
			AND hed2.Id IS NULL
			AND hed.Deleted = 0
			AND nl.Deleted = 0

		-- Delete NodeLinks
		UPDATE 
			nl
		SET
			Deleted = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeLink nl ON hed.NodeLinkId = nl.Id
		WHERE 
			HierarchyEditId = @HierarchyEditId
            AND hed.HierarchyEditDetailTypeId = 4 -- Node Link
			AND hed.HierarchyEditDetailOperationId = 3 -- Delete
			AND hed.Deleted = 0
			AND nl.Deleted = 0

		----------------------------------------------------------
		-- NodeResource
		----------------------------------------------------------
		-- Identify NodeResource changes
		SELECT
			hed.ResourceId AS ResourceId,
			nr_current.Id AS CurrentNodeResourceId,
			nr_current.VersionStatusId AS CurrentNodeResourceVersionStatusId,
			nr_current.PublicationId AS CurrentNodeResourcePublicationId,
			nr_current.NodeId AS CurrentNodeId,
			hed.NodeId AS NewNodeId,
			hed.DisplayOrder
		INTO 
			#nodeResourceChanges
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeResource nr_current ON hed.NodeResourceId = nr_current.Id
		LEFT JOIN
			hierarchy.NodeResource nr_new ON hed.ParentNodeId = nr_new.NodeId AND hed.ResourceId = nr_new.ResourceId AND nr_new.Deleted = 0
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND nr_new.Id IS NULL

		-- Delete moved NodeResource/s from their previous locations
		UPDATE
			nr
		SET
			Deleted = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.NodeResource nr
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON nr.NodeId = hed.InitialParentNodeId AND nr.ResourceId = hed.ResourceId
		LEFT OUTER JOIN 
			hierarchy.HierarchyEditDetail hed2 ON nr.NodeId = hed2.ParentNodeId AND nr.ResourceId = hed2.ResourceId
												AND hed2.HierarchyEditId = @HierarchyEditId
												AND hed2.Deleted = 0 -- Check NodeResource has not been re-used
		WHERE
				hed.HierarchyEditId = @HierarchyEditId
			AND hed.InitialParentNodeId != hed.ParentNodeId
			AND hed2.Id IS NULL

		-- Create moved NodeResource/s in their new locations
		INSERT INTO [hierarchy].[NodeResource] ([NodeId],[ResourceId],[DisplayOrder],[VersionStatusId],[PublicationId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT  DISTINCT ParentNodeId, ResourceId, DisplayOrder, 2 /* Published */, @PublicationId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM    hierarchy.HierarchyEditDetail
		WHERE   HierarchyEditId = @HierarchyEditId
			AND HierarchyEditDetailTypeId = 5 -- Node Resource
			AND NodeResourceId IS NULL
			AND ISNULL(InitialParentNodeId, 0) != ParentNodeId
			AND Deleted =0

		-- Create publication log entries for cache updates related to MoveResource.
		INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT @PublicationId, NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM
		(
		SELECT 
			CurrentNodeId AS NodeId
		FROM
			#nodeResourceChanges
		UNION
		SELECT 
			NewNodeId AS NodeId
		FROM
			#nodeResourceChanges
		) AS T1

		-- Create publication log entries for cache updates related to Move Resource Up/Down.
		INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT @PublicationId, nr.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeResource nr ON hed.NodeResourceId = nr.Id
		WHERE 
			HierarchyEditId = @HierarchyEditId
			AND hed.DisplayOrder != nr.DisplayOrder
			AND hed.Deleted = 0
			AND nr.Deleted = 0

		-- Update NodeResource 'DisplayOrder' for Edits (may be required due to Move, Move up, move down)
		UPDATE 
			nr
		SET
			DisplayOrder = hed.DisplayOrder,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.HierarchyEditDetail hed
		INNER JOIN
			hierarchy.NodeResource nr ON hed.NodeResourceId = nr.Id
		WHERE 
			HierarchyEditId = @HierarchyEditId
			AND hed.DisplayOrder != nr.DisplayOrder
			AND hed.Deleted = 0
			AND nr.Deleted = 0

		----------------------------------------------------------
		-- NodeVersion
		----------------------------------------------------------
		-- Add / Edit operations
		UPDATE 
			n
		SET
			CurrentNodeVersionId = nv.Id,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.NodeVersion nv
		INNER JOIN
			hierarchy.[Node] n ON nv.NodeId = n.Id
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON nv.Id = hed.NodeVersionId
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND (
					(
						hed.HierarchyEditDetailOperationId = 1 -- Add
						AND
						hed.HierarchyEditDetailTypeId = 3 -- Folder Node
					)
					OR
					(
						hed.HierarchyEditDetailOperationId = 2 -- Edit
						AND
						hed.HierarchyEditDetailTypeId = 4 -- Node Link
					)
				)
			AND nv.VersionStatusId = 1 -- Draft
			AND nv.Deleted = 0
			AND hed.Deleted = 0

		-- Add / Edit operations
		UPDATE 
			nv
		SET
			VersionStatusId = 2,
			PublicationId = @PublicationId,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.NodeVersion nv
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON nv.Id = hed.NodeVersionId
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND (
					(
						hed.HierarchyEditDetailOperationId = 1 -- Add
						AND
						hed.HierarchyEditDetailTypeId = 3 -- Folder Node
					)
					OR
					(
						hed.HierarchyEditDetailOperationId = 2 -- Edit
						AND
						hed.HierarchyEditDetailTypeId = 4 -- Node Link
					)
				)
			AND nv.VersionStatusId = 1 -- Draft
			AND nv.Deleted = 0
			AND hed.Deleted = 0

		-- Delete operations
		UPDATE 
			nv
		SET
			VersionStatusId = 3,
			PublicationId = @PublicationId,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.NodeVersion nv
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON nv.Id = hed.NodeVersionId
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND hed.HierarchyEditDetailOperationId IN (3) -- Delete
			AND hed.HierarchyEditDetailTypeId = 3 -- Folder Node
			AND nv.Deleted = 0
			AND hed.Deleted = 0

		----------------------------------------------------------
		-- NodePathDisplayVersion
		----------------------------------------------------------
		-- Add / Edit operations
		UPDATE 
			npdv
		SET
			VersionStatusId = 2, -- Published
			PublicationId = @PublicationId,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hierarchy.NodePathDisplayVersion npdv
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON npdv.NodePathId = hed.NodePathId
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND (
					(
						hed.HierarchyEditDetailOperationId = 1 -- Add
						OR
						hed.HierarchyEditDetailOperationId = 2 -- Edit
					)
					AND
					(
						hed.HierarchyEditDetailTypeId = 3 -- Folder Node
						OR
						hed.HierarchyEditDetailTypeId = 4 -- Node Link
					)
				)
			AND npdv.VersionStatusId = 1 -- Draft
			AND npdv.Deleted = 0
			AND hed.Deleted = 0

			-- Delete replaced NodePathDisplayVersion records
			UPDATE 
				npdv
			SET
				Deleted = 1,
				AmendUserId = @AmendUserId,
				AmendDate = @AmendDate
			FROM
				hierarchy.NodePathDisplayVersion npdv
			INNER JOIN
				hierarchy.HierarchyEditDetail hed ON npdv.NodePathId = hed.NodePathId
			LEFT OUTER JOIN
				hierarchy.HierarchyEditDetail hed2 ON hed2.NodePathId = npdv.NodePathId 
												  AND hed2.HierarchyEditId = @HierarchyEditId 
												  AND hed2.NodePathDisplayVersionId = npdv.Id 
												  AND hed2.Deleted = 0
			WHERE
				hed.HierarchyEditId = @HierarchyEditId
				AND (
						(
							hed.HierarchyEditDetailOperationId = 1 -- Add
							OR
							hed.HierarchyEditDetailOperationId = 2 -- Edit
						)
						AND
						(
							hed.HierarchyEditDetailTypeId = 3 -- Folder Node
							OR
							hed.HierarchyEditDetailTypeId = 4 -- Node Link
						)
					)
				AND npdv.VersionStatusId = 2 -- Published
				AND npdv.Deleted = 0
				AND hed.Deleted = 0
				AND hed2.Id IS NULL


		----------------------------------------------------------
		-- NodePath: generate new NodePath/s and mark redundant NodePaths as inactive
		----------------------------------------------------------
		;WITH
		  cteNodePath(CatalogueNodeId, NodeId, NodeVersionId, ParentNodeId, NodeLinkId, NodePath, CompletePathInd)
		  AS
		  (
			SELECT 
				CatalogueNodeId = CASE WHEN rn.NodeTypeId = 1 THEN rn.Id ELSE NULL END,
				n.Id AS NodeId,
				n.CurrentNodeVersionId AS NodeVersionId,
				hed.ParentNodeId,
				nl.Id AS NodeLinkId,
				NodePath = CAST(CAST(hed.ParentNodeId AS nvarchar(8)) + '\' + CAST(hed.NodeId AS nvarchar(8)) AS nvarchar(128)),
				CompletePathInd = CASE WHEN hed.ParentNodePathID = he.RootNodePathId THEN 1 ELSE 0 END
			FROM 
				hierarchy.HierarchyEdit he
			INNER JOIN 
				hierarchy.HierarchyEditDetail hed ON hed.HierarchyEditId = he.Id
			INNER JOIN
				hierarchy.[NodePath] rnp ON he.RootNodePathId = rnp.Id
			INNER JOIN
				hierarchy.[Node] rn ON rnp.NodeId = rn.Id
			INNER JOIN
				hierarchy.[Node] n ON hed.NodeId = n.Id
			INNER JOIN 
				hierarchy.[NodeLink] nl ON hed.NodeId = nl.ChildNodeId
										AND hed.ParentNodeId = nl.ParentNodeId
			WHERE 
				he.Id = @HierarchyEditId
				AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
				AND n.Deleted = 0
				AND rn.Deleted = 0
				AND hed.Deleted = 0
				AND nl.Deleted = 0
	
			UNION ALL

			SELECT 
				CatalogueNodeId = CASE WHEN rn.NodeTypeId = 1 THEN rn.Id ELSE NULL END,
				cte.NodeId AS NodeId,
				n.CurrentNodeVersionId AS NodeVersionId,
				hed.ParentNodeId,
				nl.Id AS NodeLinkId,
				NodePath = CAST(CAST(hed.ParentNodeId AS nvarchar(8)) + '\' + cte.NodePath AS nvarchar(128)),
				CompletePathInd = CASE WHEN hed.ParentNodePathID = he.RootNodePathId THEN 1 ELSE 0 END
			FROM 
				hierarchy.HierarchyEdit he
			INNER JOIN 
				hierarchy.HierarchyEditDetail hed ON hed.HierarchyEditId = he.Id		
			INNER JOIN
				hierarchy.[NodePath] rnp ON he.RootNodePathId = rnp.Id
			INNER JOIN
				hierarchy.[Node] rn ON rnp.NodeId = rn.Id
			INNER JOIN 
				hierarchy.[Node] n ON hed.NodeId = n.Id
			INNER JOIN 
				hierarchy.[NodeLink] nl ON hed.NodeId = nl.ChildNodeId
										AND hed.ParentNodeId = nl.ParentNodeId
			INNER JOIN	
				cteNodePath cte ON hed.NodeId = cte.ParentNodeId AND cte.CompletePathInd = 0
			WHERE 
				he.Id = @HierarchyEditId
				AND ISNULL(hed.HierarchyEditDetailOperationId, 0) != 3 -- exclude Delete operations
				AND n.Deleted = 0
				AND rn.Deleted = 0
				AND hed.Deleted = 0
				AND nl.Deleted = 0
			)
		SELECT cte.NodeId, cte.NodePath, cte.CatalogueNodeId, np.Id AS NodePathId, np.IsActive AS NodePathIsActive
		INTO #cteNodePath 
		FROM cteNodePath cte
		LEFT JOIN [hierarchy].[NodePath] np ON cte.NodePath = np.NodePath AND np.Deleted = 0
		WHERE CompletePathInd=1;

		DECLARE @rootNodeId int
		SELECT @rootNodeId = np.NodeId
		FROM hierarchy.HierarchyEdit he
		INNER JOIN hierarchy.NodePath np ON he.RootNodePathId = np.Id
		WHERE he.Id = @HierarchyEditId


		-- Reactivate any matched existing NodePath/s that are currently inactive
		UPDATE 
			np
		SET 
			IsActive = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM 
			hierarchy.NodePath np
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON np.NodeId = hed.NodeId AND np.NodePath = ISNULL(hed.NewNodePath, hed.InitialNodePath)
		WHERE 
			np.CatalogueNodeId = @rootNodeId
			AND hed.HierarchyEditId = @HierarchyEditId
			AND np.Deleted = 0 
			AND np.IsActive = 0

		-- Deactivate redundant NodePath/s
		UPDATE 
			np
		SET 
			IsActive = 0,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM 
			hierarchy.NodePath np
		LEFT JOIN
			hierarchy.HierarchyEditDetail hed ON np.NodeId = hed.NodeId AND np.NodePath = ISNULL(hed.NewNodePath, hed.InitialNodePath)
		WHERE 
			np.CatalogueNodeId = @rootNodeId
			AND hed.HierarchyEditId = @HierarchyEditId
			AND np.IsActive = 1 
			AND np.CatalogueNodeId != np.NodeId
			AND hed.NodeId IS NULL

	    ----------------------------------------------------------
		-- NodePath: generate new NodePath/s for refered Catalogues
		----------------------------------------------------------

		EXEC [hierarchy].[HierarchyNewNodePathForReferedCatalogue] @HierarchyEditId,@AmendUserId,@AmendDate
		----------------------------------------------------------
		-- NodePathNode
		----------------------------------------------------------		
		-- Generate NodePathNode entries
		DECLARE @NodePathId int
        DECLARE @NodeId int
        DECLARE @ParentNodeId int
		DECLARE @InitialNodePath as NVARCHAR(256)
		DECLARE @NewNodePath as NVARCHAR(256)
		DECLARE @HierarchyEditDetailOperationId int
		DECLARE @NodePathCursor as CURSOR
 
		SET @NodePathCursor = CURSOR FORWARD_ONLY FOR
        SELECT  NodePathId, NodeId, ParentNodeId, InitialNodePath, NewNodePath, HierarchyEditDetailOperationId
        FROM    hierarchy.HierarchyEditDetail
        WHERE   HierarchyEditId = 80
            AND ISNULL(InitialNodePath, '') != ISNULL(NewNodePath, InitialNodePath)
            AND (
                HierarchyEditDetailTypeId = 4 -- Node Link
                OR
                HierarchyEditDetailTypeId = 3 -- Folder Node
                )

		OPEN @NodePathCursor;
		FETCH NEXT FROM @NodePathCursor INTO @NodePathId, @NodeId, @ParentNodeId, @InitialNodePath, @NewNodePath, @HierarchyEditDetailOperationId;
			WHILE @@FETCH_STATUS = 0
		BEGIN

			IF (@HierarchyEditDetailOperationId = 3) -- Delete
			BEGIN
				-- Mark as deleted for removed nodepaths
				UPDATE npn
				SET Deleted = 1,
					AmendUserID = @AmendUserId,
					AmendDate = @AmendDate
				FROM
					hierarchy.NodePathNode npn
				WHERE NodePathId = @NodePathId
					AND Deleted = 0
			END
			Else
			BEGIN
				-- Update for changed nodepaths
				UPDATE npn
				SET NodeId = nodeInPath.[Value],
					AmendUserID = @AmendUserId,
					AmendDate = @AmendDate
				FROM
					hub.[fn_Split](@NewNodePath, '\') nodeInPath
				INNER JOIN
					hierarchy.NodePathNode npn ON npn.NodePathId = @NodePathId 
												AND npn.Depth = nodeInPath.idx
												AND npn.NodeId != nodeInPath.[Value]
				WHERE npn.Deleted = 0

				-- Insert for changed nodepaths
				INSERT INTO [hierarchy].[NodePathNode]([NodePathId],[NodeId],[Depth],Deleted,[CreateUserID],[CreateDate],[AmendUserID],[AmendDate])
				SELECT
					NodePathId = @NodePathId,
					NodeId = nodeInPath.[Value],
					Depth = nodeInPath.idx,
					Deleted = 0,
					CreateUserID = @AmendUserId,
					CreateDate = @AmendDate,
					AmendUserID = @AmendUserId,
					AmendDate = @AmendDate
				FROM
					hub.[fn_Split](@NewNodePath, '\') nodeInPath
				LEFT OUTER JOIN
					hierarchy.NodePathNode npn ON npn.NodePathId = @NodePathId 
												AND npn.Depth = nodeInPath.idx
												AND npn.NodeId = nodeInPath.[Value]
												AND npn.Deleted = 0
				WHERE   npn.Id IS NULL

				-- Mark as deleted for changed nodepaths
				UPDATE npn
				SET Deleted = 1,
					AmendUserID = @AmendUserId,
					AmendDate = @AmendDate
				FROM
					hierarchy.NodePathNode npn
				LEFT OUTER JOIN
					hub.[fn_Split](@NewNodePath, '\') nodeInPath ON npn.Depth = nodeInPath.idx
																AND npn.NodeId = nodeInPath.[Value]
																AND npn.Deleted = 0
				WHERE npn.NodePathId = @NodePathId
                    AND nodeInPath.[value] IS NULL
			END


			FETCH NEXT FROM @NodePathCursor INTO @NodePathId, @NodeId, @ParentNodeId, @InitialNodePath, @NewNodePath, @HierarchyEditDetailOperationId;

		END

		CLOSE @NodePathCursor;
		DEALLOCATE @NodePathCursor;


		--------------------------------------------------------------
		------ ResourceReference
		--------------------------------------------------------------		

		-- Activate any matched existing ResourceReference/s that are currently inactive
		UPDATE 
			rr
		SET 
			IsActive = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM 
			resources.ResourceReference rr
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON rr.ResourceId = hed.ResourceId AND rr.NodePathId = hed.ParentNodePathId
		WHERE
				HierarchyEditDetailTypeId = 5 -- Node Resource
			AND hed.HierarchyEditId = @HierarchyEditId
			AND rr.Deleted = 0 
			AND rr.IsActive = 0

		-- Delete redundant ResourceReference/s
		UPDATE 
			rr
		SET 
			Deleted = 1,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM 
			resources.ResourceReference rr
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON rr.ResourceId = hed.ResourceId AND rr.NodePathId = hed.InitialParentNodePathId
		LEFT JOIN
			hierarchy.HierarchyEditDetail hed2 ON rr.Id = hed2.ResourceReferenceId
												AND hed2.HierarchyEditId = @HierarchyEditId
												AND hed2.Deleted = 0
		WHERE 
			hed.HierarchyEditDetailTypeId = 5 -- Node Resource
			AND hed.HierarchyEditId = @HierarchyEditId
			AND rr.Deleted = 0 
			AND ISNULL(hed.InitialParentNodePathId, 0) != hed.ParentNodePathId
			AND hed2.Id IS NULL


		----------------------------------------------------------
		-- ResourceReferenceDisplayVersion
		----------------------------------------------------------
		-- Add / Edit operations
		UPDATE 
			rrdv
		SET
			VersionStatusId = 2, -- Published
			PublicationId = @PublicationId,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			resources.ResourceReferenceDisplayVersion rrdv
		INNER JOIN
			hierarchy.HierarchyEditDetail hed ON rrdv.Id = hed.ResourceReferenceDisplayVersionId
		WHERE
			hed.HierarchyEditId = @HierarchyEditId
			AND (
				hed.HierarchyEditDetailOperationId = 1 -- Add
				OR
				hed.HierarchyEditDetailOperationId = 2 -- Edit
				)
			AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
			AND rrdv.VersionStatusId = 1 -- Draft
			AND rrdv.Deleted = 0
			AND hed.Deleted = 0

			-- Delete replaced NodePathDisplayVersion records
			UPDATE 
				rrdv
			SET
				Deleted = 1,
				AmendUserId = @AmendUserId,
				AmendDate = @AmendDate
			FROM
				resources.ResourceReferenceDisplayVersion rrdv
			INNER JOIN
				hierarchy.HierarchyEditDetail hed ON rrdv.ResourceReferenceId = hed.ResourceReferenceId
			LEFT OUTER JOIN
				hierarchy.HierarchyEditDetail hed2 ON hed2.ResourceReferenceId = rrdv.ResourceReferenceId 
												  AND hed2.HierarchyEditId = @HierarchyEditId 
												  AND hed2.ResourceReferenceDisplayVersionId = rrdv.Id 
												  AND hed2.Deleted = 0
			WHERE
				hed.HierarchyEditId = @HierarchyEditId
				AND (
					hed.HierarchyEditDetailOperationId = 1 -- Add
					OR
					hed.HierarchyEditDetailOperationId = 2 -- Edit
					)
				AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
				AND rrdv.VersionStatusId = 2 -- Published
				AND rrdv.Deleted = 0
				AND hed.Deleted = 0
				AND hed2.Id IS NULL

		----------------------------------------------------------
		-- NodeResourceLookup
		-- Register all affected Nodes to the PublicationLog - used for cache refresh.
		----------------------------------------------------------
		-- IT1 - new table to provide quick lookup all Published Resources under a node
		;WITH cteNodeResource(NodeId, ParentNodeId, ResourceId)
		AS (
			SELECT 
				cte.NodeId,
				nl.ParentNodeId,
				nr.ResourceId
			FROM 
				#cteNodePath cte
			INNER JOIN
				[hierarchy].[NodeResource] nr ON cte.NodeId = nr.NodeId
			LEFT JOIN
				hierarchy.NodeLink nl ON nr.NodeId = nl.ChildNodeId AND nl.Deleted = 0
			WHERE
				nr.VersionStatusId = 2 -- Published
				AND nr.Deleted = 0
				AND nl.Deleted = 0
		
			UNION ALL

			SELECT 
				cte.ParentNodeId AS NodeId,
				nl.ParentNodeId AS ParentNodeId,
				cte.ResourceId
			FROM
				cteNodeResource cte			
			INNER JOIN
				hierarchy.NodeLink nl ON nl.ChildNodeId = cte.ParentNodeId
			WHERE
				nl.Deleted = 0
				AND nl.Deleted = 0
		  )
		SELECT DISTINCT NodeId, ParentNodeId, ResourceId
		INTO #cteNodeResource
		FROM cteNodeResource

		-- Add "root node" resource lookup info
		INSERT INTO #cteNodeResource
		SELECT nr1.ParentNodeId AS NodeId, NULL AS ParentNodeId, nr1.ResourceId
		FROM #cteNodeResource nr1
		LEFT JOIN #cteNodeResource nr2 ON nr1.ParentNodeId = nr2.NodeId
		WHERE nr2.NodeId IS NULL

		-- Inserts
		INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT @PublicationId, cte.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM #cteNodeResource cte
		LEFT JOIN hierarchy.NodeResourceLookup nrl ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId
		WHERE nrl.Id IS NULL

		INSERT INTO hierarchy.NodeResourceLookup ([NodeId],[ResourceId],[Deleted],[CreateUserID],[CreateDate],[AmendUserID],[AmendDate])
		SELECT cte.NodeId, cte.ResourceId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM #cteNodeResource cte
		LEFT JOIN hierarchy.NodeResourceLookup nrl ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId
		WHERE nrl.Id IS NULL
	
		-- Reinstate/s
		INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT @PublicationId, nrl.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM hierarchy.NodeResourceLookup nrl
		INNER JOIN #cteNodeResource cte ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId AND nrl.deleted = 1

		UPDATE nrl
		SET deleted = 0, AmendUserId = @AmendUserId, AmendDate = @AmendDate
		FROM hierarchy.NodeResourceLookup nrl
		INNER JOIN #cteNodeResource cte ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId AND nrl.deleted = 1
	
		-- Delete/s
		INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT DISTINCT @PublicationId, nrl.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM hierarchy.NodeResourceLookup nrl
		INNER JOIN (SELECT DISTINCT ResourceId FROM #cteNodeResource) cte1 ON cte1.ResourceId = nrl.ResourceId
		LEFT JOIN #cteNodeResource cte2 ON cte2.NodeId = nrl.NodeId AND cte2.ResourceId = nrl.ResourceId
		WHERE cte2.NodeId IS NULL AND nrl.Deleted = 0

		UPDATE nrl
		SET deleted = 1, AmendUserId = @AmendUserId, AmendDate = @AmendDate
		FROM hierarchy.NodeResourceLookup nrl
		INNER JOIN (SELECT DISTINCT ResourceId FROM #cteNodeResource) cte1 ON cte1.ResourceId = nrl.ResourceId
		LEFT JOIN #cteNodeResource cte2 ON cte2.NodeId = nrl.NodeId AND cte2.ResourceId = nrl.ResourceId
		WHERE cte2.NodeId IS NULL AND nrl.Deleted = 0

		-------------------------------------------------------------
		-- Mark any removed NodePathDisplayVersion records as deleted
		-------------------------------------------------------------
		UPDATE  npdv
		SET     Deleted = 1,
				AmendUserId = @AmendUserId,
				AmendDate = @AmendDate        
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN hierarchy.NodePathDisplayVersion npdv ON hed.NodePathId = npdv.NodePathId
		WHERE   HierarchyEditId = @HierarchyEditId
			AND hed.NodePathDisplayVersionId IS NULL
			AND hed.Deleted = 0
			AND npdv.Deleted = 0

		-------------------------------------------------------------
		-- Mark any removed ResourceReferenceDisplayVersion records as deleted
		-------------------------------------------------------------
		UPDATE  rrdv
		SET     Deleted = 1,
				AmendUserId = @AmendUserId,
				AmendDate = @AmendDate        
		FROM    hierarchy.HierarchyEditDetail hed
		INNER JOIN resources.ResourceReferenceDisplayVersion rrdv ON hed.ResourceReferenceId = rrdv.ResourceReferenceId
		WHERE   HierarchyEditId = @HierarchyEditId
			AND hed.ResourceReferenceDisplayVersionId IS NULL
			AND hed.Deleted = 0
			AND rrdv.Deleted = 0

		----------------------------------------------------------
		-- Mark the HierarchyEdit as 'Published'
		----------------------------------------------------------
		UPDATE 
			hierarchy.HierarchyEdit
		SET
			PublicationId = @PublicationId,
			[HierarchyEditStatusId] = 2,
			AmendUserId = @AmendUserId,
			AmendDate = @AmendDate
		WHERE
			[Id] = @HierarchyEditId

		DROP TABLE #cteNodePath
		DROP TABLE #cteNodeResource
		--DROP TABLE #nodePathResource
		DROP TABLE #nodeResourceChanges

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