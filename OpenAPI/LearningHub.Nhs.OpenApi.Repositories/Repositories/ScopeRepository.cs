namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The scope repository.
    /// </summary>
    public class ScopeRepository : GenericRepository<Scope>, IScopeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ScopeRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Scope> GetByIdAsync(int id)
        {
            return await DbContext.Scope.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// The get by catalogueNodeId async.
        /// </summary>
        /// <param name="catalogueNodeId">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Scope> GetByCatalogueNodeIdAsync(int? catalogueNodeId)
        {
            return await DbContext.Scope.AsNoTracking().FirstOrDefaultAsync(n => n.ScopeType == Nhs.Models.Enums.ScopeTypeEnum.Catalogue
                                                                                      && (catalogueNodeId.HasValue && n.CatalogueNodeId.Value == catalogueNodeId
                                                                                         || !catalogueNodeId.HasValue));
        }
    }
}