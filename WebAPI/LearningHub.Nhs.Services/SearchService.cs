// <copyright file="SearchService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

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
    using LearningHub.Nhs.Models.Search.SearchFeedback;
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
        /// <param name="findWiseHttpClient">The FindWise Http Client.</param>
        /// <param name="eventService">The event service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="mapper">The mapper.</param>
        public SearchService(
            IFindWiseHttpClient findWiseHttpClient,
            IEventService eventService,
            ILogger<SearchService> logger,
            IOptions<Settings> settings,
            IMapper mapper)
        : base(findWiseHttpClient, logger)
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
            SearchResultModel viewmodel = new SearchResultModel();

            try
            {
                // e.g. if pagesize is 10, then offset would be 0,10,20,30
                var pageSize = searchRequestModel.PageSize;
                var offset = searchRequestModel.PageIndex * pageSize;

                var client = await this.FindWiseHttpClient.GetClient(this.settings.Findwise.SearchUrl);
                var request = string.Format(
                    this.settings.Findwise.UrlSearchComponent + "?offset={1}&hits={2}&q={3}&token={4}",
                    this.settings.Findwise.CollectionIds.Resource,
                    offset,
                    pageSize,
                    this.EncodeSearchText(searchRequestModel.SearchText) + searchRequestModel.FilterText + searchRequestModel.ResourceAccessLevelFilterText + searchRequestModel.ProviderFilterText,
                    this.settings.Findwise.Token);

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
                    this.Logger.LogError($"Get Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.Logger.LogError($"Get Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
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
                var client = await this.FindWiseHttpClient.GetClient(this.settings.Findwise.SearchUrl);
                var request = string.Format(
                    this.settings.Findwise.UrlSearchComponent + "?offset={1}&hits={2}&q={3}&token={4}",
                    this.settings.Findwise.CollectionIds.Catalogue,
                    offset,
                    catalogSearchRequestModel.PageSize,
                    this.EncodeSearchText(catalogSearchRequestModel.SearchText),
                    this.settings.Findwise.Token);

                var response = await client.GetAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<SearchCatalogueResultModel>(result);
                    catalogSearchRequestModel.TotalNumberOfHits = viewmodel.Stats.TotalHits;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    this.Logger.LogError($"Get Catalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}");
                    throw new Exception("AccessDenied to FindWise Server");
                }
                else
                {
                    var error = response.Content.ReadAsStringAsync().Result.ToString();
                    this.Logger.LogError($"Get Catalogue Search Result failed in FindWise, HTTP Status Code:{response.StatusCode}, Error Message:{error}");
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
                if (string.IsNullOrEmpty(this.settings.Findwise.IndexMethod))
                {
                    this.Logger.LogWarning("The FindWiseIndexMethod is not configured. Resource not added to search results");
                }
                else
                {
                    List<SearchResourceRequestModel> resourceList = new List<SearchResourceRequestModel>();
                    resourceList.Add(searchResourceRequestModel);

                    var json = JsonConvert.SerializeObject(resourceList, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd" });

                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

                    var client = await this.FindWiseHttpClient.GetClient(this.settings.Findwise.IndexUrl);

                    var request = string.Format(this.settings.Findwise.IndexMethod, this.settings.Findwise.CollectionIds.Resource) + $"?token={this.settings.Findwise.Token}";
                    var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);
                    iterations--;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        this.Logger.LogError("Save to FindWise failed for resourceId:" + searchResourceRequestModel.Id.ToString() + "HTTP Status Code:" + response.StatusCode.ToString());
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        if (iterations < 0)
                        {
                            this.Logger.LogError("Save to FindWise failed for resourceId:" + searchResourceRequestModel.Id.ToString() + "HTTP Status Code:" + response.StatusCode.ToString());
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
        public async Task RemoveResourceFromSearchAsync(int resourceId)
        {
            try
            {
                if (string.IsNullOrEmpty(this.settings.Findwise.IndexMethod))
                {
                    this.Logger.LogWarning("The FindWiseIndexMethod is not configured. Resource not removed from search.");
                }
                else
                {
                    var client = await this.FindWiseHttpClient.GetClient(this.settings.Findwise.IndexUrl);

                    var request = string.Format(this.settings.Findwise.IndexMethod, this.settings.Findwise.CollectionIds.Resource) + $"?id={resourceId.ToString()}&token={this.settings.Findwise.Token}";

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
            var searchClickPayloadModel = this.mapper.Map<SearchFeedbackPayloadModel>(searchActionResourceModel);
            searchClickPayloadModel.TimeOfClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            searchClickPayloadModel.SearchSignal.ProfileSignature.ApplicationId = ApplicationId;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileType = ProfileType;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileId = this.settings.Findwise.CollectionIds.Resource;

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
            var searchClickPayloadModel = this.mapper.Map<SearchFeedbackPayloadModel>(searchActionCatalogueModel);
            searchClickPayloadModel.TimeOfClick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            searchClickPayloadModel.SearchSignal.ProfileSignature.ApplicationId = ApplicationId;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileType = ProfileType;
            searchClickPayloadModel.SearchSignal.ProfileSignature.ProfileId = this.settings.Findwise.CollectionIds.Catalogue;

            return await this.SendSearchEventClickAsync(searchClickPayloadModel, false);
        }

        /// <summary>
        /// Send search click payload.
        /// </summary>
        /// <param name="searchClickPayloadModel">search click payload model.</param>
        /// <param name="isResource">isResource.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<bool> SendSearchEventClickAsync(SearchFeedbackPayloadModel searchClickPayloadModel, bool isResource)
        {
            var eventType = isResource ? "resource" : "catalog";

            try
            {
                if (string.IsNullOrEmpty(this.settings.Findwise.UrlClickComponent))
                {
                    this.Logger.LogWarning($"The UrlClickComponent is not configured. {eventType} click event not send to FindWise.");
                }
                else
                {
                    var json = JsonConvert.SerializeObject(searchClickPayloadModel);
                    var base64EncodedString = BinaryFormatterHelper.Base64EncodeObject(json);

                    var request = $"{this.settings.Findwise.UrlClickComponent}?payload={base64EncodedString}";

                    var client = await this.FindWiseHttpClient.GetClient(this.settings.Findwise.SearchUrl);
                    var response = await client.PostAsync(request, null).ConfigureAwait(false);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        this.Logger.LogError($"Click event save to FindWise failed for {eventType}: {searchClickPayloadModel.ClickTargetUrl} HTTP Status Code: {response.StatusCode}");
                        throw new Exception("AccessDenied");
                    }
                    else if (!response.IsSuccessStatusCode)
                    {
                        this.Logger.LogError($"Click event save to FindWise failed for {eventType}: {searchClickPayloadModel.ClickTargetUrl} HTTP Status Code: {response.StatusCode}");
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
