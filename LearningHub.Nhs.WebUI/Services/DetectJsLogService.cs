namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;
    using LearningHub.Nhs.WebUI.Shared.Interfaces;
    using LearningHub.Nhs.WebUI.Shared.Services;
    using LearningHub.Nhs.WebUI.Shared.Models;
    /// <summary>
    /// Defines the <see cref="DetectJsLogService" />.
    /// </summary>
    public class DetectJsLogService : BaseService<DetectJsLogService>, IDetectJsLogService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetectJsLogService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        public DetectJsLogService(ILearningHubHttpClient learningHubHttpClient, ILogger<DetectJsLogService> logger)
        : base(learningHubHttpClient, logger)
        {
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(long jsEnabled, long jsDisabled)
        {
            var client = await this.LearningHubHttpClient.GetClientAsync();

            var url = $"DetectJsLog/Update?jsEnabled={jsEnabled}&jsDisabled={jsDisabled}";

            var response = await client.PostAsync(url, null).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
        }
    }
}