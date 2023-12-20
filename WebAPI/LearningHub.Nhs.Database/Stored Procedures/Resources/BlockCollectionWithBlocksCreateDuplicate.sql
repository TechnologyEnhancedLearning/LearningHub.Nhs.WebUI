-------------------------------------------------------------------------------
-- Author       Malina Slevoaca (Softwire)
-- Created      21-09-2021
-- Purpose      Duplicates a Block Collection and its blocks
--
-- Modification History
--
-- 21-09-2021  Malina Slevoaca	Initial Revision
-- 15-12-2021  Malina Slevoaca	Support Fractional duplication
-------------------------------------------------------------------------------
CREATE TYPE [resources].[IDList]
AS TABLE
(
    ID INT
)
GO

CREATE PROCEDURE [resources].[BlockCollectionWithBlocksCreateDuplicate]
    (
        @CurrentBlockCollectionId INT,
        @UserId INT,
        @BlocksToDuplicate IDList READONLY,
        @DestinationBlockCollectionId INT,
        @UserTimezoneOffset INT = NULL,
        @NewBlockCollectionId INT OUTPUT
    )
AS

BEGIN
    DECLARE @CurrentBlockIds CurrentBlockIdsType
    DECLARE @CopiedBlocksTable CopiedBlocksType
	DECLARE @BlocksToDuplicateCount int
	DECLARE @LastOrder int
	DECLARE @BlockCrossRefIds TABLE (
        OldBlockId INT,
        NewBlockId INT
    )
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

    IF @CurrentBlockCollectionId IS NOT NULL
        BEGIN
            DELETE FROM @CurrentBlockIds

            SELECT @BlocksToDuplicateCount = Count(*) FROM @BlocksToDuplicate
            IF @BlocksToDuplicateCount = 0
                BEGIN
                    INSERT INTO @CurrentBlockIds(BlockId)
                    SELECT Id
                    FROM resources.Block
                    WHERE BlockCollectionId = @CurrentBlockCollectionId
                      AND Deleted = 0
                    
                    -- Copy the BlockCollections and their Blocks
                    INSERT INTO @CopiedBlocksTable
                        EXECUTE resources.BlockCollectionCreateDuplicate @CurrentBlockCollectionId, @UserId, @UserTimezoneOffset, @NewBlockCollectionId OUTPUT
                END
            ELSE
                BEGIN
                    INSERT INTO @CurrentBlockIds(BlockId)
                    SELECT ID
                    FROM @BlocksToDuplicate
                    
                    SET @LastOrder = (SELECT MAX([Order]) FROM Block where BlockCollectionId = @DestinationBlockCollectionId);
    
                    INSERT INTO @CopiedBlocksTable
                    SELECT Id AS OriginalBlockId, NULL AS BlockId, @CurrentBlockCollectionId AS OriginalBlockCollectionId, @DestinationBlockCollectionId, [Order], Title, BlockType, 0, @UserId, @AmendDate, @UserId, @AmendDate
                    FROM resources.Block
                    WHERE Id IN (SELECT ID FROM @BlocksToDuplicate)
                    
                    MERGE INTO resources.Block
                    USING @CopiedBlocksTable AS cbt ON 1 = 0
                    WHEN NOT MATCHED BY TARGET
                        THEN INSERT (BlockCollectionId, [Order], Title, BlockType, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                            VALUES (cbt.BlockCollectionId, (SELECT ISNULL(@LastOrder, -1) + ROW_NUMBER() over (order by [Order]) FROM @CopiedBlocksTable WHERE [Order]=cbt.[Order]), cbt.Title, cbt.BlockType, cbt.Deleted, cbt.CreateUserId, cbt.CreateDate, cbt.AmendUserId, cbt.AmendDate)
                        OUTPUT cbt.OriginalBlockId, inserted.ID
                            INTO @BlockCrossRefIds (OldBlockId, NewBlockId) ;
    
                    -- Update the @CopiedBlocksTable with the newly generated NewBlockId value
                    UPDATE @CopiedBlocksTable
                    SET BlockId = NewBlockId
                        FROM @BlockCrossRefIds
                    WHERE OriginalBlockId = OldBlockId
                END

            -- Copy the Text Blocks // BlockType = 1
            EXECUTE resources.TextBlockCreateDuplicate @CurrentBlockIds, @CopiedBlocksTable, @UserId, @UserTimezoneOffset
        
            -- Copy the Whole Slide Image Blocks // BlockType = 2
            EXECUTE resources.WholeSlideImageCreateDuplicate @CurrentBlockIds, @CopiedBlocksTable, @UserId, @UserTimezoneOffset
        
            -- Copy the Media Blocks // BlockType = 3
            EXECUTE resources.MediaBlockCreateDuplicate @CurrentBlockIds, @CopiedBlocksTable, @UserId, @UserTimezoneOffset
        
            -- Copy the Question Blocks // BlockType = 4
            EXECUTE resources.QuestionBlockCreateDuplicate @CurrentBlockIds, @CopiedBlocksTable, @UserId, @UserTimezoneOffset

            -- Copy the Image Carousel Blocks // BlockType = 6
            EXECUTE resources.ImageCarouselBlockCreateDuplicate @CurrentBlockIds, @CopiedBlocksTable, @UserId, @UserTimezoneOffset
    END
    IF @DestinationBlockCollectionId IS NULL
        BEGIN
            RETURN @NewBlockCollectionId
        END
    RETURN @DestinationBlockCollectionId
END
