-------------------------------------------------------------------------------
-- Author       Tobi Awe
-- Created      30-05-2024
-- Purpose      Get all assessment content block collections of all undeleted resource version including published(current version only) versions while excluding the version being edited. 
--
-- Modification History
--
-- 30-05-2024  TD-3023	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[AssessmentContentBlockCollectionGetAll] (
	@excludeResourceVersionId INT
)
AS
BEGIN
    -- Table to hold BlockCollectionIds
    DECLARE @BlockCollectionId TABLE (Id INT);

    -- Insert BlockCollectionIds based on the provided query
    INSERT INTO @BlockCollectionId (Id)
    SELECT DISTINCT [c].AssessmentContentId as [BlockCollectionId]
    FROM [resources].AssessmentResourceVersion AS [c]
    INNER JOIN (
        SELECT [r].[Id], [r].[ResourceId], [r].[VersionStatusId]
        FROM [resources].[ResourceVersion] AS [r]
        WHERE [r].[Deleted] = CAST(0 AS bit)
    ) AS [t] ON [c].[ResourceVersionId] = [t].[Id]
    INNER JOIN (
        SELECT [r0].[Id], [r0].[CurrentResourceVersionId]
        FROM [resources].[Resource] AS [r0]
        WHERE [r0].[Deleted] = CAST(0 AS bit)
    ) AS [t0] ON [t].[ResourceId] = [t0].[Id]
    WHERE ([c].[Deleted] = CAST(0 AS bit)) 
      AND ((([c].[ResourceVersionId] <> @excludeResourceVersionId) 
            AND ([c].[Deleted] = CAST(0 AS bit))) 
           AND (([t].[VersionStatusId] <> 2) 
                OR (([t].[VersionStatusId] = 2) 
                    AND ([t0].[CurrentResourceVersionId] = [c].[ResourceVersionId])))) 
      AND [c].AssessmentContentId IS NOT NULL;

    -- Table to hold results for blocks
    DECLARE @BlockResult TABLE (
        Id INT,
        [Order] INT,
        Title NVARCHAR(200),
        BlockType INT,
        BlockCollectionId INT
    );

    -- Insert block data
    INSERT INTO @BlockResult
    SELECT Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.Block
    WHERE Deleted = 0 AND BlockCollectionId IN (SELECT Id FROM @BlockCollectionId);

    -- Select block data
    SELECT br.Id, [Order], Title, BlockType, BlockCollectionId
    FROM @BlockResult br
    JOIN @BlockCollectionId bc ON br.BlockCollectionId = bc.Id;

    -- Select text block data
    SELECT t.*
    FROM resources.TextBlock t
    INNER JOIN @BlockResult b ON b.Id = t.BlockId
    WHERE t.Deleted = 0;

    -- WholeSlide Image Block
    DECLARE @WsibResult TABLE (
        Id INT,
        BlockId INT
    );

    INSERT INTO @WsibResult
    SELECT w.Id, w.BlockId
    FROM resources.WholeSlideImageBlock w
    INNER JOIN @BlockResult b ON b.Id = w.BlockId
    WHERE w.Deleted = 0;

    SELECT * FROM @WsibResult;

    -- WholeSlide Image Block Items
    DECLARE @WsibiResult TABLE (
        Id INT,
        WholeSlideImageBlockId INT,
        WholeSlideImageId INT,
        PlaceholderText NVARCHAR(255),
        [Order] INT
    );

    INSERT INTO @WsibiResult
    SELECT wi.Id, wi.WholeSlideImageBlockId, wi.WholeSlideImageId, wi.PlaceholderText, wi.[Order]
    FROM resources.WholeSlideImageBlockItem wi
    INNER JOIN @WsibResult w ON wi.WholeSlideImageBlockId = w.Id
    WHERE wi.Deleted = 0;

    SELECT * FROM @WsibiResult;

    -- WholeSlide Image
    DECLARE @WsiResult TABLE (
        Id INT,
        Title NVARCHAR(1000),
        FileId INT
    );

    INSERT INTO @WsiResult
    SELECT wsi.Id, wsi.Title, wsi.FileId
    FROM resources.WholeSlideImage wsi
    INNER JOIN @WsibiResult wi ON wsi.Id = wi.WholeSlideImageId
    WHERE wsi.Deleted = 0;

    SELECT * FROM @WsiResult;

    -- WholeSlide Image File
    DECLARE @WsiFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @WsiFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @WsiResult wsi ON f.Id = wsi.FileId
    WHERE f.Deleted = 0;

    SELECT * FROM @WsiFileResult;

    -- WholeSlide Image File - Partial File
    SELECT pf.*
    FROM resources.PartialFile pf
    INNER JOIN @WsiFileResult wsi ON pf.FileId = wsi.Id
    WHERE pf.Deleted = 0;

    -- WholeSlide Image File - Image File
    SELECT wsif.*
    FROM resources.WholeSlideImageFile wsif
    INNER JOIN @WsiFileResult wsi ON wsif.FileId = wsi.Id
    WHERE wsif.Deleted = 0;

    -- WholeSlide Image Annotation
    DECLARE @WsiaResult TABLE (
        Id INT,
        WholeSlideImageId INT,
        ImageId INT,
        [Order] INT,
        Label NVARCHAR(255),
        Description NVARCHAR(1000),
        PinXCoordinate DECIMAL(22,19),
        PinYCoordinate DECIMAL(22,19),
        Colour INT
    );

    INSERT INTO @WsiaResult
    SELECT ia.Id, ia.WholeSlideImageId, ia.ImageId, ia.[Order], ia.Label, ia.Description, ia.PinXCoordinate, ia.PinYCoordinate, ia.Colour
    FROM resources.ImageAnnotation ia
    INNER JOIN @WsiResult wsi ON ia.WholeSlideImageId = wsi.Id
    WHERE ia.Deleted = 0;

    SELECT * FROM @WsiaResult;

    -- WholeSlide Image Annotation Mark
    SELECT iam.*
    FROM resources.ImageAnnotationMark iam
    INNER JOIN @WsiaResult wsia ON iam.ImageAnnotationId = wsia.Id
    WHERE iam.Deleted = 0;

    -- Media Block
    DECLARE @MediaBlockResult TABLE (
        Id INT,
        BlockId INT,
        MediaType INT,
        AttachmentId INT,
        ImageId INT,
        VideoId INT
    );

    INSERT INTO @MediaBlockResult
    SELECT mb.Id, mb.BlockId, mb.MediaType, mb.AttachmentId, mb.ImageId, mb.VideoId
    FROM resources.MediaBlock mb
    INNER JOIN @BlockResult b ON mb.BlockId = b.Id
    WHERE mb.Deleted = 0;

    SELECT * FROM @MediaBlockResult;

    -- Attachment
    DECLARE @AttachmentResult TABLE (
        Id INT,
        FileId INT
    );

    INSERT INTO @AttachmentResult
    SELECT a.Id, a.FileId
    FROM resources.Attachment a
    INNER JOIN @MediaBlockResult mb ON a.Id = mb.AttachmentId
    WHERE a.Deleted = 0;

    SELECT * FROM @AttachmentResult;

    -- Attachment File
    DECLARE @AttachmentFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @AttachmentFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @AttachmentResult ar ON f.Id = ar.FileId
    WHERE f.Deleted = 0;

    SELECT * FROM @AttachmentFileResult;

    -- Attachment File - Partial File
    SELECT pf.*
    FROM resources.PartialFile pf
    INNER JOIN @AttachmentFileResult af ON pf.FileId = af.Id
    WHERE pf.Deleted = 0;

    -- Image
    DECLARE @ImageResult TABLE (
        Id INT,
        FileId INT,
        AltText NVARCHAR(125),
        Description NVARCHAR(250)
    );

    INSERT INTO @ImageResult
    SELECT i.Id, i.FileId, i.AltText, i.Description
    FROM resources.Image i
    INNER JOIN @MediaBlockResult mb ON i.Id = mb.ImageId
    WHERE i.Deleted = 0;

    SELECT * FROM @ImageResult;

    -- Image File
    DECLARE @ImageFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @ImageFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @ImageResult ir ON f.Id = ir.FileId
    WHERE f.Deleted = 0;

    SELECT * FROM @ImageFileResult;

    -- Image File - Partial File
    SELECT pf.*
    FROM resources.PartialFile pf
    INNER JOIN @ImageFileResult ifr ON pf.FileId = ifr.Id
    WHERE pf.Deleted = 0;

    -- Video
    DECLARE @VideoResult TABLE (
        Id INT,
        FileId INT
    );

    INSERT INTO @VideoResult
    SELECT v.Id, v.FileId
    FROM resources.Video v
    INNER JOIN @MediaBlockResult mb ON v.Id = mb.VideoId
    WHERE v.Deleted = 0;

    SELECT * FROM @VideoResult;

    -- Video File
    DECLARE @VideoFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @VideoFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @VideoResult vr ON f.Id = vr.FileId
    WHERE f.Deleted = 0;

    SELECT * FROM @VideoFileResult;

    -- Video File - Partial File
    SELECT pf.*
    FROM resources.PartialFile pf
    INNER JOIN @VideoFileResult vf ON pf.FileId = vf.Id
    WHERE pf.Deleted = 0;

    -- Video File Detail
    SELECT vf.*
    FROM resources.VideoFile vf
    INNER JOIN @VideoFileResult vfr ON vf.FileId = vfr.Id
    WHERE vf.Deleted = 0;

    -- Question Block
    DECLARE @QuestionResult TABLE (
        Id INT,
        BlockId INT,
        QuestionBlockCollectionId INT,
        FeedbackBlockCollectionId INT,
        QuestionType INT,
        AllowReveal BIT
    );

    INSERT INTO @QuestionResult
    SELECT qb.Id, qb.BlockId, qb.QuestionBlockCollectionId, qb.FeedbackBlockCollectionId, qb.QuestionType, qb.AllowReveal
    FROM resources.QuestionBlock qb
    INNER JOIN @BlockResult b ON qb.BlockId = b.Id
    WHERE qb.Deleted = 0;

    SELECT * FROM @QuestionResult;

    -- Question Answer
    SELECT qa.*
    FROM resources.QuestionAnswer qa
    INNER JOIN @QuestionResult qr ON qa.QuestionBlockId = qr.Id
    WHERE qa.Deleted = 0;

    -- Image Carousel Block
    SELECT icb.*
    FROM resources.ImageCarouselBlock icb
    INNER JOIN @BlockResult b ON icb.BlockId = b.Id
    WHERE icb.Deleted = 0;
END