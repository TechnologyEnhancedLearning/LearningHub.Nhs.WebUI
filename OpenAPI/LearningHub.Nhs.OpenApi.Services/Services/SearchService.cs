namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchClick;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Event = LearningHub.Nhs.Models.Entities.Analytics.Event;

    /// <summary>
    /// The search service.
    /// </summary>
    public class SearchService : ISearchService
    {

        /// <summary>
        /// application id for search.
        /// </summary>
        private const string ApplicationId = "HEE";

        /// <summary>
        /// profile type for search.
        /// </summary>
        private const string ProfileType = "SEARCHER";

        private readonly IEventService eventService;
        private readonly ILearningHubService learningHubService;
        private readonly IResourceRepository resourceRepository;
        private readonly IFindwiseClient findwiseClient;
        private readonly ILogger logger;
        private readonly FindwiseConfig findwiseConfig;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchService"/> class.
        /// The search service.
        /// </summary>
        /// <param name="learningHubService">
        /// The <see cref="ILearningHubService"/>.
        /// </param>
        /// <param name="findwiseClient">
        /// The <see cref="IFindwiseClient"/>.
        /// </param>
        /// <param name="findwiseConfig">
        /// The <see cref="findwiseConfig"/>.
        /// </param>
        /// <param name="resourceRepository">
        /// The <see cref="IResourceRepository"/>.
        /// </param>
        /// <param name="logger">Logger.</param>
        public SearchService(
            ILearningHubService learningHubService,
            IEventService eventService,
            IFindwiseClient findwiseClient,
            IOptions<FindwiseConfig> findwiseConfig,
            IResourceRepository resourceRepository,
            ILogger<SearchService> logger,
            IMapper mapper)
        {
            this.learningHubService = learningHubService;
            this.eventService = eventService;
            this.findwiseClient = findwiseClient;
            this.resourceRepository = resourceRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.findwiseConfig = findwiseConfig.Value;
        }

        /// <summary>
        /// The Get Search Result Async method.
        /// </summary>
        /// <param name="searchRequestModel">The search request model.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<SearchResultModel> GetSearchResultAsync(SearchRequestModel searchRequestModel, int userId)
        {
            SearchResultModel viewmodel = new SearchResultModel();

            try
            {
                // e.g. if pagesize is 10, then offset would be 0,10,20,30
                var pageSize = searchRequestModel.PageSize;
                var offset = searchRequestModel.PageIndex * pageSize;

                var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);

                var request = string.Format(
                    this.findwiseConfig.SearchEndpointPath + "{0}?offset={1}&hits={2}&q={3}&token={4}",
                    this.findwiseConfig.CollectionIds.Resource,
                    offset,
                    pageSize,
                    this.EncodeSearchText(searchRequestModel.SearchText) + searchRequestModel.FilterText + searchRequestModel.ResourceAccessLevelFilterText + searchRequestModel.ProviderFilterText,
                    this.findwiseConfig.Token);

                if (searchRequestModel.CatalogueId.HasValue)
                {
                    request += $"&catalogue_ids={searchRequestModel.CatalogueId}";
                }

                // if sort column is requested
                if (!string.IsNullOrEmpty(searchRequestModel.SortColumn))
                {
                    var sortquery = $"&sort={searchRequestModel.SortColumn}";

                    // if sort direction option is requested
                    if (!string.IsNullOrEmpty(searchRequestModel.SortDirection))
                    {
                        var sortdirection = searchRequestModel.SortDirection.StartsWith("asc") ? "asc" : "desc";
                        sortquery = $"{sortquery}_{sortdirection}";
                    }

                    request = $"{request}{sortquery}";
                }

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<SearchResultModel>(result);
                    searchRequestModel.TotalNumberOfHits = viewmodel.Stats.TotalHits;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    this.logger.LogError($"Get Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.logger.LogError($"Get Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                    throw new Exception("Error with FindWise Server");
                }

                return viewmodel;
            }
            catch (Exception)
            {
                throw;
            }
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
            var viewmodel = new SearchCatalogueResultModel();

            try
            {
                // e.g. if pagesize is 3, then offset would be 0,3,6,9
                var offset = catalogSearchRequestModel.PageIndex * catalogSearchRequestModel.PageSize;
                var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);
                var request = string.Format(
                    this.findwiseConfig.SearchEndpointPath + "{0}?offset={1}&hits={2}&q={3}&token={4}",
                    this.findwiseConfig.CollectionIds.Catalogue,
                    offset,
                    catalogSearchRequestModel.PageSize,
                    this.EncodeSearchText(catalogSearchRequestModel.SearchText),
                    this.findwiseConfig.Token);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<SearchCatalogueResultModel>(result);
                    catalogSearchRequestModel.TotalNumberOfHits = viewmodel.Stats.TotalHits;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    this.logger.LogError($"Get Catalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.logger.LogError($"Get Catalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                    throw new Exception("Error with FindWise Server");
                }

                var remainingItems = catalogSearchRequestModel.TotalNumberOfHits - offset;
                var resultsPerPage = remainingItems >= catalogSearchRequestModel.PageSize ? catalogSearchRequestModel.PageSize : remainingItems;
                LearningHubValidationResult validationResult = await this.CreateCatalogueSearchTerm(catalogSearchRequestModel, resultsPerPage, userId);

                viewmodel.SearchId = validationResult.CreatedId ?? 0;

                return viewmodel;
            }
            catch (Exception)
            {
                throw;
            }
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
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileId = this.findwiseConfig.CollectionIds.Resource;

            return await this.SendSearchEventClickAsync(searchClickPayloadModel, true);
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



        /// <inheritdoc />
        public async Task<ResourceSearchResultModel> Search(ResourceSearchRequest query, int? currentUserId)
        {
            var findwiseResultModel = await this.findwiseClient.Search(query);

            if (findwiseResultModel.FindwiseRequestStatus != FindwiseRequestStatus.Success)
            {
                return ResourceSearchResultModel.FailedWithStatus(findwiseResultModel.FindwiseRequestStatus);
            }

            var resourceMetadataViewModels = await this.GetResourceMetadataViewModels(findwiseResultModel, currentUserId);

            var totalHits = findwiseResultModel.SearchResults?.Stats.TotalHits;

            return new ResourceSearchResultModel(
                resourceMetadataViewModels,
                findwiseResultModel.FindwiseRequestStatus,
                totalHits ?? 0);
        }

        /// <summary>
        /// The remove resource from search async method.
        /// </summary>
        /// <param name="resourceId">The resource to be removed from search.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task RemoveResourceFromSearchAsync(int resourceId)
        {
            try
            {
                if (string.IsNullOrEmpty(this.findwiseConfig.IndexMethod))
                {
                    this.logger.LogWarning("The FindWiseIndexMethod is not configured. Resource not removed from search.");
                }
                else
                {
                    var client = await this.findwiseClient.GetClient(this.findwiseConfig.IndexUrl);

                    var request = string.Format(this.findwiseConfig.IndexMethod, this.findwiseConfig.CollectionIds.Resource) + $"?id={resourceId.ToString()}&token={this.findwiseConfig.Token}";

                    var response = await client.DeleteAsync(request).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Removal of resource to search failed: " + resourceId.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Removal of resource from search failed: " + resourceId.ToString() + " : " + ex.Message);
            }
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
            try
            {
                if (string.IsNullOrEmpty(this.findwiseConfig.IndexMethod))
                {
                    this.logger.LogWarning("The FindWiseIndexMethod is not configured. Resource not added to search results");
                }
                else
                {
                    List<SearchResourceRequestModel> resourceList =[searchResourceRequestModel];

                    var json = JsonConvert.SerializeObject(resourceList, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd" });

                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    var client = await this.findwiseClient.GetClient(this.findwiseConfig.IndexUrl);

                    var request = string.Format(this.findwiseConfig.SearchBaseUrl, this.findwiseConfig.CollectionIds.Resource) + $"?token={this.findwiseConfig.Token}";
                    var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);
                    iterations--;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        this.logger.LogError("Save to FindWise failed for resourceId:" + searchResourceRequestModel.Id.ToString() + "HTTP Status Code:" + response.StatusCode.ToString());
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        if (iterations < 0)
                        {
                            this.logger.LogError("Save to FindWise failed for resourceId:" + searchResourceRequestModel.Id.ToString() + "HTTP Status Code:" + response.StatusCode.ToString());
                            throw new Exception("Posting of resource to search failed: " + stringContent);
                        }

                        await this.SendResourceForSearchAsync(searchResourceRequestModel, userId, iterations);
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Posting of resource to search failed: " + searchResourceRequestModel.Id + " : " + ex.Message);
            }
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
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileId = this.findwiseConfig.CollectionIds.Catalogue;

            return await this.SendSearchEventClickAsync(searchClickPayloadModel, false);
        }

        /// <summary>
        /// Gets AllCatalogue search results from findwise api call.
        /// </summary>
        /// <param name="catalogSearchRequestModel">The allcatalog search request model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<SearchAllCatalogueResultModel> GetAllCatalogueSearchResultsAsync(AllCatalogueSearchRequestModel catalogSearchRequestModel)
        {
            var viewmodel = new SearchAllCatalogueResultModel();
            try
            {
                var offset = catalogSearchRequestModel.PageIndex * catalogSearchRequestModel.PageSize;
                var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);
                var request = string.Format(
                    this.findwiseConfig.SearchEndpointPath + "{0}?offset={1}&hits={2}&q={3}&token={4}",
                    this.findwiseConfig.CollectionIds.Catalogue,
                    offset,
                    catalogSearchRequestModel.PageSize,
                    this.EncodeSearchText(catalogSearchRequestModel.SearchText),
                    this.findwiseConfig.Token);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<SearchAllCatalogueResultModel>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    this.logger.LogError($"Get AllCatalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.logger.LogError($"Get AllCatalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                    throw new Exception("Error with FindWise Server");
                }

                return viewmodel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The Get Auto suggestion Results Async method.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<AutoSuggestionModel> GetAutoSuggestionResultsAsync(string term)
        {
            var viewmodel = new AutoSuggestionModel();

            try
            {
                var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);
                var request = string.Format(
                   this.findwiseConfig.SearchEndpointPath + "{0}?q={1}&token={2}",
                   this.findwiseConfig.CollectionIds.AutoSuggestion,
                   this.EncodeSearchText(term),
                   this.findwiseConfig.Token);


                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<AutoSuggestionModel>(result);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    this.logger.LogError($"Get Auto Suggetion Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.logger.LogError($"Get Auto Suggetion Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
                    throw new Exception("Error with FindWise Server");
                }

                return viewmodel;
            }
            catch (Exception)
            {
                throw;
            }
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
            clickPayloadModel.SearchSignal.ProfileSignature.ProfileId = this.findwiseConfig.CollectionIds.AutoSuggestion;

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
            var eventType = isResource ? "resource" : "catalog";

            try
            {
                if (string.IsNullOrEmpty(this.findwiseConfig.UrlClickComponent))
                {
                    this.logger.LogWarning($"The UrlClickComponent is not configured. {eventType} click event not send to FindWise.");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(searchClickPayloadModel);
                    var base64EncodedString = BinaryFormatterHelper.Base64EncodeObject(json);

                    var request = $"{this.findwiseConfig.UrlClickComponent}?payload={base64EncodedString}";

                    var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);
                    var response = await client.PostAsync(request, null).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        this.logger.LogError($"Click event save to FindWise failed for {eventType}: {searchClickPayloadModel.ClickTargetUrl} HTTP Status Code: {response.StatusCode}");
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        this.logger.LogError($"Click event save to FindWise failed for {eventType}: {searchClickPayloadModel.ClickTargetUrl} HTTP Status Code: {response.StatusCode}");
                        throw new Exception($"Click event save to FindWise failed for {eventType}: {json}");
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Click event save to FindWise failed for {eventType}: {searchClickPayloadModel.ClickTargetUrl} :  {ex.Message}");
            }
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
            try
            {
                if (string.IsNullOrEmpty(this.findwiseConfig.UrlAutoSuggestionClickComponent))
                {
                    this.logger.LogWarning($"The UrlClickComponent is not configured. Auto suggestion click event not send to FindWise.");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(clickPayloadModel);
                    var base64EncodedString = BinaryFormatterHelper.Base64EncodeObject(json);

                    var request = $"{this.findwiseConfig.UrlAutoSuggestionClickComponent}?payload={base64EncodedString}";

                    var client = await this.findwiseClient.GetClient(this.findwiseConfig.SearchBaseUrl);
                    var response = await client.PostAsync(request, null).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        this.logger.LogError($"Click event save to FindWise failed for Auto suggestion: {clickPayloadModel.ClickTargetUrl} HTTP Status Code: {response.StatusCode}");
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        this.logger.LogError($"Click event save to FindWise failed for Auto suggestion: {clickPayloadModel.ClickTargetUrl} HTTP Status Code: {response.StatusCode}");
                        throw new Exception($"Click event save to FindWise failed for Auto suggestion: {json}");
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Click event save to FindWise failed for Auto suggestion: {clickPayloadModel.ClickTargetUrl} :  {ex.Message}");
            }
        }


        private string EncodeSearchText(string searchText)
        {
            string specialSearchCharacters = this.findwiseConfig.SpecialSearchCharacters;

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

        private async Task<List<ResourceMetadataViewModel>> GetResourceMetadataViewModels(
            FindwiseResultModel findwiseResultModel, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>() { };
            List<ResourceMetadataViewModel> resourceMetadataViewModels = new List<ResourceMetadataViewModel>() { };
            var documentsFound = findwiseResultModel.SearchResults?.DocumentList.Documents?.ToList() ??
                                 new List<Document>();
            var findwiseResourceIds = documentsFound.Select(d => int.Parse(d.Id)).ToList();

            if (!findwiseResourceIds.Any())
            {
                return new List<ResourceMetadataViewModel>();
            }

            var resourcesFound = await this.resourceRepository.GetResourcesFromIds(findwiseResourceIds);

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourcesFound.Select(x => x.Id).ToList();
                List<int> userIds = new List<int>() { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>() { };
            }

            resourceMetadataViewModels = resourcesFound.Select(resource => this.MapToViewModel(resource, resourceActivities.Where(x => x.ResourceId == resource.Id).ToList()))
                .OrderBySequence(findwiseResourceIds)
                .ToList();

            var unmatchedResources = findwiseResourceIds
                .Except(resourceMetadataViewModels.Select(r => r.ResourceId)).ToList();

            if (unmatchedResources.Any())
            {
                var unmatchedResourcesIdsString = string.Join(", ", unmatchedResources);
                this.logger.LogWarning(
                    "Findwise returned documents that were not found in the database with IDs: " +
                    unmatchedResourcesIdsString);
            }

            return resourceMetadataViewModels;
        }

        private ResourceMetadataViewModel MapToViewModel(Resource resource, List<ResourceActivityDTO> resourceActivities)
        {
            var hasCurrentResourceVersion = resource.CurrentResourceVersion != null;
            var hasRating = resource.CurrentResourceVersion?.ResourceVersionRatingSummary != null;

            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>() { };

            if (resourceActivities != null && resourceActivities.Count != 0)
            {
                majorVersionIdActivityStatusDescription = ActivityStatusHelper.GetMajorVersionIdActivityStatusDescriptionLSPerResource(resource, resourceActivities)
                    .ToList();
            }

            if (!hasCurrentResourceVersion)
            {
                this.logger.LogInformation(
                    $"Resource with id {resource.Id} is missing a current resource version");
            }

            if (!hasRating)
            {
                this.logger.LogInformation(
                    $"Resource with id {resource.Id} is missing a ResourceVersionRatingSummary");
            }

            var resourceTypeNameOrEmpty = resource.GetResourceTypeNameOrEmpty();
            if (resourceTypeNameOrEmpty == string.Empty)
            {
                this.logger.LogError($"Resource has unrecognised type: {resource.ResourceTypeEnum}");
            }


            return new ResourceMetadataViewModel(
                resource.Id,
                resource.CurrentResourceVersion?.Title ?? ResourceHelpers.NoResourceVersionText,
                resource.CurrentResourceVersion?.Description ?? string.Empty,
                resource.ResourceReference.Select(this.GetResourceReferenceViewModel).ToList(),
                resourceTypeNameOrEmpty,
                resource.CurrentResourceVersion?.MajorVersion ?? 0,
                resource.CurrentResourceVersion?.ResourceVersionRatingSummary?.AverageRating ?? 0.0m,
                majorVersionIdActivityStatusDescription
                );
        }

        private ResourceReferenceViewModel GetResourceReferenceViewModel(
            ResourceReference resourceReference)
        {
            return new ResourceReferenceViewModel(
                resourceReference.OriginalResourceReferenceId,
                resourceReference.GetCatalogue(),
                this.learningHubService.GetResourceLaunchUrl(resourceReference.OriginalResourceReferenceId));
        }
    }
}
