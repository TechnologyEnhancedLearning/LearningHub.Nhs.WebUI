namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchClick;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Helpers;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The search service.
    /// </summary>
    public class SearchService : BaseService<SearchService>, ISearchService
    {
        /// <summary>
        /// application id for search.
        /// </summary>
        private const string ApplicationId = "HEE";

        /// <summary>
        /// profile type for search.
        /// </summary>
        private const string ProfileType = "SEARCHER";

        private readonly Settings settings;
        private readonly IEventService eventService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// The search service.
        /// </summary>
        /// <param name="eventService">The event service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="mapper">The mapper.</param>
        public SearchService(
            IEventService eventService,
            ILogger<SearchService> logger,
            IOptions<Settings> settings,
            IMapper mapper)
        : base(logger)
        {
            this.eventService = eventService;
            this.settings = settings.Value;
            this.mapper = mapper;
        }

        /// <summary>
        /// The Get Search Result Async method.
        /// </summary>
        /// <param name="searchRequestModel">The search request model.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<SearchResultModel> GetSearchResultAsync(SearchRequestModel searchRequestModel, int userId)
        {
            this.Logger.LogWarning("Search is not currently configured. Returning empty results.");
            return await Task.FromResult(new SearchResultModel());
        }

        /// <summary>
        /// The Get Catalogue Search Result Async method.
        /// </summary>
        /// <param name="catalogSearchRequestModel">
        /// The catalog search request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<SearchCatalogueResultModel> GetCatalogueSearchResultAsync(CatalogueSearchRequestModel catalogSearchRequestModel, int userId)
        {
            return await Task.FromResult(new SearchCatalogueResultModel());
        }

        /// <summary>
        /// The send resource for search Async method.
        /// </summary>
        /// <param name="searchResourceRequestModel">The resource to be added to search.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="iterations">number of iterations.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<bool> SendResourceForSearchAsync(SearchResourceRequestModel searchResourceRequestModel, int userId, int? iterations)
        {
            return await Task.FromResult(true);
        }

        /// <summary>
        /// The submt feedback async.
        /// </summary>
        /// <param name="searchFeedbackModel">The search feedback.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The event id.</returns>
        public async Task<LearningHubValidationResult> SubmitFeedbackAsync(SearchFeedBackModel searchFeedbackModel, int userId)
        {
            var jsonobj = new
            {
                searchFeedbackModel.SearchText,
                searchFeedbackModel.Feedback,
                searchFeedbackModel.TotalNumberOfHits,
            };

            var json = JsonConvert.SerializeObject(jsonobj);

            var eventEntity = new Event();
            eventEntity.EventTypeEnum = EventTypeEnum.SearchSubmitFeedback;
            eventEntity.JsonData = json;
            eventEntity.UserId = userId;
            eventEntity.GroupId = searchFeedbackModel.GroupId;

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// Create search term event entry.
        /// </summary>
        /// <param name="searchRequestModel">search request model.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateSearchTermEvent(SearchRequestModel searchRequestModel, int userId)
        {
            // e.g. if pagesize is 10, then offset would be 0,10,20,30
            var pageSize = searchRequestModel.PageSize;
            var offset = searchRequestModel.PageIndex * pageSize;

            var remainingItems = searchRequestModel.TotalNumberOfHits - offset;
            var resultsPerPage = remainingItems >= pageSize ? pageSize : remainingItems;

            var searchEventModel = this.mapper.Map<SearchEventModel>(searchRequestModel);
            searchEventModel.ItemsViewed = resultsPerPage;
            var json = JsonConvert.SerializeObject(searchEventModel);

            var eventEntity = new Event();
            eventEntity.EventTypeEnum = searchRequestModel.EventTypeEnum;
            eventEntity.JsonData = json;
            eventEntity.UserId = userId;
            eventEntity.GroupId = searchRequestModel.GroupId;

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// Create catalogue search term.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogue search request model.</param>
        /// <param name="resultsPerPage">results per page.</param>
        /// <param name="userId">user id.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<LearningHubValidationResult> CreateCatalogueSearchTerm(CatalogueSearchRequestModel catalogueSearchRequestModel, int resultsPerPage, int userId)
        {
            var searchCatalogueEventModel = this.mapper.Map<SearchCatalogueEventModel>(catalogueSearchRequestModel);
            searchCatalogueEventModel.ItemsViewed = resultsPerPage;
            var json = JsonConvert.SerializeObject(searchCatalogueEventModel);
            var eventEntity = new Event();
            eventEntity.EventTypeEnum = catalogueSearchRequestModel.EventTypeEnum;
            eventEntity.JsonData = json;
            eventEntity.UserId = userId;
            eventEntity.GroupId = catalogueSearchRequestModel.GroupId;

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// The create resource search action async.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<LearningHubValidationResult> CreateResourceSearchActionAsync(SearchActionResourceModel searchActionResourceModel, int userId)
        {
            var jsonobj = new
            {
                searchActionResourceModel.SearchText,
                searchActionResourceModel.NodePathId,
                searchActionResourceModel.ItemIndex,
                searchActionResourceModel.NumberOfHits,
                searchActionResourceModel.TotalNumberOfHits,
                searchActionResourceModel.ResourceReferenceId,
            };

            var json = JsonConvert.SerializeObject(jsonobj);

            var eventEntity = new Event
            {
                EventTypeEnum = EventTypeEnum.SearchLaunchResource,
                JsonData = json,
                UserId = userId,
                GroupId = searchActionResourceModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// The create catalogue search action async.
        /// </summary>
        /// <param name="searchActionCatalogueModel">
        /// The search action request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<LearningHubValidationResult> CreateCatalogueSearchActionAsync(SearchActionCatalogueModel searchActionCatalogueModel, int userId)
        {
            var jsonobj = new
            {
                searchActionCatalogueModel.SearchText,
                searchActionCatalogueModel.NodePathId,
                searchActionCatalogueModel.ItemIndex,
                searchActionCatalogueModel.NumberOfHits,
                searchActionCatalogueModel.TotalNumberOfHits,
                searchActionCatalogueModel.CatalogueId,
            };

            var json = JsonConvert.SerializeObject(jsonobj);

            var eventEntity = new Event();
            eventEntity.EventTypeEnum = EventTypeEnum.SearchLaunchCatalogue;
            eventEntity.JsonData = json;
            eventEntity.UserId = userId;
            eventEntity.GroupId = searchActionCatalogueModel.GroupId;

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// The create catalogue resource search action async.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action request model.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<LearningHubValidationResult> CreateCatalogueResourceSearchActionAsync(SearchActionResourceModel searchActionResourceModel, int userId)
        {
            var jsonobj = new
            {
                searchActionResourceModel.SearchText,
                searchActionResourceModel.NodePathId,
                searchActionResourceModel.ItemIndex,
                searchActionResourceModel.ResourceReferenceId,
                searchActionResourceModel.NumberOfHits,
                searchActionResourceModel.TotalNumberOfHits,
            };

            var json = JsonConvert.SerializeObject(jsonobj);

            var eventEntity = new Event();
            eventEntity.EventTypeEnum = EventTypeEnum.LaunchCatalogueResource;
            eventEntity.JsonData = json;
            eventEntity.UserId = userId;
            eventEntity.GroupId = searchActionResourceModel.GroupId;

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// The remove resource from search async method.
        /// </summary>
        /// <param name="resourceId">The resource to be removed from search.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task RemoveResourceFromSearchAsync(int resourceId)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Create catalogue search term event.
        /// </summary>
        /// <param name="catalogueSearchRequestModel">catalogue search request model.</param>
        /// <param name="userId">user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateCatalogueSearchTermEvent(CatalogueSearchRequestModel catalogueSearchRequestModel, int userId)
        {
            // e.g. if pagesize is 3, then offset would be 0,3,6,9
            var offset = catalogueSearchRequestModel.PageIndex * catalogueSearchRequestModel.PageSize;

            var remainingItems = catalogueSearchRequestModel.TotalNumberOfHits - offset;
            var resultsPerPage = remainingItems >= catalogueSearchRequestModel.PageSize ? catalogueSearchRequestModel.PageSize : remainingItems;

            var searchCatalogueEventModel = this.mapper.Map<SearchCatalogueEventModel>(catalogueSearchRequestModel);
            searchCatalogueEventModel.ItemsViewed = resultsPerPage;
            var json = JsonConvert.SerializeObject(searchCatalogueEventModel);
            var eventEntity = new Event();
            eventEntity.EventTypeEnum = catalogueSearchRequestModel.EventTypeEnum;
            eventEntity.JsonData = json;
            eventEntity.UserId = userId;
            eventEntity.GroupId = catalogueSearchRequestModel.GroupId;

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <summary>
        /// The create resource search action async.
        /// </summary>
        /// <param name="searchActionResourceModel">
        /// The search action request model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> SendResourceSearchEventClickAsync(SearchActionResourceModel searchActionResourceModel)
        {
            var searchClickPayloadModel = this.mapper.Map<SearchClickPayloadModel>(searchActionResourceModel);
            searchClickPayloadModel.TimeOfClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            searchClickPayloadModel.SearchSignal.ProfileSignature.ApplicationId = ApplicationId;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileType = ProfileType;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileId = "Resource";

            return await this.SendSearchEventClickAsync(searchClickPayloadModel, true);
        }

        /// <summary>
        /// The create catalogue search action async.
        /// </summary>
        /// <param name="searchActionCatalogueModel">
        /// The search action request model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> SendCatalogueSearchEventAsync(SearchActionCatalogueModel searchActionCatalogueModel)
        {
            var searchClickPayloadModel = this.mapper.Map<SearchClickPayloadModel>(searchActionCatalogueModel);
            searchClickPayloadModel.TimeOfClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            searchClickPayloadModel.SearchSignal.ProfileSignature.ApplicationId = ApplicationId;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileType = ProfileType;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileId = "Catalogue";

            return await this.SendSearchEventClickAsync(searchClickPayloadModel, false);
        }

        /// <summary>
        /// Gets AllCatalogue search results from findwise api call.
        /// </summary>
        /// <param name="catalogSearchRequestModel">The allcatalog search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<SearchAllCatalogueResultModel> GetAllCatalogueSearchResultsAsync(AllCatalogueSearchRequestModel catalogSearchRequestModel)
        {
            return await Task.FromResult(new SearchAllCatalogueResultModel());
        }

        /// <summary>
        /// The Get Auto suggestion Results Async method.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AutoSuggestionModel> GetAutoSuggestionResultsAsync(string term)
        {
            return await Task.FromResult(new AutoSuggestionModel());
        }

        /// <summary>
        /// The AutoSuggestion Event action async.
        /// </summary>
        /// <param name="clickPayloadModel">
        /// The clic kPayload Model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> SendAutoSuggestionEventAsync(AutoSuggestionClickPayloadModel clickPayloadModel)
        {
            clickPayloadModel.TimeOfClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            clickPayloadModel.SearchSignal.ProfileSignature.ApplicationId = ApplicationId;
            clickPayloadModel.SearchSignal.ProfileSignature.ProfileType = ProfileType;
            clickPayloadModel.SearchSignal.ProfileSignature.ProfileId = "AutoSuggestion";

            return await this.SendAutoSuggestionEventClickAsync(clickPayloadModel);
        }

        /// <summary>
        /// Send search click payload.
        /// </summary>
        /// <param name="searchClickPayloadModel">search click payload model.</param>
        /// <param name="isResource">isResource.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<bool> SendSearchEventClickAsync(SearchClickPayloadModel searchClickPayloadModel, bool isResource)
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Send auto suggestion click payload.
        /// </summary>
        /// <param name="clickPayloadModel">search click payload model.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<bool> SendAutoSuggestionEventClickAsync(AutoSuggestionClickPayloadModel clickPayloadModel)
        {
            return await Task.FromResult(false);
        }

        private string EncodeSearchText(string searchText)
        {
            string specialSearchCharacters = this.settings.SpecialSearchCharacters;

            // Add backslash to the start of the string
            specialSearchCharacters = specialSearchCharacters.Replace(@"\", null);
            specialSearchCharacters = @"\" + specialSearchCharacters;

            for (int i = 0; i < specialSearchCharacters.Length; i++)
            {
                searchText = searchText.Replace(specialSearchCharacters[i].ToString(), @"\" + specialSearchCharacters[i]);
            }

            searchText = HttpUtility.UrlEncode(searchText);
            return searchText;
        }
    }
}
