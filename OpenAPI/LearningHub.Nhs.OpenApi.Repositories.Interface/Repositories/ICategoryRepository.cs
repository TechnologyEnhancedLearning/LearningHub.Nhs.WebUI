namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The ProviderRepository interface.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// The get by node version id async.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<CatalogueNodeVersionCategory> GetCategoryByCatalogueIdAsync(int nodeVersionId);
    }
}
