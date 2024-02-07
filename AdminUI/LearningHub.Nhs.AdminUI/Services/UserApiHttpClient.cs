// <copyright file="UserApiHttpClient.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

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
    /// The user api http client.
    /// </summary>
    public class UserApiHttpClient : BaseHttpClient, IUserApiHttpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserApiHttpClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        /// <param name="webSettings">The web settings.</param>
        /// <param name="client">The http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="cacheService">The cache service.</param>
        public UserApiHttpClient(
            IHttpContextAccessor httpContextAccessor,
            IOptions<WebSettings> webSettings,
            HttpClient client,
            ILogger<UserApiHttpClient> logger,
            ICacheService cacheService)
            : base(httpContextAccessor, webSettings.Value, client, logger, cacheService)
        {
        }

        /// <summary>
        /// Gets the usr api url.
        /// </summary>
        public override string ApiUrl => this.WebSettings.UserApiUrl;
    }
}