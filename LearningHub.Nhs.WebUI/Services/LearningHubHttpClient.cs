namespace LearningHub.Nhs.WebUI.Services
{
    using System.Net.Http;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The learning hub http client.
    /// </summary>
    public class LearningHubHttpClient : BaseHttpClient, ILearningHubHttpClient // qqqq
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubHttpClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        /// <param name="webSettings">The web settings.</param>
        /// <param name="authConfig">The auth config.</param>
        /// <param name="client">The http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cache service.</param>
        public LearningHubHttpClient(
            IHttpContextAccessor httpContextAccessor,
            IOptions<Settings> webSettings,
            LearningHubAuthServiceConfig authConfig,
            HttpClient client,
            ILogger<LearningHubHttpClient> logger,
            ICacheService cacheService)
            : base(httpContextAccessor, webSettings.Value, authConfig, client, logger, cacheService)
        {
        }

        /// <summary>
        /// Gets the learning hub api url.
        /// </summary>
        public override string ApiUrl => this.WebSettings.LearningHubApiUrl;
    }
}
