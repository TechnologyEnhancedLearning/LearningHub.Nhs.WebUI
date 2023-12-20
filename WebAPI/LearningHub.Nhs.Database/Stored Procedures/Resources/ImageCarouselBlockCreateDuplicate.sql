-------------------------------------------------------------------------------
-- Author       Jonathan Dixon (Softwire)
-- Created      01-12-2021
-- Purpose      Duplicates an Image Carousel Block
--
-- Modification History
--
-- 02-12-2021   Jonathan Dixon	Initial Revision
-- 06-12-2021   Jonathan Dixon  Description duplication refactor
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[ImageCarouselBlockCreateDuplicate]
    (
        @CurrentBlockIds CurrentBlockIdsType READONLY,
        @CopiedBlocksTable CopiedBlocksType READONLY,
        @UserId INT,
	    @UserTimezoneOffset int = NULL
    )
AS

BEGIN
    DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
    DECLARE @CurrentImageCarouselEntityIds TABLE (BlockId INT, ImageCarouselBlockId INT, ImageBlockCollectionId INT)
    DECLARE @CurrentBlockId INT
    DECLARE @CurrentImageCarouselBlockId INT
    DECLARE @CurrentImageBlockCollectionId INT

    DECLARE @CopiedImageBlocksTable CopiedBlocksType
    DECLARE @CurrentImageContentBlockIds CurrentBlockIdsType
    DECLARE @TemporaryImageBlocksTable CopiedBlocksType

    DECLARE ImageCarouselBlockCursor CURSOR FOR
    SELECT BlockId, ImageCarouselBlockId, ImageBlockCollectionId FROM @CurrentImageCarouselEntityIds

    -- Get entities from ImageCarouselBlock (Block collections)
    INSERT INTO @CurrentImageCarouselEntityIds (ImageCarouselBlockId, BlockId, ImageBlockCollectionId)
    SELECT icb.Id, icb.BlockId, icb.ImageBlockCollectionId
    FROM resources.ImageCarouselBlock AS icb
         JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = icb.BlockId
    WHERE icb.BlockId IN (SELECT BlockId FROM @CurrentBlockIds)
        AND cb.BlockType = 6
        AND icb.Deleted = 0

    -- PART 1
    -- For each ICB: duplicate the image block collection
    OPEN ImageCarouselBlockCursor
    FETCH NEXT FROM ImageCarouselBlockCursor INTO @CurrentBlockId, @CurrentImageCarouselBlockId, @CurrentImageBlockCollectionId

    WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Needed to ensure there is no PK error occuring when duplicating a resource with multiple pages   
            INSERT @CopiedImageBlocksTable
                EXECUTE resources.BlockCollectionCreateDuplicate @CurrentImageBlockCollectionId, @UserId, @UserTimezoneOffset, NULL
        
            INSERT INTO resources.ImageCarouselBlock (BlockId, Description, ImageBlockCollectionId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT cbt.BlockId, icb.Description, ci.BlockCollectionId, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.ImageCarouselBlock AS icb
                     LEFT JOIN @CopiedBlocksTable AS cbt ON cbt.OriginalBlockId = @CurrentBlockId,
                 resources.Block as bi
                     RIGHT JOIN @CopiedImageBlocksTable AS ci ON ci.OriginalBlockCollectionId = @CurrentImageBlockCollectionId
            WHERE icb.Id = @CurrentImageCarouselBlockId
              AND bi.BlockCollectionId = @CurrentImageBlockCollectionId
            GROUP BY cbt.BlockId, icb.Description, ci.BlockCollectionId
    
            FETCH NEXT FROM ImageCarouselBlockCursor INTO  @CurrentBlockId, @CurrentImageCarouselBlockId, @CurrentImageBlockCollectionId
        END

    CLOSE ImageCarouselBlockCursor
    DEALLOCATE ImageCarouselBlockCursor
        
    -- PART 2
    -- Duplicate the Media Blocks in the ImageBlockCollections
    INSERT INTO @CurrentImageContentBlockIds (BlockId, BlockCollectionId)
    SELECT b.Id, b.BlockCollectionId
    FROM resources.Block AS b
    WHERE b.BlockCollectionId IN (SELECT ImageBlockCollectionId FROM @CurrentImageCarouselEntityIds)
    AND b.Deleted = 0

    EXECUTE resources.MediaBlockCreateDuplicate @CurrentImageContentBlockIds, @CopiedImageBlocksTable, @UserId, @UserTimezoneOffset

END