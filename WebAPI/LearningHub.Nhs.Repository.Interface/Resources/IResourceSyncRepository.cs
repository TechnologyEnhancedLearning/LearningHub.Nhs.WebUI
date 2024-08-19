namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The IResourceSyncRepository.
    /// </summary>
    public interface IResourceSyncRepository
    {
        /// <summary>
        /// The GetSyncListForUser.
        /// </summary>
        /// <param name="userId">The userid.</param>
        /// <param name="includeResources">If the resource property should be populated.</param>
        /// <returns>The sync list for the user.</returns>
        IQueryable<ResourceSync> GetSyncListForUser(int userId, bool includeResources);

        /// <summary>
        /// The AddToSyncListAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        Task AddToSyncListAsync(int userId, List<int> resourceIds);

        /// <summary>
        /// The RemoveFromSyncListAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="resourceIds">The resourceIds.</param>
        /// <returns>The task.</returns>
        Task RemoveFromSyncListAsync(int userId, List<int> resourceIds);

        /// <summary>
        /// The SetSyncedForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The task.</returns>
        Task SetSyncedForUserAsync(int userId);
    }
}
