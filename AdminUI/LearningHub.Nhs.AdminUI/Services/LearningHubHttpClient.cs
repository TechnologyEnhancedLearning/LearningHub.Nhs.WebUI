namespace LearningHub.Nhs.AdminUI.Services
{
    using System.Net.Http;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Caching;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The learning hub http client.
    /// </summary>
    public class LearningHubHttpClient : BaseHttpClient, ILearningHubHttpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubHttpClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        /// <param name="webSettings">The web settings.</param>
        /// <param name="client">The http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cache service.</param>
        public LearningHubHttpClient(
            IHttpContextAccessor httpContextAccessor,
            IOptions<WebSettings> webSettings,
            HttpClient client,
            ILogger<LearningHubHttpClient> logger,
            ICacheService cacheService)
            : base(httpContextAccessor, webSettings.Value, client, logger, cacheService)
        {
        }

        /// <summary>
        /// Gets the learning hub api url.
        /// </summary>
        public override string ApiUrl => this.WebSettings.LearningHubApiUrl;
    }
}