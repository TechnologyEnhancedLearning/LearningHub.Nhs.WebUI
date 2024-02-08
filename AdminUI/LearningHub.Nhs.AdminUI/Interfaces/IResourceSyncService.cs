namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Resource;

    /// <summary>
    /// Defines the <see cref="IResourceSyncService" />.
    /// </summary>
    public interface IResourceSyncService
    {
        /// <summary>
        /// The AddToSyncListAsync.
        /// </summary>
        /// <param name="resourceIds">The resourceIds<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task AddToSyncListAsync(List<int> resourceIds);

        /// <summary>
        /// The GetResourceSyncs.
        /// </summary>
        /// <returns>The <see cref="List{ResourceAdminSearchResultViewModel}"/>.</returns>
        Task<List<ResourceAdminSearchResultViewModel>> GetResourceSyncs();

        /// <summary>
        /// The RemoveFromSyncListAsync.
        /// </summary>
        /// <param name="resourceIds">The resourceIds<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task RemoveFromSyncListAsync(List<int> resourceIds);

        /// <summary>
        /// The SyncSingle.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ApiResponse}"/>.</returns>
        Task<ApiResponse> SyncSingle(int resourceId);

        /// <summary>
        /// The SyncWithFindwise.
        /// </summary>
        /// <returns>The <see cref="Task{ApiResponse}"/>.</returns>
        Task<ApiResponse> SyncWithFindwise();
    }
}
