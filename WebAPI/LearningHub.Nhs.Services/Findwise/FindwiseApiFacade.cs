namespace LearningHub.Nhs.Services.Findwise
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Search;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    /// <summary>
    /// The FindwiseApiFacade.
    /// </summary>
    public class FindwiseApiFacade : IFindwiseApiFacade
    {
        private readonly IFindWiseHttpClient findWiseHttpClient;
        private readonly Settings settings;
        private readonly ILogger<FindwiseApiFacade> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindwiseApiFacade"/> class.
        /// </summary>
        /// <param name="findWiseHttpClient">The findWiseHttpClient.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public FindwiseApiFacade(
            IFindWiseHttpClient findWiseHttpClient,
            IOptions<Settings> options,
            ILogger<FindwiseApiFacade> logger)
        {
            this.findWiseHttpClient = findWiseHttpClient;
            this.settings = options.Value;
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
            var request = string.Format(this.settings.Findwise.IndexMethod, this.settings.Findwise.CollectionIds.Catalogue)
                + $"?token={this.settings.Findwise.Token}";
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
            var request = string.Format(this.settings.Findwise.IndexMethod, this.settings.Findwise.CollectionIds.Resource)
                + $"?token={this.settings.Findwise.Token}";
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
                this.settings.Findwise.IndexMethod,
                this.settings.Findwise.CollectionIds.Resource)
                + $"?{idQueryString}&token={this.settings.Findwise.Token}";
            var response = await this.DeleteAsync(request);
            this.ValidateResponse(response, $"resources: {string.Join(',', resourceIds)}");
        }

        private void ValidateResponse(HttpResponseMessage response, string dataForError)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                this.logger.LogError($"Updating FindWise failed for {dataForError}  HTTP Status Code:" + response.StatusCode.ToString());
                throw new Exception("AccessDenied");
            }
            else if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError($"Updating FindWise failed for {dataForError}  HTTP Status Code:" + response.StatusCode.ToString());
                throw new Exception("Posting of resource to search failed: " + dataForError);
            }
        }

        private async Task<HttpResponseMessage> PostAsync<T>(string request, T obj)
            where T : class, new()
        {
            var content = this.GetContent(obj);
            var client = await this.GetClientAsync();
            return await client.PostAsync(request, content).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> DeleteAsync(string request)
        {
            var client = await this.GetClientAsync();
            return await client.DeleteAsync(request).ConfigureAwait(false);
        }

        private async Task<HttpClient> GetClientAsync()
        {
            return await this.findWiseHttpClient.GetClient(this.settings.Findwise.IndexUrl);
        }

        private StringContent GetContent<T>(T obj)
            where T : class, new()
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { DateFormatString = "yyyy-MM-dd" });

            return new StringContent(json, UnicodeEncoding.UTF8, "application/json");
        }
    }
}
