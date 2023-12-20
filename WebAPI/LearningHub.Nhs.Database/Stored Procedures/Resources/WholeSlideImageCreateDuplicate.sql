-------------------------------------------------------------------------------
-- Author       Julian Ng (Softwire)
-- Created      12-08-2021
-- Purpose      Duplicates a whole slide image block (and all sub-entities)
--
-- Modification History
--
-- 15-08-2021  Julian Ng	Initial Revision
-- 03-03-2022  Nat Dean-Lewis   renaming WholeSlideImageAnnotation -> ImageAnnotation
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[WholeSlideImageCreateDuplicate]
    (
        @CurrentBlockIds CurrentBlockIdsType READONLY,
        @CopiedBlocksTable CopiedBlocksType READONLY,
        @UserId INT,
        @UserTimezoneOffset int = NULL
    )
AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

    DECLARE @CurrentWSIEntityIds TABLE (BlockId INT, WSIBlockId INT, WSIBlockItemId INT, WSIId INT, WSIFileId INT, FileId INT)
    DECLARE WSICursor CURSOR FOR
        SELECT BlockId, WSIBlockId, WSIBlockItemId, WSIId, WSIFileId, FileId FROM @CurrentWSIEntityIds
    DECLARE @CurrentBlockId INT
    DECLARE @CurrentWSIBlockItemId INT
    DECLARE @CurrentWSIBlockId INT
    DECLARE @CurrentWSIId INT
    DECLARE @CurrentWSIFileId INT
    DECLARE @CurrentFileId INT
    DECLARE @WSIBlockIdCount INT
    DECLARE @NewFileId INT
    DECLARE @NewWholeSlideImageBlockId INT
    DECLARE @NewWholeSlideImageId INT
    DECLARE @WholeSlideImageMap TABLE (CurrentWSIId INT, NewWSIId INT)
    DECLARE @ImageAnnotationMap TABLE (CurrentWSIId INT, CurrentImageAnnotationId INT, NewWSIId INT, NewImageAnnotationId INT)
	DECLARE @FileIdStore TABLE(ID INT);

-- Get all WSI entity IDs from the current set of Blocks
    INSERT INTO @CurrentWSIEntityIds (BlockId, WSIBlockId, WSIBlockItemId, WSIId, WSIFileId, FileId)
    SELECT wsib.BlockId, wsib.Id, wsibi.Id, wsi.Id, wsif.Id, f.Id
    FROM resources.WholeSlideImageBlock AS wsib
        LEFT JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = wsib.BlockId
        LEFT JOIN resources.WholeSlideImageBlockItem AS wsibi ON  wsib.Id = wsibi.WholeSlideImageBlockId
        LEFT JOIN resources.WholeSlideImage AS wsi ON  wsi.Id = wsibi.WholeSlideImageId
        LEFT JOIN resources.WholeSlideImageFile AS wsif ON wsif.FileId = wsi.FileId
        LEFT JOIN resources.[File] AS f ON f.Id = wsi.FileId
    WHERE wsib.BlockId IN (SELECT BlockId FROM @CurrentBlockIds)
    AND cb.BlockType = 2
    AND wsib.Deleted = 0

    OPEN WSICursor
    FETCH NEXT FROM WSICursor INTO @CurrentBlockId, @CurrentWSIBlockId, @CurrentWSIBlockItemId, @CurrentWSIId, @CurrentWSIFileId, @CurrentFileId

    WHILE @@FETCH_STATUS = 0
        BEGIN
			-- reset the fileId to null to enable placeholder slides to be duplicated
            SELECT @NewFileId = NULL
            
            -- Copy File
            INSERT INTO resources.[File] (FileTypeId, FileChunkDetailId, FileName, FilePath, FileSizeKb, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            OUTPUT inserted.Id into @FileIdStore(ID)
            SELECT FileTypeId, FileChunkDetailId, FileName, FilePath, FileSizeKb, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.[File] AS f
            WHERE f.Id = @CurrentFileId
            
            -- Set Id value as the FileIdStore's value, and then delete that row
            SELECT @NewFileId = Id from @FileIdStore
            DELETE FROM @FileIdStore

            -- Copy Whole Slide Image File
            INSERT INTO resources.WholeSlideImageFile (FileId, Status, ProcessingErrorMessage, Width, Height, DeepZoomTileSize, DeepZoomOverlap, Layers, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT @NewFileId, Status, ProcessingErrorMessage, Width, Height, DeepZoomTileSize, DeepZoomOverlap, Layers, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.[WholeSlideImageFile] AS wsif
            WHERE wsif.Id = @CurrentWSIFileId

            -- Copy Whole Slide Image
            INSERT INTO resources.WholeSlideImage (Title, FileId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT Title, @NewFileId, 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.[WholeSlideImage] AS wsi
            WHERE wsi.Id = @CurrentWSIId
            SELECT @NewWholeSlideImageId = SCOPE_IDENTITY()

            -- This table is used in the annotations duplication below, outside of the CURSOR
            INSERT INTO @WholeSlideImageMap (CurrentWSIId, NewWSIId)
            VALUES (@CurrentWSIId, @NewWholeSlideImageId)

            -- Copy Whole Slide Image Block
            SELECT @WSIBlockIdCount = COUNT(BlockId)
            FROM resources.WholeSlideImageBlock
            WHERE BlockId IN (SELECT BlockId FROM @CopiedBlocksTable WHERE OriginalBlockId = @CurrentBlockId)

            -- For a given WSIBlock...
            IF (@WSIBlockIdCount = 0)
                BEGIN
                    --...copy that block (if it doesn't exist)...
                    INSERT INTO resources.WholeSlideImageBlock (BlockId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                    SELECT cb.BlockId, 0, @UserId, @AmendDate, @UserId, @AmendDate
                    FROM resources.WholeSlideImageBlock AS wsib
                        JOIN @CopiedBlocksTable AS cb ON cb.OriginalBlockId = wsib.BlockId
                    WHERE wsib.Id = @CurrentWSIBlockId
                    SELECT @NewWholeSlideImageBlockId = SCOPE_IDENTITY()
                END
            ELSE
                BEGIN
                    --...get the duplicated WSIBlock Id...
                    SELECT @NewWholeSlideImageBlockId = Id
                    FROM resources.WholeSlideImageBlock
                    WHERE BlockId IN (SELECT BlockId FROM @CopiedBlocksTable WHERE OriginalBlockId = @CurrentBlockId)
                END

            --...and then duplicate all the WSIBlockItems within the WSIBlock.
            INSERT INTO resources.WholeSlideImageBlockItem (WholeSlideImageBlockId, WholeSlideImageId, PlaceholderText, [Order], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
            SELECT @NewWholeSlideImageBlockId, @NewWholeSlideImageId, PlaceholderText, [Order], 0, @UserId, @AmendDate, @UserId, @AmendDate
            FROM resources.WholeSlideImageBlockItem AS wsibi
            WHERE wsibi.Id = @CurrentWSIBlockItemId

            FETCH NEXT FROM WSICursor INTO @CurrentBlockId, @CurrentWSIBlockId, @CurrentWSIBlockItemId, @CurrentWSIId, @CurrentWSIFileId, @CurrentFileId
        END

    CLOSE WSICursor
    DEALLOCATE WSICursor

    -- Annotations and AnnotationMarks are handled outside of the CURSOR loop above to de-complicate the issue with the
    -- one-to-many relationship between WSI and IAnnotations.

    -- Copy Whole Slide Image Annotation
    INSERT INTO resources.ImageAnnotation (WholeSlideImageId, [Order], Label, Description, PinXCoordinate, PinYCoordinate, Colour, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT wsim.NewWSIId, [Order], Label, Description, PinXCoordinate, PinYCoordinate, Colour, 0, @UserId, @AmendDate, @UserId, @AmendDate
    FROM resources.ImageAnnotation AS ia
        JOIN @WholeSlideImageMap AS wsim ON wsim.CurrentWSIId = ia.WholeSlideImageId
        JOIN @CurrentWSIEntityIds AS cwei ON cwei.WSIId = wsim.CurrentWSIId
    WHERE ia.WholeSlideImageId = cwei.WSIId

-- Update the map table so that we know what each duplicated WSI's annotation IDs are
    INSERT INTO @ImageAnnotationMap
    SELECT wsim.CurrentWSIId, currentIa.Id, wsim.NewWSIId, newIa.Id
    FROM resources.ImageAnnotation AS newIa
        JOIN @WholeSlideImageMap AS wsim ON wsim.NewWSIId = newIa.WholeSlideImageId
        JOIN resources.ImageAnnotation AS currentIa ON currentIa.WholeSlideImageId = wsim.CurrentWSIId
    WHERE wsim.CurrentWSIId = currentIa.WholeSlideImageId
        AND newIa.[Order] = currentIa.[Order]

-- Copy Whole Slide Image Annotation Marks
    INSERT INTO resources.ImageAnnotationMark (ImageAnnotationId, [Type], MarkShapeData)
    SELECT wsim.NewImageAnnotationId, [Type], MarkShapeData
    FROM resources.ImageAnnotationMark AS iam
        JOIN resources.ImageAnnotation AS ia ON ia.Id = iam.ImageAnnotationId
        JOIN @ImageAnnotationMap AS wsim ON wsim.CurrentWSIId = ia.WholeSlideImageId
    WHERE wsim.CurrentImageAnnotationId = iam.ImageAnnotationId

END