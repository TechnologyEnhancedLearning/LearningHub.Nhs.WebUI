namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The assessment resource version repository.
    /// </summary>
    public class AssessmentResourceVersionRepository : GenericRepository<AssessmentResourceVersion>, IAssessmentResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public AssessmentResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by Resource Version Id async.
        /// </summary>
        /// <param name="resourceVersionId">The Resource Version Id.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<AssessmentResourceVersion> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return await GetAll().FirstOrDefaultAsync(arv => arv.ResourceVersionId == resourceVersionId);
        }

        /// <summary>
        /// Get the assessment content by Block Collection Id async.
        /// </summary>
        /// <param name="assessmentContentId">The Block Collection Id.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<AssessmentResourceVersion> GetByAssessmentContentBlockCollectionIdAsync(int assessmentContentId)
        {
            return await GetAll().FirstOrDefaultAsync(arv => arv.AssessmentContentId == assessmentContentId);
        }
    }
}