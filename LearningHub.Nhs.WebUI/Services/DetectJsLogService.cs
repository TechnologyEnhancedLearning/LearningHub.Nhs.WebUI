namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines the <see cref="DetectJsLogService" />.
    /// </summary>
    public class DetectJsLogService : BaseService<DetectJsLogService>, IDetectJsLogService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetectJsLogService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="openApiHttpClient">The Open Api Http Client.</param>
        /// <param name="logger">Logger.</param>
        public DetectJsLogService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<DetectJsLogService> logger)
          : base(learningHubHttpClient, openApiHttpClient, logger)
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