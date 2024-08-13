-------------------------------------------------------------------------------
-- Author       Swapna Abraham
-- Created      07-08-2024
-- Purpose      Moving a resource from a referenced path should remove it from all instances of the referenced path
--
-- Modification History
--
-- 08-08-2024  SA	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditRemoveResourceReferencesOnMoveResource]
(
    @HierarchyEditDetailId int,
	@ParentNodeId INT,
    @UserId int,
    @UserTimezoneOffset int = NULL
)
AS
BEGIN

    BEGIN TRY

        BEGIN TRAN

        DECLARE @AmendDate datetimeoffset(7)
            = ISNULL(
                        TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset),
                        SYSDATETIMEOFFSET()
                    )

        DECLARE @CurrentResourceId INT,
                @ReferenceHierarchyEditDetailId INT,
                @RootNodePathId INT

        SELECT @CurrentResourceId = hed.ResourceId
        FROM [hierarchy].[HierarchyEditDetail] hed
        WHERE hed.Id = @HierarchyEditDetailId

        -- Declare the cursor
        DECLARE NodeCursor CURSOR FOR 
                  SELECT Id AS ReferenceHierarchyEditDetailId
                  FROM hierarchy.HierarchyEditDetail
				  WHERE ResourceId = @CurrentResourceId
				        AND ParentNodeId = @ParentNodeId
                        AND Id != @HierarchyEditDetailId                                                                                

        -- Open the cursor
        OPEN NodeCursor;

        -- Fetch the first row from the cursor
        FETCH NEXT FROM NodeCursor
        INTO @ReferenceHierarchyEditDetailId;

        -- Loop until no more rows are returned
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Execute the update statement for the current row
            UPDATE hed
            SET HierarchyEditDetailOperationId = CASE
                                                     WHEN HierarchyEditDetailOperationId = 1 THEN
                                                         HierarchyEditDetailOperationId
                                                     ELSE
                                                         5
                                                 END, -- Remove reference
                Deleted = 1,
                AmendUserId = @UserId,
                AmendDate = @AmendDate
            FROM [hierarchy].[HierarchyEditDetail] hed
            WHERE hed.Id = @ReferenceHierarchyEditDetailId;

			UPDATE hed
            SET HierarchyEditDetailOperationId = CASE
                                                     WHEN hed.HierarchyEditDetailOperationId IS NULL THEN
                                                         2
                                                     ELSE
                                                         hed.HierarchyEditDetailOperationId
                                                 END,
                DisplayOrder = hed.DisplayOrder - 1,
                AmendUserId = @UserId,
                AmendDate = @AmendDate
            FROM [hierarchy].[HierarchyEditDetail] hed
                INNER JOIN [hierarchy].[HierarchyEditDetail] hed_remove
                    ON hed.HierarchyEditId = hed_remove.HierarchyEditId
                       AND hed.ParentNodeId = hed_remove.ParentNodeId
            WHERE hed_remove.Id = @ReferenceHierarchyEditDetailId
                  AND hed.DisplayOrder > hed_remove.DisplayOrder
                  AND hed.ResourceId IS NULL
                  AND hed.Deleted = 0

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

        SELECT @ErrorMessage = ERROR_MESSAGE(),
               @ErrorSeverity = ERROR_SEVERITY(),
               @ErrorState = ERROR_STATE();

        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END

GO