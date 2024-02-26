namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;

    /// <summary>
    /// The BlockRepository interface.
    /// </summary>
    public interface IBlockCollectionRepository : IGenericRepository<BlockCollection>
    {
        /// <summary>
        /// Delete the Block Collection.
        /// </summary>
        /// <param name="userId">The User Id.</param>
        /// <param name="blockCollectionId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteBlockCollection(int userId, int blockCollectionId);

        /// <summary>
        /// Gets the Block Collection.
        /// </summary>
        /// <param name="blockCollectionId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<BlockCollection> GetBlockCollection(int? blockCollectionId);

        /// <summary>
        /// Gets the Question blocks for a particular blockCollectionId.
        /// </summary>
        /// <param name="blockCollectionId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<Block>> GetQuestionBlocks(int blockCollectionId);
    }
}
