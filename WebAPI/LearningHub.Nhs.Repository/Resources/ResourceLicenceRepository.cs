namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource licence repository.
    /// </summary>
    public class ResourceLicenceRepository : GenericRepository<ResourceLicence>, IResourceLicenceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLicenceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceLicenceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get all resource licences async.
        /// </summary>
        /// <param name="includeDeleted">Indicate if deleted records are included.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<ResourceLicence> GetAll(bool includeDeleted = false)
        {
            return base.GetAll()
                .Where(l => includeDeleted || !l.Deleted)
                .AsNoTracking();
        }
    }
}
