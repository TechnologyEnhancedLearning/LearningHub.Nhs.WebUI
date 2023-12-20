-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      12-08-2021
-- Purpose      Duplicates a Text Block
--
-- Modification History
--
-- 25-08-2021  Julian Ng	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[TextBlockCreateDuplicate]
    (
        @CurrentBlockIds CurrentBlockIdsType READONLY,
        @CopiedBlocksTable CopiedBlocksType READONLY,
        @UserId INT,
	    @UserTimezoneOffset int = NULL
    )
AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
    INSERT INTO resources.TextBlock (BlockId, Content, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT cb.BlockId, Content, 0, @UserId, @AmendDate, @UserId, @AmendDate
    FROM resources.TextBlock AS tb
        JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = tb.BlockId
    WHERE tb.BlockId IN (SELECT BlockId FROM @CurrentBlockIds)
        AND cb.BlockType = 1
        AND tb.Deleted = 0
END