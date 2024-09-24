-------------------------------------------------------------------------------
-- Author       Swapnamol Abraham
-- Created      18-09-2024
-- Purpose      Create a resource reference within a Hierarchy Edit.
--
-- Modification History
--
-- 18-09-2024  SA	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditCreateResourceReference]
(
	@HierarchyEditDetailId int,
	@MoveToHierarchyEditDetailId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	BEGIN TRY

		BEGIN TRAN	

		DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

		DECLARE @HierarchyEditId int
		DECLARE @ResourceId int, @ParentNodeId INT
		SELECT @HierarchyEditId = HierarchyEditId, @ResourceId = ResourceId,@ParentNodeId = ParentNodeId FROM [hierarchy].[HierarchyEditDetail] WHERE Id = @HierarchyEditDetailId
		

	 -- creating a resource reference into a referenced folder should affect all instances of the referenced folder

		DECLARE @CurrentNodeId INT,
                @ReferenceHierarchyEditDetailId INT

        SELECT @CurrentNodeId = hed.NodeId
        FROM [hierarchy].[HierarchyEditDetail] hed
        WHERE hed.Id = @MoveToHierarchyEditDetailId 
		      AND HierarchyEditId = @HierarchyEditId
		
        -- Declare the cursor

		 DECLARE NodeCursor CURSOR FOR 
                  SELECT Id AS ReferenceHierarchyEditDetailId
                  FROM hierarchy.HierarchyEditDetail
                                         WHERE NodeId = @CurrentNodeId     
											   AND HierarchyEditId = @HierarchyEditId
											   AND deleted = 0
        -- Open the cursor
        OPEN NodeCursor;

        -- Fetch the first row from the cursor
        FETCH NEXT FROM NodeCursor
        INTO @ReferenceHierarchyEditDetailId;

        -- Loop until no more rows are returned
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Execute the update statement for the current row
          EXEC [hierarchy].[HierarchyEditReferenceResource] @HierarchyEditDetailId, @ReferenceHierarchyEditDetailId, @UserId,@UserTimezoneOffset

            -- Fetch the next row from the cursor
            FETCH NEXT FROM NodeCursor
            INTO @ReferenceHierarchyEditDetailId;
        END

        -- Close and deallocate the cursor
        CLOSE NodeCursor;
        DEALLOCATE NodeCursor;

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