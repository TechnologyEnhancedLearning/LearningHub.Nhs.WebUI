namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using CatalogueViewModel = Nhs.Models.Catalogue.CatalogueViewModel;

    /// <summary>
    /// The CatalogueService interface.
    /// </summary>
    public interface ICatalogueService
    {
        /// <summary>
        /// The Get Basic Catalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNode.</param>
        /// <returns>The catalogues.</returns>
        CatalogueBasicViewModel GetBasicCatalogue(int catalogueNodeId);

        /// <summary>
        /// get all catalogues async.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        public Task<BulkCatalogueViewModel> GetAllCatalogues();

        /// <summary>
        /// The GetCatalogues.
        /// </summary>
        /// <param name="searchTerm">The searchTerm.</param>
        /// <returns>The catalogues.</returns>
        List<CatalogueViewModel> GetCatalogues(string searchTerm);

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The catalogue view model.</returns>
        Task<CatalogueViewModel> GetCatalogueAsync(int id);

        /// <summary>
        /// The GetResources.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="id">The catalogueId.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="sortColumn">The sortColumn.</param>
        /// <param name="sortDirection">The sortDirection.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The catalogueResourcesViewModel.</returns>
        Task<CatalogueResourcesViewModel> GetResourcesAsync(int userId, int id, int page, int pageSize, string sortColumn = "", string sortDirection = "", string filter = "");


        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="requestViewModel">requestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<CatalogueResourceResponseViewModel> GetResourcesAsync(CatalogueResourceRequestViewModel requestViewModel);

        /// <summary>
        /// AccessRequest.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueAccessRequestId">The catalogueAccessRequestId.</param>
        /// <returns>The catalogue access request.</returns>
        Task<CatalogueAccessRequestViewModel> AccessRequestAsync(int userId, int catalogueAccessRequestId);

        /// <summary>
        /// Get Restricted Catalogue Summary for the supplied catalogue node id.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>A RestrictedCatalogueSummaryViewModel.</returns>
        RestrictedCatalogueSummaryViewModel GetRestrictedCatalogueSummary(int catalogueNodeId);

        /// <summary>
        /// The RequestAccessAsync.
        /// </summary>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <param name="reference">The reference.</param>
        /// <param name="vm">The view model.</param>
        /// <param name="accessType">The accessType.</param>
        /// <returns>The bool.</returns>
        Task<bool> RequestAccessAsync(int currentUserId, string reference, CatalogueAccessRequestViewModel vm, string accessType);

        /// <summary>
        /// The InviteUserAsync.
        /// </summary>
        /// <param name="currentUserId">The currentUserId.</param>
        /// <param name="vm">The view model.</param>
        /// <returns>The bool.</returns>
        Task<bool> InviteUserAsync(int currentUserId, RestrictedCatalogueInviteUserViewModel vm);


        /// <summary>
        /// Returns true if the catalogue is editable by the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="catalogueId">The catalogue id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> CanUserEditCatalogueAsync(int userId, int catalogueId);


        /// <summary>
        /// The GetCatalogues.
        /// </summary>
        /// <param name="catalogueIds">The catalogueIds.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<CatalogueViewModel>> GetCatalogues(List<int> catalogueIds);

        /// <summary>
        /// Returns basic catalogues for user.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogues for the current user.</returns>
        List<CatalogueBasicViewModel> GetCataloguesForUser(int userId);

        /// <summary>
        /// The GetRolesForCatalogueSearch.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The current user.</param>
        /// <returns>The roleUserGroups.</returns>
        Task<List<RoleUserGroup>> GetRoleUserGroupsForCatalogueSearch(int catalogueNodeId, int userId);

        /// <summary>
        /// The get catalogues by node id async.
        /// </summary>
        /// <param name="nodeIds">The nodeIds.</param>
        /// <returns>The catalogues.</returns>
        List<CatalogueViewModel> GetCataloguesByNodeId(IEnumerable<int> nodeIds);

        /// <summary>
        /// Get Catalogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogue view model.</returns>
        Task<CatalogueViewModel> GetCatalogueAsync(string reference, int userId);

        /// <summary>
        /// The CreateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogue">The catalogue.</param>
        /// <returns>The catalogue id.</returns>
        Task<LearningHubValidationResult> CreateCatalogueAsync(int userId, CatalogueViewModel catalogue);

        /// <summary>
        /// The AddCategoryToCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userid.</param>
        /// <param name="catalogue">The catalogue.</param>
        /// <returns></returns>
        Task<LearningHubValidationResult> AddCategoryToCatalogueAsync(int userId, CatalogueViewModel catalogue);

        /// <summary>
        /// The RemoveCategoryFromCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogue">The catalogue.</param>
        /// <returns>The catalogue id.</returns>
        Task<LearningHubValidationResult> RemoveCategoryFromCatalogueAsync(int userId, CatalogueViewModel catalogue);

        /// <summary>
        /// The UpdateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogue">The catalogue.</param>
        /// <returns>The catalogue view model.</returns>
        Task<LearningHubValidationResult> UpdateCatalogueAsync(int userId, CatalogueViewModel catalogue);

        /// <summary>
        /// The ShowCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueId">The catalogueId.</param>
        /// <returns>The task.</returns>
        Task<LearningHubValidationResult> ShowCatalogueAsync(int userId, int catalogueId);

        /// <summary>
        /// The HideCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueId">The catalogueId.</param>
        /// <returns>The task.</returns>
        Task HideCatalogueAsync(int userId, int catalogueId);

        /// <summary>
        /// The AccessDetailsAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="reference">The reference.</param>
        /// <returns>The catalogue access details.</returns>
        Task<CatalogueAccessDetailsViewModel> AccessDetailsAsync(int userId, string reference);


        /// <summary>
        /// The GetCatalogueAccessRequests.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogue access requests.</returns>
        List<CatalogueAccessRequestViewModel> GetCatalogueAccessRequests(int catalogueNodeId, int userId);

        /// <summary>
        /// The GetLatestCatalogueAccessRequestAsync.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The latest catalogue access request.</returns>
        CatalogueAccessRequestViewModel GetLatestCatalogueAccessRequest(int catalogueNodeId, int userId);

        /// <summary>
        /// Gets the restricted catalogues access requests for the supplied request view model.
        /// </summary>
        /// <param name="requestModel">The request model.</param>
        /// <returns>The access requests.</returns>
        List<RestrictedCatalogueAccessRequestViewModel> GetRestrictedCatalogueAccessRequests(RestrictedCatalogueAccessRequestsRequestViewModel requestModel);

        /// <summary>
        /// The RestrictedCatalogueUsersAsync.
        /// </summary>
        /// <param name="restrictedCatalogueUsersRequestViewModel">The restrictedCatalogueUsersRequestViewModel.</param>
        /// <returns>The users.</returns>
        RestrictedCatalogueUsersViewModel GetRestrictedCatalogueUsers(RestrictedCatalogueUsersRequestViewModel restrictedCatalogueUsersRequestViewModel);

        /// <summary>
        /// The RejectAccessAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <param name="responseMessage">The responseMessage.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> RejectAccessAsync(int userId, int accessRequestId, string responseMessage);

        /// <summary>
        /// The AcceptAccessRequest.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="accessRequestId">The accessRequestId.</param>
        /// <returns>The validation result.</returns>
        Task<LearningHubValidationResult> AcceptAccessAsync(int userId, int accessRequestId);

        /// <summary>
        /// GetAllCataloguesAsync.
        /// </summary>
        /// <param name="filterChar">filterChar.</param>
        /// <param name="userId">userId.</param>
        /// <returns>The allcatalogue result based on letters.</returns>
        Task<AllCatalogueResponseViewModel> GetAllCataloguesAsync(string filterChar, int userId);

        /// <summary>
        /// The UpdateCatalogueOwnerAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="catalogueOwner">The catalogue owner.</param>
        /// <returns>The catalogue view model.</returns>
        Task<LearningHubValidationResult> UpdateCatalogueOwnerAsync(int userId, CatalogueOwnerViewModel catalogueOwner);

    }
}