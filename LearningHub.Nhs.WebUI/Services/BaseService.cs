// <copyright file="BaseService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Services
{
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Tha base service.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public abstract class BaseService<T>
    {
        private readonly ILogger logger;
        private readonly ILearningHubHttpClient learningHubHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{T}"/>.</param>
        protected BaseService(ILearningHubHttpClient learningHubHttpClient, ILogger<T> logger)
        {
            this.learningHubHttpClient = learningHubHttpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the LearningHubHttpClient.
        /// </summary>
        protected ILearningHubHttpClient LearningHubHttpClient => this.learningHubHttpClient;

        /// <summary>
        /// Gets the Logger.
        /// </summary>
        protected ILogger Logger => this.logger;
    }
}
