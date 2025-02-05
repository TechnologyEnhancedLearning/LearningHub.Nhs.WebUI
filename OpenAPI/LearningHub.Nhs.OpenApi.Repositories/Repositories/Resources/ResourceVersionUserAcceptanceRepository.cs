namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;

    /// <summary>
    /// The resource version user acceptance repository.
    /// </summary>
    public class ResourceVersionUserAcceptanceRepository : GenericRepository<ResourceVersionUserAcceptance>, IResourceVersionUserAcceptanceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionUserAcceptanceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionUserAcceptanceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
