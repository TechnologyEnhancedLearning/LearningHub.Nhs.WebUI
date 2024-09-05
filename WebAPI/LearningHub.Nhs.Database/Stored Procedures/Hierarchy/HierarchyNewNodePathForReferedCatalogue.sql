-------------------------------------------------------------------------------
-- Author       Sarathlal 
-- Created      21-08-2024
-- Purpose      Publishing catalogues needs to update referencing catalogues
--
-- Modification History
-- 21-08-2024  SS	Initial version
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyNewNodePathForReferedCatalogue]
(
	@HierarchyEditId INT,
	@AmendUserId INT,
	@AmendDate  datetimeoffset(7)
)

AS

BEGIN
	BEGIN TRY

		BEGIN TRAN
			DECLARE @NodePathId int
			DECLARE @PrimaryCatalogueNodeId int
			DECLARE @NodeId int
			DECLARE @ParentNodeId int
			DECLARE @NewNodePath as NVARCHAR(256)
			DECLARE @DisplayOrder int
			DECLARE @NewNodePathCursor as CURSOR
			SET @NewNodePathCursor = CURSOR FORWARD_ONLY FOR
				SELECT
						PrimaryCatalogueNodeId,
						ParentNodeId,
						NodeId, 
						hed.NewNodePath,
						DisplayOrder,
						NodePathId
					FROM 
						hierarchy.HierarchyEditDetail hed
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
							)
						AND NodeLinkId IS NULL
						AND [Deleted] = 0
			OPEN @NewNodePathCursor;
					FETCH NEXT FROM @NewNodePathCursor INTO @PrimaryCatalogueNodeId,@ParentNodeId,@NodeId,@NewNodePath,@DisplayOrder,@NodePathId;
						WHILE @@FETCH_STATUS = 0
						BEGIN
						IF NOT EXISTS (SELECT 1 FROM hierarchy.NodePath WHERE NodeId=@NodeId AND NodePath=NodePath+'\'+CAST(@NodeId AS VARCHAR(100)))
							BEGIN
								INSERT INTO hierarchy.NodePath (NodeId, NodePath, CatalogueNodeId, IsActive, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
									SELECT @NodeId,NP.NodePath+'\'+CAST(@NodeId AS VARCHAR(100)),NP.CatalogueNodeId,1,0,@AmendUserId,@AmendDate,@AmendUserId,@AmendDate FROM 
									hub.[fn_Split](@NewNodePath, '\') nodeInPath
									INNER JOIN hierarchy.NodePath NP ON NP.NodeId=nodeInPath.value AND CatalogueNodeId!=@PrimaryCatalogueNodeId
									WHERE nodeInPath.value=@ParentNodeId

							END
						

						FETCH NEXT FROM @NewNodePathCursor INTO @PrimaryCatalogueNodeId,@ParentNodeId,@NodeId,@NewNodePath,@DisplayOrder,@NodePathId;

					END

			CLOSE @NewNodePathCursor;
			DEALLOCATE @NewNodePathCursor;
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