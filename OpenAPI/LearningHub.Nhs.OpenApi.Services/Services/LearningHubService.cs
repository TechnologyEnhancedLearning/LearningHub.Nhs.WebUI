// <copyright file="LearningHubService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.Extensions.Options;

    /// <inheritdoc />
    public class LearningHubService : ILearningHubService
    {
        private readonly LearningHubConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubService"/> class.
        /// </summary>
        /// <param name="config"><see cref="config"/>.</param>
        public LearningHubService(IOptions<LearningHubConfig> config)
        {
            this.config = config.Value;
        }

        /// <inheritdoc />
        public string GetResourceLaunchUrl(int resourceReferenceId)
        {
            return this.config.BaseUrl + this.config.LaunchResourceEndpoint + resourceReferenceId;
        }
    }
}
