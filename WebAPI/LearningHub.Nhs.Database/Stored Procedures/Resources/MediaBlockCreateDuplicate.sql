-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      12-08-2021
-- Purpose      Duplicates a media block (and all sub-entities)
--
-- Modification History
--
-- 15-08-2021  Julian Ng	Initial Revision
-- 27-08-2021  Julian Ng	Replace WHILE loop with CURSOR iterator
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[MediaBlockCreateDuplicate]
    (
        @CurrentBlockIds CurrentBlockIdsType READONLY,
        @CopiedBlocksTable CopiedBlocksType READONLY,
        @UserId INT,
	    @UserTimezoneOffset int = NULL
    )
AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
    DECLARE @CurrentMediaEntityIds TABLE (BlockId INT, MediaBlockId INT, MediaType INT, AttachmentId INT, ImageId INT, VideoId INT)
    DECLARE @CurrentBlockId INT
    DECLARE @CurrentMediaBlockId INT
    DECLARE @CurrentMediaType INT
    DECLARE @CurrentAttachmentId INT
    DECLARE @CurrentImageId INT
    DECLARE @CurrentVideoId INT
    DECLARE MediaCursor CURSOR FOR
        SELECT BlockId, MediaBlockId, MediaType, AttachmentId, ImageId, VideoId FROM @CurrentMediaEntityIds

    DECLARE @CurrentFileId INT
    DECLARE @NewFileId INT
    DECLARE @NewAttachmentId INT
    DECLARE @NewImageId INT
    DECLARE @NewVideoId INT

    -- Get all Media Block entity IDs from the current set of Blocks
    INSERT INTO @CurrentMediaEntityIds (BlockId, MediaBlockId, MediaType, AttachmentId, ImageId, VideoId)
    SELECT mb.BlockId, mb.Id, mb.MediaType, a.Id, i.Id, v.Id 
    FROM resources.MediaBlock AS mb
        LEFT JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = mb.BlockId
        LEFT JOIN resources.Attachment AS a ON  a.Id = mb.AttachmentId
        LEFT JOIN resources.Image AS i ON i.Id = mb.ImageId
        LEFT JOIN resources.Video AS v ON v.Id = mb.VideoId
    WHERE mb.BlockId IN (SELECT BlockId FROM @CurrentBlockIds)
    AND cb.BlockType = 3
    AND cb.Deleted = 0

    OPEN MediaCursor
    FETCH NEXT FROM MediaCursor INTO @CurrentBlockId, @CurrentMediaBlockId, @CurrentMediaType, @CurrentAttachmentId, @CurrentImageId, @CurrentVideoId

    WHILE @@FETCH_STATUS = 0
        BEGIN
            -- We want to reset these at the beginning of each loop so that we only insert one ID in the MediaBlock
            -- row at the end.
            SET @NewAttachmentId = NULL
            SET @NewImageId = NULL
            SET @NewVideoId = NULL

            IF (@CurrentMediaType = 0)
                BEGIN
                    SELECT @CurrentFileId = FileId
                    FROM resources.Attachment AS a
                    WHERE a.Id = @CurrentAttachmentId
                END
            IF (@CurrentMediaType = 1)
                BEGIN
                    SELECT @CurrentFileId = FileId
                    FROM resources.Image AS i
                    WHERE i.Id = @CurrentImageId
                END
            IF (@CurrentMediaType = 2)
                BEGIN
                    SELECT @CurrentFileId = FileId
                    FROM resources.Video AS v
                    WHERE v.Id = @CurrentVideoId
                END

            -- Copy File
            INSERT INTO resources.[File] (FileTypeId, FileChunkDetailId, FileName, FilePath, FileSizeKb, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT FileTypeId, FileChunkDetailId, FileName, FilePath, FileSizeKb, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.[File] AS f
            WHERE f.Id = @CurrentFileId
            SELECT @NewFileId = SCOPE_IDENTITY()

           -- Copy appropriate Media entity
            IF (@CurrentMediaType = 0)
                BEGIN
                    INSERT INTO resources.Attachment (FileId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                    SELECT @NewFileId, 0, @UserId, @AmendDate, @UserId, @AmendDate
                    FROM resources.[Attachment] AS a
                    WHERE a.Id = @CurrentAttachmentId
                    SELECT @NewAttachmentId = SCOPE_IDENTITY()
                END
            IF (@CurrentMediaType = 1)
                BEGIN
                    INSERT INTO resources.Image (FileId, AltText, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                    SELECT @NewFileId, AltText, Description, 0, @UserId, @AmendDate, @UserId, @AmendDate
                    FROM resources.[Image] AS i
                    WHERE i.Id = @CurrentImageId
                    SELECT @NewImageId = SCOPE_IDENTITY()
                END
            IF (@CurrentMediaType = 2)
                BEGIN
                    INSERT INTO resources.Video (FileId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                    SELECT @NewFileId, 0, @UserId, @AmendDate, @UserId, @AmendDate
                    FROM resources.[Video] AS v
                    WHERE v.Id = @CurrentVideoId
                    SELECT @NewVideoId = SCOPE_IDENTITY()

                    INSERT INTO resources.VideoFile (FileId, Status, ProcessingErrorMessage, CaptionsFileId, TranscriptFileId, AzureAssetOutputFilePath, LocatorUri, EncodeJobName, DurationInMilliseconds, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                    SELECT @NewFileId, Status, ProcessingErrorMessage, CaptionsFileId, TranscriptFileId, AzureAssetOutputFilePath, LocatorUri, EncodeJobName, DurationInMilliseconds, 0, @UserId, @AmendDate, @UserId, @AmendDate
                    FROM resources.VideoFile AS vf
                        LEFT JOIN resources.[Video] AS v ON v.FileId = vf.FileId
                    WHERE v.Id = @CurrentVideoId
                END
            
            -- Copy Media Block
            INSERT INTO resources.MediaBlock (BlockId, MediaType, AttachmentId, ImageId, VideoId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT cb.BlockId, @CurrentMediaType, @NewAttachmentId, @NewImageId, @NewVideoId, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.MediaBlock AS mb
                 LEFT JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = @CurrentBlockId
            WHERE mb.Id = @CurrentMediaBlockId

            FETCH NEXT FROM MediaCursor INTO @CurrentBlockId, @CurrentMediaBlockId, @CurrentMediaType, @CurrentAttachmentId, @CurrentImageId, @CurrentVideoId
        END

    CLOSE MediaCursor
    DEALLOCATE MediaCursor
        
END