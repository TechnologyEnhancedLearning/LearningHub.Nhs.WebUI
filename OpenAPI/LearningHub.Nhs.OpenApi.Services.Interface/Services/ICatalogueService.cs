namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// The CatalogueService interface.
    /// </summary>
    public interface ICatalogueService
    {
        /// <summary>
        /// get all catalogues async.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        public Task<BulkCatalogueViewModel> GetAllCatalogues();

        /// <summary>
        /// The GetCatalogue.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The catalogue view model.</returns>
        Task<Nhs.Models.Catalogue.CatalogueViewModel> GetCatalogueAsync(int id);

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

    }
}