namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Extensions;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    /// <summary>
    /// The abstract api http client.
    /// </summary>
    public abstract class BaseHttpClient
    {
        private static readonly ConcurrentDictionary<int, SemaphoreSlim> TokenRefreshLocks = new ();

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HttpClient httpClient;
        private readonly LearningHubAuthServiceConfig authConfig;
        private readonly ILogger logger;
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHttpClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        /// <param name="webSettings">The web settings.</param>
        /// <param name="authConfig">The auth config.</param>
        /// <param name="client">The http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cacheService.</param>
        public BaseHttpClient(
            IHttpContextAccessor httpContextAccessor,
            Settings webSettings,
            LearningHubAuthServiceConfig authConfig,
            HttpClient client,
            ILogger logger,
            ICacheService cacheService)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.WebSettings = webSettings;
            this.authConfig = authConfig;
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
        protected Settings WebSettings { get; }

        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<HttpClient> GetClientAsync()
        {
            string accessToken = string.Empty;

            // get the current HttpContext to access the tokens
            var currentContext = this.httpContextAccessor.HttpContext;

            if (this.WebSettings.EnableTempDebugging?.ToLower() == "true")
            {
                this.logger.LogError(
                    "Temp Debugging: LearningHubHttpClient > GetClientAsync User is authenticated. User={User}",
                    currentContext?.User?.Identity?.GetCurrentName());
            }

            if (currentContext?.User?.Identity?.IsAuthenticated == true)
            {
                // should we renew access and refresh tokens?
                // get expires_at value
                var expiresAt = await currentContext.GetTokenAsync("expires_at");

                var tokenExpired =
                    string.IsNullOrWhiteSpace(expiresAt) ||
                    DateTimeOffset.Parse(expiresAt)
                        .AddSeconds(-60)
                        .ToUniversalTime() < DateTime.UtcNow;
                // compare
                if (tokenExpired)
                {
                    var currentUserId =
                        currentContext.User.Identity.GetCurrentUserId();

                    if (currentUserId > 0)
                    {
                        var tokenLock = TokenRefreshLocks.GetOrAdd(
                            currentUserId,
                            _ => new SemaphoreSlim(1, 1));

                        await tokenLock.WaitAsync();

                        try
                        {
                            // Re-check after acquiring the lock.
                            expiresAt =
                                await currentContext.GetTokenAsync("expires_at");

                            tokenExpired =
                                string.IsNullOrWhiteSpace(expiresAt) ||
                                DateTimeOffset.Parse(expiresAt)
                                    .AddSeconds(-60)
                                    .ToUniversalTime() < DateTime.UtcNow;

                            if (tokenExpired)
                            {
                                accessToken = await this.RenewTokensAsync();
                            }
                            else
                            {
                                accessToken =
                                    await currentContext.GetTokenAsync(
                                        OpenIdConnectParameterNames.AccessToken);
                            }
                        }
                        finally
                        {
                            tokenLock.Release();
                        }
                    }
                }
                else
                {
                    // get access token
                    accessToken =
                        await currentContext.GetTokenAsync(
                            OpenIdConnectParameterNames.AccessToken);
                }

                if (!this.httpClient.DefaultRequestHeaders.Contains("x-tz-offset"))
                {
                    var tzOffset =
                        await this.cacheService.GetAsync<int?>(
                            currentContext.User.GetTimezoneOffsetCacheKey());

                    if (tzOffset.HasValue)
                    {
                        this.httpClient.DefaultRequestHeaders.Add(
                            "x-tz-offset",
                            tzOffset.Value.ToString());
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
            this.httpClient.DefaultRequestHeaders.Add("Client-Identity-Key", this.WebSettings.LHClientIdentityKey);
        }

        /// <summary>
        /// The renew tokens.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<string> RenewTokensAsync()
        {
            if (this.WebSettings.EnableTempDebugging != null && this.WebSettings.EnableTempDebugging.ToLower() == "true")
            {
                this.logger.LogError("Temp Debugging: LearningHubHttpClient > RenewTokensAsync User is authenticated. ClientID=" + this.authConfig.ClientId);
            }

            // get the current HttpContext to access the tokens
            var currentContext = this.httpContextAccessor.HttpContext;

            // get the meta data
            var discoveryClient = await this.httpClient.GetDiscoveryDocumentAsync(this.authConfig.Authority);

            // create a new token client to get the new tokens
            var tokenClient = new TokenClient(this.httpClient, new TokenClientOptions()
            {
                Address = discoveryClient.TokenEndpoint,
                ClientId = this.authConfig.ClientId,
                ClientSecret = this.authConfig.ClientSecret,
            });

            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                if (this.WebSettings.EnableTempDebugging != null && this.WebSettings.EnableTempDebugging.ToLower() == "true")
                {
                    this.logger.LogError("Temp Debugging: LearningHubHttpClient > RenewTokensAsync. tokenResult.IsError=false");
                }

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
                if (this.WebSettings.EnableTempDebugging != null && this.WebSettings.EnableTempDebugging.ToLower() == "true")
                {
                    this.logger.LogError("Temp Debugging: LearningHubHttpClient > RenewTokensAsync. tokenResult.IsError=true. ErrorDescription=" + tokenResult.ErrorDescription);
                }

                throw new Exception("Problem encountered while refeshing tokens", tokenResult.Exception);
            }
        }
    }
}
