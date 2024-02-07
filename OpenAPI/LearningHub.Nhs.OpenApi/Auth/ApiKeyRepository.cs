// <copyright file="ApiKeyRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Auth
{
    using System.Collections.Generic;
    using System.Linq;
    using AspNetCore.Authentication.ApiKey;
    using LearningHub.NHS.OpenAPI.Configuration;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using Microsoft.Extensions.Options;

    /// <inheritdoc />
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly List<IApiKey> apiKeys;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyRepository"/> class.
        /// </summary>
        /// <param name="config">The app configuration.</param>
        public ApiKeyRepository(IOptions<AuthConfiguration> config)
        {
            var authConfiguration = config.Value;
            this.apiKeys = authConfiguration.Clients != null
                ? ExtractApiKeys(authConfiguration.Clients).ToList()
                : new List<IApiKey>();
        }

        /// <inheritdoc />
        public IApiKey? GetApiKeyOrNull(string key)
        {
            return this.apiKeys.FirstOrDefault(c => c.Key == key);
        }

        private static IEnumerable<IApiKey> ExtractApiKeys(IEnumerable<ApiKeyClient> clients)
        {
            return clients.SelectMany(
                client => client.Keys.Select(
                    key => new ApiKey(key, client.Name)));
        }
    }
}
