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
    /// The BFF (Backend for Frontend) pattern is used to simplify client-side code and centralize API access, application services directly call external apis currently but they could use the bff and then introduce caching there too potentially for seperation of infastructure concerns.
    /// Unauthorized requests will be redirected to the login page so 302s are expected when unauthorized, and redirecting for using a Blazor island component for example may not be desireable so these responses need to be handled by the caller.
    /// This controller is designed to be used with a clientside calls i.e. Blazor utilizing the BFF pattern, which enables same site cookie authentication and avoid the necessity of storing tokens in client storage
    /// The bff prefix is followed by the API name (e.g. "learninghub", "userapi") and the path to the specific endpoint to enable easy routing to different APIs.
    /// See confluence for more details on the BFF pattern and how to use this controller.
    /// </summary>
    /// The authorize same site cookie is used for security between client and server. API calls relying on policys such as AuthorizeOrCallFromLH may not be proxied as they require the Authorization header to be present.
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
        /// <param name="bffPathValidationOptions">The options for validating BFF paths.</param>
        public BFFController(
            ILogger<BFFController> logger,
            ILearningHubHttpClient learningHubClient,
            IUserApiHttpClient userAPIClient,
            IOpenApiHttpClient openAPIClient,
            IOptions<BFFPathValidationOptions> bffPathValidationOptions)
            : base(logger)
        {
            // Clients the BFF is being given access to, these are the only clients that can be used to proxy requests.
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
            string sanitizedPath = path?.Trim('/').ToLowerInvariant() ?? string.Empty;
            string sanitizedApiName = apiName?.Trim('/').ToLowerInvariant() ?? string.Empty;

            IAPIHttpClient apiClient;
            try
            {
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
                this.Logger.LogError(e, "Failed to find API client for {ApiName}", sanitizedApiName);
                return this.BadRequest($"Unknown API alias: {sanitizedApiName}");
            }

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

            /*
                 No headers for Auth, host, connection, user agent, added becaue all security is handled by serverside httpclients via baseclient
                 BaseHttpClient should handle content-type, timezone and tokens.
                 Note: We do not forward the Authorization header as the BFF pattern uses same-site cookies for authentication.
                 This means the BFF controller is responsible for handling authentication and authorization.
                 We also do not forward the Host header as it may not match the target API's expected host.
                 Header copying would only be needed if: APIs start checking for custom client headers (X-Custom-Header, X-Correlation-Id, etc.)
            */

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

        /*
         Handle redirects with token preservation
         if we are redirected the client may not handle it as it isnt the token holder so we need to continue using the bff until we get the outcome
         if the BFF caller is not expecting redirects but only data they should handle the 302 response and redirect themselves.
         E.g. A compontent that uses the BFF to fetch data may not be appropriate for redirecting to a specific page so the consuming client may need to have a way of handling page redirects.
        */
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

                // Our data apis are expected to return JSON, but we can handle other content types if necessary.
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
                this.Logger.LogError(" Black listed path {path} was requested and blocked", normalizedPath);
                return false;
            }

            // Check whitelist
            return this.bffPathValidationOptions.Value.AllowedPathPrefixes.Any(prefix => normalizedPath.StartsWith(prefix.ToLowerInvariant()));
        }
    }
}
