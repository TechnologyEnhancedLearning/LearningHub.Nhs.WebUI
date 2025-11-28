namespace LearningHub.Nhs.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchClick;
    using LearningHub.Nhs.WebUI.Filters;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Search;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.FeatureManagement;
    using Settings = LearningHub.Nhs.WebUI.Configuration.Settings;

    /// <summary>
    /// The SearchController.
    /// </summary>
    [Authorize]
    [Route("[controller]")]
    [ServiceFilter(typeof(LoginWizardFilter))]
    public class SearchController : BaseController
    {
        private readonly ISearchService searchService;
        private readonly IFileService fileService;
        private readonly IFeatureManager featureManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The httpClientFactory.</param>
        /// <param name="hostingEnvironment">The hostingEnvironment.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="searchService">The searchService.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="fileService">The fileService.</param>
        /// <param name="featureManager"> The Feature flag manager.</param>
        public SearchController(
            IHttpClientFactory httpClientFactory,
            IWebHostEnvironment hostingEnvironment,
            IOptions<Settings> settings,
            ISearchService searchService,
            ILogger<SearchController> logger,
            IFileService fileService,
            IFeatureManager featureManager)
        : base(hostingEnvironment, httpClientFactory, logger, settings.Value)
        {
            this.searchService = searchService;
            this.fileService = fileService;
            this.featureManager = featureManager;
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <param name="search">Search object.</param>
        /// <param name="noSortFilterError">Whether sort or filter applied.</param>
        /// <param name="emptyFeedbackError">Whether feedback submitted.</param>
        /// <param name="filterApplied">filter applied.</param>
        /// <returns>The actionResult.</returns>
        [HttpGet("results")]
        public async Task<IActionResult> Index(SearchRequestViewModel search, bool noSortFilterError = false, bool emptyFeedbackError = false, bool filterApplied = false)
        {
            search.SearchId ??= 0;
            search.GroupId = !string.IsNullOrWhiteSpace(search.GroupId) && Guid.TryParse(search.GroupId, out Guid groupId) ? groupId.ToString() : Guid.NewGuid().ToString();

            // Fix: Ensure an instance of IFeatureManager is injected and used
            var azureSearchEnabled = Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.AzureSearch)).Result;
            SearchResultViewModel searchResult = new SearchResultViewModel();

            if (azureSearchEnabled)
            {
                searchResult = await this.searchService.PerformSearch(this.User, search);
            }
            else
            {
                searchResult = await this.searchService.PerformSearchInFindwise(this.User, search);
            }

            if (search.SearchId == 0 && searchResult.ResourceSearchResult != null)
            {
                var searchId = await this.searchService.RegisterSearchEventsAsync(
                    search,
                    SearchFormActionTypeEnum.BasicSearch,
                    searchResult.ResourceSearchResult.TotalHits,
                    searchResult.CatalogueSearchResult != null ? searchResult.CatalogueSearchResult.TotalHits : 0);

                searchResult.ResourceSearchResult.SearchId = searchId;
                if (searchResult.CatalogueSearchResult != null)
                {
                    searchResult.CatalogueSearchResult.SearchId = searchId;
                }
            }

            if (filterApplied)
            {
                await this.searchService.RegisterSearchEventsAsync(search, SearchFormActionTypeEnum.ApplyFilter, searchResult.ResourceSearchResult?.TotalHits ?? 0);
            }

            if (noSortFilterError)
            {
                this.ViewBag.SelectFilterError = true;
            }

            if (emptyFeedbackError)
            {
                this.ViewBag.EmptyFeedbackError = true;
            }

            return this.View("Index", searchResult);
        }

        /// <summary>
        /// The IndexPost.
        /// </summary>
        /// <param name="search">Search object.</param>
        /// <param name="resourceCount">The resource result count.</param>
        /// <param name="filters">The search filter.</param>
        /// <param name="resourceAccessLevelId">The search resource access level id.</param>
        /// <param name="providerfilters">The provider filter.</param>
        /// <param name="sortby">The sort by.</param>
        /// <param name="groupId">The search group id.</param>
        /// <param name="searchId">The search id.</param>
        /// <param name="actionType">The action type.</param>
        /// <param name="resourceCollectionFilter">The show filter.</param>
        /// <param name="feedback">The feedback.</param>
        /// <returns>The actionResult.</returns>
        [HttpPost("results")]
        public async Task<IActionResult> IndexPost([FromQuery] SearchRequestViewModel search, int resourceCount, [FromForm] IEnumerable<string> filters, [FromForm] int? resourceAccessLevelId, [FromForm] IEnumerable<string> providerfilters, [FromForm] int? sortby, [FromForm] string groupId, [FromForm] int searchId, [FromQuery] string actionType, [FromForm] IEnumerable<string> resourceCollectionFilter, string feedback)
        {
            if (actionType == "feedback")
            {
                if (!string.IsNullOrWhiteSpace(feedback))
                {
                    var feedbackModel = new SearchFeedBackModel
                    {
                        SearchText = search.Term,
                        Feedback = feedback,
                        TotalNumberOfHits = resourceCount,
                        GroupId = !string.IsNullOrWhiteSpace(search.GroupId) && Guid.TryParse(search.GroupId, out Guid guidValue) ? guidValue : Guid.NewGuid(),
                    };

                    await this.searchService.SubmitFeedbackAsync(feedbackModel);
                    search.FeedbackSubmitted = true;
                }
                else
                {
                    return await this.Index(search, emptyFeedbackError: true);
                }
            }
            else
            {
                var existingFilters = (search.Filters ?? new List<string>()).OrderBy(t => t);
                var newFilters = filters.OrderBy(t => t);
                var filterUpdated = !newFilters.SequenceEqual(existingFilters);

                var resourceAccessLevelFilterUpdated = (resourceAccessLevelId ?? 0) != (search.ResourceAccessLevelId ?? 0);
                var existingProviderFilters = (search.ProviderFilters ?? new List<string>()).OrderBy(t => t);
                var newProviderFilters = providerfilters.OrderBy(t => t);
                var filterProviderUpdated = !newProviderFilters.SequenceEqual(existingProviderFilters);
                var existingResourceCollectionFilter = (search.ResourceCollectionFilter ?? new List<string>()).OrderBy(t => t);
                var newResourceCollectionFilter = resourceCollectionFilter.OrderBy(t => t);
                var filterResourceCollectionUpdated = !newResourceCollectionFilter.SequenceEqual(existingResourceCollectionFilter);

                // No sort or resource type filter updated or resource access level filter updated or provider filter applied or resource collection filter applied
                if ((search.Sortby ?? 0) == sortby && !filterUpdated && !resourceAccessLevelFilterUpdated && !filterProviderUpdated && !filterResourceCollectionUpdated)
                {
                    return await this.Index(search, noSortFilterError: true);
                }

                if (search.ResourcePageIndex > 0 && (filterUpdated || resourceAccessLevelFilterUpdated || filterProviderUpdated || filterResourceCollectionUpdated))
                {
                    search.ResourcePageIndex = null;
                }
            }

            search.Filters = filters;
            search.ProviderFilters = providerfilters;
            search.Sortby = sortby;
            search.GroupId = groupId;
            search.SearchId = searchId;
            search.ResourceAccessLevelId = resourceAccessLevelId;
            search.ResourceCollectionFilter = resourceCollectionFilter;

            var routeValues = new RouteValueDictionary(search)
            {
                { "filterApplied", true },
            };

            return this.RedirectToAction("Index", routeValues);
        }

        /// <summary>
        /// The RecordResourceNavigation.
        /// </summary>
        /// <param name="search">Search object.</param>
        /// <param name="resourceCount">The resource result count.</param>
        /// <returns>The actionResult.</returns>
        [HttpGet("record-resource-navigation")]
        public async Task<IActionResult> RecordResourceNavigation(SearchRequestViewModel search, int resourceCount)
        {
            await this.searchService.RegisterSearchEventsAsync(search, SearchFormActionTypeEnum.ResourceNextPageChange, resourceCount);

            return this.RedirectToAction("Index", search);
        }

        /// <summary>
        /// The RecordResourceNavigation.
        /// </summary>
        /// <param name="search">Search object.</param>
        /// <param name="catalogueCount">The catalogue result count.</param>
        /// <returns>The actionResult.</returns>
        [HttpGet("record-catalogue-navigation")]
        public async Task<IActionResult> RecordCatalogueNavigation(SearchRequestViewModel search, int catalogueCount)
        {
            await this.searchService.RegisterSearchEventsAsync(search, SearchFormActionTypeEnum.CatalogueNextPageChange, catalogueCount: catalogueCount);

            return this.RedirectToAction("Index", search);
        }

        /// <summary>
        /// The RecordClickedSearchResult.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="nodePathId">The nodePathId.</param>
        /// <param name="itemIndex">The itemIndex.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="totalNumberOfHits">The totalNumberOfHits.</param>
        /// <param name="searchText">The searchText.</param>
        /// <param name="resourceReferenceId">The resourceReferenceId.</param>
        /// <param name="groupId">The groupdId.</param>
        /// <param name="searchId">The search id.</param>
        /// <param name="timeOfSearch">time of search.</param>
        /// <param name="userQuery">user query.</param>
        /// <param name="query">search query.</param>
        /// <param name="title">the title.</param>
        [HttpGet("record-resource-click")]
        public void RecordResourceClick(string url, int nodePathId, int itemIndex, int pageIndex, int totalNumberOfHits, string searchText, int resourceReferenceId, Guid groupId, string searchId, long timeOfSearch, string userQuery, string query, string title)
        {
            var searchActionResourceModel = new SearchActionResourceModel
            {
                NodePathId = nodePathId,
                ItemIndex = itemIndex,
                NumberOfHits = pageIndex * this.Settings.FindwiseSettings.ResourceSearchPageSize,
                TotalNumberOfHits = totalNumberOfHits,
                SearchText = searchText,
                ResourceReferenceId = resourceReferenceId,
                GroupId = groupId,
                SearchId = searchId,
                TimeOfSearch = timeOfSearch,
                UserQuery = userQuery,
                Query = query,
                Title = title,
            };

            this.searchService.CreateResourceSearchActionAsync(searchActionResourceModel);
            this.Response.Redirect(url);
        }

        /// <summary>
        /// The RecordClickedCatalogueSearchResult.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="nodePathId">The nodePathId.</param>
        /// <param name="itemIndex">The itemIndex.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <param name="totalNumberOfHits">The totalNumberOfHits.</param>
        /// <param name="searchText">The searchText.</param>
        /// <param name="catalogueId">catalogue id.</param>
        /// <param name="groupId">The groupdId.</param>
        /// <param name="searchId">The search id.</param>
        /// <param name="timeOfSearch">time of search.</param>
        /// <param name="userQuery">user query.</param>
        /// <param name="query">search query.</param>
        /// <param name="name">the name.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet("record-catalogue-click")]
        public async Task<IActionResult> RecordCatalogueClick(string url, int nodePathId, int itemIndex, int pageIndex, int totalNumberOfHits, string searchText, int catalogueId, Guid groupId, string searchId, long timeOfSearch, string userQuery, string query, string name)
        {
            SearchActionCatalogueModel searchActionCatalogueModel = new SearchActionCatalogueModel
            {
                CatalogueId = catalogueId,
                NodePathId = nodePathId,
                ItemIndex = itemIndex,
                NumberOfHits = pageIndex * this.Settings.FindwiseSettings.CatalogueSearchPageSize,
                TotalNumberOfHits = totalNumberOfHits,
                SearchText = searchText,
                GroupId = groupId,
                SearchId = searchId,
                TimeOfSearch = timeOfSearch,
                UserQuery = userQuery,
                Query = query,
                Name = name,
            };

            await this.searchService.CreateCatalogueSearchActionAsync(searchActionCatalogueModel);
            return this.Redirect(url);
        }

        /// <summary>
        /// The Image.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The ActionResult.</returns>
        [HttpGet("Image/{name}")]
        public async Task<IActionResult> Image(string name)
        {
            var file = await this.fileService.DownloadFileAsync("CatalogueImageDirectory", name);
            if (file != null)
            {
                return this.File(file.Content, file.ContentType);
            }
            else
            {
                return this.Ok(this.Content("No file found"));
            }
        }

        /// <summary>
        /// GetAutoSuggestion returns the auto suggestion options.
        /// </summary>
        /// <param name="term">search term.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet("GetAutoSuggestion")]
        public async Task<IActionResult> GetAutoSuggestion(string term)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("AccessDenied", "Home");
            }

            var autoSuggestions = await this.searchService.GetAutoSuggestionList(term);

            var azureSearchEnabled = Task.Run(() => this.featureManager.IsEnabledAsync(FeatureFlags.AzureSearch)).Result;

            if (!azureSearchEnabled)
            {
                return this.PartialView("_AutoComplete", autoSuggestions);
            }

            return this.PartialView("_AutoSuggest", autoSuggestions);
        }

        /// <summary>
        /// Records the AutoSuggestion Click logs.
        /// </summary>
        /// <param name="term">the term.</param>
        /// <param name="url">the searchType.</param>
        /// <param name="clickTargetUrl">click Payload Model.</param>
        /// <param name="itemIndex">itemIndex.</param>
        /// <param name="totalNumberOfHits">total Number Of Hits.</param>
        /// <param name="containerId">containerId.</param>
        /// <param name="name">name.</param>
        /// <param name="query">query.</param>
        /// <param name="userQuery">userQuery.</param>
        /// <param name="searchId">searchId.</param>
        /// <param name="timeOfSearch">timeOfSearch.</param>
        /// <param name="title">title.</param>
        /// <returns>Action result.</returns>
        [HttpGet("record-autosuggestion-click")]
        public IActionResult RecordAutoSuggestionClick(string term, string url, string clickTargetUrl, int itemIndex, int totalNumberOfHits, string containerId, string name, string query, string userQuery, string searchId, long timeOfSearch, string title)
        {
            AutoSuggestionClickPayloadModel clickPayloadModel = new AutoSuggestionClickPayloadModel
            {
                ClickTargetUrl = clickTargetUrl,
                ContainerId = containerId,
                HitNumber = itemIndex,
                TimeOfClick = timeOfSearch,
                DocumentFields = new SearchClickDocumentModel
                {
                    Name = name,
                    Title = title,
                },
                SearchSignal = new SearchClickSignalModel
                {
                    Query = query,
                    SearchId = searchId,
                    TimeOfSearch = timeOfSearch,
                    UserQuery = userQuery,
                    Stats = new SearchClickStatsModel
                    {
                        TotalHits = totalNumberOfHits,
                    },
                },
            };

            this.searchService.SendAutoSuggestionClickActionAsync(clickPayloadModel);
            return this.Redirect(url);
        }
    }
}