-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      12-08-2021
-- Purpose      Duplicates a Question Block
--
-- Modification History
--
-- 16-08-2021  Julian Ng	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[QuestionBlockCreateDuplicate]
    (
        @CurrentBlockIds CurrentBlockIdsType READONLY,
        @CopiedBlocksTable CopiedBlocksType READONLY,
        @UserId INT,
	    @UserTimezoneOffset int = NULL
    )
AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
    DECLARE @CurrentQuestionEntityIds TABLE (BlockId INT, QuestionBlockId INT, QuestionBlockCollectionId INT, FeedbackBlockCollectionId INT)
    DECLARE @CurrentBlockId INT
    DECLARE @CurrentQuestionBlockId INT
    DECLARE @NewQuestionBlockId INT
    DECLARE @CurrentQuestionBlockCollectionId INT
    DECLARE @CurrentFeedbackBlockCollectionId INT
    DECLARE BlockCursor CURSOR FOR
        SELECT BlockId, QuestionBlockId, QuestionBlockCollectionId, FeedbackBlockCollectionId FROM @CurrentQuestionEntityIds

    DECLARE @CurrentQuestionAnswerBlockCollectionIds TABLE (BlockCollectionId INT)
    DECLARE @CurrentQuestionAnswerBlockCollectionId INT
    DECLARE @CopiedQuestionAnswerTable CopiedBlocksType
    DECLARE @QuestionAnswerCount INT
    DECLARE @CurrentQuestionAnswerTextBlockIds CurrentBlockIdsType
    DECLARE @TemporaryQuestionAnswerTable CopiedBlocksType
    
    DECLARE @CopiedQuestionBlocksTable CopiedBlocksType
    DECLARE @CurrentQuestionContentBlockIds CurrentBlockIdsType

    DECLARE @CopiedFeedbackBlocksTable CopiedBlocksType
    DECLARE @CurrentFeedbackContentBlockIds CurrentBlockIdsType

    -- Get all Question entity IDs from the current set of Blocks
    INSERT INTO @CurrentQuestionEntityIds (QuestionBlockId, BlockId, QuestionBlockCollectionId, FeedbackBlockCollectionId)
    SELECT qb.Id, qb.BlockId, qb.QuestionBlockCollectionId, qb.FeedbackBlockCollectionId
    FROM resources.QuestionBlock AS qb
        JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = qb.BlockId
    WHERE qb.BlockId IN (SELECT BlockId FROM @CurrentBlockIds)
        AND cb.BlockType = 4
        AND qb.Deleted = 0
    
    -- PART 1
    -- This cursor iterates through each of the existing questions and duplicates each of the BlockCollections, Blocks and QuestionBlocks
    OPEN BlockCursor
    FETCH NEXT FROM BlockCursor INTO @CurrentBlockId, @CurrentQuestionBlockId, @CurrentQuestionBlockCollectionId, @CurrentFeedbackBlockCollectionId

    WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Duplicate Question and Feedback blocks
            INSERT @CopiedQuestionBlocksTable
                EXECUTE resources.BlockCollectionCreateDuplicate @CurrentQuestionBlockCollectionId, @UserId, @UserTimezoneOffset, NULL

            INSERT INTO @CopiedFeedbackBlocksTable
                EXECUTE resources.BlockCollectionCreateDuplicate @CurrentFeedbackBlockCollectionId, @UserId, @UserTimezoneOffset, NULL

            INSERT INTO resources.QuestionBlock (BlockId, QuestionBlockCollectionId, FeedbackBlockCollectionId, QuestionType, AllowReveal, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT cbt.BlockId, cq.BlockCollectionId, cf.BlockCollectionId, qb.QuestionType, qb.AllowReveal, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.QuestionBlock AS qb
                 LEFT JOIN @CopiedBlocksTable AS cbt ON cbt.OriginalBlockId = @CurrentBlockId,
                 resources.Block as bq
                 RIGHT JOIN @CopiedQuestionBlocksTable AS cq ON cq.OriginalBlockCollectionId = @CurrentQuestionBlockCollectionId,
                 resources.Block as bf
                 RIGHT JOIN @CopiedFeedbackBlocksTable AS cf ON cf.OriginalBlockCollectionId = @CurrentFeedbackBlockCollectionId
            WHERE qb.Id = @CurrentQuestionBlockId
              AND bq.BlockCollectionId = @CurrentQuestionBlockCollectionId
              AND bf.BlockCollectionId = @CurrentFeedbackBlockCollectionId
            GROUP BY cbt.BlockId, cq.BlockCollectionId, cf.BlockCollectionId, qb.QuestionType, qb.AllowReveal
            SELECT @NewQuestionBlockId = SCOPE_IDENTITY()

            -- This must be set inside of the cursor loop so that questions on multiple pages have their answers updated properly
            SET @QuestionAnswerCount = 1

            -- Duplicate QuestionAnswer blocks
            INSERT INTO @CurrentQuestionAnswerBlockCollectionIds
            SELECT BlockCollectionId from resources.QuestionAnswer
            WHERE QuestionBlockId = @CurrentQuestionBlockId
            
            WHILE @QuestionAnswerCount <= (SELECT COUNT(BlockCollectionId) FROM @CurrentQuestionAnswerBlockCollectionIds)
                BEGIN
                    WITH QuestionAnswers AS (
                        SELECT ROW_NUMBER() OVER (ORDER BY BlockCollectionId) AS RowNum, BlockCollectionId
                        FROM @CurrentQuestionAnswerBlockCollectionIds)
                    SELECT @CurrentQuestionAnswerBlockCollectionId = BlockCollectionId FROM QuestionAnswers
                    WHERE RowNum = @QuestionAnswerCount

                    -- Needed to ensure there is no PK error occuring when duplicating a resource with multiple pages
                    INSERT INTO @TemporaryQuestionAnswerTable
                    EXECUTE resources.BlockCollectionCreateDuplicate @CurrentQuestionAnswerBlockCollectionId, @UserId, @UserTimezoneOffset, NULL

                    INSERT INTO @CopiedQuestionAnswerTable
                        SELECT * FROM @TemporaryQuestionAnswerTable as tqat
                        WHERE NOT EXISTS (
                            SELECT 1
                             FROM @CopiedQuestionAnswerTable
                             WHERE OriginalBlockId = tqat.OriginalBlockId
                        );
                        
                    DELETE FROM @TemporaryQuestionAnswerTable
                    
                    SET @QuestionAnswerCount = @QuestionAnswerCount + 1
            END

            INSERT INTO resources.QuestionAnswer (QuestionBlockId, [Order], Status, BlockCollectionId, ImageAnnotationOrder, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT @NewQuestionBlockId, qa.[Order], qa.Status, cqabci.BlockCollectionId, qa.ImageAnnotationOrder, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.QuestionAnswer AS qa
                 LEFT JOIN (SELECT DISTINCT OriginalBlockCollectionId, BlockCollectionId FROM @CopiedQuestionAnswerTable) as cqabci ON cqabci.OriginalBlockCollectionId = qa.BlockCollectionId
            WHERE qa.QuestionBlockId = @CurrentQuestionBlockId
                OR qa.BlockCollectionId IS NULL
                AND cqabci.OriginalBlockCollectionId = qa.BlockCollectionId

            FETCH NEXT FROM BlockCursor INTO @CurrentBlockId, @CurrentQuestionBlockId, @CurrentQuestionBlockCollectionId, @CurrentFeedbackBlockCollectionId
        END

    CLOSE BlockCursor
    DEALLOCATE BlockCursor

    -- PART 2        
    -- Duplicate the Text and Media Blocks (and Whole Slide Image block for Image Zone questions) in the QuestionBlockCollections
    INSERT INTO @CurrentQuestionContentBlockIds (BlockId, BlockCollectionId)
    SELECT b.Id, b.BlockCollectionId
    FROM resources.Block AS b
    WHERE b.BlockCollectionId IN (SELECT QuestionBlockCollectionId FROM @CurrentQuestionEntityIds)
    AND b.Deleted = 0
    
    EXECUTE resources.TextBlockCreateDuplicate @CurrentQuestionContentBlockIds, @CopiedQuestionBlocksTable, @UserId, @UserTimezoneOffset

    EXECUTE resources.WholeSlideImageCreateDuplicate @CurrentQuestionContentBlockIds, @CopiedQuestionBlocksTable, @UserId, @UserTimezoneOffset

    EXECUTE resources.MediaBlockCreateDuplicate @CurrentQuestionContentBlockIds, @CopiedQuestionBlocksTable, @UserId, @UserTimezoneOffset
    
    

    -- PART 3
    -- Duplicate the Text and Media Blocks in the FeedbackBlockCollections
    INSERT INTO @CurrentFeedbackContentBlockIds (BlockId, BlockCollectionId)
    SELECT b.Id, b.BlockCollectionId
    FROM resources.Block AS b
    WHERE b.BlockCollectionId IN (SELECT FeedbackBlockCollectionId FROM @CurrentQuestionEntityIds)
    AND b.Deleted = 0

    EXECUTE resources.TextBlockCreateDuplicate @CurrentFeedbackContentBlockIds, @CopiedFeedbackBlocksTable, @UserId, @UserTimezoneOffset

    EXECUTE resources.MediaBlockCreateDuplicate @CurrentFeedbackContentBlockIds, @CopiedFeedbackBlocksTable, @UserId, @UserTimezoneOffset
    
    -- PART 4
    -- Duplicate the Question Answer TextBlocks
    INSERT INTO @CurrentQuestionAnswerTextBlockIds (BlockId, BlockCollectionId)
    SELECT b.Id, b.BlockCollectionId
    FROM resources.Block AS b
    WHERE b.BlockCollectionId IN (SELECT BlockCollectionId FROM @CurrentQuestionAnswerBlockCollectionIds)
    AND b.Deleted = 0
    
    EXECUTE resources.TextBlockCreateDuplicate @CurrentQuestionAnswerTextBlockIds, @CopiedQuestionAnswerTable, @UserId, @UserTimezoneOffset
    EXECUTE resources.MediaBlockCreateDuplicate @CurrentQuestionAnswerTextBlockIds, @CopiedQuestionAnswerTable, @UserId, @UserTimezoneOffset

END