namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using LearningHub.Nhs.WebUI.Configuration;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// This controller allows proxying of requests to different APIs using same site cookie authentication.
    /// It uses the http clients registered in the DI container.
    /// The BFF (Backend for Frontend) pattern is used to simplify client-side code and centralize API access.
    /// Unauthorized requests will be redirected to the login page so 302s are expected when unauthorized, and redirecting for using a Blazor island component for example may not be desireable so these responses need to be handled by the caller.
    /// This controller is designed to be used with a clientside calls i.e. Blazor utilizing the BFF pattern, which enables same site cookie authentication and avoid the necessity of storing tokens in client storage
    /// The bff prefix is followed by the API name (e.g. "learninghub", "userapi") and the path to the specific endpoint to enable easy routing to different APIs.
    /// See confluence for more details on the BFF pattern and how to use this controller.
    /// </summary>
    [Authorize]
    [Route("bff/{apiName}/{**path}")]
    [ApiController]
    public class BFFController : BaseApiController
    {
        private readonly IOptions<BFFPathValidationOptions> bffPathValidationOptions;

        /// <summary>
        /// The list of API clients that can be used to proxy requests.
        /// </summary>
        private List<IAPIHttpClient> apiClients;

        /// <summary>
        /// Initializes a new instance of the <see cref="BFFController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used for logging.</param>
        /// <param name="learningHubClient">The HTTP client for the Learning Hub API.</param>
        /// <param name="userAPIClient">The HTTP client for the User API.</param>
        /// <param name="openAPIClient">The HTTP client for the Open API.</param>
        /// <param name="settings">The application settings.</param>
        /// <param name="bffPathValidationOptions">The options for validating BFF paths.</param>
        public BFFController(
            ILogger<BFFController> logger,
            ILearningHubHttpClient learningHubClient,
            IUserApiHttpClient userAPIClient,
            IOpenApiHttpClient openAPIClient,
            IOptions<BFFPathValidationOptions> bffPathValidationOptions)
            : base(logger)
        {
            // qqqq should i be using the httpfactory -  i think no
            // services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>();
            // services.AddHttpClient<IUserApiHttpClient, UserApiHttpClient>();
            // services.AddHttpClient<ILearningHubReportApiClient, LearningHubReportApiClient>();
            // services.AddHttpClient<IMoodleHttpClient, MoodleHttpClient>();
            this.apiClients = new List<IAPIHttpClient>()
            {
                learningHubClient,
                userAPIClient,
                openAPIClient,
            };

            this.bffPathValidationOptions = bffPathValidationOptions;
        }

        /// <summary>
        /// Takes an API name and a path, and proxies the request to the appropriate API provided that api is part of the client list and the path is allowed and not blocked.
        /// </summary>
        /// <param name="apiName">The name of the API to which the request should be proxied.</param>
        /// <param name="path">The path of the endpoint within the specified API.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the proxied request.</returns>
        [HttpGet]
        [HttpPost]
        [HttpPut]
        [HttpDelete]
        [HttpPatch]
        public async Task<IActionResult> ProxyRequest(string apiName, string path)
        {
            // qqqq
            // we want to do this - oh but it wont be in domain!
            // it will be https://lh-web.dev.local/bff/ the api name so we need to set the name the same ... it doesnt need to be the same but may aswell
            // the other local ones are https://lh-web.dev.local/api/
            // https://lh-openapi.dev.local/
            // "LearningHubUrl": "https://lh-web.dev.local/",
            // "ELfhHubUrl": "https://test-portal.e-lfhtech.org.uk ",
            // "LearningHubApiUrl": "https://lh-api.dev.local/api/",
            // "UserApiUrl": "https://lh-userapi.dev.local/api/",
            // "LearningHubAdminUrl": "https://lh-admin.dev.local/",
            string sanitizedPath = path?.Trim('/').ToLowerInvariant() ?? string.Empty;
            string sanitizedApiName = apiName?.Trim('/').ToLowerInvariant() ?? string.Empty;

            IAPIHttpClient apiClient;
            try
            {
                // qqqq qqqq !!!! need single end point to test
                // ApiUrl = "https://lh-api.dev.local/api/"
                // "lh-api.dev.local"
                apiClient = this.apiClients.Single(x =>
                {
                    try
                    {
                        var uri = new Uri(x.ApiUrl);
                        return uri.Host.ToLowerInvariant() == sanitizedApiName;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                return this.BadRequest($"Unknown API alias: {sanitizedApiName}");
            }

            // api/catalogue/getlatestcatalogueaccessrequest/500 qqqq
            if (!this.IsPathAllowed(sanitizedPath))
            {
                return this.Forbid("This path is not allowed via BFF proxy.");
            }

            var client = await apiClient.GetClientAsync();
            string targetUrl = $"{apiClient.ApiUrl.TrimEnd('/')}/{path}";

            // Add query parameters from the original request
            if (this.Request.QueryString.HasValue)
            {
                targetUrl += this.Request.QueryString.Value;
            }

            // No headers added becaue all security is handled by host httpclients via baseclient
            // Copy body if necessary (for POST, PUT, PATCH, etc.)
            var method = new HttpMethod(this.Request.Method);
            var requestMessage = new HttpRequestMessage(method, targetUrl);

            if (this.Request.ContentLength > 0 &&
                !string.Equals(this.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(this.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase))
            {
                requestMessage.Content = new StreamContent(this.Request.Body);
                if (!string.IsNullOrEmpty(this.Request.ContentType))
                {
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(this.Request.ContentType);
                }
            }

            try
            {
                var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                // Handle redirects with token preservation
                // if we are redirected the client may not handle it as it isnt the token holder so we need to continue using the bff until we get the outcome
                // qqqq we would avoid hitting authorization because we dont want to redirect the component to a page its the mvc that would want redirecting, the mvc page to another mvc page. So we may never need this
                if (response.StatusCode == System.Net.HttpStatusCode.Redirect ||
                    response.StatusCode == System.Net.HttpStatusCode.Found ||
                    response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect ||
                    response.StatusCode == System.Net.HttpStatusCode.PermanentRedirect)
                {
                    return await this.HandleRedirect(response, apiClient);
                }

                var content = await response.Content.ReadAsStringAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";

                return new ContentResult
                {
                    Content = content,
                    ContentType = contentType,
                    StatusCode = (int)response.StatusCode,
                };
            }
            catch (HttpRequestException ex)
            {
                this.Logger.LogError(ex, "Error proxying request to {TargetUrl}", targetUrl);
                return this.StatusCode(500, "An error occurred while processing the request.");
            }
        }

        // qqqq make sure this gets tested
        private async Task<IActionResult> HandleRedirect(HttpResponseMessage response, IAPIHttpClient apiClient)
        {
            var location = response.Headers.Location?.ToString();

            if (string.IsNullOrEmpty(location))
            {
                return this.StatusCode((int)response.StatusCode, "Redirect location not found");
            }

            // Check if the redirect location is relative or absolute
            string redirectUrl;
            if (Uri.IsWellFormedUriString(location, UriKind.Absolute))
            {
                redirectUrl = location;
            }
            else
            {
                // Handle relative redirects
                var baseUri = new Uri(apiClient.ApiUrl);
                redirectUrl = new Uri(baseUri, location).ToString();
            }

            // Create a new request for the redirect
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, redirectUrl);

            // Add authentication token to the redirect request (apiClient handles this)
            // No additional headers needed - apiClient is already configured
            try
            {
                var client = await apiClient.GetClientAsync();
                var redirectResponse = await client.SendAsync(redirectRequest);
                var content = await redirectResponse.Content.ReadAsStringAsync();
                var contentType = redirectResponse.Content.Headers.ContentType?.MediaType ?? "application/json";

                return new ContentResult
                {
                    Content = content,
                    ContentType = contentType,
                    StatusCode = (int)redirectResponse.StatusCode,
                };
            }
            catch (HttpRequestException ex)
            {
                this.Logger.LogError(ex, "Error following redirect to {RedirectUrl}", redirectUrl);
                return this.StatusCode(500, "An error occurred while following the redirect.");
            }
        }

        /// <summary>
        /// Validates the path against allowed and blocked segments.
        /// </summary>
        private bool IsPathAllowed(string path)
        {
            var normalizedPath = path?.Trim('/').ToLowerInvariant() ?? string.Empty;

            // Check blacklist first
            if (this.bffPathValidationOptions.Value.BlockedPathSegments.Any(blocked => normalizedPath.Contains(blocked.ToLowerInvariant())))
            {
                return false;
            }

            // Check whitelist
            return this.bffPathValidationOptions.Value.AllowedPathPrefixes.Any(prefix => normalizedPath.StartsWith(prefix.ToLowerInvariant()));
        }
    }
}
