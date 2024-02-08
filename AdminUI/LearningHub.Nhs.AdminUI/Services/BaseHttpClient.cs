namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using LearningHub.Nhs.AdminUI.Configuration;
    using LearningHub.Nhs.AdminUI.Extensions;
    using LearningHub.Nhs.Caching;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    /// <summary>
    /// The abstract api http client.
    /// </summary>
    public abstract class BaseHttpClient
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient;
        private readonly ILogger logger;
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHttpClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        /// <param name="webSettings">The web settings.</param>
        /// <param name="client">The http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cacheService.</param>
        public BaseHttpClient(
            IHttpContextAccessor httpContextAccessor,
            WebSettings webSettings,
            HttpClient client,
            ILogger logger,
            ICacheService cacheService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.WebSettings = webSettings;
            this.httpClient = client;
            this.logger = logger;
            this.cacheService = cacheService;
            this.Initialise();
        }

        /// <summary>
        /// Gets the api url.
        /// </summary>
        public abstract string ApiUrl { get; }

        /// <summary>
        /// Gets the web settings.
        /// </summary>
        protected WebSettings WebSettings { get; }

        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<HttpClient> GetClientAsync()
        {
            string accessToken = string.Empty;

            // get the current HttpContext to access the tokens
            var currentContext = this.httpContextAccessor.HttpContext;

            if (currentContext.User.Identity.IsAuthenticated)
            {
                // should we renew access and refresh tokens?
                // get expires_at value
                var expires_at = await currentContext.GetTokenAsync("expires_at");

                // compare
                if (string.IsNullOrWhiteSpace(expires_at)
                    || (DateTimeOffset.Parse(expires_at).AddSeconds(-60).ToUniversalTime() < DateTime.UtcNow))
                {
                    accessToken = await this.RenewTokensAsync();
                }
                else
                {
                    // get access token
                    accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                }

                if (!this.httpClient.DefaultRequestHeaders.Contains("x-tz-offset"))
                {
                    var tzOffset = await this.cacheService.GetAsync<int?>(currentContext.User.GetTimezoneOffsetCacheKey());
                    if (tzOffset.HasValue)
                    {
                        this.httpClient.DefaultRequestHeaders.Add("x-tz-offset", tzOffset.Value.ToString());
                    }
                }
            }

            this.httpClient.SetBearerToken(accessToken);
            return this.httpClient;
        }

        /// <summary>
        /// The initialise.
        /// </summary>
        private void Initialise()
        {
            this.httpClient.BaseAddress = new Uri(this.ApiUrl);
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// The renew tokens.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<string> RenewTokensAsync()
        {
            // get the current HttpContext to access the tokens
            var currentContext = this.httpContextAccessor.HttpContext;

            // get the meta data
            var discoveryClient = await this.httpClient.GetDiscoveryDocumentAsync(this.WebSettings.AuthenticationServiceUrl);

            // create a new token client to get the new tokens
            var tokenClient = new TokenClient(this.httpClient, new TokenClientOptions()
            {
                Address = discoveryClient.TokenEndpoint,
                ClientId = this.WebSettings.ClientId,
                ClientSecret = this.WebSettings.LearningHubSecret,
            });

            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                // update the tokens & expiry value
                var updatedTokens = new List<AuthenticationToken>();
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = tokenResult.IdentityToken,
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = tokenResult.AccessToken,
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = tokenResult.RefreshToken,
                });

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture),
                });

                // get authenticated result, containing the current principle & properties
                var currentAuthenticateResult = await currentContext.AuthenticateAsync("Cookies");

                // store the updated tokens
                currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

                // sign in
                await currentContext.SignInAsync("Cookies", currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

                // return the new access token
                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception("Problem encountered while refeshing tokens", tokenResult.Exception);
            }
        }
    }
}
