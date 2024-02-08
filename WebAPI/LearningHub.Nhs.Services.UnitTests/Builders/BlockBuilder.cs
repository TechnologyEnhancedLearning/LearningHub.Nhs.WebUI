namespace LearningHub.Nhs.Services.UnitTests.Builders
{
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A Helper class to easily construct Block Objects.
    /// </summary>
    public class BlockBuilder
    {
        /// <summary>
        /// The order of the Block.
        /// </summary>
        private int order;

        /// <summary>
        /// The title of the block.
        /// </summary>
        private string title;

        /// <summary>
        /// The BlockType of the block.
        /// </summary>
        private BlockType blockType;

        /// <summary>
        /// The text block of the block.
        /// </summary>
        private TextBlockViewModel textBlock;

        /// <summary>
        /// The whole slide image block of the block.
        /// </summary>
        private WholeSlideImageBlockViewModel wholeSlideImageBlock;

        /// <summary>
        /// The media block of the block.
        /// </summary>
        private MediaBlockViewModel mediaBlock;

        /// <summary>
        /// The question block of the block.
        /// </summary>
        private QuestionBlockViewModel questionBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockBuilder"/> class.
        /// </summary>
        public BlockBuilder()
        {
            this.order = 0;
            this.title = "Block";
            this.blockType = BlockType.Text;
            this.textBlock = new TextBlockBuilder().Build();
        }

        /// <summary>
        /// Changes the order of the block.
        /// </summary>
        /// <param name="order"> The new order of the block.</param>
        /// <returns>The current instance of the block builder.</returns>
        public BlockBuilder WithOrder(int order)
        {
            this.order = order;
            return this;
        }

        /// <summary>
        /// Makes the block a page break.
        /// </summary>
        /// <returns>The current instance of the block builder.</returns>
        public BlockBuilder AsPageBreak()
        {
            this.NullifyBlock();
            this.blockType = BlockType.PageBreak;
            return this;
        }

        /// <summary>
        /// Changes the block to have a certain text block.
        /// </summary>
        /// <param name="textBlock"> The text block to be set inside the block.</param>
        /// <returns>The current instance of the block builder.</returns>
        public BlockBuilder WithTextBlock(TextBlockViewModel textBlock)
        {
            this.NullifyBlock();
            this.blockType = BlockType.Text;
            this.textBlock = textBlock;
            return this;
        }

        /// <summary>
        /// Changes the block to have a certain question block.
        /// </summary>
        /// <param name="questionBlock"> The question block to be set inside the block.</param>
        /// <returns>The current instance of the block builder.</returns>
        public BlockBuilder WithQuestionBlock(QuestionBlockViewModel questionBlock)
        {
            this.NullifyBlock();
            this.blockType = BlockType.Question;
            this.questionBlock = questionBlock;
            return this;
        }

        /// <summary>
        /// Builds the block.
        /// </summary>
        /// <returns>The created block.</returns>
        public BlockViewModel Build()
        {
            return new BlockViewModel
            {
                Order = this.order,
                Title = this.title,
                BlockType = this.blockType,
                TextBlock = this.textBlock,
                QuestionBlock = this.questionBlock,
                WholeSlideImageBlock = this.wholeSlideImageBlock,
                MediaBlock = this.mediaBlock,
            };
        }

        private void NullifyBlock()
        {
            this.textBlock = null;
            this.mediaBlock = null;
            this.wholeSlideImageBlock = null;
            this.questionBlock = null;
        }
    }
}