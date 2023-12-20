CREATE PROCEDURE [resources].[BlockCollectionDelete] (
	@BlockCollectionId INT
)
AS
BEGIN

	BEGIN TRANSACTION;
	BEGIN TRY

		DECLARE @BlockResult TABLE (Id int);
		INSERT INTO @BlockResult SELECT Id FROM resources.Block WHERE BlockCollectionId = @BlockCollectionId;

		-- Text Block
		DELETE t FROM resources.TextBlock t INNER JOIN @BlockResult b ON b.Id = t.BlockId;
	
	
		/****** WholeSlide Image Block Start ******/

		DECLARE @WholeSlideImageBlockResult TABLE (Id int);
		INSERT INTO @WholeSlideImageBlockResult SELECT w.Id FROM resources.WholeSlideImageBlock w INNER JOIN @BlockResult b ON b.Id = w.BlockId;

		DECLARE @WholeSlideImageBlockItemResult TABLE (Id int, WholeSlideImageId int);
		INSERT INTO @WholeSlideImageBlockItemResult SELECT wi.Id, wi.WholeSlideImageId FROM resources.WholeSlideImageBlockItem wi INNER JOIN @WholeSlideImageBlockResult w ON wi.WholeSlideImageBlockId = w.Id;

		-- WholeSlide Image Block Items
		DELETE wi FROM resources.WholeSlideImageBlockItem wi INNER JOIN @WholeSlideImageBlockItemResult wir ON wi.Id = wir.Id;

		-- WholeSlide Image Block
		DELETE w FROM resources.WholeSlideImageBlock w INNER JOIN @WholeSlideImageBlockResult wr ON w.Id = wr.Id;

		DECLARE @WholeSlideImageResult TABLE (Id int, FileId int);
		INSERT INTO @WholeSlideImageResult SELECT wsi.Id, wsi.FileId FROM resources.WholeSlideImage wsi INNER JOIN @WholeSlideImageBlockItemResult wi ON wsi.Id = wi.WholeSlideImageId;

		DECLARE @ImageAnnotationResult TABLE (Id int);
		INSERT INTO @ImageAnnotationResult	SELECT ia.Id FROM resources.ImageAnnotation ia INNER JOIN @WholeSlideImageResult wsi ON ia.WholeSlideImageId = wsi.Id;

		-- WholeSlide Image Annotation Mark
		DELETE iam FROM resources.ImageAnnotationMark iam INNER JOIN @ImageAnnotationResult wsia ON iam.ImageAnnotationId = wsia.Id;

		-- WholeSlide Image Annotation
		DELETE ia FROM resources.ImageAnnotation ia INNER JOIN @ImageAnnotationResult wsi ON ia.Id = wsi.Id;

		-- WholeSlide Image
		DELETE wsi FROM resources.WholeSlideImage wsi INNER JOIN @WholeSlideImageResult wi ON wsi.Id = wi.Id;

		/****** WholeSlide Image End ******/
	
	
		/****** Media Block Start ******/

		DECLARE @MediaBlockResult TABLE (
			Id int,
			AttachmentId int,
			ImageId int,
			VideoId int
		);

		INSERT INTO @MediaBlockResult
		SELECT mb.Id, mb.AttachmentId, mb.ImageId, mb.VideoId
		FROM resources.MediaBlock mb INNER JOIN @BlockResult b ON mb.BlockId = b.Id;
	
		-- Media Block
		DELETE mb FROM resources.MediaBlock mb INNER JOIN @MediaBlockResult b ON mb.Id = b.Id;

		-- Attachment
		DELETE a FROM resources.Attachment a INNER JOIN @MediaBlockResult mb ON a.Id = mb.AttachmentId;

		-- Image
		DELETE i FROM resources.[Image] i INNER JOIN @MediaBlockResult mb ON i.Id = mb.ImageId;
	
		-- Video
		DELETE v FROM resources.Video v INNER JOIN @MediaBlockResult mb ON v.Id = mb.VideoId;
	
		/****** Media Block End ******/

	
		/****** Question Block Start ******/

		DECLARE @QuestionBlockResult TABLE (Id int);
		INSERT INTO @QuestionBlockResult SELECT qb.Id FROM resources.QuestionBlock qb INNER JOIN @BlockResult b ON qb.BlockId = b.Id;

		-- Question Answer
		DELETE qa FROM resources.QuestionAnswer qa INNER JOIN @QuestionBlockResult qbr ON qa.QuestionBlockId = qbr.Id;

		-- Question Block
		DELETE qb FROM resources.QuestionBlock qb INNER JOIN @QuestionBlockResult qbr ON qb.Id = qbr.Id;
	
		/****** Question Block End ******/

		-- Image Carousel Block
		DELETE icb FROM resources.ImageCarouselBlock icb INNER JOIN @BlockResult b ON icb.BlockId = b.Id;

		-- Blocks
		DELETE b FROM resources.[Block] b WHERE BlockCollectionId = @BlockCollectionId;

		-- Block collection
		DELETE b FROM resources.[BlockCollection] b WHERE Id = @BlockCollectionId;
	
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
     DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
     DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
     DECLARE @ErrorState INT = ERROR_STATE();

		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION
			RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		END
	END CATCH
END