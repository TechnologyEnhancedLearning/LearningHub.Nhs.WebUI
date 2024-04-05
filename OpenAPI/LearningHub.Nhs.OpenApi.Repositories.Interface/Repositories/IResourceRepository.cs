namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// Resource repository interface.
    /// </summary>
    public interface IResourceRepository
    {
        /// <summary>
        /// Gets resources from ids.
        /// </summary>
        /// <param name="resourceIds"><see cref="resourceIds"/>.</param>
        /// <returns>Resources with details.</returns>
        public Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds);

        /// <summary>
        /// Gets resource references by their original resource reference Ids.
        /// </summary>
        /// <param name="originalResourceReferenceIds"><see cref="originalResourceReferenceIds"/>.</param>
        /// <returns>Resource references.</returns>
        public Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
            IEnumerable<int> originalResourceReferenceIds);
    }
}
