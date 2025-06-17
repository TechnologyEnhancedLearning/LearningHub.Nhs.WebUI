namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// Resource repository interface.
    /// </summary>
    public interface IResourceRepository : IGenericRepository<Resource>
    {
        /// <summary>
        /// Gets resources from ids.
        /// </summary>
        /// <param name="resourceIds"><see cref="resourceIds"/>.</param>
        /// <returns>Resources with details.</returns>
        Task<IEnumerable<Resource>> GetResourcesFromIds(IEnumerable<int> resourceIds);

        /// <summary>
        /// Gets resource references by their original resource reference Ids.
        /// </summary>
        /// <param name="originalResourceReferenceIds"><see cref="originalResourceReferenceIds"/>.</param>
        /// <returns>Resource references.</returns>
        Task<IEnumerable<ResourceReference>> GetResourceReferencesByOriginalResourceReferenceIds(
            IEnumerable<int> originalResourceReferenceIds);

        /// <summary>
        /// Gets resource activity for resourceReferenceIds and userIds.
        /// </summary>
        /// <param name="resourceReferenceIds"><see cref="resourceReferenceIds"/>.</param>
        /// <param name="userIds"></param>
        /// <returns>ResourceActivityDTO.</returns>
        Task<IEnumerable<ResourceActivityDTO>> GetResourceActivityPerResourceMajorVersion(IEnumerable<int>? resourceReferenceIds, IEnumerable<int>? userIds);

        /// <summary>
        /// GetAchievedCertificatedResourceIds
        /// </summary>
        /// <param name="currentUserId"><see cref="currentUserId"/>.</param>
        Task<List<int>> GetAchievedCertificatedResourceIds(int currentUserId);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Resource> GetByIdAsync(int id);


        /// <summary>
        /// Returns true if the user has any resources published.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>If the user has any resources published.</returns>
        Task<bool> UserHasPublishedResourcesAsync(int userId);

        /// <summary>
        /// The create resource async.
        /// </summary>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<int> CreateResourceAsync(ResourceTypeEnum resourceType, string title, string description, int userId);

        /// <summary>
        /// The get by resourve version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Resource> GetByResourceVersionIdAsync(int resourceVersionId);

        /// <summary>
        /// Returns a bool to indicate if the resourceVersionId corresponds to a current version of a resource.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> IsCurrentVersionAsync(int resourceVersionId);

    }
}
