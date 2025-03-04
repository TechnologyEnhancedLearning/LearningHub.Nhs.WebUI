namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ResourceReferenceRepository interface.
    /// </summary>
    public interface IResourceReferenceRepository : IGenericRepository<ResourceReference>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeNodePath">The include NodePath.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceReference> GetByIdAsync(int id, bool includeNodePath);

        /// <summary>
        /// The get by original resource reference id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeNodePath">The include NodePath.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceReference> GetByOriginalResourceReferenceIdAsync(int id, bool includeNodePath);

        /// <summary>
        /// The get default by resource id async.
        /// </summary>
        /// <param name="resourceId">The resource id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ResourceReference> GetDefaultByResourceIdAsync(int resourceId);
    }
}
