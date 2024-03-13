namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// The ResourceRepository interface.
    /// </summary>
    public interface IResourceRepository : IGenericRepository<Resource>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Resource> GetByIdAsync(int id);

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
        /// Returns true if the user has any resources published.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>If the user has any resources published.</returns>
        Task<bool> UserHasPublishedResourcesAsync(int userId);

        /// <summary>
        /// The transfer resource ownership.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <param name="newOwnerUsername">The new owner username.</param>
        /// <param name="userId">The user id.</param>
        void TransferResourceOwnership(
            int resourceId,
            string newOwnerUsername,
            int userId);

        /// <summary>
        /// Returns a bool to indicate if the resourceVersionId corresponds to a current version of a resource.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> IsCurrentVersionAsync(int resourceVersionId);
    }
}
