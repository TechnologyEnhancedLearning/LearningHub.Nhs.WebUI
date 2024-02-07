// <copyright file="FindwiseClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.HttpClients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;

    /// <summary>
    /// The FindWise Http Client.
    /// </summary>
    public sealed class FindwiseClient : IFindwiseClient
    {
        private readonly FindwiseConfig findwiseConfig;
        private readonly ILogger<FindwiseClient> logger;

        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindwiseClient"/> class.
        /// </summary>
        /// <param name="findwiseConfig">Config.</param>
        /// <param name="logger">No info.</param>
        public FindwiseClient(
            IOptions<FindwiseConfig> findwiseConfig,
            ILogger<FindwiseClient> logger)
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri(findwiseConfig.Value.SearchBaseUrl) };

            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            this.logger = logger;
            this.findwiseConfig = findwiseConfig.Value;
        }

        /// <inheritdoc />
        public async Task<FindwiseResultModel> Search(ResourceSearchRequest searchRequestModel)
        {
            var endpointPath = this.findwiseConfig.SearchEndpointPath + this.findwiseConfig.CollectionIds.Resource;
            var queryParams = this.GetQueryParams(searchRequestModel);
            var requestUrl = QueryHelpers.AddQueryString(endpointPath, queryParams);

            try
            {
                var response = await this.httpClient.GetAsync(requestUrl).ConfigureAwait(false);

                if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                {
                    return FindwiseResultModel.Failure(FindwiseRequestStatus.AccessDenied);
                }

                if (!response.IsSuccessStatusCode)
                {
                    this.logger.LogError("FindWise search failed; HTTP Status Code: {Code}", response.StatusCode);

                    return FindwiseResultModel.Failure(FindwiseRequestStatus.BadRequest);
                }

                var result = await response.Content.ReadAsStringAsync();

                return FindwiseResultModel.Success(JsonConvert.DeserializeObject<SearchResultModel>(result));
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Error occurred on request to Findwise.");

                switch (e)
                {
                    case HttpRequestException:
                        return FindwiseResultModel.Failure(FindwiseRequestStatus.RequestException);

                    case TaskCanceledException:
                        return FindwiseResultModel.Failure(FindwiseRequestStatus.Timeout);

                    case InvalidOperationException:
                        return FindwiseResultModel.Failure(FindwiseRequestStatus.BadRequest);

                    default:
                        throw;
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.httpClient.Dispose();
        }

        private Dictionary<string, StringValues> GetQueryParams(ResourceSearchRequest searchRequest)
        {
            var parameters = new Dictionary<string, StringValues>
            {
                { "resource_type", searchRequest.ResourceTypes.Select(HttpUtility.UrlEncode).ToArray() },
                { "token", this.findwiseConfig.Token },
                { "q", this.EscapeSpecialCharactersForFindwise(searchRequest.SearchText) },
                { "hits", searchRequest.Limit.ToString() },
                { "offset", searchRequest.Offset.ToString() },
            };

            if (searchRequest.CatalogueId.HasValue)
            {
                parameters.Add("catalogue_ids", searchRequest.CatalogueId.ToString());
            }

            return parameters;
        }

        private string EscapeSpecialCharactersForFindwise(string searchText)
        {
            // Ensure backslash is at the start of the special characters list
            var specialSearchCharacters = @"\" + this.findwiseConfig.SpecialSearchCharacters.Replace(@"\", null);

            foreach (var specialCharacter in specialSearchCharacters)
            {
                searchText = searchText.Replace(
                    specialCharacter.ToString(),
                    @"\" + specialCharacter);
            }

            searchText = HttpUtility.UrlEncode(searchText);
            return searchText;
        }
    }
}
