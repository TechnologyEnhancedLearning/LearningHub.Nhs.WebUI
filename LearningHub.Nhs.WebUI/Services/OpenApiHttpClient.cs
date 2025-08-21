namespace LearningHub.Nhs.WebUI.Services
{
    using System.Net.Http;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.WebUI.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The open api http client.
    /// </summary>
    public class OpenApiHttpClient : BaseHttpClient, IOpenApiHttpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiHttpClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        /// <param name="webSettings">The web settings.</param>
        /// <param name="authConfig">The auth config.</param>
        /// <param name="client">The http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cache service.</param>
        public OpenApiHttpClient(
            IHttpContextAccessor httpContextAccessor,
            IOptions<Settings> webSettings,
            LearningHubAuthServiceConfig authConfig,
            HttpClient client,
            ILogger<OpenApiHttpClient> logger,
            ICacheService cacheService)
            : base(httpContextAccessor, webSettings.Value, authConfig, client, logger, cacheService)
        {
        }

        /// <summary>
        /// Gets the open api url.
        /// </summary>
        public override string ApiUrl => this.WebSettings.OpenApiUrl;
    }
}
