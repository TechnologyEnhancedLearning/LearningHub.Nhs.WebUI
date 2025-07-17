using LearningHub.Nhs.Models.Entities.Reporting;
using LearningHub.Nhs.Services;
using LearningHub.Nhs.Services.Interface;
using LearningHub.Nhs.WebUI.BlazorPageHosting;
using LearningHub.Nhs.WebUI.Configuration;
using LearningHub.Nhs.WebUI.Interfaces;
using LearningHub.Nhs.WebUI.Services;
using LearningHub.Nhs.WebUI.Shared.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    //[Route("api/[controller]")]
    [Authorize] //may get 302s may need httpclient to handle these in blazor
    [Route("bff/{apiName}/{**path}")]
    [ApiController]
    public class BFFController : BaseApiController
    {
        //public List<BaseHttpClient> apiClients;
        public List<IAPIHttpClient> apiClients;
        private readonly IOptions<Settings> _settings;
        /// <summary>
        /// Initializes a new instance of the <see cref="BFFController"/> class.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="settings"></param>
        /// //qqqq should i be using the interfaces if so need to add basehttpclient to them
        public BFFController(ILogger<BFFController> logger,
            ILearningHubHttpClient learningHubClient,
            IUserApiHttpClient userApiClient,
            IOptions<Settings> settings)
        : base(logger)
        {
            // qqqq should i be using the httpfactory
            //services.AddHttpClient<ILearningHubHttpClient, LearningHubHttpClient>();
            //services.AddHttpClient<IUserApiHttpClient, UserApiHttpClient>();
            //services.AddHttpClient<ILearningHubReportApiClient, LearningHubReportApiClient>();
            //services.AddHttpClient<IMoodleHttpClient, MoodleHttpClient>();
            // looks like nowhere is using them and i prefer specific clients
            //     apiClients = new List<BaseHttpClient>()
            apiClients = new List<IAPIHttpClient>()
            {
                learningHubClient,
                userApiClient
            };

            _settings = settings;
        }

        //qqqq these are just examples at the moment and maybe they should be in some configuration
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
        public async Task<IActionResult> ProxyRequest(string apiName, string path)
        {

            //we want to do this - oh but it wont be in domain!
            // it will be https://lh-web.dev.local/bff/ the api name so we need to set the name the same ... it doesnt need to be the same but may aswell
            // the other local ones are https://lh-web.dev.local/api/

            //https://lh-openapi.dev.local/




            //"LearningHubUrl": "https://lh-web.dev.local/",
            //"ELfhHubUrl": "https://test-portal.e-lfhtech.org.uk ",
            //"LearningHubApiUrl": "https://lh-api.dev.local/api/",
            //"UserApiUrl": "https://lh-userapi.dev.local/api/",
            //"LearningHubAdminUrl": "https://lh-admin.dev.local/",

            string sanitizedPath = path?.Trim('/').ToLowerInvariant() ?? string.Empty;
            string sanitizedApiName = apiName?.Trim('/').ToLowerInvariant() ?? string.Empty;//qqqq or uri method eitherway needs to be standardised

            //BaseHttpClient apiClient;
            IAPIHttpClient apiClient;
            try 
            {
                // qqqq qqqq !!!! need single end point to test
                //ApiUrl = "https://lh-api.dev.local/api/"
                //"lh-api.dev.local"
                //apiClient = apiClients.Single(x => x.ApiUrl == sanitizedApiName);
                apiClient = apiClients.Single(x =>
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
            catch(Exception e)
            {
                return BadRequest($"Unknown API alias: {sanitizedApiName}");
            }
            //api/catalogue/getlatestcatalogueaccessrequest/500
            if (!IsPathAllowed(sanitizedPath))
            {
                return Forbid("This path is not allowed via BFF proxy.");
            }
            var client = await apiClient.GetClientAsync();
            string targetUrl = $"{apiClient.ApiUrl.TrimEnd('/')}/{path}";
            // Add query parameters from the original request
            if (Request.QueryString.HasValue)
            {
                targetUrl += Request.QueryString.Value;
            }
            // No headers added becaue all security is handled by host httpclients via baseclient
            // Copy body if necessary (for POST, PUT, PATCH, etc.)

            var method = new HttpMethod(Request.Method);
            var requestMessage = new HttpRequestMessage(method, targetUrl);

            if (Request.ContentLength > 0 &&
                !string.Equals(Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase))
            {
                requestMessage.Content = new StreamContent(Request.Body);
                if (!string.IsNullOrEmpty(Request.ContentType))
                {
                    requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(Request.ContentType);
                }
            }

            try
            {
                var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

                // Handle redirects with token preservation
                // if we are redirected the client may not handle it as it isnt the token holder so we need to continue using the bff until we get the outcome
                // qqqq we would avoid hitting authorization because we dont want to redirect the component to a page its the mvc that would want redirecting, the mvc page to another mvc page.
                // we may never need this

                if (response.StatusCode == System.Net.HttpStatusCode.Redirect ||
                    response.StatusCode == System.Net.HttpStatusCode.Found ||
                    response.StatusCode == System.Net.HttpStatusCode.TemporaryRedirect ||
                    response.StatusCode == System.Net.HttpStatusCode.PermanentRedirect)
                {
                    return await HandleRedirect(response, apiClient);
                }

                var content = await response.Content.ReadAsStringAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/json";

                return new ContentResult
                {
                    Content = content,
                    ContentType = contentType,
                    StatusCode = (int)response.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "Error proxying request to {TargetUrl}", targetUrl);
                return StatusCode(500, "An error occurred while processing the request.");
            }


        }
        // qqqq make sure this gets tested
        // qqqq do we need basehttclient interface here keep thinking we need more interfaces for the clients
        //private async Task<IActionResult> HandleRedirect(HttpResponseMessage response, BaseHttpClient apiClient)
        private async Task<IActionResult> HandleRedirect(HttpResponseMessage response, IAPIHttpClient apiClient)
        {
            var location = response.Headers.Location?.ToString();

            if (string.IsNullOrEmpty(location))
            {
                return StatusCode((int)response.StatusCode, "Redirect location not found");
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
                    StatusCode = (int)redirectResponse.StatusCode
                };
            }
            catch (HttpRequestException ex)
            {
                Logger.LogError(ex, "Error following redirect to {RedirectUrl}", redirectUrl);
                return StatusCode(500, "An error occurred while following the redirect.");
            }
        }
        // qqqq white and black list per api
        // Whitelist: only allow paths that start with these prefixes
        //"lh-api.dev.local"
        private static readonly string[] AllowedPathPrefixes =
        { 
            "search/",
            "catalogue/",
            "Resource/GetArticleDetails",
            "Resource/GetAudioDetails",
            "Resource/GetFileTypes"
        };

        // Blacklist: deny any paths that contain these
        private static readonly string[] BlockedPathSegments =
        {
            "Admin",
            "User/Delete",
            "Sensitive"
        };
        private static bool IsPathAllowed(string path)
        {
            var normalizedPath = path?.Trim('/').ToLowerInvariant() ?? string.Empty;

            // Check blacklist first
            if (BlockedPathSegments.Any(blocked => normalizedPath.Contains(blocked.ToLowerInvariant())))
            {
                return false;
            }

            // Check whitelist
            return AllowedPathPrefixes.Any(prefix => normalizedPath.StartsWith(prefix.ToLowerInvariant()));
        }

      
    }
}
