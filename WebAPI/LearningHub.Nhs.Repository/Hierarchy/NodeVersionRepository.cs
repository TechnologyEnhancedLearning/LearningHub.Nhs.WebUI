namespace LearningHub.Nhs.Repository.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;

    /// <summary>
    /// The NodeVersionRepository.
    /// </summary>
    public class NodeVersionRepository : GenericRepository<NodeVersion>, INodeVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NodeVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
