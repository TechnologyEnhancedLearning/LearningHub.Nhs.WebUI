namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
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

        /// <summary>
        /// Gets resource activity for resourceReferenceIds and userIds.
        /// </summary>
        /// <param name="resourceReferenceIds"><see cref="resourceReferenceIds"/>.</param>
        /// <param name="userIds"></param>
        /// <returns>ResourceActivityDTO.</returns>
        Task<IEnumerable<ResourceActivityDTO>> GetResourceActivityPerResourceMajorVersion(IEnumerable<int>? resourceReferenceIds, IEnumerable<int>? userIds);

        /// <summary>
        /// GetResourceReferencesForAssessments
        /// </summary>
        /// <param name="resourceIds"><see cref="resourceIds"/>.</param>
        /// <returns>ResourceActivityDTO.</returns>
        public Task<IEnumerable<ResourceReference>> GetResourceReferencesForAssessments(List<int> resourceIds);


    }
}
