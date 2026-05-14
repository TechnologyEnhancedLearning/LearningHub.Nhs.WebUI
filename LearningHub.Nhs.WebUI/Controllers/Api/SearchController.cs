namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.WebUI.Helpers;
    using LearningHub.Nhs.WebUI.Interfaces;
    using LearningHub.Nhs.WebUI.Models;
    using LearningHub.Nhs.WebUI.Models.Search;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Defines the <see cref="SearchController" />.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="searchService">Resource service.</param>
        /// <param name="telemetryClient">Application Insights telemetry client.</param>
        public SearchController(ISearchService searchService, TelemetryClient telemetryClient)
        {
            this.searchService = searchService;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// The GetSearchResults.
        /// </summary>
        /// <param name="searchString">The searchString.</param>
        /// <param name="filterString">The filterString.</param>
        /// <param name="sortItemIndex">The sortItemIndex.</param>
        /// <param name="groupId">The groupId.</param>
        /// <param name="eventTypeEnum">The event type enum.</param>
        /// <param name="searchId">The searchId.</param>
        /// <param name="hits">The hits.</param>
        /// <param name="catalogueId">The catalogueId.</param>
        /// <returns>The SearchViewModel.</returns>
        [HttpGet("GetSearchResults")]
        public async Task<SearchViewModel> GetSearchResults(string searchString, string filterString, int sortItemIndex, Guid groupId, EventTypeEnum eventTypeEnum = EventTypeEnum.Search, int searchId = 0, int hits = 0, int? catalogueId = null)
        {
            var searchViewModel = new SearchViewModel();
            var searchSortItemList = SearchHelper.GetSearchSortList();

            if (searchString == null | searchString == string.Empty)
            {
                searchViewModel.SortItemList = searchSortItemList;
                searchViewModel.SortItemSelected = searchSortItemList.Where(n => n.SearchSortType == SearchSortTypeEnum.Relevance).FirstOrDefault();
                return searchViewModel;
            }

            var selectedSortItem = SearchHelper.GetSearchSortList().Where(x => x.SearchSortType == (SearchSortTypeEnum)sortItemIndex).FirstOrDefault();

            var searchRequestModel = new SearchRequestModel
            {
                SearchText = searchString,
                FilterText = filterString,
                SortColumn = selectedSortItem.Value,
                SortDirection = selectedSortItem?.SortDirection,
                PageSize = hits,
                SearchId = searchId,
                EventTypeEnum = eventTypeEnum,
                CatalogueId = catalogueId,
                GroupId = groupId,
            };

            searchViewModel = await this.searchService.GetSearchResultAsync(searchRequestModel);
            searchViewModel.SortItemList = searchSortItemList;
            searchViewModel.SortItemSelected = searchSortItemList.Where(n => n.SearchSortType == (SearchSortTypeEnum)sortItemIndex).FirstOrDefault();

            return searchViewModel;
        }

        /// <summary>
        /// The GetCatalogueSearchResults.
        /// </summary>
        /// <param name="searchString">The searchString.</param>
        /// <param name="pageIndex">The offset.</param>
        /// <param name="groupId">The groupId.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="searchId">The searchId.</param>
        /// <param name="eventTypeEnum">The eventTypeEnum.</param>
        /// <returns>The searchViewModel.</returns>
        [HttpGet("GetCatalogueSearchResults")]
        public async Task<SearchViewModel> GetCatalogueSearchResults(
            string searchString,
            int pageIndex,
            Guid groupId,
            int pageSize = 3,
            int searchId = 0,
            EventTypeEnum eventTypeEnum = EventTypeEnum.SearchCatalogue)
        {
            CatalogueSearchRequestModel catalogueSearchRequestModel = new CatalogueSearchRequestModel();
            catalogueSearchRequestModel.SearchText = searchString;
            catalogueSearchRequestModel.PageIndex = pageIndex;
            catalogueSearchRequestModel.PageSize = pageSize;
            catalogueSearchRequestModel.SearchId = searchId;
            catalogueSearchRequestModel.EventTypeEnum = eventTypeEnum;
            catalogueSearchRequestModel.GroupId = groupId;

            var searchViewModel = await this.searchService.GetCatalogueSearchResultAsync(catalogueSearchRequestModel);
            return searchViewModel;
        }

        /// <summary>
        /// The RecordClickedSearchResult.
        /// </summary>
        /// <param name="model">The search action resource model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost("RecordClickedSearchResult")]
        public async Task<IActionResult> RecordClickedSearchResult(SearchActionResourceModel model)
        {
            return this.Ok(await this.searchService.CreateResourceSearchActionAsync(model));
        }

        /// <summary>
        /// The RecordClickedCatalogueSearchResult.
        /// </summary>
        /// <param name="model">The search action catalogue model.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost("RecordClickedCatalogueSearchResult")]
        public async Task<IActionResult> RecordClickedCatalogueSearchResult(SearchActionCatalogueClickModel model)
        {
            SearchActionCatalogueModel searchActionCatalogueModel = new SearchActionCatalogueModel
            {
                CatalogueId = model.CatalogueId,
                NodePathId = model.NodePathId,
                ItemIndex = model.ItemIndex,
                NumberOfHits = model.NumberOfHits,
                TotalNumberOfHits = model.TotalNumberOfHits,
                SearchText = model.SearchText,
                ResourceReferenceId = model.ResourceReferenceId,
                GroupId = model.GroupId,
                Query = model.Query,
                UserQuery = model.UserQuery,
                SearchId = model.SearchId,
                TimeOfSearch = model.TimeOfSearch,
            };

            return this.Ok(await this.searchService.CreateCatalogueSearchActionAsync(searchActionCatalogueModel));
        }

        /// <summary>
        /// Records search result click telemetry for Azure Search observability.
        /// </summary>
        /// <param name="model">The click telemetry payload.</param>
        /// <returns>An <see cref="IActionResult"/>.</returns>
        [HttpPost("RecordResultClickTelemetry")]
        public IActionResult RecordResultClickTelemetry(SearchResultClickTelemetryModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ResultUrl))
            {
                return this.BadRequest();
            }

            var properties = new Dictionary<string, string>
            {
                { "CorrelationId", model.CorrelationId ?? string.Empty },
                { "SessionId", model.SessionId ?? string.Empty },
                { "QueryText", model.QueryText ?? string.Empty },
                { "QueryMode", model.QueryMode ?? string.Empty },
                { "ResultUrl", model.ResultUrl ?? string.Empty },
                { "ResultTitle", model.ResultTitle ?? string.Empty },
                { "ResultType", model.ResultType ?? string.Empty },
                { "OpenInNewTab", model.OpenInNewTab.ToString() },
                { "InteractionType", model.InteractionType ?? string.Empty },
            };

            var metrics = new Dictionary<string, double>
            {
                { "ResultRank", model.ResultRank },
                { "ResourceReferenceId", model.ResourceReferenceId },
                { "NodePathId", model.NodePathId },
            };

            this.telemetryClient.TrackEvent("SearchResultClickTelemetry", properties, metrics);

            return this.Ok();
        }
    }
}
