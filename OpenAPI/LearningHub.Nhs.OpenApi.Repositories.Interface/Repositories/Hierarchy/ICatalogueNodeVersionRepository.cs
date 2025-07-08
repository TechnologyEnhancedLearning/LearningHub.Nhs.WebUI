namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Hierarchy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The ICatalogueNodeVersionRepository.
    /// </summary>
    public interface ICatalogueNodeVersionRepository : IGenericRepository<CatalogueNodeVersion>
    {
        /// <summary>
        /// The get catalogues.
        /// </summary>
        /// <param name="catalogueIds">The catalogue ids.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<List<CatalogueNodeVersion>> GetCatalogues(List<int> catalogueIds);

        /// <summary>
        /// The get catalogues.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<CatalogueNodeVersion>> GetPublishedCatalogues();

        /// <summary>
        /// The get catalogues for user.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        IQueryable<CatalogueNodeVersion> GetPublishedCataloguesForUserAsync(int userId);

        /// <summary>
        /// The get basic catalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        CatalogueNodeVersion GetBasicCatalogue(int catalogueNodeId);

        /// <summary>
        /// The UpdateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="vm">The catalogue view model.</param>
        /// <returns>The task.</returns>
        Task UpdateCatalogueAsync(int userId, CatalogueViewModel vm);

        /// <summary>
        /// The UpdateCatalogueOwnerAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="vm">The catalogue owner view model.</param>
        /// <returns>The task.</returns>
        Task UpdateCatalogueOwnerAsync(int userId, CatalogueOwnerViewModel vm);

        /// <summary>
        /// The CreateCatalogueAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <param name="vm">The catalogue view model.</param>
        /// <returns>The catalogueNodeVersionId.</returns>
        Task<int> CreateCatalogueAsync(int userId, CatalogueViewModel vm);

        /// <summary>
        /// Get Catlogue by reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>The CatalogueViewModel.</returns>
        Task<CatalogueViewModel> GetCatalogueAsync(string reference);

        /// <summary>
        /// Get list of Restricted Catalogue AccessRequests for the supplied request.
        /// </summary>
        /// <param name="restrictedCatalogueAccessRequestsRequestViewModel">The restrictedCatalogueAccessRequestsRequestViewModel.</param>
        /// <returns>A RestrictedCatalogueAccessRequestsViewModel.</returns>
        List<RestrictedCatalogueAccessRequestViewModel> GetRestrictedCatalogueAccessRequests(RestrictedCatalogueAccessRequestsRequestViewModel restrictedCatalogueAccessRequestsRequestViewModel);

        /// <summary>
        /// Get list of RestrictedCatalogueUsersRequestViewModel for the supplied request.
        /// </summary>
        /// <param name="restrictedCatalogueUsersRequestViewModel">The restrictedCatalogueUsersRequestViewModel.</param>
        /// <returns>A RestrictedCatalogueUsersViewModel.</returns>
        RestrictedCatalogueUsersViewModel GetRestrictedCatalogueUsers(RestrictedCatalogueUsersRequestViewModel restrictedCatalogueUsersRequestViewModel);

        /// <summary>
        /// The ShowCatalogue.
        /// </summary>
        /// <param name="userId">The UserId.</param>
        /// <param name="nodeId">The NodeId.</param>
        /// <returns>The Task.</returns>
        Task ShowCatalogue(int userId, int nodeId);

        /// <summary>
        /// Get Restricted Catalogue Summary for the supplied catalogue node id.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId.</param>
        /// <returns>A RestrictedCatalogueUsersViewModel.</returns>
        RestrictedCatalogueSummaryViewModel GetRestrictedCatalogueSummary(int catalogueNodeId);

        /// <summary>
        /// Gets catalogues for dashboard based on type.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>Catalogue totals and records.</returns>
        (int TotalCount, List<DashboardCatalogueDto> Catalogues) GetCatalogues(string dashboardType, int pageNumber, int userId);

        /// <summary>
        /// Check if a Catalogue with a specific name exists or not.
        /// </summary>
        /// <param name="name">The catalogue name.</param>
        /// <returns>True if the catalogue exists, otherwise false.</returns>
        Task<bool> ExistsAsync(string name);

        /// <summary>
        /// Gets the Node Id for a particular catalogue name.
        /// </summary>
        /// <param name="catalogueName">The catalogue name.</param>
        /// <returns>The catalogue's node id.</returns>
        Task<int> GetNodeIdByCatalogueName(string catalogueName);

        /// <summary>
        /// Gets the catalogues count in alphabet list.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogues alphabet count list.</returns>
        List<AllCatalogueAlphabetModel> GetAllCataloguesAlphaCount(int userId);

        /// <summary>
        /// Gets catalogues based on filter character.
        /// </summary>
        /// <param name="pageSize">The pageSize.</param>
        /// <param name="filterChar">The filterChar.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogues.</returns>
        Task<List<AllCatalogueViewModel>> GetAllCataloguesAsync(int pageSize, string filterChar, int userId);

        /// <summary>
        /// Gets catalogues based on filter character.
        /// </summary>
        /// <param name="filterChar">The filterChar.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>The catalogues.</returns>
        Task<List<AllCatalogueViewModel>> GetAllCataloguesAsync(string filterChar, int userId);
    }
}
