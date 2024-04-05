namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The case resource version repository.
    /// </summary>
    public class CaseResourceVersionRepository : GenericRepository<CaseResourceVersion>, ICaseResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaseResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public CaseResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by Resource Version Id async.
        /// </summary>
        /// <param name="resourceVersionId">The Resource Version Id.</param>
        /// <returns>The <see cref="Task{TResult}"/>.</returns>
        public async Task<CaseResourceVersion> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return await this.GetAll().FirstOrDefaultAsync(crv => crv.ResourceVersionId == resourceVersionId);
        }
    }
}
