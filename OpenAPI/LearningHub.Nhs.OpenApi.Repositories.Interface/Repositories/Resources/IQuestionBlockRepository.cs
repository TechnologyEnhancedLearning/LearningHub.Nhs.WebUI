namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;

    /// <summary>
    /// The QuestionBlockRepository interface.
    /// </summary>
    public interface IQuestionBlockRepository : IGenericRepository<QuestionBlock>
    {
        /// <summary>
        /// The get by Block Collection Id async.
        /// </summary>
        /// <param name="blockCollectionId">The block collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<QuestionBlock> GetByQuestionBlockCollectionIdAsync(int blockCollectionId);
    }
}