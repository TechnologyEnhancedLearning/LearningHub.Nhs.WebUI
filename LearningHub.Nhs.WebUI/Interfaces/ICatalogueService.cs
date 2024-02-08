namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// Defines the <see cref="ICatalogueService" />.
    /// </summary>
    public interface ICatalogueService
    {
        /// <summary>
        /// GetCatalogue.
        /// </summary>
        /// <param name="reference">reference.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueViewModel> GetCatalogueAsync(string reference);

        /// <summary>
        /// GetCatalogue.
        /// </summary>
        /// <param name="catalogueNodeVersionId">The catalogueNodeVersionId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueViewModel> GetCatalogueAsync(int catalogueNodeVersionId);

        /// <summary>
        /// GetCatalogueRecorded.
        /// </summary>
        /// <param name="reference">reference.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueViewModel> GetCatalogueRecordedAsync(string reference);

        /// <summary>
        /// GetCataloguesForUser.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<CatalogueBasicViewModel>> GetCataloguesForUserAsync();

        /// <summary>
        /// GetResourcesAsync.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueResourceResponseViewModel> GetResourcesAsync(CatalogueResourceRequestViewModel requestViewModel);

        /// <summary>
        /// Can the current user edit the catalogue.
        /// </summary>
        /// <param name="catalogueId">The catalogue id.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        Task<bool> CanCurrentUserEditCatalogue(int catalogueId);

        /// <summary>
        /// Gets the access details for the catalogue.
        /// </summary>
        /// <param name="reference">The catalogue reference.</param>
        /// <returns>The access details.</returns>
        Task<CatalogueAccessDetailsViewModel> AccessDetailsAsync(string reference);

        /// <summary>
        /// Gets the latest catalogue access request for the catalogue, for the logged in user.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogue node id.</param>
        /// <returns>The latest catalogue access request details.</returns>
        Task<CatalogueAccessRequestViewModel> GetLatestCatalogueAccessRequestAsync(int catalogueNodeId);

        /// <summary>
        /// The RequestAccessAsync.
        /// </summary>
        /// <param name="reference">The catalogue reference.</param>
        /// <param name="vm">The view model.</param>
        /// <returns>The task.</returns>
        Task<LearningHubValidationResult> RequestAccessAsync(string reference, CatalogueAccessRequestViewModel vm);

        /// <summary>
        /// The InviteUserAsync.
        /// </summary>
        /// <param name="vm">The view model.</param>
        /// <returns>The task.</returns>
        Task<LearningHubValidationResult> InviteUserAsync(RestrictedCatalogueInviteUserViewModel vm);

        /// <summary>
        /// Gets the access requests for the supplied request model.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<RestrictedCatalogueAccessRequestViewModel>> GetRestrictedCatalogueAccessRequestsAsync(RestrictedCatalogueAccessRequestsRequestViewModel requestModel);

        /// <summary>
        /// GetRestrictedCatalogueSummary.
        /// </summary>
        /// <param name="catalogueNodeId">catalogueNodeId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<RestrictedCatalogueSummaryViewModel> GetRestrictedCatalogueSummary(int catalogueNodeId);

        /// <summary>
        /// Gets the activity records for the detailed activity tab of My Learning screen.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<RestrictedCatalogueUsersViewModel> GetRestrictedCatalogueUsersAsync(RestrictedCatalogueUsersRequestViewModel requestModel);

        /// <summary>
        /// The RejectAccessRequestAsync.
        /// </summary>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="rejectionReason">The rejectionReason.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> RejectAccessRequestAsync(int accessRequestId, string rejectionReason);

        /// <summary>
        /// The AcceptAccessRequestAsync.
        /// </summary>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="accessRequestUserId">The accessRequestUserId.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> AcceptAccessRequestAsync(int accessRequestId, int accessRequestUserId);

        /// <summary>
        /// The RejectAccessRequestAsync.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> DismissAccessRequestAsync(int catalogueNodeId);

        /// <summary>
        /// The GetCatalogueAccessRequestAsync.
        /// </summary>
        /// <param name="catalogueAccessRequestId">The catalogueAccessRequestId.</param>
        /// <returns>The catalogue access request.</returns>
        Task<CatalogueAccessRequestViewModel> GetCatalogueAccessRequestAsync(int catalogueAccessRequestId);

        /// <summary>
        /// The RemoveUserFromRestrictedAccess.
        /// </summary>
        /// <param name="userUserGroupId">The user - user group id.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> RemoveUserFromRestrictedAccessUserGroup(int userUserGroupId);
    }
}
