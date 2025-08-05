namespace LearningHub.Nhs.AdminUI.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Helpers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Models.Catalogue;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.Provider;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="CatalogueService" />.
    /// </summary>
    public class CatalogueService : BaseService, ICatalogueService
    {
        /// <summary>
        /// Defines the _facade.
        /// </summary>
        private readonly IOpenApiFacade facade;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="openApiFacade">The openApiFacade<see cref="IOpenApiFacade"/>.</param>
        public CatalogueService(
            ILearningHubHttpClient learningHubHttpClient,
            IOpenApiHttpClient openApiHttpClient,
            IOpenApiFacade openApiFacade)
        : base(learningHubHttpClient)
        {
            this.facade = openApiFacade;
        }

        /// <summary>
        /// The CreateCatalogueAsync.
        /// </summary>
        /// <param name="catalogue">The catalogue<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<ApiResponse> CreateCatalogueAsync(CatalogueViewModel catalogue)
        {
            return await this.facade.PostAsync<ApiResponse, CatalogueViewModel>("Catalogue/Catalogues", catalogue);
        }

        /// <summary>
        /// The GetCatalogueAsync.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{CatalogueViewModel}"/>.</returns>
        public async Task<CatalogueViewModel> GetCatalogueAsync(int id)
        {
            return await this.facade.GetAsync<CatalogueViewModel>($"Catalogue/Catalogues/{id}");
        }

        /// <summary>
        /// The GetCataloguesAsync.
        /// </summary>
        /// <param name="searchTerm">The searchTerm<see cref="string"/>.</param>
        /// <returns>The <see cref="List{CatalogueViewModel}"/>.</returns>
        public async Task<List<CatalogueViewModel>> GetCataloguesAsync(string searchTerm)
        {
            return await this.facade.GetAsync<List<CatalogueViewModel>>("Catalogue/Catalogues?searchTerm=" + searchTerm);
        }

        /// <summary>
        /// The GetResourcesAsync.
        /// </summary>
        /// <param name="catalogueId">The catalogueId<see cref="int"/>.</param>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="Task{CatalogueResourcesViewModel}"/>.</returns>
        public async Task<CatalogueResourcesViewModel> GetResourcesAsync(int catalogueId, PagingRequestModel pagingRequestModel)
        {
            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
            return await this.facade.GetAsync<CatalogueResourcesViewModel>($"Catalogue/Resources/{catalogueId}"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{WebUtility.UrlEncode(filter)}");
        }

        /// <summary>
        /// The HideCatalogueAsync.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task HideCatalogueAsync(int nodeId)
        {
            await this.facade.PostAsync("Catalogue/HideCatalogue", new CatalogueBasicViewModel { NodeId = nodeId });
        }

        /// <summary>
        /// The ShowCatalogueAsync.
        /// </summary>
        /// <param name="nodeId">The nodeId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> ShowCatalogueAsync(int nodeId)
        {
            return await this.facade.PostAsync<ApiResponse, CatalogueBasicViewModel>("Catalogue/ShowCatalogue", new CatalogueBasicViewModel { NodeId = nodeId });
        }

        /// <summary>
        /// The UpdateCatalogueAsync.
        /// </summary>
        /// <param name="catalogue">The catalogue<see cref="CatalogueViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> UpdateCatalogueAsync(CatalogueViewModel catalogue)
        {
            return await this.facade.PutAsync("Catalogue/Catalogues", catalogue);
        }

        /// <summary>
        /// The UpdateCatalogueOwnerAsync.
        /// </summary>
        /// <param name="catalogueOwner">The catalogue owner<see cref="CatalogueOwnerViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ApiResponse> UpdateCatalogueOwnerAsync(CatalogueOwnerViewModel catalogueOwner)
        {
            return await this.facade.PutAsync("Catalogue/UpdateCatalogueOwner", catalogueOwner);
        }
    }
}
