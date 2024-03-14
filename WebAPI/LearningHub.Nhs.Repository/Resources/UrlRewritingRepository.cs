namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// The UrlRewriting repository.
    /// </summary>
    public class UrlRewritingRepository : GenericRepository<UrlRewriting>, IUrlRewritingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlRewritingRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public UrlRewritingRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="fullHistoricUrl">The fullHistoricUrl.</param>
        /// <returns>bool.</returns>
        public bool Exists(string fullHistoricUrl)
        {
            return this.DbContext.UrlRewriting.Any(r => r.FullHistoricUrl == fullHistoricUrl && !r.Deleted);
        }
    }
}
