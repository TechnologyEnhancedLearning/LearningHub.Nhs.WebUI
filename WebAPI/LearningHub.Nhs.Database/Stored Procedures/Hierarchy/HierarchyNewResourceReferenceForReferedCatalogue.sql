-------------------------------------------------------------------------------
-- Author       Sarathlal 
-- Created      11-09-2024 
-- Purpose      Move resource in primary catalogue after external reference should reflect secondary catalogue
--
-- Modification History
-- 11-09-2024  SS	Initial version
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyNewResourceReferenceForReferedCatalogue]
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
			DECLARE @ResourceId INT
			DECLARE @PrimaryCatalogueNodeId int
			DECLARE @NodeId int
			DECLARE @ParentNodeId int
			DECLARE @NewNodePath as NVARCHAR(256)
			DECLARE @ResourceReferenceId INT
			DECLARE @ResourceReferenceCursor as CURSOR
			SET @ResourceReferenceCursor = CURSOR FORWARD_ONLY FOR
				SELECT
						ResourceId,
						PrimaryCatalogueNodeId,
						ParentNodeId,
						NodeId, 
						hed.NewNodePath,
						NodePathId,
						ResourceReferenceId
					FROM 
						hierarchy.HierarchyEditDetail hed
					WHERE
						HierarchyEditId = @HierarchyEditId
						AND 
							(
							
							HierarchyEditDetailTypeId = 5 -- Resource
							)
						AND (
							HierarchyEditDetailOperationId = 1 -- Add
							OR
							HierarchyEditDetailOperationId = 2 -- Edit
							)
						AND NodeLinkId IS NULL
						AND [Deleted] = 0
			OPEN @ResourceReferenceCursor;
					FETCH NEXT FROM @ResourceReferenceCursor INTO @ResourceId,@PrimaryCatalogueNodeId,@ParentNodeId,@NodeId,@NewNodePath,@NodePathId,@ResourceReferenceId;
						WHILE @@FETCH_STATUS = 0
						BEGIN
						
						INSERT INTO [resources].[ResourceReference]([ResourceId],[NodePathId],[OriginalResourceReferenceId],[IsActive],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
									SELECT @ResourceId,NP.Id,@ResourceReferenceId,1,0,@AmendUserId,@AmendDate,@AmendUserId,@AmendDate 
									FROM 
									hub.[fn_Split](@NewNodePath, '\') nodeInPath
									INNER JOIN hierarchy.NodePath NP ON NP.NodeId=nodeInPath.value AND CatalogueNodeId!=@PrimaryCatalogueNodeId
									WHERE nodeInPath.value=@ParentNodeId
			
						FETCH NEXT FROM @ResourceReferenceCursor INTO @ResourceId,@PrimaryCatalogueNodeId,@ParentNodeId,@NodeId,@NewNodePath,@NodePathId,@ResourceReferenceId;

					END

			CLOSE @ResourceReferenceCursor;
			DEALLOCATE @ResourceReferenceCursor;
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