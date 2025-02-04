namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Search operations.
    /// </summary>
    [Route("Search")]
    [ApiController]
    public class SearchController : OpenApiControllerBase
    {
        private readonly ISearchService searchService;
        private readonly ICatalogueService catalogueService;
        private readonly IBookmarkRepository bookmarkRepository;
        private readonly IResourceService resourceService;
        private readonly IProviderService providerService;
        private readonly FindwiseConfig findwiseConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="searchService">The search service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="catalogueService">The catalogue service.</param>
        /// <param name="resourceService">The resource service.</param>
        /// <param name="providerService">The provider service.</param>
        /// <param name="findwiseConfig">The findwise config.</param>
        /// <param name="bookmarkRepository">The bookmarkRepository.</param>
        public SearchController(
                IUserService userService,
                ISearchService searchService,
                ILogger<SearchController> logger,
                ICatalogueService catalogueService,
                IResourceService resourceService,
                IProviderService providerService,
                IOptions<FindwiseConfig> findwiseConfig,
                IBookmarkRepository bookmarkRepository)
        {
            this.searchService = searchService;
            this.catalogueService = catalogueService;
            this.resourceService = resourceService;
            this.providerService = providerService;
            this.bookmarkRepository = bookmarkRepository;
            this.findwiseConfig = findwiseConfig.Value;
        }

        /// <summary>
        /// Get AllCatalogue search result.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">The catalogue search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("GetAllCatalogueSearchResult")]
        public async Task<IActionResult> GetAllCatalogueSearchResult(AllCatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            var vm = await this.GetAllCatalogueResults(catalogueSearchRequestModel);
            return this.Ok(vm);
        }

        /// <summary>
        /// Get All catalogue search results.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">The catalog search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<SearchAllCatalogueViewModel> GetAllCatalogueResults(AllCatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            var results = await this.searchService.GetAllCatalogueSearchResultsAsync(catalogueSearchRequestModel);

            var documents = results.DocumentList.Documents.ToList();
            var documentIds = documents.Select(x => int.Parse(x.Id)).ToList();
            var catalogues = this.catalogueService.GetCataloguesByNodeId(documentIds);
            var bookmarks = this.bookmarkRepository.GetAll().Where(b => documentIds.Contains(b.NodeId ?? -1) && b.UserId == this.CurrentUserId.GetValueOrDefault());
            var allProviders = await this.providerService.GetAllAsync();

            foreach (var document in documents)
            {
                var catalogue = catalogues.SingleOrDefault(x => x.NodeId == int.Parse(document.Id));
                if (catalogue == null)
                {
                    continue;
                }

                // catalogue.No
                document.Url = catalogue.Url;
                document.BannerUrl = catalogue.BannerUrl;
                document.BadgeUrl = catalogue.BadgeUrl;
                document.CardImageUrl = catalogue.CardImageUrl;
                document.NodePathId = catalogue.NodePathId;

                if (catalogue.RestrictedAccess)
                {
                    var roleUserGroups = await this.catalogueService.GetRoleUserGroupsForCatalogueSearch(catalogue.NodeId, this.CurrentUserId.GetValueOrDefault());
                    document.RestrictedAccess = catalogue.RestrictedAccess;
                    document.HasAccess = roleUserGroups.Any(x => x.UserGroup.UserUserGroup.Any(y => y.UserId == this.CurrentUserId)
                       && (x.RoleId == (int)RoleEnum.Editor || x.RoleId == (int)RoleEnum.LocalAdmin || x.RoleId == (int)RoleEnum.Reader));
                }

                var bookmark = bookmarks.FirstOrDefault(x => x.NodeId == int.Parse(document.Id));
                if (bookmark != null)
                {
                    document.BookmarkId = bookmark?.Id;
                    document.IsBookmarked = !bookmark?.Deleted ?? false;
                }

                if (document.ProviderIds?.Count > 0)
                {
                    document.Providers = allProviders.Where(n => document.ProviderIds.Contains(n.Id)).ToList();
                }
            }

            var searchViewModel = new SearchAllCatalogueViewModel
            {
                DocumentModel = documents,
                SearchString = catalogueSearchRequestModel.SearchText,
                Hits = results.DocumentList.Documents.Count(),
                DescriptionMaximumLength = this.findwiseConfig.MaximumDescriptionLength,
                ErrorOnAPI = results.ErrorsOnAPICall,
                Facets = results.Facets,
            };

            if (results.Stats != null)
            {
                searchViewModel.TotalHits = results.Stats.TotalHits;
            }

            searchViewModel.SearchId = catalogueSearchRequestModel.SearchId > 0 ? catalogueSearchRequestModel.SearchId : results.SearchId;

            return searchViewModel;
        }
    }
}