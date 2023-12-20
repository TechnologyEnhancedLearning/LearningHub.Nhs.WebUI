-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      25-08-2021
-- Purpose      Duplicates a Block Collection
--
-- Modification History
--
-- 25-08-2021  Julian Ng	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[BlockCollectionCreateDuplicate]
    (
        @CurrentBlockCollectionId INT,
        @UserId INT,
	    @UserTimezoneOffset int = NULL,
        @NewBlockCollectionId INT OUTPUT
    )
AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
    DECLARE @CopiedBlocksTable CopiedBlocksType
    DECLARE @BlockCrossRefIds TABLE (
        OldBlockId INT,
        NewBlockId INT
    )
    
    -- Create a new Block Collection
    INSERT INTO resources.BlockCollection (Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT Deleted, @UserId, @AmendDate, @UserId, @AmendDate
    FROM resources.BlockCollection
    WHERE Id = @CurrentBlockCollectionId
    SELECT @NewBlockCollectionId = SCOPE_IDENTITY()

    -- Copy Blocks
    -- Save details that we want to duplicate into @CopiedBlocksTable
    INSERT INTO @CopiedBlocksTable
    SELECT Id AS OriginalBlockId, NULL AS BlockId, @CurrentBlockCollectionId AS OriginalBlockCollectionId, @NewBlockCollectionId, [Order], Title, BlockType, 0, @UserId, @AmendDate, @UserId, @AmendDate
    FROM resources.Block
    WHERE BlockCollectionId = @CurrentBlockCollectionId
    AND Deleted = 0
    ORDER BY OriginalBlockId

    -- Merge the saved details into the Block table, saving the original and new Block IDs into @BlockCrossRefIds table
    MERGE INTO resources.Block
    USING @CopiedBlocksTable AS cbt ON 1 = 0
    WHEN NOT MATCHED BY TARGET
        THEN INSERT (BlockCollectionId, [Order], Title, BlockType, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
             VALUES (cbt.BlockCollectionId, cbt.[Order], cbt.Title, cbt.BlockType, cbt.Deleted, cbt.CreateUserId, cbt.CreateDate, cbt.AmendUserId, cbt.AmendDate)
        OUTPUT cbt.OriginalBlockId, inserted.ID
            INTO @BlockCrossRefIds (OldBlockId, NewBlockId) ;

    -- Update the @CopiedBlocksTable with the newly generated NewBlockId value
    UPDATE @CopiedBlocksTable
    SET BlockId = NewBlockId
    FROM @BlockCrossRefIds
    WHERE OriginalBlockId = OldBlockId

    -- Returns table as output
    SELECT * FROM @CopiedBlocksTable
END