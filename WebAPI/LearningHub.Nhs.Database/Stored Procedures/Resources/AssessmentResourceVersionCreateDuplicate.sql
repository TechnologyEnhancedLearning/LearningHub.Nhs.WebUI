-------------------------------------------------------------------------------
-- Author       Malina Slevoaca (Softwire)
-- Created      20-09-2021
-- Purpose      Duplicates an assessment resource version by copying all of its
--              BlockCollections (and sub entities)
--
-- Modification History
--
-- 20-09-2021  Malina Slevoaca	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[AssessmentResourceVersionCreateDuplicate]
    (
        @ResourceVersionId INT,
        @CurrentResourceVersionId INT,
        @UserId INT,
        @UserTimezoneOffset INT = NULL
    )
AS

BEGIN
    DECLARE @CurrentAssessmentContentId INT
    DECLARE @NewAssessmentContentId INT
    DECLARE @CurrentEndGuidanceId INT
	DECLARE @NewEndGuidanceId INT
    DECLARE @Blocks IDList

    SELECT @CurrentAssessmentContentId = AssessmentContentId, @CurrentEndGuidanceId = EndGuidanceId
    FROM resources.AssessmentResourceVersion
    WHERE ResourceVersionId = @CurrentResourceVersionId
    AND Deleted = 0

    EXECUTE resources.BlockCollectionWithBlocksCreateDuplicate @CurrentAssessmentContentId, @UserId, @Blocks, null, @UserTimezoneOffset, @NewAssessmentContentId OUTPUT
    EXECUTE resources.BlockCollectionWithBlocksCreateDuplicate @CurrentEndGuidanceId, @UserId, @Blocks, null, @UserTimezoneOffset, @NewEndGuidanceId OUTPUT

    -- Create new Assessment Resource Version
    INSERT INTO resources.AssessmentResourceVersion (ResourceVersionId, AssessmentType, AssessmentContentId, EndGuidanceId, MaximumAttempts, PassMark, AnswerInOrder, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT @ResourceVersionId, AssessmentType, @NewAssessmentContentId, @NewEndGuidanceId, MaximumAttempts, PassMark, AnswerInOrder, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET()
    FROM resources.AssessmentResourceVersion
    WHERE ResourceVersionId = @CurrentResourceVersionId
END
