namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchClick;
    using LearningHub.Nhs.Models.Validation;
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
        private readonly IResourceService resourceService;
        private readonly IResourceReferenceService resourceReferenceService;
        private readonly ICatalogueService catalogueService;
        private readonly IBookmarkRepository bookmarkRepository;
        private readonly IProviderService providerService;
        private readonly FindwiseConfig findwiseConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="searchService">The search service.</param>
        /// <param name="resourceService">The resource service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="catalogueService">The catalogue service.</param>
        /// <param name="providerService">The provider service.</param>
        /// <param name="findwiseConfig">The findwise config.</param>
        /// <param name="bookmarkRepository">The bookmarkRepository.</param>
        public SearchController(
                IUserService userService,
                ISearchService searchService,
                IResourceService resourceService,
                IResourceReferenceService resourceReferenceService,
                ILogger<SearchController> logger,
                ICatalogueService catalogueService,
                IProviderService providerService,
                IOptions<FindwiseConfig> findwiseConfig,
                IBookmarkRepository bookmarkRepository)
        {
            this.searchService = searchService;
            this.resourceService = resourceService;
            this.resourceReferenceService = resourceReferenceService;
            this.catalogueService = catalogueService;
            this.providerService = providerService;
            this.bookmarkRepository = bookmarkRepository;
            this.findwiseConfig = findwiseConfig.Value;
        }

        /// <summary>
        /// Get result.
        /// </summary>
        /// <param name="searchRequestModel">The search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("GetResults")]
        public async Task<IActionResult> GetResults(SearchRequestModel searchRequestModel)
        {
            var searchViewModel = new SearchViewModel();

            searchViewModel = await this.GetSearchResults(searchRequestModel, searchViewModel);
            return this.Ok(searchViewModel);
        }

        /// <summary>
        /// Get result.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">
        /// The catalog search request model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost]
        [Route("GetCatalogueResults")]
        public async Task<IActionResult> GetCatalogueResults(CatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            var vm = await this.GetCatalogueSearchResults(catalogueSearchRequestModel);
            return this.Ok(vm);
        }

        /// <summary>
        /// The submit feedback.
        /// </summary>
        /// <param name="searchFeedbackModel">The search feedback.</param>
        /// <returns>The task.</returns>
        [HttpPost]
        [Route("SubmitFeedback")]
        public async Task<IActionResult> SubmitFeedback(SearchFeedBackModel searchFeedbackModel)
        {
            var vr = await this.searchService.SubmitFeedbackAsync(searchFeedbackModel, this.CurrentUserId.GetValueOrDefault());

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create resoruce search action.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action resource model.
        /// </param>
        /// <returns>
        /// Nothing.
        /// </returns>
        [HttpPost]
        [Route("CreateResourceSearchAction")]
        public async Task<IActionResult> CreateResourceSearchAction(SearchActionResourceModel searchActionResourceModel)
        {
            var vr = await this.searchService.CreateResourceSearchActionAsync(searchActionResourceModel, this.CurrentUserId.GetValueOrDefault());
            var eventCreated = await this.searchService.SendResourceSearchEventClickAsync(searchActionResourceModel);

            if (vr.IsValid && eventCreated)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create catalogue search action.
        /// </summary>
        /// <param name="searchActionCatalogueModel">
        /// The search action catalogue model.
        /// </param>
        /// <returns>
        /// Nothing.
        /// </returns>
        [HttpPost]
        [Route("CreateCatalogueSearchAction")]
        public async Task<IActionResult> CreateCatalogueSearchAction(SearchActionCatalogueModel searchActionCatalogueModel)
        {
            var vr = await this.searchService.CreateCatalogueSearchActionAsync(searchActionCatalogueModel, this.CurrentUserId.GetValueOrDefault());
            var eventCreated = await this.searchService.SendCatalogueSearchEventAsync(searchActionCatalogueModel);

            if (vr.IsValid && eventCreated)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create search term action.
        /// </summary>
        /// <param name="searchRequestModel">The search term action request model.</param>
        /// <returns>Nothing.</returns>
        [HttpPost]
        [Route("CreateSearchTermAction")]
        public async Task<IActionResult> CreateSearchTermAction(SearchRequestModel searchRequestModel)
        {
            LearningHubValidationResult validationResult = await this.searchService.CreateSearchTermEvent(searchRequestModel, this.CurrentUserId.GetValueOrDefault());

            if (validationResult.IsValid)
            {
                return this.Ok(new ApiResponse(true, validationResult));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, validationResult));
            }
        }

        /// <summary>
        /// Create catalogue search term action.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">The catalogue search term action request model.</param>
        /// <returns>Nothing.</returns>
        [HttpPost]
        [Route("CreateCatalogSearchTermAction")]
        public async Task<IActionResult> CreateCatalogueSearchTermAction(CatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            LearningHubValidationResult validationResult = await this.searchService.CreateCatalogueSearchTermEvent(catalogueSearchRequestModel, this.CurrentUserId.GetValueOrDefault());

            if (validationResult.IsValid)
            {
                return this.Ok(new ApiResponse(true, validationResult));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, validationResult));
            }
        }

        /// <summary>
        /// Send AutoSuggestion Click action.
        /// </summary>
        /// <param name="clickPayloadModel">
        /// The click Payload model.
        /// </param>
        /// <returns>
        /// Nothing.
        /// </returns>
        [HttpPost]
        [Route("SendAutoSuggestionClickAction")]
        public async Task<IActionResult> SendAutoSuggestionClickAction(AutoSuggestionClickPayloadModel clickPayloadModel)
        {
            var eventCreated = await this.searchService.SendAutoSuggestionEventAsync(clickPayloadModel);

            if (eventCreated)
            {
                return this.Ok(new ApiResponse(true));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false));
            }
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
        /// Get AutoSuggestionResults.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetAutoSuggestionResult/{term}")]
        public async Task<IActionResult> GetAutoSuggestionResults(string term)
        {
            var autosuggestionViewModel = await this.GetAutoSuggestions(term);
            return this.Ok(autosuggestionViewModel);
        }

        /// <summary>
        /// Get search result.
        /// </summary>
        /// <param name="searchRequestModel">The search request model.</param>
        /// <param name="searchViewModel">The search view model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<SearchViewModel> GetSearchResults(SearchRequestModel searchRequestModel, SearchViewModel searchViewModel)
        {
            var results = await this.searchService.GetSearchResultAsync(searchRequestModel, this.CurrentUserId.GetValueOrDefault());
            var documents = results.DocumentList.Documents.ToList();
            var catalogueIds = results.DocumentList.Documents.Select(x => x.CatalogueIds.FirstOrDefault()).Where(x => x != 0).ToHashSet().ToList();
            var catalogues = this.catalogueService.GetCataloguesByNodeId(catalogueIds);
            var allProviders = await this.providerService.GetAllAsync();

            foreach (var document in documents)
            {
                if (document.ProviderIds?.Count > 0)
                {
                    document.Providers = allProviders.Where(n => document.ProviderIds.Contains(n.Id)).ToList();
                }

                if (document.CatalogueIds.Any(x => x == 1))
                {
                    continue;
                }

                var catalogue = catalogues.SingleOrDefault(x => x.NodeId == document.CatalogueIds.SingleOrDefault());

                if (catalogue == null)
                {
                    continue;
                }

                document.CatalogueUrl = catalogue.Url;
                document.CatalogueBadgeUrl = catalogue.BadgeUrl;
                document.CatalogueName = catalogue.Name;

                if (catalogue.RestrictedAccess)
                {
                    var roleUserGroups = await this.catalogueService.GetRoleUserGroupsForCatalogueSearch(catalogue.NodeId, this.CurrentUserId.GetValueOrDefault());
                    document.CatalogueRestrictedAccess = catalogue.RestrictedAccess;
                    document.CatalogueHasAccess = roleUserGroups.Any(x => x.UserGroup.UserUserGroup.Any(y => y.UserId == this.CurrentUserId)
                        && (x.RoleId == (int)RoleEnum.Editor || x.RoleId == (int)RoleEnum.LocalAdmin || x.RoleId == (int)RoleEnum.Reader));
                }
            }

            searchViewModel.DocumentModel = results.DocumentList.Documents.ToList();
            searchViewModel.SearchString = searchRequestModel.SearchText;
            searchViewModel.Hits = results.DocumentList.Documents.Count();
            searchViewModel.DescriptionMaximumLength = this.findwiseConfig.MaximumDescriptionLength;
            searchViewModel.ErrorOnAPI = results.ErrorsOnAPICall;
            searchViewModel.Facets = results.Facets;

            if (results.Stats != null)
            {
                searchViewModel.TotalHits = results.Stats.TotalHits;
            }

            searchViewModel.SearchId = searchRequestModel.SearchId > 0 ? searchRequestModel.SearchId : results.SearchId;

            // Add additional resource attributes
            foreach (var document in searchViewModel.DocumentModel)
            {
                if (int.TryParse(document.Id, out int resourceId))
                {
                    var resource = await this.resourceService.GetResourceByIdAsync(resourceId);

                    if (resource != null && resource.CurrentResourceVersionId.HasValue)
                    {
                        document.ResourceVersionId = resource.CurrentResourceVersionId.Value;

                        var resourceRef = await this.resourceReferenceService.GetByIdAsync(document.ResourceReferenceId);
                        if (resourceRef != null)
                        {
                            document.NodePathId = resourceRef.NodePathId;
                        }
                    }
                }
            }

            // Add related Catalogue information to the search results.
            var relatedCatalogueIds = new List<int>();
            foreach (var document in results.DocumentList.Documents)
            {
                foreach (int catalogueId in document.CatalogueIds)
                {
                    if (relatedCatalogueIds.IndexOf(catalogueId) == -1)
                    {
                        relatedCatalogueIds.Add(catalogueId);
                    }
                }
            }

            searchViewModel.Feedback = results.Feedback;
            searchViewModel.RelatedCatalogues = await this.catalogueService.GetCatalogues(catalogueIds);
            searchViewModel.Spell = results.Spell;

            return searchViewModel;
        }

        /// <summary>
        /// Get catalogue search result.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">The catalog search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<SearchCatalogueViewModel> GetCatalogueSearchResults(CatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            var results = await this.searchService.GetCatalogueSearchResultAsync(catalogueSearchRequestModel, this.CurrentUserId.GetValueOrDefault());

            var documents = results.DocumentList.Documents.ToList();
            var documentIds = documents.Select(x => int.Parse(x.Id)).ToList();
            var catalogues = this.catalogueService.GetCataloguesByNodeId(documentIds);
            var bookmarks = this.bookmarkRepository.GetAll().Where(b => documentIds.Contains(b.NodeId ?? -1) && b.UserId == this.CurrentUserId);
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

            var searchViewModel = new SearchCatalogueViewModel
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
            searchViewModel.Feedback = results.Feedback;
            searchViewModel.Spell = results.Spell;

            return searchViewModel;
        }


        /// <summary>
        /// Get AutoSuggestion Results.
        /// </summary>
        /// <param name="term">term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<AutoSuggestionModel> GetAutoSuggestions(string term)
        {
            var autosuggestionModel = await this.searchService.GetAutoSuggestionResultsAsync(term);
            if (autosuggestionModel != null)
            {
                var documents = autosuggestionModel.CatalogueDocument.CatalogueDocumentList;
                var documentIds = documents.Select(x => int.Parse(x.Id)).ToList();
                var catalogues = this.catalogueService.GetCataloguesByNodeId(documentIds);

                foreach (var document in documents)
                {
                    var catalogue = catalogues.SingleOrDefault(x => x.NodeId == int.Parse(document.Id));
                    if (catalogue == null)
                    {
                        continue;
                    }

                    document.Url = catalogue.Url;
                }

                autosuggestionModel.CatalogueDocument.CatalogueDocumentList = documents;
            }

            return autosuggestionModel;
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