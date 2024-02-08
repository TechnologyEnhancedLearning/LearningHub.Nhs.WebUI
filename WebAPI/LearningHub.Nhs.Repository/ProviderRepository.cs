namespace LearningHub.Nhs.Repository
{
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The provider repository.
    /// </summary>
    public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ProviderRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <inheritdoc />
        public async Task<Provider> GetByIdAsync(int id)
        {
            return await this.DbContext.Provider.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
        }

        /// <inheritdoc />
        public async Task<Provider> GetByIdAsync(int id, bool includeChildren)
        {
            if (includeChildren)
            {
                return await this.DbContext.Provider
                    .Where(r => !r.Deleted)
                    .Include(r => r.UserProvider)
                    .ThenInclude(pr => pr.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id);
            }
            else
            {
                return await this.DbContext.Provider.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
            }
        }

        /// <summary>
        /// The get by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<Provider> GetProvidersByUserIdAsync(int userId)
        {
            return this.DbContext.Set<UserProvider>()
             .Include(up => up.Provider)
                .Where(up => up.UserId == userId && !up.Deleted).AsNoTracking()
                .Select(up => up.Provider);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<Provider> GetProvidersByResourceIdAsync(int resourceVersionId)
        {
            return this.DbContext.Set<ResourceVersionProvider>()
             .Include(up => up.Provider)
                .Where(up => up.ResourceVersionId == resourceVersionId && !up.Deleted).AsNoTracking()
                .Select(up => up.Provider);
        }

        /// <summary>
        /// The get by node version id async.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<Provider> GetProvidersByCatalogueIdAsync(int nodeVersionId)
        {
            return this.DbContext.Set<CatalogueNodeVersionProvider>()
             .Include(up => up.Provider)
                .Where(up => up.CatalogueNodeVersionId == nodeVersionId && !up.Deleted).AsNoTracking()
                .Select(up => up.Provider);
        }
    }
}
