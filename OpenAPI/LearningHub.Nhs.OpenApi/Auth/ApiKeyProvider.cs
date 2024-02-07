// <copyright file="ApiKeyProvider.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.NHS.OpenAPI.Auth
{
    using System;
    using System.Threading.Tasks;
    using AspNetCore.Authentication.ApiKey;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    public class ApiKeyProvider : IApiKeyProvider
    {
        private readonly ILogger<IApiKeyProvider> logger;
        private readonly IApiKeyRepository apiKeyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="apiKeyRepository">The ApiKeyRepository.</param>
        public ApiKeyProvider(ILogger<IApiKeyProvider> logger, IApiKeyRepository apiKeyRepository)
        {
            this.logger = logger;
            this.apiKeyRepository = apiKeyRepository;
        }

        /// <inheritdoc />
        public async Task<IApiKey?> ProvideAsync(string key)
        {
            try
            {
                return await Task.FromResult(this.apiKeyRepository.GetApiKeyOrNull(key));
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed to get API key.");
                throw;
            }
        }
    }
}
