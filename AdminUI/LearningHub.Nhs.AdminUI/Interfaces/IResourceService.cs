// <copyright file="IResourceService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Defines the <see cref="IResourceService" />.
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// The GetResourceAdminSearchPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{ResourceAdminSearchResultViewModel}"/>.</returns>
        Task<PagedResultSet<ResourceAdminSearchResultViewModel>> GetResourceAdminSearchPageAsync(PagingRequestModel pagingRequestModel);

        /// <summary>
        /// The GetResourceVersionEventsAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionEventViewModel}"/>.</returns>
        Task<List<ResourceVersionEventViewModel>> GetResourceVersionEventsAsync(int resourceVersionId);

        /// <summary>
        /// The GetResourceVersionValidationResultAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceVersionEventViewModel}}"/>.</returns>
        Task<ResourceVersionValidationResultViewModel> GetResourceVersionValidationResultAsync(int resourceVersionId);

        /// <summary>
        /// The GetResourceVersionExtendedViewModelAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="ResourceVersionExtendedViewModel"/>.</returns>
        Task<ResourceVersionExtendedViewModel> GetResourceVersionExtendedViewModelAsync(int resourceVersionId);

        /// <summary>
        /// The GetResourceVersionsAsync.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <returns>The <see cref="List{ResourceVersionViewModel}"/>.</returns>
        Task<List<ResourceVersionViewModel>> GetResourceVersionsAsync(int resourceId);

        /// <summary>
        /// The RevertToDraft.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> RevertToDraft(int resourceVersionId);

        /// <summary>
        /// The TransferResourceOwnership.
        /// </summary>
        /// <param name="resourceId">The resourceId<see cref="int"/>.</param>
        /// <param name="newResourceOwner">The newResourceOwner<see cref="string"/>.</param>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> TransferResourceOwnership(int resourceId, string newResourceOwner, int resourceVersionId);

        /// <summary>
        /// The UnpublishResourceVersionAsync.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId<see cref="int"/>.</param>
        /// <param name="details">The details<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        Task<LearningHubValidationResult> UnpublishResourceVersionAsync(int resourceVersionId, string details);

        /// <summary>
        /// Create resource version event.
        /// </summary>
        /// <param name="resourceVersionId">resource version id.</param>
        /// <param name="resourceVersionEventType">resource version event.</param>
        /// <param name="details">details.</param>
        /// <param name="userId">user id.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<LearningHubValidationResult> CreateResourceVersionEvent(int resourceVersionId, ResourceVersionEventTypeEnum resourceVersionEventType, string details, int userId);
    }
}
