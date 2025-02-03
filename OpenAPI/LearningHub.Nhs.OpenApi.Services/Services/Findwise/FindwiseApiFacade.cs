namespace LearningHub.Nhs.OpenApi.Services.Services.Findwise
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The FindwiseApiFacade.
    /// </summary>
    public class FindwiseApiFacade : IFindwiseApiFacade
    {
        private readonly IFindwiseClient findWiseHttpClient;
        private readonly FindwiseConfig findwiseConfig;
        private readonly ILogger<FindwiseApiFacade> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindwiseApiFacade"/> class.
        /// </summary>
        /// <param name="findWiseHttpClient">The findWiseHttpClient.</param>
        /// <param name="findwiseConfig">The options.</param>
        /// <param name="logger">The logger.</param>
        public FindwiseApiFacade(
             IFindwiseClient findWiseHttpClient,
             IOptions<FindwiseConfig> findwiseConfig,
             ILogger<FindwiseApiFacade> logger)
        {
            this.findWiseHttpClient = findWiseHttpClient;
            this.findwiseConfig = findwiseConfig.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Modifies the information Findwise has for the catalogues provided.
        /// Documents not in Findwise will be added.
        /// Documents that already exist in Findwise will be replaced.
        /// </summary>
        /// <param name="catalogues">The catalogues to add/replace in the index.</param>
        /// <returns>The task.</returns>
        public async Task AddOrReplaceAsync(List<SearchCatalogueRequestModel> catalogues)
        {
            var request = string.Format(this.findwiseConfig.IndexMethod, this.findwiseConfig.CollectionIds.Catalogue)
                + $"?token={this.findwiseConfig.Token}";
            var response = await this.PostAsync(request, catalogues);
            this.ValidateResponse(response, $"catalogues: {string.Join(',', catalogues.Select(x => x.Id))}");
        }

        /// <summary>
        /// Modifies the information Findwise has for the resources provided.
        /// Documents not in Findwise will be added.
        /// Documents that already exist in Findwise will be replaced.
        /// </summary>
        /// <param name="resources">The resources to add/replace in the index.</param>
        /// <returns>The task.</returns>
        public async Task AddOrReplaceAsync(List<SearchResourceRequestModel> resources)
        {
            var request = string.Format(this.findwiseConfig.IndexMethod, this.findwiseConfig.CollectionIds.Resource)
                + $"?token={this.findwiseConfig.Token}";
            var response = await this.PostAsync(request, resources);
            this.ValidateResponse(response, $"resources: {string.Join(',', resources.Select(x => x.Id))}");
        }

        /// <summary>
        /// Removes the documents from Findwise.
        /// </summary>
        /// <param name="resources">The resources to remove from Findwise.</param>
        /// <returns>The task.</returns>
        public async Task RemoveAsync(List<SearchResourceRequestModel> resources)
        {
            var resourceIds = resources.Select(x => x.Id);
            var idQueryString = string.Join(
                '&',
                resourceIds.Select(x => $"id={x}"));
            var request = string.Format(
                this.findwiseConfig.IndexMethod,
                this.findwiseConfig.CollectionIds.Resource)
                + $"?{idQueryString}&token={this.findwiseConfig.Token}";
            var response = await this.DeleteAsync(request);
            this.ValidateResponse(response, $"resources: {string.Join(',', resourceIds)}");
        }

        private void ValidateResponse(HttpResponseMessage response, string dataForError)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                logger.LogError($"Updating FindWise failed for {dataForError}  HTTP Status Code:" + response.StatusCode.ToString());
                throw new Exception("AccessDenied");
            }
            else if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Updating FindWise failed for {dataForError}  HTTP Status Code:" + response.StatusCode.ToString());
                throw new Exception("Posting of resource to search failed: " + dataForError);
            }
        }

        private async Task<HttpResponseMessage> PostAsync<T>(string request, T obj)
            where T : class, new()
        {
            var content = GetContent(obj);
            var client = await GetClientAsync();
            return await client.PostAsync(request, content).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> DeleteAsync(string request)
        {
            var client = await GetClientAsync();
            return await client.DeleteAsync(request).ConfigureAwait(false);
        }

        private async Task<HttpClient> GetClientAsync()
        {
            return await this.findWiseHttpClient.GetClient(this.findwiseConfig.IndexUrl);
        }

        private StringContent GetContent<T>(T obj)
            where T : class, new()
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd" });

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
