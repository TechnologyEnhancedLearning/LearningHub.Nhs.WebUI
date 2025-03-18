namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
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
            return await DbContext.Provider.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
        }

        /// <inheritdoc />
        public async Task<Provider> GetByIdAsync(int id, bool includeChildren)
        {
            if (includeChildren)
            {
                return await DbContext.Provider
                    .Where(r => !r.Deleted)
                    .Include(r => r.UserProvider)
                    .ThenInclude(pr => pr.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.Id == id);
            }
            else
            {
                return await DbContext.Provider.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id && !n.Deleted);
            }
        }

        /// <summary>
        /// The get by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public IQueryable<Provider> GetProvidersByUserIdAsync(int userId)
        {
            return DbContext.Set<UserProvider>()
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
            return DbContext.Set<ResourceVersionProvider>()
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
            return DbContext.Set<CatalogueNodeVersionProvider>()
             .Include(up => up.Provider)
                .Where(up => up.CatalogueNodeVersionId == nodeVersionId && !up.Deleted).AsNoTracking()
                .Select(up => up.Provider);
        }
    }
}
