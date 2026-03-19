namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.JiraRoadmap;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="JiraRoadmapService" />.
    /// </summary>
    public class JiraRoadmapService : BaseService<JiraRoadmapService>, IJiraRoadmapService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JiraRoadmapService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="openApiHttpClient">The openApiHttpClient<see cref="IOpenApiHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{JiraRoadmapService}"/>.</param>
        public JiraRoadmapService(
            ILearningHubHttpClient learningHubHttpClient,
            IOpenApiHttpClient openApiHttpClient,
            ILogger<JiraRoadmapService> logger)
            : base(learningHubHttpClient, openApiHttpClient, logger)
        {
        }

        /// <inheritdoc />
        public async Task<RoadmapResponseDto> GetPublicRoadmapIssues()
        {
            var client = await this.OpenApiHttpClient.GetClientAsync();
            var request = "JiraRoadmap/GetRoadmapIssues";
            var response = await client.GetAsync(request).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            RoadmapResponseDto roadmapResponse = new RoadmapResponseDto();
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                roadmapResponse = JsonConvert.DeserializeObject<RoadmapResponseDto>(result);
            }

            return roadmapResponse;
        }
    }
}