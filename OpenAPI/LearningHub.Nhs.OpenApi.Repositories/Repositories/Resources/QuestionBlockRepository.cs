namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The question answer repository.
    /// </summary>
    public class QuestionBlockRepository : GenericRepository<QuestionBlock>, IQuestionBlockRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionBlockRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public QuestionBlockRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by Block Collection Id async.
        /// </summary>
        /// <param name="blockCollectionId">The block collection Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<QuestionBlock> GetByQuestionBlockCollectionIdAsync(int blockCollectionId)
        {
            return await GetAll().Include(qb => qb.Block)
                .ThenInclude(b => b.BlockCollection)
                .ThenInclude(bc => bc.CaseResourceVersion)
                .FirstOrDefaultAsync(qb =>
                    qb.QuestionBlockCollectionId == blockCollectionId ||
                    qb.FeedbackBlockCollectionId == blockCollectionId);
        }
    }
}