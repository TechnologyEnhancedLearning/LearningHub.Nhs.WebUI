CREATE PROCEDURE [resources].[BlockCollectionGet] (
	@BlockCollectionId INT
)
AS
BEGIN

	DECLARE @BlockResult TABLE
	(
		Id int,
		[Order] int,
		Title nvarchar(200),
		BlockType int
	);

	INSERT INTO @BlockResult
	SELECT Id, [Order], Title, BlockType FROM resources.Block WHERE Deleted = 0 AND BlockCollectionId = @BlockCollectionId;

	-- Block
	SELECT *, @BlockCollectionId BlockCollectionId FROM @BlockResult;

	-- Text Block
	SELECT t.* FROM resources.TextBlock t INNER JOIN @BlockResult b ON b.Id = t.BlockId WHERE t.Deleted = 0;

	DECLARE @WsibResult TABLE (
		Id int,
		BlockId int
	);

	INSERT INTO @WsibResult
	SELECT w.Id, w.BlockId FROM resources.WholeSlideImageBlock w INNER JOIN @BlockResult b ON b.Id = w.BlockId WHERE w.Deleted = 0;

	-- WholeSlide Image Block
	SELECT * FROM @WsibResult;

	DECLARE @WsibiResult TABLE (
		Id int,
		WholeSlideImageBlockId int,
		WholeSlideImageId int,
		PlaceholderText nvarchar(255),
		[Order] int
	);

	INSERT INTO @WsibiResult
	SELECT wi.Id, wi.WholeSlideImageBlockId, wi.WholeSlideImageId, wi.PlaceholderText, wi.[Order] 
	FROM resources.WholeSlideImageBlockItem wi INNER JOIN @WsibResult w ON wi.WholeSlideImageBlockId = w.Id WHERE wi.Deleted = 0;

	-- WholeSlide Image Block Items
	SELECT * FROM @WsibiResult;

	DECLARE @WsiResult TABLE (
		Id int,
		Title nvarchar(1000),
		FileId int
	);

	INSERT INTO @WsiResult
	SELECT wsi.Id, wsi.Title, wsi.FileId
	FROM resources.WholeSlideImage wsi INNER JOIN @WsibiResult wi ON wsi.Id = wi.WholeSlideImageId WHERE wsi.Deleted = 0;

	-- WholeSlide Image
	SELECT * FROM @WsiResult;

	DECLARE @WsiFileResult TABLE (
		Id int,
		FileTypeId int,
		FileChunkDetailId int,
		[FileName] nvarchar(255),
		FilePath nvarchar(1024),
		FileSizekb int
	);

	INSERT INTO @WsiFileResult
	SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
	FROM resources.[File] f INNER JOIN @WsiResult wsi ON f.Id = wsi.FileId WHERE f.Deleted = 0;

	-- WholeSlide Image File
	SELECT * FROM @WsiFileResult;

	-- WholeSlide Image File - Partial File
	SELECT pf.*	FROM resources.PartialFile pf INNER JOIN @WsiFileResult wsi ON pf.FileId = wsi.Id WHERE pf.Deleted = 0;

	-- WholeSlide Image File - Image File
	SELECT wsif.* FROM resources.WholeSlideImageFile wsif INNER JOIN @WsiFileResult wsi ON wsif.FileId = wsi.Id WHERE wsif.Deleted = 0;

	DECLARE @WsiaResult TABLE (
		Id int,
		WholeSlideImageId int,
		ImageId int,
		[Order] int,
		Label nvarchar(255),
		Description nvarchar(1000),
		PinXCoordinate decimal(22,19),
		PinYCoordinate decimal(22,19),
		Colour int
	);

	INSERT INTO @WsiaResult
	SELECT ia.Id, ia.WholeSlideImageId, ia.ImageId, ia.[Order], ia.Label, ia.Description, ia.PinXCoordinate, ia.PinYCoordinate, ia.Colour
	FROM resources.ImageAnnotation ia INNER JOIN @WsiResult wsi ON ia.WholeSlideImageId = wsi.Id WHERE ia.Deleted = 0;

	-- WholeSlide Image Annotation
	SELECT * FROM @WsiaResult;

	-- WholeSlide Image Annotation Mark
	SELECT iam.* FROM resources.ImageAnnotationMark iam INNER JOIN @WsiaResult wsia ON iam.ImageAnnotationId = wsia.Id WHERE iam.Deleted = 0;

	DECLARE @MediaBlockResult TABLE (
		Id int,
		BlockId int,
		MediaType int,
		AttachmentId int,
		ImageId int,
		VideoId int
	);

	INSERT INTO @MediaBlockResult
	SELECT mb.Id, mb.BlockId, mb.MediaType, mb.AttachmentId, mb.ImageId, mb.VideoId
	FROM resources.MediaBlock mb INNER JOIN @BlockResult b ON mb.BlockId = b.Id WHERE mb.Deleted = 0;

	-- Media Block
	SELECT * FROM @MediaBlockResult;

	DECLARE @AttachmentResult TABLE (
		Id int,
		FileId int
	);

	INSERT INTO @AttachmentResult
	SELECT a.Id, a.FileId
	FROM resources.Attachment a INNER JOIN @MediaBlockResult mb ON a.Id = mb.AttachmentId WHERE a.Deleted = 0;

	-- Attachment
	SELECT * FROM @AttachmentResult;

	DECLARE @AttachmentFileResult TABLE (
		Id int,
		FileTypeId int,
		FileChunkDetailId int,
		[FileName] nvarchar(255),
		FilePath nvarchar(1024),
		FileSizekb int
	);

	INSERT INTO @AttachmentFileResult
	SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
	FROM resources.[File] f INNER JOIN @AttachmentResult ar ON f.Id = ar.FileId WHERE f.Deleted = 0;

	-- Attachment File
	SELECT * FROM @AttachmentFileResult;

	-- Attachment File - Partial File
	SELECT pf.*	FROM resources.PartialFile pf INNER JOIN @AttachmentFileResult af ON pf.FileId = af.Id WHERE pf.Deleted = 0;

	DECLARE @ImageResult TABLE (
		Id int,
		FileId int,
		AltText nvarchar(125),
		Description nvarchar(250)
	);

	INSERT INTO @ImageResult
	SELECT i.Id, i.FileId, i.AltText, i.Description
	FROM resources.Image i INNER JOIN @MediaBlockResult mb ON i.Id = mb.ImageId WHERE i.Deleted = 0;

	-- Image
	SELECT * FROM @ImageResult;

	DECLARE @ImageFileResult TABLE (
		Id int,
		FileTypeId int,
		FileChunkDetailId int,
		[FileName] nvarchar(255),
		FilePath nvarchar(1024),
		FileSizekb int
	);

	INSERT INTO @ImageFileResult
	SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
	FROM resources.[File] f INNER JOIN @ImageResult ir ON f.Id = ir.FileId WHERE f.Deleted = 0;

	-- Image File
	SELECT * FROM @ImageFileResult;

	-- Image File - Partial File
	SELECT pf.*	FROM resources.PartialFile pf INNER JOIN @ImageFileResult ifr ON pf.FileId = ifr.Id WHERE pf.Deleted = 0;

	DECLARE @VideoResult TABLE (
		Id int,
		FileId int
	);

	INSERT INTO @VideoResult
	SELECT v.Id, v.FileId FROM resources.Video v INNER JOIN @MediaBlockResult mb ON v.Id = mb.VideoId WHERE v.Deleted = 0;

	-- Video
	SELECT * FROM @VideoResult;

	DECLARE @VideoFileResult TABLE (
		Id int,
		FileTypeId int,
		FileChunkDetailId int,
		[FileName] nvarchar(255),
		FilePath nvarchar(1024),
		FileSizekb int
	);

	INSERT INTO @VideoFileResult
	SELECT f.Id, f.FileTypeId, f.FileChunkDetailId, f.[FileName], f.FilePath, f.FileSizeKb
	FROM resources.[File] f INNER JOIN @VideoResult vr ON f.Id = vr.FileId WHERE f.Deleted = 0;

	-- Video File
	SELECT * FROM @VideoFileResult;

	-- Video File - Partial File
	SELECT pf.*	FROM resources.PartialFile pf INNER JOIN @VideoFileResult vf ON pf.FileId = vf.Id WHERE pf.Deleted = 0;

	-- Video File Detail
	SELECT vf.*	FROM resources.VideoFile vf INNER JOIN @VideoFileResult vfr ON vf.FileId = vfr.Id WHERE vf.Deleted = 0;

	DECLARE @QuestionResult TABLE (
		Id int,
		BlockId int,
		QuestionBlockCollectionId int,
		FeedbackBlockCollectionId int,
		QuestionType int,
		AllowReveal bit
	);

	INSERT INTO @QuestionResult
	SELECT qb.Id, qb.BlockId, qb.QuestionBlockCollectionId, qb.FeedbackBlockCollectionId, qb.QuestionType, qb.AllowReveal
	FROM resources.QuestionBlock qb INNER JOIN @BlockResult b ON qb.BlockId = b.Id WHERE qb.Deleted = 0;

	-- Question Block
	SELECT * FROM @QuestionResult;

	-- Question Answer
	SELECT qa.*	FROM resources.QuestionAnswer qa INNER JOIN @QuestionResult qr ON qa.QuestionBlockId = qr.Id WHERE qa.Deleted = 0;

	-- Image Carousel Block
	SELECT icb.* FROM resources.ImageCarouselBlock icb INNER JOIN @BlockResult b ON icb.BlockId = b.Id WHERE icb.Deleted = 0;

END
GO