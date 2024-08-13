-------------------------------------------------------------------------------
-- Author       Swapna Abraham
-- Created      07-08-2024
-- Purpose      Moving a folder from a referenced path should remove it from all instances of the referenced path
--
-- Modification History
--
-- 07-08-2024  SA	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditRemoveNodeReferencesOnMoveNode]
(
    @HierarchyEditDetailId int,
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

        DECLARE @CurrentNodeId INT,
                @ReferenceHierarchyEditDetailId INT,
                @RootNodePathId INT,
                @ChildHierachyEditDetailId INT

        SELECT @CurrentNodeId = hed.NodeId
        FROM [hierarchy].[HierarchyEditDetail] hed
        WHERE hed.Id = @HierarchyEditDetailId

        --SELECT @ReferenceHierarchyEditDetailId = hed.id,
        --       @RootNodePathId = NodePathId
        --FROM hierarchy.HierarchyEditDetail hed
        --WHERE hed.NodeId = @CurrentNodeId
        --      AND hed.Id != @HierarchyEditDetailId
        -- Declare the cursor
        DECLARE NodeCursor CURSOR FOR WITH RecursiveNodes
                                      AS (
                                         -- Anchor member: select the root node
                                         SELECT id,
                                                NodePathId,
                                                ParentNodePathId
                                         FROM hierarchy.HierarchyEditDetail
                                         WHERE NodeId = @CurrentNodeId
                                               AND Id != @HierarchyEditDetailId
                                         UNION ALL

                                         -- Recursive member: select child nodes
                                         SELECT n.id,
                                                n.NodePathId,
                                                n.ParentNodePathId
                                         FROM hierarchy.HierarchyEditDetail n
                                             INNER JOIN RecursiveNodes rn
                                                 ON n.ParentNodePathId = rn.NodePathId
                                         )
        SELECT Id AS ChildHierachyEditDetailId
        FROM RecursiveNodes
        order by Id

        -- Open the cursor
        OPEN NodeCursor;

        -- Fetch the first row from the cursor
        FETCH NEXT FROM NodeCursor
        INTO @ChildHierachyEditDetailId;

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
            WHERE hed.Id = @ChildHierachyEditDetailId;

            -- Fetch the next row from the cursor
            FETCH NEXT FROM NodeCursor
            INTO @ChildHierachyEditDetailId;
        END

        -- Close and deallocate the cursor
        CLOSE NodeCursor;
        DEALLOCATE NodeCursor;

        -- Decrement display order of sibling nodes with higher display order

        -- Declare the cursor
        DECLARE HierarchyCursor CURSOR FOR
        SELECT hed.Id AS ReferenceHierarchyEditDetailId
        FROM hierarchy.HierarchyEditDetail hed
        WHERE hed.NodeId = @CurrentNodeId
              AND hed.Id != @HierarchyEditDetailId;

        -- Open the cursor
        OPEN HierarchyCursor;

        -- Fetch the first row from the cursor
        FETCH NEXT FROM HierarchyCursor
        INTO @ReferenceHierarchyEditDetailId;

        -- Loop through all rows fetched by the cursor
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Process each row
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
            FETCH NEXT FROM HierarchyCursor
            INTO @ReferenceHierarchyEditDetailId;
        END;

        -- Close the cursor
        CLOSE HierarchyCursor;

        -- Deallocate the cursor
        DEALLOCATE HierarchyCursor;

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