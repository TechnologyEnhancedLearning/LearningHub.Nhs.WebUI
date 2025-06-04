namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Resource.ResourceDisplay;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The card service.
    /// </summary>
    public class CardService : BaseService<CardService>, ICardService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CardService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The Learning Hub Http Client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">The logger.</param>
        public CardService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<CardService> logger)
             : base(learningHubHttpClient, openApiHttpClient, logger)
        {
        }

        /// <summary>
        /// Get contributions totals based on catalogue.
        /// </summary>
        /// <param name="catalogueId">The catalogue Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyContributionsTotalsViewModel> GetMyContributionsTotalsAsync(int catalogueId)
        {
            MyContributionsTotalsViewModel totals = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetMyContributionsTotals/{catalogueId.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                totals = JsonConvert.DeserializeObject<MyContributionsTotalsViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return totals;
        }

        /// <summary>
        /// Get contributions based on catalogue.
        /// </summary>
        /// <param name="resourceContributionsRequestViewModel">The resource contributions request viewmodel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<ContributedResourceCardViewModel>> GetContributionsAsync(ResourceContributionsRequestViewModel resourceContributionsRequestViewModel)
        {
            List<ContributedResourceCardViewModel> myContributionCards = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var json = JsonConvert.SerializeObject(resourceContributionsRequestViewModel);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var request = $"Resource/GetContributions";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                myContributionCards = JsonConvert.DeserializeObject<List<ContributedResourceCardViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return myContributionCards;
        }

        /// <summary>
        /// Get my resource.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<MyResourceViewModel> GetMyResourceViewModelAsync()
        {
            MyResourceViewModel myresourcecards = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/GetMyResourceViewModel";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                myresourcecards = JsonConvert.DeserializeObject<MyResourceViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return myresourcecards;
        }

        /// <summary>
        /// Get resource for extended card.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceCardExtendedViewModel> GetResourceCardExtendedViewModelAsync(int id)
        {
            ResourceCardExtendedViewModel resourceCardExtendedViewModel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Resource/ResourceCardExtendedViewModel/{id.ToString()}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                resourceCardExtendedViewModel = JsonConvert.DeserializeObject<ResourceCardExtendedViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return resourceCardExtendedViewModel;
        }
    }
}
