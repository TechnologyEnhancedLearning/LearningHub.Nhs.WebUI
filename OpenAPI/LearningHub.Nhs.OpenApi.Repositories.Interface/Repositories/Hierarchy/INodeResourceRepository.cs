namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Hierarchy;

    /// <summary>
    /// The NodeResourceRepository interface.
    /// </summary>
    public interface INodeResourceRepository : IGenericRepository<NodeResource>
    {
        /// <summary>
        /// The get by resource id async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<NodeResource>> GetByResourceIdAsync(int resourceId);

        /// <summary>
        /// The get catalogue locations for resource.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The CatalogueLocationViewModel list.</returns>
        List<CatalogueLocationViewModel> GetCatalogueLocationsForResource(int resourceId);

        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="nodeId">nodeId.</param>
        /// <param name="catalogueOrder">catalogueOrder.</param>
        /// <param name="offset">offset.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueResourceResponseViewModel> GetResourcesAsync(int nodeId, CatalogueOrder catalogueOrder, int offset);

        /// <summary>
        /// Get All published resources id.
        /// </summary>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        Task<IEnumerable<int>> GetAllPublishedResourceAsync();

        /// <summary>
        /// Creates or updates the NodeResource record for a draft resource in a node.
        /// </summary>
        /// <param name="nodeId">The nodeId.</param>
        /// <param name="resourceId">The resourceId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="T:Task{IEnumerable{int}}"/>.</returns>
        Task<int> CreateOrUpdateAsync(int nodeId, int resourceId, int userId);
    }
}
