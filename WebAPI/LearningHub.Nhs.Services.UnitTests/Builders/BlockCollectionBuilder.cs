namespace LearningHub.Nhs.Services.UnitTests.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Resource.Blocks;

    /// <summary>
    /// A Helper class to easily construct TextBlockObjects.
    /// </summary>
    public class BlockCollectionBuilder
    {
        /// <summary>
        /// The blocks in the block collection.
        /// </summary>
        private List<BlockViewModel> blocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCollectionBuilder"/> class.
        /// </summary>
        public BlockCollectionBuilder()
        {
            this.blocks = (new BlockViewModel[] { new BlockBuilder().Build() }).ToList();
        }

        /// <summary>
        /// Adds a block to the end of the block collection.
        /// </summary>
        /// <param name="block"> The block to be set.</param>
        /// <returns>The current instance of the block collection builder.</returns>
        public BlockCollectionBuilder AddBlock(BlockViewModel block)
        {
            block.Order = this.blocks.Count;
            this.blocks.Add(block);
            return this;
        }

        /// <summary>
        /// Removes all the blocks.
        /// </summary>
        /// <returns>The current instance of the block collection builder.</returns>
        public BlockCollectionBuilder RemoveAllBlocks()
        {
            this.blocks = new List<BlockViewModel>();
            return this;
        }

        /// <summary>
        /// Builds the block collection.
        /// </summary>
        /// <returns>The created block collection.</returns>
        public BlockCollectionViewModel Build()
        {
            return new BlockCollectionViewModel
            {
                Blocks = this.blocks,
            };
        }
    }
}