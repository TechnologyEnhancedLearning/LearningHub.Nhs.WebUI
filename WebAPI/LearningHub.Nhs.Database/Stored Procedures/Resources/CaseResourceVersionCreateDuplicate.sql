-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      12-08-2021
-- Purpose      Duplicates a case resource version by copying all of its
--              BlockCollections (and sub entities)
--
-- Modification History
--
-- 13-08-2021  Julian Ng	    Initial Revision
-- 17-08-2021  Julian Ng	    Markups
-- 21-09-2021  Malina Slevoaca  Use procedure for duplicating block collection
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[CaseResourceVersionCreateDuplicate]
    (
        @ResourceVersionId INT,
        @CurrentResourceVersionId INT,
        @UserId INT,
	    @UserTimezoneOffset int = NULL
    )
AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
    DECLARE @CurrentBlockCollectionId INT
    DECLARE @NewBlockCollectionId INT
    DECLARE @CurrentBlockIds CurrentBlockIdsType
    DECLARE @CopiedBlocksTable CopiedBlocksType 
    DECLARE @Blocks IDList

    -- Copy original Case Resource Version Block Collection (text, videos, etc) if exists
    SELECT @CurrentBlockCollectionId = BlockCollectionId
    FROM resources.CaseResourceVersion
    WHERE ResourceVersionId = @CurrentResourceVersionId
    AND Deleted = 0

    EXECUTE resources.BlockCollectionWithBlocksCreateDuplicate @CurrentBlockCollectionId, @UserId, @Blocks, null, @UserTimezoneOffset, @NewBlockCollectionId OUTPUT

    -- Create new Case Resource Version
    INSERT INTO resources.CaseResourceVersion (ResourceVersionId, BlockCollectionId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT @ResourceVersionId, @NewBlockCollectionId, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET()
    FROM resources.CaseResourceVersion
    WHERE ResourceVersionId = @CurrentResourceVersionId
END
