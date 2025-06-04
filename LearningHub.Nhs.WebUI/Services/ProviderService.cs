namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ProviderService" />.
    /// </summary>
    public class ProviderService : BaseService<ProviderService>, IProviderService
    {
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderService"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">Logger.</param>
        public ProviderService(ICacheService cacheService, ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<ProviderService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
        {
            this.cacheService = cacheService;
        }

        /// <summary>
        /// Get Providers.
        /// </summary>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        public async Task<List<ProviderViewModel>> GetProviders()
        {
            return await this.cacheService.GetOrFetchAsync("AllProviders", () => this.GetAllProviders());
        }

        /// <summary>
        /// Get providers for the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        public async Task<List<ProviderViewModel>> GetProvidersForUserAsync(int userId)
        {
            List<ProviderViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Provider/GetProvidersByUserId/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ProviderViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// Get providers for the resource.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        public async Task<List<ProviderViewModel>> GetProvidersForResourceAsync(int resourceVersionId)
        {
            List<ProviderViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Provider/GetProvidersByResource/{resourceVersionId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ProviderViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// Get provider by id.
        /// </summary>
        /// <param name="providerId">The provider id.</param>
        /// <returns>The <see cref="Task{ProviderViewModel}"/>.</returns>
        public ProviderViewModel GetProviderById(int providerId)
        {
            var providers = this.GetProviders().Result;
            return providers.Where(n => n.Id == providerId).FirstOrDefault();
        }

        private async Task<List<ProviderViewModel>> GetAllProviders()
        {
            List<ProviderViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Provider/all";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<ProviderViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
