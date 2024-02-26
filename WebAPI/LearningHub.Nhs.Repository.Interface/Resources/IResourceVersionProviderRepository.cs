namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ResourceVersionEventRepository interface.
    /// </summary>
    public interface IResourceVersionProviderRepository : IGenericRepository<ResourceVersionProvider>
    {
        /// <summary>
        /// Delete resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="providerId">provider id.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int resourceVersionId, int providerId, int userId);

        /// <summary>
        /// Delete all resource version provider.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAllAsync(int resourceVersionId, int userId);
    }
}
