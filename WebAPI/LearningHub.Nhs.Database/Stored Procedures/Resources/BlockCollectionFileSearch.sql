-------------------------------------------------------------------------------
-- Author       Tobi Awe
-- Created      30-05-2024
-- Purpose      search all assessment/case block collections files


-- Modification History
--
-- 31-05-2024  TD-3023	Initial Revision
-- 04-06-2024  TD-3023  Included answer blocks to the search
-------------------------------------------------------------------------------


CREATE PROCEDURE [resources].[BlockCollectionFileSearch] (
	@excludeResourceVersionId INT,
	@resourceType INT,
	@filePath nvarchar(max) output
)
AS
BEGIN
    -- Table to hold BlockCollectionIds
    DECLARE @BlockCollectionId TABLE (Id INT);
	DECLARE @AllBlockAssociatedFiles TABLE (FileId INT, FilePath NVARCHAR(200));
	SET @filePath = NULL;

    -- Insert BlockCollectionIds based on the provided query
   IF @resourceType = 10
BEGIN
   --case
INSERT INTO @BlockCollectionId (Id)
SELECT DISTINCT c.BlockCollectionId
FROM [resources].[CaseResourceVersion] AS c
INNER JOIN [resources].[ResourceVersion] AS r 
    ON c.ResourceVersionId = r.Id
INNER JOIN [resources].[Resource] AS r0 
    ON r.ResourceId = r0.Id
WHERE 
    c.Deleted = 0
    AND r.Deleted = 0
    AND r0.Deleted = 0
    AND c.ResourceVersionId <> @excludeResourceVersionId
    AND (r.VersionStatusId <> 2 OR (r.VersionStatusId = 2 AND r0.CurrentResourceVersionId = c.ResourceVersionId))
    AND c.BlockCollectionId IS NOT NULL;

END
ELSE IF @resourceType = 11
BEGIN
   --assessment
   INSERT INTO @BlockCollectionId (Id)
SELECT DISTINCT Id
FROM (
    SELECT 
        c.AssessmentContentId AS Id
    FROM 
        [resources].AssessmentResourceVersion AS c
    INNER JOIN 
        [resources].ResourceVersion AS r ON c.ResourceVersionId = r.Id
    INNER JOIN 
        [resources].Resource AS r0 ON r.ResourceId = r0.Id
    WHERE 
        c.Deleted = CAST(0 AS bit) 
        AND r.Deleted = CAST(0 AS bit)
        AND r0.Deleted = CAST(0 AS bit)
        AND c.ResourceVersionId <> @excludeResourceVersionId
        AND (r.VersionStatusId <> 2 OR (r.VersionStatusId = 2 AND r0.CurrentResourceVersionId = c.ResourceVersionId))
        AND c.AssessmentContentId IS NOT NULL

    UNION ALL

    SELECT 
        c.EndGuidanceId AS Id
    FROM 
        [resources].AssessmentResourceVersion AS c
    INNER JOIN 
        [resources].ResourceVersion AS r ON c.ResourceVersionId = r.Id
    INNER JOIN 
        [resources].Resource AS r0 ON r.ResourceId = r0.Id
    WHERE 
        c.Deleted = CAST(0 AS bit) 
        AND r.Deleted = CAST(0 AS bit)
        AND r0.Deleted = CAST(0 AS bit)
        AND c.ResourceVersionId <> @excludeResourceVersionId
        AND (r.VersionStatusId <> 2 OR (r.VersionStatusId = 2 AND r0.CurrentResourceVersionId = c.ResourceVersionId))
        AND c.EndGuidanceId IS NOT NULL
) as assessmentBlockCollections;
END

	 -- Table to hold temp results for blocks
    DECLARE @TempBlockResult TABLE (
        Id INT,
        [Order] INT,
        Title NVARCHAR(200),
        BlockType INT,
        BlockCollectionId INT
    );

	DECLARE @QABlock TABLE (
        Id INT,
        [Order] INT,
        Title NVARCHAR(200),
        BlockType INT,
        BlockCollectionId INT
    );

    -- Table to hold results for blocks
    DECLARE @BlockResult TABLE (
        Id INT,
        [Order] INT,
        Title NVARCHAR(200),
        BlockType INT,
        BlockCollectionId INT
    );

    -- Insert temp block data
    INSERT INTO @TempBlockResult
    SELECT Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.Block
    WHERE Deleted = 0 AND BlockCollectionId IN (SELECT Id FROM @BlockCollectionId);

	-- Insert ImageCarouselBlockCollectionId
	INSERT INTO @BlockCollectionId (Id)
	SELECT icb.ImageBlockCollectionId
    FROM resources.ImageCarouselBlock icb
    INNER JOIN @TempBlockResult b ON icb.BlockId = b.Id
    WHERE icb.Deleted = 0;

--Insert QuestionBlockCollectionId and FeedbackBlockCollectionId
	INSERT INTO @BlockCollectionId (Id)
	SELECT qb.QuestionBlockCollectionId
	FROM resources.QuestionBlock qb
	INNER JOIN @TempBlockResult b ON qb.BlockId = b.Id
	WHERE qb.Deleted = 0

	UNION

	SELECT qb.FeedbackBlockCollectionId
	FROM resources.QuestionBlock qb
	INNER JOIN @TempBlockResult b ON qb.BlockId = b.Id
	WHERE qb.Deleted = 0;

	-- Insert answer question block data
    INSERT INTO @QABlock
	SELECT qb.Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.QuestionBlock qb
    INNER JOIN @TempBlockResult b ON qb.BlockId = b.Id
    WHERE qb.Deleted = 0;

    --Insert AnswerBlockcollectionId
	 	INSERT INTO @BlockCollectionId (Id)
	SELECT qa.BlockCollectionId
    FROM resources.QuestionAnswer qa
    INNER JOIN @QABlock b ON qa.QuestionBlockId = b.Id
    WHERE qa.Deleted = 0;


	 -- Insert block data
    INSERT INTO @BlockResult
    SELECT Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.Block
    WHERE Deleted = 0 AND BlockCollectionId IN (SELECT Id FROM @BlockCollectionId);



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

   -- SELECT * FROM @WsibResult;

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

    --SELECT * FROM @WsibiResult;

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

	
    --SELECT * FROM @WsiResult;

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

	INSERT INTO @AllBlockAssociatedFiles 
	SELECT Id,FilePath FROM @WsiFileResult;



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

	
    --SELECT * FROM @MediaBlockResult;

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

    --SELECT * FROM @AttachmentResult;

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

	INSERT INTO @AllBlockAssociatedFiles
    SELECT Id,FilePath FROM @AttachmentFileResult;

    -- Attachment File - Partial File
	INSERT INTO @AllBlockAssociatedFiles
    SELECT pf.FileId,af.FilePath
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

    --SELECT * FROM @ImageResult;

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

	INSERT INTO @AllBlockAssociatedFiles
    SELECT Id,FilePath FROM @ImageFileResult;


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

   -- SELECT * FROM @VideoResult;

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

	INSERT INTO @AllBlockAssociatedFiles
    SELECT Id,FilePath FROM @VideoFileResult;

	--SELECT * FROM @AllBlockAssociatedFiles;
----------------------------------------------------------------------------------------------------------------------

 

    -- Table to hold BlockCollectionIds
    DECLARE @_BlockCollectionId TABLE (Id INT);
	DECLARE @_QuestionBlockCollectionId TABLE (Id INT);
	DECLARE @_ImageCarouselBlockCollectionId TABLE (Id INT);
	DECLARE @_AllBlockAssociatedFiles TABLE (FileId INT, FilePath NVARCHAR(200));
	DECLARE @_QABlock TABLE (Id INT,[Order] INT,Title NVARCHAR(200),BlockType INT,BlockCollectionId INT);

    -- Insert BlockCollectionIds based on the provided query
IF @resourceType = 10
BEGIN
   --case
INSERT INTO @_BlockCollectionId (Id)
SELECT DISTINCT c.BlockCollectionId
FROM [resources].[CaseResourceVersion] AS c
INNER JOIN [resources].[ResourceVersion] AS r 
    ON c.ResourceVersionId = r.Id
INNER JOIN [resources].[Resource] AS r0 
    ON r.ResourceId = r0.Id
WHERE 
    c.Deleted = 0
    AND c.ResourceVersionId = @excludeResourceVersionId

END
ELSE IF @resourceType = 11
BEGIN
   --assessment
INSERT INTO @_BlockCollectionId (Id)
SELECT DISTINCT Id
FROM (
    SELECT 
        c.AssessmentContentId AS Id
    FROM 
        [resources].AssessmentResourceVersion AS c
    INNER JOIN 
        [resources].ResourceVersion AS r ON c.ResourceVersionId = r.Id
    INNER JOIN 
        [resources].Resource AS r0 ON r.ResourceId = r0.Id
    WHERE 
        c.Deleted = CAST(0 AS bit) 
        AND c.ResourceVersionId = @excludeResourceVersionId

    UNION ALL

    SELECT 
        c.EndGuidanceId AS Id
    FROM 
        [resources].AssessmentResourceVersion AS c
    INNER JOIN 
        [resources].ResourceVersion AS r ON c.ResourceVersionId = r.Id
    INNER JOIN 
        [resources].Resource AS r0 ON r.ResourceId = r0.Id
    WHERE 
        c.Deleted = CAST(0 AS bit) 
        AND r.Deleted = CAST(0 AS bit)
        AND r0.Deleted = CAST(0 AS bit)
        AND c.ResourceVersionId <> @excludeResourceVersionId
) as assessmentBlockCollections;
END


	 -- Table to hold temp results for blocks
    DECLARE @_TempBlockResult TABLE (
        Id INT,
        [Order] INT,
        Title NVARCHAR(200),
        BlockType INT,
        BlockCollectionId INT
    );

    -- Table to hold results for blocks
    DECLARE @_BlockResult TABLE (
        Id INT,
        [Order] INT,
        Title NVARCHAR(200),
        BlockType INT,
        BlockCollectionId INT
    );

    -- Insert temp block data
    INSERT INTO @_TempBlockResult
    SELECT Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.Block
    WHERE Deleted = 0 AND BlockCollectionId IN (SELECT Id FROM @_BlockCollectionId);

	-- Insert ImageCarouselBlockCollectionId
	INSERT INTO @_BlockCollectionId (Id)
	SELECT icb.ImageBlockCollectionId
    FROM resources.ImageCarouselBlock icb
    INNER JOIN @_TempBlockResult b ON icb.BlockId = b.Id
    WHERE icb.Deleted = 0;

	--Insert QuestionBlockCollectionId and FeedbackBlockCollectionId
	INSERT INTO @_BlockCollectionId (Id)
	SELECT qb.QuestionBlockCollectionId
	FROM resources.QuestionBlock qb
	INNER JOIN @_TempBlockResult b ON qb.BlockId = b.Id
	WHERE qb.Deleted = 0

	UNION

	SELECT qb.FeedbackBlockCollectionId
	FROM resources.QuestionBlock qb
	INNER JOIN @_TempBlockResult b ON qb.BlockId = b.Id
	WHERE qb.Deleted = 0;

	-- Insert answer question block data
    INSERT INTO @_QABlock
	SELECT qb.Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.QuestionBlock qb
    INNER JOIN @_TempBlockResult b ON qb.BlockId = b.Id
    WHERE qb.Deleted = 0;

    --Insert AnswerBlockcollectionId
	 	INSERT INTO @_BlockCollectionId (Id)
	SELECT qa.BlockCollectionId
    FROM resources.QuestionAnswer qa
    INNER JOIN @_QABlock b ON qa.QuestionBlockId = b.Id
    WHERE qa.Deleted = 0;



	 -- Insert block data
    INSERT INTO @_BlockResult
    SELECT Id, [Order], Title, BlockType, BlockCollectionId
    FROM resources.Block
    WHERE Deleted = 0 AND BlockCollectionId IN (SELECT Id FROM @_BlockCollectionId);



    -- WholeSlide Image Block
    DECLARE @_WsibResult TABLE (
        Id INT,
        BlockId INT
    );

    INSERT INTO @_WsibResult
    SELECT w.Id, w.BlockId
    FROM resources.WholeSlideImageBlock w
    INNER JOIN @_BlockResult b ON b.Id = w.BlockId
    WHERE w.Deleted = 0;

   -- SELECT * FROM @_WsibResult;

    -- WholeSlide Image Block Items
    DECLARE @_WsibiResult TABLE (
        Id INT,
        WholeSlideImageBlockId INT,
        WholeSlideImageId INT,
        PlaceholderText NVARCHAR(255),
        [Order] INT
    );

    INSERT INTO @_WsibiResult
    SELECT wi.Id, wi.WholeSlideImageBlockId, wi.WholeSlideImageId, wi.PlaceholderText, wi.[Order]
    FROM resources.WholeSlideImageBlockItem wi
    INNER JOIN @_WsibResult w ON wi.WholeSlideImageBlockId = w.Id
    WHERE wi.Deleted = 0;

    --SELECT * FROM @_WsibiResult;

    -- WholeSlide Image
    DECLARE @_WsiResult TABLE (
        Id INT,
        Title NVARCHAR(1000),
        FileId INT
    );

    INSERT INTO @_WsiResult
    SELECT wsi.Id, wsi.Title, wsi.FileId
    FROM resources.WholeSlideImage wsi
    INNER JOIN @_WsibiResult wi ON wsi.Id = wi.WholeSlideImageId
    WHERE wsi.Deleted = 0;

	
    --SELECT * FROM @_WsiResult;

    -- WholeSlide Image File
    DECLARE @_WsiFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @_WsiFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @_WsiResult wsi ON f.Id = wsi.FileId
    WHERE f.Deleted = 0;

	INSERT INTO @_AllBlockAssociatedFiles 
	SELECT Id,FilePath FROM @_WsiFileResult;



    -- Media Block
    DECLARE @_MediaBlockResult TABLE (
        Id INT,
        BlockId INT,
        MediaType INT,
        AttachmentId INT,
        ImageId INT,
        VideoId INT
    );

    INSERT INTO @_MediaBlockResult
    SELECT mb.Id, mb.BlockId, mb.MediaType, mb.AttachmentId, mb.ImageId, mb.VideoId
    FROM resources.MediaBlock mb
    INNER JOIN @_BlockResult b ON mb.BlockId = b.Id
    WHERE mb.Deleted = 0;

	
    --SELECT * FROM @_MediaBlockResult;

    -- Attachment
    DECLARE @_AttachmentResult TABLE (
        Id INT,
        FileId INT
    );

    INSERT INTO @_AttachmentResult
    SELECT a.Id, a.FileId
    FROM resources.Attachment a
    INNER JOIN @_MediaBlockResult mb ON a.Id = mb.AttachmentId
    WHERE a.Deleted = 0;

    --SELECT * FROM @_AttachmentResult;

    -- Attachment File
    DECLARE @_AttachmentFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @_AttachmentFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @_AttachmentResult ar ON f.Id = ar.FileId
    WHERE f.Deleted = 0;

	INSERT INTO @_AllBlockAssociatedFiles
    SELECT Id,FilePath FROM @_AttachmentFileResult;

    -- Attachment File - Partial File
	INSERT INTO @_AllBlockAssociatedFiles
    SELECT pf.FileId,af.FilePath
    FROM resources.PartialFile pf
    INNER JOIN @_AttachmentFileResult af ON pf.FileId = af.Id
    WHERE pf.Deleted = 0;

    -- Image
    DECLARE @_ImageResult TABLE (
        Id INT,
        FileId INT,
        AltText NVARCHAR(125),
        Description NVARCHAR(250)
    );

    INSERT INTO @_ImageResult
    SELECT i.Id, i.FileId, i.AltText, i.Description
    FROM resources.Image i
    INNER JOIN @_MediaBlockResult mb ON i.Id = mb.ImageId
    WHERE i.Deleted = 0;

    --SELECT * FROM @_ImageResult;

    -- Image File
    DECLARE @_ImageFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @_ImageFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @_ImageResult ir ON f.Id = ir.FileId
    WHERE f.Deleted = 0;

	INSERT INTO @_AllBlockAssociatedFiles
    SELECT Id,FilePath FROM @_ImageFileResult;


    -- Video
    DECLARE @_VideoResult TABLE (
        Id INT,
        FileId INT
    );

    INSERT INTO @_VideoResult
    SELECT v.Id, v.FileId
    FROM resources.Video v
    INNER JOIN @_MediaBlockResult mb ON v.Id = mb.VideoId
    WHERE v.Deleted = 0;

   -- SELECT * FROM @_VideoResult;

    -- Video File
    DECLARE @_VideoFileResult TABLE (
        Id INT,
        FileTypeId INT,
        FileChunkDetailId INT,
        [FileName] NVARCHAR(255),
        FilePath NVARCHAR(1024),
        FileSizekb INT
    );

    INSERT INTO @_VideoFileResult
    SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
    FROM resources.[File] f
    INNER JOIN @_VideoResult vr ON f.Id = vr.FileId
    WHERE f.Deleted = 0;

	INSERT INTO @_AllBlockAssociatedFiles
    SELECT Id,FilePath FROM @_VideoFileResult;

	--SELECT * FROM @_AllBlockAssociatedFiles;
----------------------------------------------------------------------------------------------------------------------

 DECLARE @SearchResult TABLE (
        Id INT,
        FilePath NVARCHAR(1024),
        MatchId INT,
		MatchedPath NVARCHAR(1024)
    );

	INSERT INTO @SearchResult
    SELECT f.FileId, f.FilePath, isMatched.FileId, isMatched.FilePath
    FROM @_AllBlockAssociatedFiles f
    LEFT JOIN @AllBlockAssociatedFiles isMatched ON (f.FileId = isMatched.FileId or f.FilePath = isMatched.FilePath)


	DECLARE @result VARCHAR(MAX);

    SELECT @result= STUFF((
        SELECT ', ' + FilePath
        FROM @SearchResult WHERE MatchId is NULL
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 1, 2, '');

    SET @filePath = @result;
	END