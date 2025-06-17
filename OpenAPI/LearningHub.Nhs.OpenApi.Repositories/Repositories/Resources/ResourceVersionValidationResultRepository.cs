namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Repositories.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version validation result repository.
    /// </summary>
    public class ResourceVersionValidationResultRepository : GenericRepository<ResourceVersionValidationResult>, IResourceVersionValidationResultRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionValidationResultRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionValidationResultRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The create async override, persists child collection.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionValidationResult">The resource version validation result.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task<int> CreateAsync(int userId, ResourceVersionValidationResult resourceVersionValidationResult)
        {
            foreach (var rule in resourceVersionValidationResult.ResourceVersionValidationRuleResults)
            {
                SetAuditFieldsForCreate(userId, rule);
            }

            try
            {
                return await base.CreateAsync(userId, resourceVersionValidationResult);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersionValidationResult> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return await this.DbContext.ResourceVersionValidationResult.AsNoTracking()
                .Include(r => r.ResourceVersion).AsNoTracking()
                .Include(r => r.ResourceVersionValidationRuleResults).AsNoTracking()
                .Include(r => r.CreateUser).AsNoTracking()
                .Where(r => r.ResourceVersionId == resourceVersionId && r.ResourceVersion.VersionStatusEnum == LearningHub.Nhs.Models.Enums.VersionStatusEnum.FailedToPublish)
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();
        }
    }
}
