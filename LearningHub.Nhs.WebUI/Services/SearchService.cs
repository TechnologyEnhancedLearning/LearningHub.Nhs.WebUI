namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Principal;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using HtmlAgilityPack;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models.Search;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="SearchService" />.
    /// </summary>
    public class SearchService : BaseService<SearchService>, ISearchService
    {
        private readonly Settings settings;
        private IProviderService providerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="providerService">Provider service.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="settings">Settings.</param>
        public SearchService(ILearningHubHttpClient learningHubHttpClient, IProviderService providerService, ILogger<SearchService> logger, IOptions<Settings> settings)
        : base(learningHubHttpClient, logger)
        {
            this.settings = settings.Value;
            this.providerService = providerService;
        }

        /// <summary>
        /// Performs a search - either a combined resource and catalogue search, or just a resource search if
        /// searching within a catalogue.
        /// </summary>
        /// <param name="user">user.</param>
        /// <param name="searchRequest">The SearchRequestViewModel.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<SearchResultViewModel> PerformSearch(IPrincipal user, SearchRequestViewModel searchRequest)
        {
            var searchSortType = 0;
            if (searchRequest.Sortby.HasValue && Enum.IsDefined(typeof(SearchSortTypeEnum), searchRequest.Sortby))
            {
                searchSortType = searchRequest.Sortby.Value;
            }

            var searchString = searchRequest.Term?.Trim() ?? string.Empty;
            var searchSortItemList = SearchHelper.GetSearchSortList();
            var selectedSortItem = searchSortItemList.Where(x => x.SearchSortType == (SearchSortTypeEnum)searchSortType).FirstOrDefault();
            var groupId = Guid.Parse(searchRequest.GroupId);

            var resourceSearchPageSize = this.settings.FindwiseSettings.ResourceSearchPageSize;
            var catalogueSearchPageSize = this.settings.FindwiseSettings.CatalogueSearchPageSize;

            var resourceSearchRequestModel = new SearchRequestModel
            {
                SearchId = searchRequest.SearchId.Value,
                SearchText = searchString,
                FilterText = searchRequest.Filters?.Any() == true ? $"&resource_type={string.Join("&resource_type=", searchRequest.Filters)}" : string.Empty,
                ProviderFilterText = searchRequest.ProviderFilters?.Any() == true ? $"&provider_ids={string.Join("&provider_ids=", searchRequest.ProviderFilters)}" : string.Empty,
                SortColumn = selectedSortItem.Value,
                SortDirection = selectedSortItem?.SortDirection,
                PageIndex = searchRequest.ResourcePageIndex ?? 0,
                PageSize = resourceSearchPageSize,
                GroupId = groupId,
                CatalogueId = searchRequest.CatalogueId,
                ResourceAccessLevelFilterText = searchRequest.ResourceAccessLevelId.HasValue && searchRequest.ResourceAccessLevelId != (int)ResourceAccessibilityEnum.None ? $"&resource_access_level={searchRequest.ResourceAccessLevelId.Value}" : string.Empty,
            };

            var catalogueSearchRequestModel = new CatalogueSearchRequestModel()
            {
                SearchId = searchRequest.SearchId.Value,
                SearchText = searchString,
                EventTypeEnum = EventTypeEnum.SearchCatalogue,
                PageIndex = searchRequest.CataloguePageIndex ?? 0,
                PageSize = catalogueSearchPageSize,
                GroupId = groupId,
            };

            SearchViewModel resourceResult = null;
            SearchCatalogueViewModel catalogueResult = null;

            if (searchString != string.Empty)
            {
                var resourceResultTask = this.GetSearchResultAsync(resourceSearchRequestModel);

                if (searchRequest.CatalogueId.HasValue)
                {
                    // Search within a catalogue - resources only.
                    await resourceResultTask;
                    resourceResult = resourceResultTask.Result;
                }
                else
                {
                    // Normal search - resources and catalogues.
                    var catalogueResultTask = this.GetCatalogueSearchResultAsync(catalogueSearchRequestModel);

                    await Task.WhenAll(resourceResultTask, catalogueResultTask);

                    resourceResult = resourceResultTask.Result;
                    catalogueResult = catalogueResultTask.Result;
                }

                var searchfilters = new List<SearchFilterModel>();
                var resourceAccessLevelFilters = new List<SearchFilterModel>();

                var providerfilters = new List<SearchFilterModel>();

                if (resourceResult != null && resourceResult.Facets != null && resourceResult.Facets.Length > 0)
                {
                    var filters = resourceResult.Facets.Where(x => x.Id == "resource_type").First().Filters;

                    foreach (var filteritem in filters.Select(x => x.DisplayName.ToLower()).Distinct())
                    {
                        var filter = filters.Where(x => x.DisplayName == filteritem).FirstOrDefault();

                        if (filter != null && UtilityHelper.FindwiseResourceTypeDict.ContainsKey(filter.DisplayName))
                        {
                            var resourceTypeEnum = UtilityHelper.FindwiseResourceTypeDict[filter.DisplayName];
                            var searchfilter = new SearchFilterModel() { DisplayName = UtilityHelper.GetPrettifiedResourceTypeName(resourceTypeEnum), Count = filter.Count, Value = filteritem, Selected = searchRequest.Filters?.Contains(filter.DisplayName) ?? false };
                            searchfilters.Add(searchfilter);
                        }
                    }

                    if (user.IsInRole("BasicUser"))
                    {
                        var accessLevelFilters = resourceResult.Facets.Where(x => x.Id == "resource_access_level").First().Filters;

                        var generalAccessValue = (int)ResourceAccessibilityEnum.GeneralAccess;
                        var basicUserAudienceFilterItem = accessLevelFilters.Where(x => x.DisplayName == generalAccessValue.ToString()).FirstOrDefault();
                        var basicResourceAccesslevelCount = basicUserAudienceFilterItem?.Count ?? 0;
                        var basicUserAudienceFilter = new SearchFilterModel() { DisplayName = ResourceAccessLevelHelper.GetPrettifiedResourceAccessLevelOptionDisplayName(ResourceAccessibilityEnum.GeneralAccess), Count = basicResourceAccesslevelCount, Value = generalAccessValue.ToString(), Selected = (searchRequest.ResourceAccessLevelId ?? 0) == generalAccessValue };
                        resourceAccessLevelFilters.Add(basicUserAudienceFilter);
                    }

                    filters = resourceResult.Facets.Where(x => x.Id == "provider_ids").First().Filters;

                    if (filters.Length > 0)
                    {
                        var providers = await this.providerService.GetProviders();
                        var provider_ids = providers.Select(n => n.Id).ToList();

                        foreach (var filteritem in filters.Select(x => x.DisplayName.ToLower()).Distinct())
                        {
                            var filter = filters.Where(x => x.DisplayName == filteritem).FirstOrDefault();

                            if (filter != null && provider_ids.Contains(Convert.ToInt32(filter.DisplayName)))
                            {
                                var provider = providers.Where(n => n.Id == Convert.ToInt32(filter.DisplayName)).FirstOrDefault();

                                var searchfilter = new SearchFilterModel() { DisplayName = provider.Name, Count = filter.Count, Value = filteritem, Selected = searchRequest.ProviderFilters?.Contains(filter.DisplayName) ?? false };
                                providerfilters.Add(searchfilter);
                            }
                        }
                    }
                }

                resourceResult.SortItemList = searchSortItemList;
                resourceResult.SortItemSelected = selectedSortItem;
                resourceResult.SearchFilters = searchfilters;
                resourceResult.SearchResourceAccessLevelFilters = resourceAccessLevelFilters;
                resourceResult.SearchProviderFilters = providerfilters;
            }

            var searchResultViewModel = new SearchResultViewModel
            {
                SearchString = searchString,
                GroupId = groupId,
                FeedbackSubmitted = searchRequest.FeedbackSubmitted ?? false,
                ResourceCurrentPageIndex = searchRequest.ResourcePageIndex ?? 0,
                CatalogueCurrentPageIndex = searchRequest.CataloguePageIndex ?? 0,
                ResourceSearchResult = resourceResult,
                CatalogueSearchResult = catalogueResult,

                ResourceResultPaging = new SearchResultPagingModel
                {
                    CurrentPage = searchRequest.ResourcePageIndex ?? 0,
                    PageSize = resourceSearchPageSize,
                    TotalItems = resourceResult?.TotalHits ?? 0,
                },

                CatalogueResultPaging = new SearchResultPagingModel
                {
                    CurrentPage = searchRequest.CataloguePageIndex ?? 0,
                    PageSize = catalogueSearchPageSize,
                    TotalItems = catalogueResult?.TotalHits ?? 0,
                },
            };

            return searchResultViewModel;
        }

        /// <summary>
        /// Records the analytics events associated with a search.
        /// </summary>
        /// <param name="search">The SearchRequestViewModel.</param>
        /// <param name="action">The SearchFormActionTypeEnum.</param>
        /// <param name="resourceCount">The resourceCount.</param>
        /// <param name="catalogueCount">The catalogueCount.</param>
        /// <returns>The event ID.</returns>
        public async Task<int> RegisterSearchEventsAsync(SearchRequestViewModel search, SearchFormActionTypeEnum action, int resourceCount = 0, int catalogueCount = 0)
        {
            var eventId = 0;
            var resourceSearchPageSize = this.settings.FindwiseSettings.ResourceSearchPageSize;
            var catalogueSearchPageSize = this.settings.FindwiseSettings.CatalogueSearchPageSize;

            var sortBy = search.Sortby.HasValue ? (SearchSortTypeEnum)search.Sortby : SearchSortTypeEnum.Relevance;

            var allproviders = await this.providerService.GetProviders();

            var searchTermEventRequestModel = new SearchRequestModel
            {
                SearchId = search.SearchId.Value,
                PageIndex = search.ResourcePageIndex ?? 0,
                PageSize = resourceSearchPageSize,
                TotalNumberOfHits = resourceCount,
                SearchText = search.Term,
                GroupId = Guid.Parse(search.GroupId),
                SortDirection = "descending",
                SortColumn = sortBy.ToString(),
                FilterText = search.Filters?.Any() == true ? string.Join(",", search.Filters) : null,
                ResourceAccessLevelFilterText = search.ResourceAccessLevelId.HasValue ? search.ResourceAccessLevelId.Value.ToString() : null,
                ProviderFilterText = search.ProviderFilters?.Any() == true ? string.Join(",", allproviders.Where(n => search.ProviderFilters.Contains(n.Id.ToString())).Select(x => x.Name).ToList()) : null,
            };

            switch (action)
            {
                case SearchFormActionTypeEnum.ApplyFilter:
                    searchTermEventRequestModel.EventTypeEnum = EventTypeEnum.SearchSort;
                    await this.CreateSearchTermEventAsync(searchTermEventRequestModel);

                    searchTermEventRequestModel.EventTypeEnum = EventTypeEnum.SearchFilter;
                    eventId = await this.CreateSearchTermEventAsync(searchTermEventRequestModel);

                    break;

                case SearchFormActionTypeEnum.ResourceNextPageChange:
                case SearchFormActionTypeEnum.ResourcePreviousPageChange:
                    searchTermEventRequestModel.EventTypeEnum = EventTypeEnum.SearchResourcePageChange;
                    eventId = await this.CreateSearchTermEventAsync(searchTermEventRequestModel);
                    break;

                case SearchFormActionTypeEnum.CatalogueNextPageChange:
                case SearchFormActionTypeEnum.CataloguePreviousPageChange:
                    var catalogSearchTermEventRequestModel = new CatalogueSearchRequestModel
                    {
                        SearchId = search.SearchId.Value,
                        SearchText = search.Term,
                        EventTypeEnum = EventTypeEnum.SearchCataloguePageChange,
                        PageIndex = search.CataloguePageIndex ?? 0,
                        PageSize = catalogueSearchPageSize,
                        GroupId = Guid.Parse(search.GroupId),
                        TotalNumberOfHits = catalogueCount,
                    };

                    eventId = await this.CreateCatalogueSearchTermEventAsync(catalogSearchTermEventRequestModel);
                    break;

                case SearchFormActionTypeEnum.SubmitFeedback:
                    break;

                case SearchFormActionTypeEnum.SearchWithinCatalogue:
                    searchTermEventRequestModel.EventTypeEnum = EventTypeEnum.SearchWithinCatalogue;
                    eventId = await this.CreateSearchTermEventAsync(searchTermEventRequestModel);
                    break;

                default:
                    searchTermEventRequestModel.EventTypeEnum = EventTypeEnum.Search;
                    eventId = await this.CreateSearchTermEventAsync(searchTermEventRequestModel);

                    var catalogueSearchTermRequestModel = new CatalogueSearchRequestModel
                    {
                        SearchId = eventId,
                        PageIndex = search.CataloguePageIndex ?? 0,
                        PageSize = catalogueSearchPageSize,
                        TotalNumberOfHits = catalogueCount,
                        SearchText = search.Term,
                        GroupId = Guid.Parse(search.GroupId),
                        EventTypeEnum = EventTypeEnum.SearchCatalogue,
                    };

                    break;
            }

            return eventId;
        }

        /// <inheritdoc/>
        public async Task<int> CreateResourceSearchActionAsync(SearchActionResourceModel searchActionResourceModel)
        {
            SearchViewModel searchViewModel = new SearchViewModel();

            try
            {
                int createId = 0;
                var client = await this.LearningHubHttpClient.GetClientAsync();

                var json = JsonConvert.SerializeObject(searchActionResourceModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/CreateResourceSearchAction";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                    if (!apiResponse.Success)
                    {
                        throw new Exception("Error creating Resource Search Action!");
                    }

                    createId = apiResponse.ValidationResult.CreatedId ?? 0;
                }
                else
                {
                    throw new Exception(string.Format("Error creating Resource Search Action!"));
                }

                return createId;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error creating Resource Search Action: {0}", ex.Message));
            }
        }

        /// <summary>
        /// The Create Catalogue Search Action.
        /// </summary>
        /// <param name="searchActionCatalogueModel">The search action catalogue model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateCatalogueSearchActionAsync(SearchActionCatalogueModel searchActionCatalogueModel)
        {
            try
            {
                int createId = 0;
                var client = await this.LearningHubHttpClient.GetClientAsync();

                var json = JsonConvert.SerializeObject(searchActionCatalogueModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/CreateCatalogueSearchAction";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                    if (!apiResponse.Success)
                    {
                        throw new Exception("Error creating Catalogue Search Action!");
                    }

                    createId = apiResponse.ValidationResult.CreatedId ?? 0;
                }
                else
                {
                    throw new Exception(string.Format("Error creating Catalogue Search Action!"));
                }

                return createId;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error creating Catalogue Search Action: {0}", ex.Message));
            }
        }

        /// <inheritdoc/>
        public async Task<int> CreateSearchTermEventAsync(SearchRequestModel searchRequestModel)
        {
            try
            {
                int createId = 0;
                var client = await this.LearningHubHttpClient.GetClientAsync();

                var json = JsonConvert.SerializeObject(searchRequestModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/CreateSearchTermAction";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                    if (!apiResponse.Success)
                    {
                        throw new Exception("Error creating Search Action!");
                    }

                    createId = apiResponse.ValidationResult.CreatedId ?? 0;
                }
                else
                {
                    throw new Exception(string.Format("Error creating Search Term Action"));
                }

                return createId;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error creating Search Term Action: {0}", ex.Message));
            }
        }

        /// <inheritdoc/>
        public async Task<SearchViewModel> GetSearchResultAsync(SearchRequestModel searchRequestModel)
        {
            SearchViewModel searchViewModel = new SearchViewModel();

            try
            {
                var client = await this.LearningHubHttpClient.GetClientAsync();

                searchRequestModel.SearchText = this.DecodeProblemCharacters(searchRequestModel.SearchText);

                var json = JsonConvert.SerializeObject(searchRequestModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/GetResults";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    searchViewModel = JsonConvert.DeserializeObject<SearchViewModel>(result);

                    if (searchViewModel.DocumentModel != null
                        && searchViewModel.DocumentModel.Any())
                    {
                        searchViewModel.DocumentModel.ForEach(x => x.Description = this.RemoveHtmlTags(x.Description));
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return searchViewModel;
            }
            catch (Exception ex)
            {
                searchViewModel.ErrorOnAPI = true;
                this.Logger.LogError(string.Format("Error occurred in GetSearchResultAsync: {0}", ex.Message));
                return searchViewModel;
            }
        }

        /// <inheritdoc/>
        public async Task<int> SubmitFeedbackAsync(SearchFeedBackModel model)
        {
            try
            {
                int createId = 0;

                var client = await this.LearningHubHttpClient.GetClientAsync();
                var request = this.settings.LearningHubApiUrl + "Search/SubmitFeedback";
                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(request, content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                    if (!apiResponse.Success)
                    {
                        throw new Exception("Error submitting Search feedback!");
                    }

                    createId = apiResponse.ValidationResult.CreatedId ?? 0;
                }
                else
                {
                    throw new Exception(string.Format("Error submitting Search feedback!"));
                }

                return createId;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error submitting Search feedback: {0}", ex.Message));
            }
        }

        /// <inheritdoc/>
        public async Task<SearchCatalogueViewModel> GetCatalogueSearchResultAsync(CatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            SearchCatalogueViewModel searchViewModel = new SearchCatalogueViewModel();

            try
            {
                var client = await this.LearningHubHttpClient.GetClientAsync();

                catalogueSearchRequestModel.SearchText = this.DecodeProblemCharacters(catalogueSearchRequestModel.SearchText);

                var json = JsonConvert.SerializeObject(catalogueSearchRequestModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/GetCatalogueResults";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    searchViewModel = JsonConvert.DeserializeObject<SearchCatalogueViewModel>(result);

                    if (searchViewModel.DocumentModel != null
                        && searchViewModel.DocumentModel.Count != 0)
                    {
                        searchViewModel.DocumentModel.ForEach(x => x.Description = this.RemoveHtmlTags(x.Description));
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return searchViewModel;
            }
            catch (Exception ex)
            {
                searchViewModel.ErrorOnAPI = true;
                this.Logger.LogError(string.Format("Error occurred in GetSearchResultAsync: {0}", ex.Message));
                return searchViewModel;
            }
        }

        /// <summary>
        /// Create catalogue search term event.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogue search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<int> CreateCatalogueSearchTermEventAsync(CatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            try
            {
                int createId = 0;
                var client = await this.LearningHubHttpClient.GetClientAsync();

                var json = JsonConvert.SerializeObject(catalogueSearchRequestModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/CreateCatalogSearchTermAction";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result) as ApiResponse;

                    if (!apiResponse.Success)
                    {
                        throw new Exception("Error creating Catalog Search Term Action!");
                    }

                    createId = apiResponse.ValidationResult.CreatedId ?? 0;
                }
                else
                {
                    throw new Exception(string.Format("Error creating Catalog Search Term Action"));
                }

                return createId;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error creating Catalog Search Term Action: {0}", ex.Message));
            }
        }

        /// <summary>
        /// GetAllCatalogueSearchResultAsync.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogueSearchRequestModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<SearchAllCatalogueViewModel> GetAllCatalogueSearchResultAsync(AllCatalogueSearchRequestModel catalogueSearchRequestModel)
        {
            SearchAllCatalogueViewModel searchViewModel = new SearchAllCatalogueViewModel();

            try
            {
                var client = await this.LearningHubHttpClient.GetClientAsync();

                catalogueSearchRequestModel.SearchText = this.DecodeProblemCharacters(catalogueSearchRequestModel.SearchText);

                var json = JsonConvert.SerializeObject(catalogueSearchRequestModel);
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                var request = $"Search/GetAllCatalogueSearchResult";
                var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    searchViewModel = JsonConvert.DeserializeObject<SearchAllCatalogueViewModel>(result);

                    if (searchViewModel.DocumentModel != null
                        && searchViewModel.DocumentModel.Count != 0)
                    {
                        searchViewModel.DocumentModel.ForEach(x => x.Description = this.RemoveHtmlTags(x.Description));
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    throw new Exception("AccessDenied");
                }

                return searchViewModel;
            }
            catch (Exception ex)
            {
                searchViewModel.ErrorOnAPI = true;
                this.Logger.LogError(string.Format("Error occurred in GetAllCatalogueSearchResultAsync: {0}", ex.Message));
                return searchViewModel;
            }
        }

        /// <summary>
        /// The RemoveHtmlTags.
        /// </summary>
        /// <param name="html">The html<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string RemoveHtmlTags(string html)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(html);
                string text = HttpUtility.HtmlDecode(document.DocumentNode.InnerText);
                return text;
            }
            catch
            {
                return html;
            }
        }

        private string DecodeProblemCharacters(string inputString)
        {
            try
            {
                return inputString.Replace("&percnt;", "%")
                    .Replace("&quot;", "\"")
                    .Replace("&plus;", "+")
                    .Replace("&sol;", "/")
                    .Replace("&bsol;", "\\");
            }
            catch
            {
                return inputString;
            }
        }
    }
}
