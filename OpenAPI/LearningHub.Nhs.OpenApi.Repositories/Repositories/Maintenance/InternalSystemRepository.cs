namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Maintenance
{
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Maintenance;

    /// <summary>
    /// The InternalSystemRepository.
    /// </summary>
    public class InternalSystemRepository : GenericRepository<InternalSystem>, IInternalSystemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="tzOffsetManager">
        /// The Timezone offset manager.
        /// </param>
        public InternalSystemRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
