namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Resource.Blocks;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The block collection repository.
    /// </summary>
    public class BlockCollectionRepository : GenericRepository<BlockCollection>, IBlockCollectionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCollectionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public BlockCollectionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="blockCollection">The block collection.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task<int> CreateAsync(int userId, BlockCollection blockCollection)
        {
            this.SetAuditFieldsOnChildren(userId, blockCollection, isCreate: true);

            return await base.CreateAsync(userId, blockCollection);
        }

        /// <summary>
        /// Delete the Block Collection.
        /// </summary>
        /// <param name="userId">The User Id.</param>
        /// <param name="blockCollectionId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteBlockCollection(int userId, int blockCollectionId)
        {
            BlockCollection blockCollection = this.DbContext.BlockCollection
                .Include(b => b.Blocks)
                .ThenInclude(b => b.QuestionBlock)
                .ThenInclude(b => b.Answers)
                .Include(bc => bc.Blocks)
                .ThenInclude(b => b.ImageCarouselBlock)
                .FirstOrDefault(bc => bc.Id == blockCollectionId);

            if (blockCollection == null)
            {
                return;
            }

            var questionBlockCollection = blockCollection.Blocks
                .Where(b => b.QuestionBlock != null)
                .SelectMany(b =>
                {
                    var qb = b.QuestionBlock;
                    return qb.Answers
                                .Where(t => t.BlockCollectionId.HasValue)
                                .Select(a => a.BlockCollectionId.Value)
                                .Concat(new[] { qb.QuestionBlockCollectionId, qb.FeedbackBlockCollectionId });
                });

            var imageBlockCollection = blockCollection.Blocks
                .Where(b => b.ImageCarouselBlock != null)
                .Select(b => b.ImageCarouselBlock.ImageBlockCollectionId);

            var collectionIds = new[] { blockCollectionId }.Concat(imageBlockCollection.Concat(questionBlockCollection));

            foreach (var id in collectionIds)
            {
                using (var lhContext = new LearningHubDbContext(this.DbContext.Options))
                {
                    _ = lhContext.Database.ExecuteSqlRawAsync("resources.BlockCollectionDelete @p0", new SqlParameter("@p0", SqlDbType.Int) { Value = id });
                }
            }
        }

        /// <summary>
        /// Gets the Block Collection (including child Blocks, TextBlocks, WholeSlideImageBlocks and Files).
        /// </summary>
        /// <param name="blockCollectionId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<BlockCollection> GetBlockCollection(int? blockCollectionId)
        {
            if (!blockCollectionId.HasValue)
            {
                return null;
            }

            var command = new SqlCommand
            {
                CommandText = "[resources].[BlockCollectionGet]",
                CommandType = CommandType.StoredProcedure,
                Parameters = { new SqlParameter("@BlockCollectionId", SqlDbType.Int) { Value = blockCollectionId } },
            };

            var results = this.DbContext.MultipleResults(command)
                            .With<Block>()
                            .With<TextBlock>()
                            .With<WholeSlideImageBlock>()
                            .With<WholeSlideImageBlockItem>()
                            .With<WholeSlideImage>()
                            .With<File>()
                            .With<PartialFile>()
                            .With<WholeSlideImageFile>()
                            .With<ImageAnnotation>()
                            .With<ImageAnnotationMark>()
                            .With<MediaBlock>()
                            .With<Attachment>()
                            .With<File>()
                            .With<PartialFile>()
                            .With<Image>()
                            .With<File>()
                            .With<PartialFile>()
                            .With<Video>()
                            .With<File>()
                            .With<PartialFile>()
                            .With<VideoFile>()
                            .With<QuestionBlock>()
                            .With<QuestionAnswer>()
                            .With<ImageCarouselBlock>()
                            .Execute();

            var blocks = results[0].OfType<Block>().ToArray();
            var textBlocks = results[1].OfType<TextBlock>();
            var wholeSlideImgBlocks = GetWholeSlideImageBlocks(results);
            var mediaBlocks = GetMediaBlocks(results);
            var questionBlocks = GetQuestionBlocks(results);
            var imgCarouselBlocks = results[23].OfType<ImageCarouselBlock>();

            foreach (var block in blocks)
            {
                block.TextBlock = textBlocks.FirstOrDefault(t => t.BlockId == block.Id);
                block.WholeSlideImageBlock = wholeSlideImgBlocks.FirstOrDefault(w => w.BlockId == block.Id);
                block.MediaBlock = mediaBlocks.FirstOrDefault(m => m.BlockId == block.Id);
                block.QuestionBlock = questionBlocks.FirstOrDefault(q => q.BlockId == block.Id);
                block.ImageCarouselBlock = imgCarouselBlocks.FirstOrDefault(i => i.BlockId == block.Id);
            }

            var blockCollection = new BlockCollection
            {
                Id = blockCollectionId.Value,
                Blocks = blocks,
            };

            await Task.WhenAll(blockCollection.Blocks
                .Where(block => block.QuestionBlock != null)
                .Select(block => this.FillInPartialQuestionBlock(block.QuestionBlock)));

            await Task.WhenAll(blockCollection.Blocks
                .Where(block => block.ImageCarouselBlock != null)
                .Select(block => this.FillInPartialImageCarouselBlock(block.ImageCarouselBlock)));

            return blockCollection;
        }

        /// <summary>
        /// Gets the Question blocks for a particular blockCollectionId.
        /// </summary>
        /// <param name="blockCollectionId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<Block>> GetQuestionBlocks(int blockCollectionId)
        {
            List<Block> questionBlocks = await this.DbContext.Block
                .AsNoTracking()
                .Include(b => b.QuestionBlock)
                .ThenInclude(qb => qb.Answers)
                .Where(b => b.BlockCollectionId == blockCollectionId && b.BlockType == BlockType.Question)
                .OrderBy(b => b.Order)
                .ToListAsync();

            return questionBlocks;
        }

        private static IEnumerable<WholeSlideImageBlock> GetWholeSlideImageBlocks(List<IEnumerable> results)
        {
            var wholeSlideImageBlocks = results[2].OfType<WholeSlideImageBlock>();
            var wholeSlideImageBlockItems = results[3].OfType<WholeSlideImageBlockItem>();
            var wholeSlideImages = results[4].OfType<WholeSlideImage>();
            var wholeSlideFiles = results[5].OfType<File>();
            var wholeSlideImagePartialFiles = results[6].OfType<PartialFile>();
            var wholeSlideImageFiles = results[7].OfType<WholeSlideImageFile>();
            var imageAnnotations = results[8].OfType<ImageAnnotation>();
            var imageAnnotationMarks = results[9].OfType<ImageAnnotationMark>();

            foreach (var annotation in imageAnnotations)
            {
                annotation.ImageAnnotationMarks = imageAnnotationMarks.Where(i => i.ImageAnnotationId == annotation.Id).ToArray();
            }

            foreach (var img in wholeSlideImages)
            {
                img.ImageAnnotations = imageAnnotations.Where(i => i.WholeSlideImageId == img.Id).ToArray();
                img.File = wholeSlideFiles.FirstOrDefault(i => i.Id == img.FileId);
                if (img.File != null)
                {
                    img.File.PartialFile = wholeSlideImagePartialFiles.FirstOrDefault(f => f.FileId == img.File.Id);
                    img.File.WholeSlideImageFile = wholeSlideImageFiles.FirstOrDefault(f => f.FileId == img.File.Id);
                }
            }

            foreach (var wsiBlockItem in wholeSlideImageBlockItems)
            {
                wsiBlockItem.WholeSlideImage = wholeSlideImages.FirstOrDefault(w => w.Id == wsiBlockItem.WholeSlideImageId);
            }

            foreach (var wsiBlock in wholeSlideImageBlocks)
            {
                wsiBlock.WholeSlideImageBlockItems = wholeSlideImageBlockItems.Where(w => w.WholeSlideImageBlockId == wsiBlock.Id).ToArray();
            }

            return wholeSlideImageBlocks;
        }

        private static IEnumerable<MediaBlock> GetMediaBlocks(List<IEnumerable> results)
        {
            var mediaBlocks = results[10].OfType<MediaBlock>();
            var attachments = results[11].OfType<Attachment>();
            var attachmentFiles = results[12].OfType<File>();
            var attachmentPartialFiles = results[13].OfType<PartialFile>();
            var images = results[14].OfType<Image>();
            var imageFiles = results[15].OfType<File>();
            var imagePartialFiles = results[16].OfType<PartialFile>();
            var videos = results[17].OfType<Video>();
            var videoFiles = results[18].OfType<File>();
            var videoPartialFiles = results[19].OfType<PartialFile>();
            var videoDetailFiles = results[20].OfType<VideoFile>();

            foreach (var mb in mediaBlocks)
            {
                mb.Attachment = attachments.FirstOrDefault(a => a.Id == mb.AttachmentId);
                if (mb.Attachment != null)
                {
                    mb.Attachment.File = attachmentFiles.FirstOrDefault(i => i.Id == mb.Attachment.FileId);
                    if (mb.Attachment.File != null)
                    {
                        mb.Attachment.File.PartialFile = attachmentPartialFiles.FirstOrDefault(f => f.FileId == mb.Attachment.File.Id);
                    }
                }

                mb.Image = images.FirstOrDefault(i => i.Id == mb.ImageId);
                if (mb.Image != null)
                {
                    mb.Image.File = imageFiles.FirstOrDefault(i => i.Id == mb.Image.FileId);
                    if (mb.Image.File != null)
                    {
                        mb.Image.File.PartialFile = imagePartialFiles.FirstOrDefault(f => f.FileId == mb.Image.File.Id);
                    }
                }

                mb.Video = videos.FirstOrDefault(v => v.Id == mb.VideoId);
                if (mb.Video != null)
                {
                    mb.Video.File = videoFiles.FirstOrDefault(i => i.Id == mb.Video.FileId);
                    if (mb.Video.File != null)
                    {
                        mb.Video.File.PartialFile = videoPartialFiles.FirstOrDefault(f => f.FileId == mb.Video.File.Id);
                        mb.Video.File.VideoFile = videoDetailFiles.FirstOrDefault(f => f.FileId == mb.Video.File.Id);
                    }
                }
            }

            return mediaBlocks;
        }

        private static IEnumerable<QuestionBlock> GetQuestionBlocks(List<IEnumerable> results)
        {
            var questionBlocks = results[21].OfType<QuestionBlock>();
            var questionAnswers = results[22].OfType<QuestionAnswer>();
            foreach (var qb in questionBlocks)
            {
                qb.Answers = questionAnswers.Where(q => q.QuestionBlockId == qb.Id).ToArray();
            }

            return questionBlocks;
        }

        private async Task FillInPartialQuestionBlock(QuestionBlock questionBlock)
        {
            var questionTask = this.GetBlockCollection(questionBlock.QuestionBlockCollectionId);
            var feedbackTask = this.GetBlockCollection(questionBlock.FeedbackBlockCollectionId);
            var answersTask = Task.WhenAll(questionBlock.Answers
                .Select(async answer =>
                {
                    var blockCollection = await this.GetBlockCollection(answer.BlockCollectionId);
                    answer.QuestionBlock = questionBlock;
                    answer.BlockCollection = blockCollection;
                }));

            questionBlock.QuestionBlockCollection = await questionTask;
            questionBlock.FeedbackBlockCollection = await feedbackTask;
            await answersTask;
        }

        private async Task FillInPartialImageCarouselBlock(ImageCarouselBlock imageCarouselBlock)
        {
            var imageTask = this.GetBlockCollection(imageCarouselBlock.ImageBlockCollectionId);
            imageCarouselBlock.ImageBlockCollection = await imageTask;
        }

        private void SetAuditFieldsOnChildren(int userId, BlockCollection blockCollection, bool isCreate)
        {
            this.SetAuditFieldsForCreateOrDelete(userId, blockCollection, isCreate);
            foreach (var block in blockCollection.Blocks)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, block, isCreate);
                this.SetAuditFieldsOnChildren(userId, block, isCreate);
            }
        }

        private void SetAuditFieldsOnChildren(int userId, Block block, bool isCreate)
        {
            if (block.TextBlock != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, block.TextBlock, isCreate);
            }

            if (block.MediaBlock != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, block.MediaBlock, isCreate);
                this.SetAuditFieldsOnChildren(userId, block.MediaBlock, isCreate);
            }

            if (block.WholeSlideImageBlock != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, block.WholeSlideImageBlock, isCreate);
                this.SetAuditFieldsOnChildren(userId, block.WholeSlideImageBlock, isCreate);
            }

            if (block.QuestionBlock != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, block.QuestionBlock, isCreate);
                this.SetAuditFieldsOnChildren(userId, block.QuestionBlock, isCreate);
            }

            if (block.ImageCarouselBlock != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, block.ImageCarouselBlock, isCreate);
                this.SetAuditFieldsOnChildren(userId, block.ImageCarouselBlock, isCreate);
            }
        }

        private void SetAuditFieldsOnChildren(int userId, MediaBlock mediaBlock, bool isCreate)
        {
            if (mediaBlock.Attachment != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, mediaBlock.Attachment, isCreate);
            }
            else if (mediaBlock.Image != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, mediaBlock.Image, isCreate);
            }
            else if (mediaBlock.Video != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, mediaBlock.Video, isCreate);
            }
        }

        private void SetAuditFieldsOnChildren(int userId, WholeSlideImageBlock wholeSlideImageBlock, bool isCreate)
        {
            foreach (WholeSlideImageBlockItem wholeSlideImageBlockItem in wholeSlideImageBlock.WholeSlideImageBlockItems)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, wholeSlideImageBlockItem, isCreate);
                this.SetAuditFieldsOnChildren(userId, wholeSlideImageBlockItem, isCreate);
            }
        }

        private void SetAuditFieldsOnChildren(int userId, WholeSlideImageBlockItem wholeSlideImageBlockItem, bool isCreate)
        {
            if (wholeSlideImageBlockItem.WholeSlideImage != null)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, wholeSlideImageBlockItem.WholeSlideImage, isCreate);
            }
        }

        private void SetAuditFieldsOnChildren(int userId, QuestionBlock questionBlock, bool isCreate)
        {
            this.SetAuditFieldsForCreateOrDelete(userId, questionBlock.QuestionBlockCollection, isCreate);
            this.SetAuditFieldsOnChildren(userId, questionBlock.QuestionBlockCollection, isCreate);

            this.SetAuditFieldsForCreateOrDelete(userId, questionBlock.FeedbackBlockCollection, isCreate);
            this.SetAuditFieldsOnChildren(userId, questionBlock.FeedbackBlockCollection, isCreate);

            foreach (QuestionAnswer answer in questionBlock.Answers)
            {
                this.SetAuditFieldsForCreateOrDelete(userId, answer, isCreate);
                if (answer.BlockCollection != null)
                {
                    this.SetAuditFieldsForCreateOrDelete(userId, answer.BlockCollection, isCreate);
                    this.SetAuditFieldsOnChildren(userId, answer.BlockCollection, isCreate);
                }
            }
        }

        private void SetAuditFieldsOnChildren(int userId, ImageCarouselBlock imageCarouselBlock, bool isCreate)
        {
            // All blocks in an image carousel MUST be image blocks.
            var imageCarouselBlockIsValid = imageCarouselBlock.ImageBlockCollection.Blocks.All(block => block.MediaBlock.MediaType == MediaType.Image);
            if (!imageCarouselBlockIsValid)
            {
                throw new Exception("Image Carousels can only contain image blocks.");
            }

            this.SetAuditFieldsForCreateOrDelete(userId, imageCarouselBlock.ImageBlockCollection, isCreate);
            this.SetAuditFieldsOnChildren(userId, imageCarouselBlock.ImageBlockCollection, isCreate);
        }
    }
}
