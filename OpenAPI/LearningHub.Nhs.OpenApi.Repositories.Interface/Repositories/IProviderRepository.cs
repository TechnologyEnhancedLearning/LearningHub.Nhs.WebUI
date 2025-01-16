namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ProviderRepository interface.
    /// </summary>
    public interface IProviderRepository : IGenericRepository<Provider>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Provider> GetByIdAsync(int id);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeChildren">The include children.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Provider> GetByIdAsync(int id, bool includeChildren);

        /// <summary>
        /// The get by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        IQueryable<Provider> GetProvidersByUserIdAsync(int userId);

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        IQueryable<Provider> GetProvidersByResourceIdAsync(int resourceVersionId);

        /// <summary>
        /// The get by node version id async.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        IQueryable<Provider> GetProvidersByCatalogueIdAsync(int nodeVersionId);
    }
}
