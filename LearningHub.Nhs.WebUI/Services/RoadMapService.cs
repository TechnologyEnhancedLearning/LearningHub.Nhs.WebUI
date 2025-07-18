namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.RoadMap;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.Shared.Services;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="RoadMapService" />.
    /// </summary>
    public class RoadMapService : BaseService<RoadMapService>, IRoadMapService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoadMapService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learing hub http client.</param>
        /// <param name="logger">Logger.</param>
        public RoadMapService(ILearningHubHttpClient learningHubHttpClient, ILogger<RoadMapService> logger)
        : base(learningHubHttpClient, logger)
        {
        }

        /// <summary>
        /// The GetUpdatesAsync.
        /// </summary>
        /// <param name="numberOfResults">numberOfResults.</param>
        /// <returns>The RoadMapResponseViewModel.</returns>
        public async Task<RoadMapResponseViewModel> GetUpdatesAsync(int numberOfResults)
        {
            var viewmodel = new RoadMapResponseViewModel();

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"Roadmap/Updates/{numberOfResults}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<RoadMapResponseViewModel>(result);
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
