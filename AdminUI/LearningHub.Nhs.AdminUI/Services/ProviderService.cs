namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Helpers;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Models.Validation;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="ProviderService" />.
    /// </summary>
    public class ProviderService : BaseService, IProviderService
    {
        private readonly ICacheService cacheService;

        /// <summary>
        /// Defines the _facade.
        /// </summary>
        private readonly IOpenApiFacade facade;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderService"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">open api http client.</param>
        /// <param name="openApiFacade">The openApiFacade<see cref="IOpenApiFacade"/>.</param>
        public ProviderService(ICacheService cacheService, ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, IOpenApiFacade openApiFacade)
        : base(learningHubHttpClient)
        {
            this.cacheService = cacheService;
            this.facade = openApiFacade;
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetProviders()
        {
            return await this.cacheService.GetOrFetchAsync("AllProviders", () => this.GetAllProviders());
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetProvidersByUserIdAsync(int userId)
        {
            return await this.facade.GetAsync<List<ProviderViewModel>>($"Provider/GetProvidersByUserId/{userId}");
        }

        /// <inheritdoc />
        public async Task<LearningHubValidationResult> UpdateUserProviderAsync(int userId, string providerIdList)
        {
            var userProviderUpdateModel = new UserProviderUpdateViewModel();
            userProviderUpdateModel.UserId = userId;
            userProviderUpdateModel.ProviderIds = providerIdList?.Split(',').Select(int.Parse).ToList();

            var json = JsonConvert.SerializeObject(userProviderUpdateModel);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Provider/UpdateUserProvider";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        private async Task<List<ProviderViewModel>> GetAllProviders()
        {
            return await this.facade.GetAsync<List<ProviderViewModel>>($"Provider/All");
        }
    }
}
