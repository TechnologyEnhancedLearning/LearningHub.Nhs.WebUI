namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// Resource repository interface.
    /// </summary>
    public interface IResourceRepository
    {
        /// <summary>
        /// Gets resource activity for resourceReferenceIds and userIds.
        /// </summary>
        /// <param name="resourceReferenceIds"><see cref="resourceReferenceIds"/>.</param>
        /// <param name="userIds"></param>
        /// <returns>ResourceActivityDTO.</returns>
        Task<IEnumerable<ResourceActivityDTO>> GetResourceActivityPerResourceMajorVersion(IEnumerable<int>? resourceReferenceIds, IEnumerable<int>? userIds);
        /// <summary>
        /// Gets GetResourceReferenceAndCatalogues using stored procedure GetResourceReferenceAndCatalogues.
        /// </summary>
        /// <param name="resourceReferenceIds">.</param>
        /// <param name="originalResourceReferenceIds">.</param>
        /// <returns>ResourceActivityDTO.</returns>
        Task<IEnumerable<ResourceReferenceAndCatalogueDTO>> GetResourceReferenceAndCatalogues(IEnumerable<int> resourceReferenceIds, IEnumerable<int> originalResourceReferenceIds);
    }
}
