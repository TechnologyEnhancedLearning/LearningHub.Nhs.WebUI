namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System;
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
    /// The category repository.
    /// </summary>
    public class CategoryRepository : GenericRepository<CatalogueNodeVersionCategory>, ICategoryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="categoryRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public CategoryRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by node version id async.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<CatalogueNodeVersionCategory> GetCategoryByCatalogueIdAsync(int nodeVersionId)
        {
            try
            {
                return await DbContext.CatalogueNodeVersionCategory.Where(n => n.CatalogueNodeVersionId == nodeVersionId && !n.Deleted).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
