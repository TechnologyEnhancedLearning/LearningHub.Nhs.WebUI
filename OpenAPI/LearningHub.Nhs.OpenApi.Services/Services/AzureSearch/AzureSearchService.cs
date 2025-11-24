namespace LearningHub.Nhs.OpenApi.Services.Services.AzureSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Azure.Search.Documents;
    using Azure.Search.Documents.Models;
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Models.Search.SearchClick;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Models.ViewModels.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.AzureSearch;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Helpers.Search;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Event = LearningHub.Nhs.Models.Entities.Analytics.Event;


    /// <summary>
    /// The Azure AI Search service implementation.
    /// Provides search functionality with facet caching and parallel query execution.
    /// </summary>
    public class AzureSearchService : ISearchService
    {
        /// <summary>
        /// Default cache expiration time in minutes for facet results.
        /// </summary>
        private const int DefaultFacetCacheExpirationMinutes = 5;
        private readonly IEventService eventService;
        private readonly ILearningHubService learningHubService;
        private readonly IResourceRepository resourceRepository;
        private readonly ICachingService cachingService;
        private readonly SearchClient searchClient;
        private readonly ILogger<AzureSearchService> logger;
        private readonly AzureSearchConfig azureSearchConfig;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchService"/> class.
        /// </summary>
        /// <param name="learningHubService">The learning hub service.</param>
        /// <param name="eventService">The event service.</param>
        /// <param name="azureSearchConfig">The Azure Search configuration.</param>
        /// <param name="resourceRepository">The resource repository.</param>
        /// <param name="cachingService">The caching service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The mapper.</param>
        public AzureSearchService(
            ILearningHubService learningHubService,
            IEventService eventService,
            IOptions<AzureSearchConfig> azureSearchConfig,
            IResourceRepository resourceRepository,
            ICachingService cachingService,
            ILogger<AzureSearchService> logger,
            IMapper mapper)
        {
            this.learningHubService = learningHubService;
            this.eventService = eventService;
            this.resourceRepository = resourceRepository;
            this.cachingService = cachingService;
            this.logger = logger;
            this.mapper = mapper;
            this.azureSearchConfig = azureSearchConfig.Value;

            this.searchClient = AzureSearchClientFactory.CreateQueryClient(this.azureSearchConfig);
        }

        /// <inheritdoc/>
        public async Task<SearchResultModel> GetSearchResultAsync(SearchRequestModel searchRequestModel, int userId, CancellationToken cancellationToken = default)
        {
            var viewmodel = new SearchResultModel();

            try
            {
                var searchQueryType = SearchQueryType.Full;
                var pageSize = searchRequestModel.PageSize;
                var offset = searchRequestModel.PageIndex * pageSize;

                // Normalize resource_type filter values
                var filters = SearchFilterBuilder.CombineAndNormaliseFilters(searchRequestModel.FilterText, searchRequestModel.ProviderFilterText);

                // Build query string
                var query = searchQueryType == SearchQueryType.Full
                    ? LuceneQueryBuilder.BuildLuceneQuery(searchRequestModel.SearchText)
                    : searchRequestModel.SearchText;

                Dictionary<string, string> sortBy = new Dictionary<string, string>()
                {
                    { searchRequestModel.SortColumn, searchRequestModel.SortDirection }
                };

                var searchOptions = SearchOptionsBuilder.BuildSearchOptions(searchQueryType, offset, pageSize, filters, sortBy, true);
                SearchResults<Models.ServiceModels.AzureSearch.SearchDocument> filteredResponse = await this.searchClient.SearchAsync<Models.ServiceModels.AzureSearch.SearchDocument>(query, searchOptions, cancellationToken);

                // Execute filtered search and unfiltered facet query in parallel
                //var filteredSearchTask1 = ExecuteSearchAsync(
                //    query,
                //    searchQueryType,
                //    offset,
                //    pageSize,
                //    filters,
                //    sortBy,
                //    includeFacets: true,
                //    cancellationToken);

                //var unfilteredFacetsTask = GetUnfilteredFacetsAsync(
                //    searchRequestModel.SearchText,
                //    searchQueryType,
                //    cancellationToken);

                // await Task.WhenAll(filteredSearchTask);

                //   var filteredResponse = await filteredSearchTask;
                //var unfilteredFacets = await unfilteredFacetsTask;
                
                // Map documents
                var documents = filteredResponse.GetResults()
                    .Select(result =>
                    {
                        var doc = result.Document;
                        doc.ParseManualTags();

                        return new Document
                        {
                            Id = doc.Id,
                            Title = doc.Title,
                            Description = doc.Description,
                            ResourceType = MapToResourceType(doc.ResourceType),
                            ProviderIds = doc.ProviderIds?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
                            CatalogueIds =
                                doc.ResourceType == "catalogue"
                                    ? new List<int> { Convert.ToInt32(doc.Id) }  
                                    : (
                                        doc.CatalogueId?
                                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(id => int.TryParse(id, out var val) ? val : 0)
                                            .ToList()
                                        ?? new List<int>()
                                    ),
                            Rating = Convert.ToDecimal(doc.Rating),                            
                            Author = doc.Author,
                            Authors = doc.Author?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToList(),
                            AuthoredDate = doc.DateAuthored?.ToString(),
                            ResourceReferenceId = int.TryParse(doc.ResourceReferenceId, out var id) ? id : 0,
                            Click = new SearchClickModel { Payload = new SearchClickPayloadModel { HitNumber = 1 }, Url = "binon" }
                        };
                    })
                    .ToList();

                viewmodel.DocumentList = new Documentlist
                {
                    Documents = documents.ToArray()
                };

                var unfilteredFacets = await GetUnfilteredFacetsAsync(
                   searchRequestModel.SearchText,
                   filteredResponse.Facets,
                   cancellationToken);

                // Merge facets from filtered and unfiltered results
                viewmodel.Facets = AzureSearchFacetHelper.MergeFacets(filteredResponse.Facets, unfilteredFacets, filters);

                var count = Convert.ToInt32(filteredResponse.TotalCount);
                viewmodel.Stats = new Stats
                {
                    TotalHits = count
                };
                searchRequestModel.TotalNumberOfHits = viewmodel.Stats.TotalHits;

                return viewmodel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Azure Search query failed for search text: {SearchText}", searchRequestModel.SearchText);
                throw;
            }
        }

        private string MapToResourceType(string resourceType)
        {
            if (string.IsNullOrWhiteSpace(resourceType))
                return ResourceTypeEnum.Undefined.ToString();

            string cleanedResourceType = resourceType
                .Trim()
                .ToLower()
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "");

            if (cleanedResourceType.StartsWith("scorm"))
                cleanedResourceType = ResourceTypeEnum.Scorm.ToString();
            else if (cleanedResourceType.StartsWith("web"))
                cleanedResourceType = ResourceTypeEnum.WebLink.ToString();
            else if (cleanedResourceType.Contains("file"))
                cleanedResourceType = ResourceTypeEnum.GenericFile.ToString();

            return cleanedResourceType;
        }
        
        /// <summary>
        /// Gets unfiltered facets for a search term, using caching.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="facets">The facet results.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The unfiltered facet results.</returns>
        private async Task<IDictionary<string, IList<FacetResult>>> GetUnfilteredFacetsAsync(
            string searchText,
            IDictionary<string, IList<FacetResult>> facets,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"AllFacets_{searchText?.ToLowerInvariant() ?? "*"}";
            var cacheResponse = await this.cachingService.GetAsync<IDictionary<string, IList<CacheableFacetResult>>>(cacheKey);

            if (cacheResponse.ResponseEnum == CacheReadResponseEnum.Found)
            {
                // Convert cached DTO back to FacetResult dictionary
                return AzureSearchFacetHelper.ConvertFromCacheable(cacheResponse.Item);
            }

            if (facets != null)
            {
                // Convert to cacheable DTO before caching
                var cacheableFacets = AzureSearchFacetHelper.ConvertToCacheable(facets);
                await this.cachingService.SetAsync(cacheKey, cacheableFacets, DefaultFacetCacheExpirationMinutes, slidingExpiration: true);
            }

            return facets ?? new Dictionary<string, IList<FacetResult>>();
        }

        /// <inheritdoc/>
        public async Task<SearchCatalogueResultModel> GetCatalogueSearchResultAsync(CatalogueSearchRequestModel catalogSearchRequestModel, int userId, CancellationToken cancellationToken = default)
        {
            var viewmodel = new SearchCatalogueResultModel();

            try
            {
                var offset = catalogSearchRequestModel.PageIndex * catalogSearchRequestModel.PageSize;

                // Build filters for catalogue search
                var filters = new Dictionary<string, List<string>>
                {
                    { "resource_collection", new List<string> { "Catalogue" } }
                };

                var searchOptions = new SearchOptions
                {
                    Skip = offset,
                    Size = catalogSearchRequestModel.PageSize,
                    IncludeTotalCount = true,
                    Filter = SearchFilterBuilder.BuildFilterExpression(filters)
                };

                SearchResults<Models.ServiceModels.AzureSearch.SearchDocument> response = await this.searchClient.SearchAsync<Models.ServiceModels.AzureSearch.SearchDocument>(
                    catalogSearchRequestModel.SearchText,
                    searchOptions,
                    cancellationToken);

                var documentList = new CatalogueDocumentList
                {
                    Documents = response.GetResults()
                    .Select(result =>
                    {
                        var doc = result.Document;
                        doc.ParseManualTags();

                        return new CatalogueDocument
                        {
                            Id = doc.Id,
                            Name = doc.Title,
                            Description = doc.Description,
                            Click = new SearchClickModel { Payload = new SearchClickPayloadModel { HitNumber = 1 }, Url = "binon" }
                        };
                    })
                    .ToArray()
                };

                viewmodel.DocumentList = documentList;
                viewmodel.Stats = new Stats
                {
                    TotalHits = Convert.ToInt32(response.TotalCount)
                };
                catalogSearchRequestModel.TotalNumberOfHits = viewmodel.Stats.TotalHits;

                var remainingItems = catalogSearchRequestModel.TotalNumberOfHits - offset;
                var resultsPerPage = remainingItems >= catalogSearchRequestModel.PageSize ? catalogSearchRequestModel.PageSize : remainingItems;
                var validationResult = await this.CreateCatalogueSearchTerm(catalogSearchRequestModel, resultsPerPage, userId);

                viewmodel.SearchId = validationResult.CreatedId ?? 0;

                return viewmodel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Azure Search catalogue query failed for search text: {SearchText}", catalogSearchRequestModel.SearchText);
                throw;
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

            var eventEntity = new Event
            {
                EventTypeEnum = EventTypeEnum.SearchLaunchCatalogue,
                JsonData = json,
                UserId = userId,
                GroupId = searchActionCatalogueModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <inheritdoc/>
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

            var eventEntity = new Event
            {
                EventTypeEnum = EventTypeEnum.LaunchCatalogueResource,
                JsonData = json,
                UserId = userId,
                GroupId = searchActionResourceModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> SubmitFeedbackAsync(SearchFeedBackModel searchFeedbackModel, int userId)
        {
            var jsonobj = new
            {
                searchFeedbackModel.SearchText,
                searchFeedbackModel.Feedback,
                searchFeedbackModel.TotalNumberOfHits,
            };

            var json = JsonConvert.SerializeObject(jsonobj);

            var eventEntity = new Event
            {
                EventTypeEnum = EventTypeEnum.SearchSubmitFeedback,
                JsonData = json,
                UserId = userId,
                GroupId = searchFeedbackModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateSearchTermEvent(SearchRequestModel searchRequestModel, int userId)
        {
            var pageSize = searchRequestModel.PageSize;
            var offset = searchRequestModel.PageIndex * pageSize;

            var remainingItems = searchRequestModel.TotalNumberOfHits - offset;
            var resultsPerPage = remainingItems >= pageSize ? pageSize : remainingItems;

            var searchEventModel = this.mapper.Map<SearchEventModel>(searchRequestModel);
            searchEventModel.ItemsViewed = resultsPerPage;
            var json = JsonConvert.SerializeObject(searchEventModel);

            var eventEntity = new Event
            {
                EventTypeEnum = searchRequestModel.EventTypeEnum,
                JsonData = json,
                UserId = userId,
                GroupId = searchRequestModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateCatalogueSearchTermEvent(CatalogueSearchRequestModel catalogueSearchRequestModel, int userId)
        {
            var offset = catalogueSearchRequestModel.PageIndex * catalogueSearchRequestModel.PageSize;

            var remainingItems = catalogueSearchRequestModel.TotalNumberOfHits - offset;
            var resultsPerPage = remainingItems >= catalogueSearchRequestModel.PageSize ? catalogueSearchRequestModel.PageSize : remainingItems;

            var searchCatalogueEventModel = this.mapper.Map<SearchCatalogueEventModel>(catalogueSearchRequestModel);
            searchCatalogueEventModel.ItemsViewed = resultsPerPage;
            var json = JsonConvert.SerializeObject(searchCatalogueEventModel);
            var eventEntity = new Event
            {
                EventTypeEnum = catalogueSearchRequestModel.EventTypeEnum,
                JsonData = json,
                UserId = userId,
                GroupId = catalogueSearchRequestModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <inheritdoc/>
        public async Task<bool> SendResourceSearchEventClickAsync(SearchActionResourceModel searchActionResourceModel)
        {
            // Azure Search doesn't need click tracking like Findwise
            // Log the event but return true
            this.logger.LogInformation($"Search click event logged for resource {searchActionResourceModel.ResourceReferenceId}");
            return await Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<LearningHubValidationResult> CreateCatalogueSearchTerm(CatalogueSearchRequestModel catalogueSearchRequestModel, int resultsPerPage, int userId)
        {
            var searchCatalogueEventModel = this.mapper.Map<SearchCatalogueEventModel>(catalogueSearchRequestModel);
            searchCatalogueEventModel.ItemsViewed = resultsPerPage;
            var json = JsonConvert.SerializeObject(searchCatalogueEventModel);
            var eventEntity = new Event
            {
                EventTypeEnum = catalogueSearchRequestModel.EventTypeEnum,
                JsonData = json,
                UserId = userId,
                GroupId = catalogueSearchRequestModel.GroupId,
            };

            return await this.eventService.CreateAsync(userId, eventEntity);
        }

        /// <inheritdoc/>
        public async Task<ResourceSearchResultModel> Search(ResourceSearchRequest query, int? currentUserId)
        {
            try
            {
                var searchOptions = new SearchOptions
                {
                    Skip = query.Offset,
                    Size = query.Limit,
                    IncludeTotalCount = true,
                };

                SearchResults<Models.ServiceModels.AzureSearch.SearchDocument> response = await this.searchClient.SearchAsync<Models.ServiceModels.AzureSearch.SearchDocument>(
                    query.SearchText,
                    searchOptions);

                var documentsFound = response.GetResults().Select(r => r.Document).ToList();
                var findwiseResourceIds = documentsFound.Select(d => int.Parse(d.Id)).ToList();

                if (!findwiseResourceIds.Any())
                {
                    return new ResourceSearchResultModel(
                        new List<ResourceMetadataViewModel>(),
                        LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise.FindwiseRequestStatus.Success,
                        0);
                }

                var resourceMetadataViewModels = await this.GetResourceMetadataViewModels(findwiseResourceIds, currentUserId);

                var totalHits = (int)(response.TotalCount ?? 0);

                return new ResourceSearchResultModel(
                    resourceMetadataViewModels,
                    LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise.FindwiseRequestStatus.Success,
                    totalHits);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Azure Search query failed");
                return ResourceSearchResultModel.FailedWithStatus(
                    LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise.FindwiseRequestStatus.Timeout);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveResourceFromSearchAsync(int resourceId)
        {
            try
            {
                // We are not currently implementing delete in Azure Search, it is handled via data source indexer
                // var adminClient = AzureSearchClientFactory.CreateAdminClient(this.azureSearchConfig);

                // await adminClient.DeleteDocumentsAsync("id", new[] { resourceId.ToString() });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to remove resource {resourceId} from Azure Search");
                throw new Exception($"Removal of resource from search failed: {resourceId} : {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SendResourceForSearchAsync(SearchResourceRequestModel searchResourceRequestModel, int userId, int? iterations)
        {
            try
            {
                // We are not currently implementing in Azure Search, it is handled via data source indexer

                // var adminClient = AzureSearchClientFactory.CreateAdminClient(this.azureSearchConfig);
                // var document = new Models.ServiceModels.AzureSearch.SearchDocument{};
                // await adminClient.IndexDocumentsAsync(IndexDocumentsBatch.Upload(new[] { document }));
                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Failed to add resource {searchResourceRequestModel.Id} to Azure Search");
                throw new Exception($"Posting of resource to search failed: {searchResourceRequestModel.Id} : {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SendCatalogueSearchEventAsync(SearchActionCatalogueModel searchActionCatalogueModel)
        {
            // We are not currently implementing in Azure Search, it is handled via data source indexer
            return await Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<SearchAllCatalogueResultModel> GetAllCatalogueSearchResultsAsync(AllCatalogueSearchRequestModel catalogSearchRequestModel)
        {
            var viewmodel = new SearchAllCatalogueResultModel();
            CancellationToken cancellationToken = default;
            try
            {
                var offset = catalogSearchRequestModel.PageIndex * catalogSearchRequestModel.PageSize;
                var filters = new Dictionary<string, List<string>>
                {
                     { "resource_collection", new List<string> { "catalogue" } }
                };

                var searchOptions = new SearchOptions
                {
                    Skip = offset,
                    Size = catalogSearchRequestModel.PageSize,
                    IncludeTotalCount = true,
                    Filter = SearchFilterBuilder.BuildFilterExpression(filters)
                };

                SearchResults<Models.ServiceModels.AzureSearch.SearchDocument> response = await this.searchClient.SearchAsync<Models.ServiceModels.AzureSearch.SearchDocument>(
                    catalogSearchRequestModel.SearchText, searchOptions, cancellationToken);

                var documentList = new CatalogueDocumentList
                {
                    Documents = response.GetResults()
                    .Select(result =>
                    {
                        var doc = result.Document;
                        doc.ParseManualTags();

                        return new CatalogueDocument
                        {
                            Id = doc.Id,
                            Name = doc.Title,
                            Description = doc.Description,
                            Click = new SearchClickModel { Payload = new SearchClickPayloadModel { HitNumber = 1 }, Url = "binon" }
                        };
                    })
                    .ToArray()
                };

                viewmodel.DocumentList = documentList;
                viewmodel.Stats = new Stats
                {
                    TotalHits = Convert.ToInt32(response.TotalCount)
                };
                catalogSearchRequestModel.TotalNumberOfHits = viewmodel.Stats.TotalHits;

                return viewmodel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Azure Search all catalogue query failed");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AutoSuggestionModel> GetAutoSuggestionResultsAsync(string term, CancellationToken cancellationToken = default)
        {
            var viewmodel = new AutoSuggestionModel();

            try
            {
                var searchOptions = new SearchOptions
                {
                    Size = 10,
                };

                var response = await this.searchClient.SearchAsync<Models.ServiceModels.AzureSearch.SearchDocument>(
                    term,
                    searchOptions,
                    cancellationToken);

                var suggestOptions = new SuggestOptions
                {
                    Size = 50,
                    UseFuzzyMatching = true
                };
                suggestOptions.SearchFields.Add("title");
                suggestOptions.SearchFields.Add("description");
                suggestOptions.SearchFields.Add("manual_tag");
                suggestOptions.Select.Add("id");
                suggestOptions.Select.Add("title");
                suggestOptions.Select.Add("description");
                suggestOptions.Select.Add("manual_tag");
                suggestOptions.Select.Add("resource_type");
                suggestOptions.Select.Add("resource_collection");

                var autoOptions = new AutocompleteOptions
                {
                    Mode = AutocompleteMode.OneTermWithContext,
                    Size = 50
                };

                var searchText = LuceneQueryBuilder.EscapeLuceneSpecialCharacters(term);
                var suggesterName = this.azureSearchConfig.SuggesterName;

                // Fire both requests in parallel for performance
                var suggestTask = this.searchClient.SuggestAsync<Models.ServiceModels.AzureSearch.SearchDocument>(searchText, suggesterName, suggestOptions, cancellationToken);
                var autoTask = this.searchClient.AutocompleteAsync(searchText, suggesterName, autoOptions, cancellationToken);

                await Task.WhenAll(suggestTask, autoTask);

                var suggestResponse = await suggestTask;
                var autoResponse = await autoTask;

                var suggestResults = suggestResponse.Value.Results
                    .Where(r => !string.IsNullOrEmpty(r.Document?.Title))
                    .Select(r => new
                    {
                        Id = r.Document.Id,
                        Text = r.Document.Title.Trim(),
                        Type = r.Document.ResourceCollection ?? "Suggestion"
                    });

                var autoResults = autoResponse.Value.Results
                    .Where(r => !string.IsNullOrWhiteSpace(r.Text))
                    .Select((r, index) => new
                    {
                        Id = "A" + (index + 1),
                        Text = r.Text.Trim(),
                        Type = "AutoComplete"
                    });

                var combined = suggestResults
                    .Concat(autoResults)
                    .GroupBy(r => r.Text, StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.First())
                    .ToList();

                viewmodel.Stats = new Stats
                {
                    TotalHits = combined.Count
                };

                var autoSuggestionResource = new AutoSuggestionResource
                {
                    TotalHits = suggestResults.Count(),
                    ResourceDocumentList = suggestResults
                        .Where(a => a.Type == "resource")
                        .Select(item => new AutoSuggestionResourceDocument
                        {
                            Id = item.Id?.Substring(1),
                            Title = item.Text,
                            Click = new AutoSuggestionClickModel { Payload = new AutoSuggestionClickPayloadModel { HitNumber = 1 }, Url = "binon" }
                        })
                        .Take(3)
                        .ToList()
                };

                var autoSuggestionCatalogue = new AutoSuggestionCatalogue
                {
                    TotalHits = suggestResults.Count(),
                    CatalogueDocumentList = suggestResults
                        .Where(a => a.Type == "catalogue")
                        .Select(item => new AutoSuggestionCatalogueDocument
                        {
                            Id = item.Id?.Substring(1),
                            Name = item.Text,
                            Click = new AutoSuggestionClickModel { Payload = new AutoSuggestionClickPayloadModel { HitNumber = 1 }, Url = "binon" }
                        })
                        .Take(3)
                        .ToList()
                };

                var autoSuggestionConcept = new AutoSuggestionConcept
                {
                    TotalHits = autoResults.Count(),
                    ConceptDocumentList = autoResults
                        .Select(item => new AutoSuggestionConceptDocument
                        {
                            Id = item.Id?.Substring(1),
                            Concept = item.Text,
                            Title = item.Text,
                            Click = new AutoSuggestionClickModel { Payload = new AutoSuggestionClickPayloadModel { HitNumber = 1 }, Url = "binon" }
                        })
                        .ToList()
                };

                viewmodel.ResourceDocument = autoSuggestionResource;
                viewmodel.CatalogueDocument = autoSuggestionCatalogue;
                viewmodel.ConceptDocument = autoSuggestionConcept;

                return viewmodel;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Azure Search auto-suggestion query failed for term: {Term}", term);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SendAutoSuggestionEventAsync(AutoSuggestionClickPayloadModel clickPayloadModel)
        {
            // We are not currently implementing in Azure Search, it is handled via data source indexer
            return await Task.FromResult(true);
        }

        private async Task<List<ResourceMetadataViewModel>> GetResourceMetadataViewModels(
            List<int> findwiseResourceIds, int? currentUserId)
        {
            List<ResourceActivityDTO> resourceActivities = new List<ResourceActivityDTO>();
            List<ResourceMetadataViewModel> resourceMetadataViewModels = new List<ResourceMetadataViewModel>();

            if (!findwiseResourceIds.Any())
            {
                return new List<ResourceMetadataViewModel>();
            }

            var resourcesFound = await this.resourceRepository.GetResourcesFromIds(findwiseResourceIds);

            if (currentUserId.HasValue)
            {
                List<int> resourceIds = resourcesFound.Select(x => x.Id).ToList();
                List<int> userIds = new List<int> { currentUserId.Value };

                resourceActivities = (await this.resourceRepository.GetResourceActivityPerResourceMajorVersion(resourceIds, userIds))?.ToList() ?? new List<ResourceActivityDTO>();
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
                    "Azure Search returned documents that were not found in the database with IDs: " +
                    unmatchedResourcesIdsString);
            }

            return resourceMetadataViewModels;
        }

        private ResourceMetadataViewModel MapToViewModel(Resource resource, List<ResourceActivityDTO> resourceActivities)
        {
            var hasCurrentResourceVersion = resource.CurrentResourceVersion != null;
            var hasRating = resource.CurrentResourceVersion?.ResourceVersionRatingSummary != null;

            List<MajorVersionIdActivityStatusDescription> majorVersionIdActivityStatusDescription = new List<MajorVersionIdActivityStatusDescription>();

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
                majorVersionIdActivityStatusDescription);
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
