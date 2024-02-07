// <copyright file="IResourceSyncService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The IResourceSyncService.
    /// </summary>
    public interface IResourceSyncService
    {
        /// <summary>
        /// The BuildSearchResourceRequestModel.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The task.</returns>
        Task<SearchResourceRequestModel> BuildSearchResourceRequestModel(int resourceVersionId);

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
        /// The GetSyncListResourcesForUser.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The list of resources.</returns>
        List<ResourceAdminSearchResultViewModel> GetSyncListResourcesForUser(int userId);

        /// <summary>
        /// The GetSyncListForUser.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="includeResources">If the resource property should be populated.</param>
        /// <returns>The resource syncs.</returns>
        List<ResourceSync> GetSyncListForUser(int userId, bool includeResources);

        /// <summary>
        /// The SyncForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The task.</returns>
        Task<LearningHubValidationResult> SyncForUserAsync(int userId);

        /// <summary>
        /// The sync single async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The task.</returns>
        Task<LearningHubValidationResult> SyncSingleAsync(int resourceVersionId);
    }
}
