namespace LearningHub.Nhs.WebUI.Middleware
{
    using System.Threading.Tasks;
    using AspNetCoreRateLimit;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Defines the <see cref="LHIPRateLimitMiddleware" />.
    /// </summary>
    public class LHIPRateLimitMiddleware : IpRateLimitMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LHIPRateLimitMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="processingStrategy">The processingStrategy.</param>
        /// <param name="options">The options.</param>
        /// <param name="policyStore">The policyStore.</param>
        /// <param name="config">The config.</param>
        /// <param name="logger">The logger.</param>
        public LHIPRateLimitMiddleware(
            RequestDelegate next,
            IProcessingStrategy processingStrategy,
            IOptions<IpRateLimitOptions> options,
            IIpPolicyStore policyStore,
            IRateLimitConfiguration config,
            ILogger<IpRateLimitMiddleware> logger)
            : base(
                  next,
                  processingStrategy,
                  options,
                  policyStore,
                  config,
                  logger)
        {
        }

        /// <summary>
        /// The ReturnQuotaExceededResponse method.
        /// </summary>
        /// <param name="httpContext">The httpContext.</param>
        /// <param name="rule">The rule.</param>
        /// <param name="retryAfter">The retryAfter.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task ReturnQuotaExceededResponse(
            HttpContext httpContext,
            RateLimitRule rule,
            string retryAfter)
        {
            httpContext.Response.Headers["Location"] = "/TooManyRequests";
            httpContext.Response.StatusCode = 302;
            return httpContext.Response.WriteAsync(string.Empty);
        }
    }
}
