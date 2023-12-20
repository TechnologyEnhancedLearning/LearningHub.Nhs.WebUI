// <copyright file="BaseService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.ReportApi.Services
{
    using LearningHub.Nhs.ReportApi.Services.Interface;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Tha base service.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public abstract class BaseService<T>
    {
        private readonly ILogger logger;
        private readonly ILearningHubApiFacade learningHubApiFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="learningHubApiFacade">The learningHubApiFacade<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{T}"/>.</param>
        protected BaseService(ILearningHubApiFacade learningHubApiFacade, ILogger<T> logger)
        {
            this.learningHubApiFacade = learningHubApiFacade;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the LearningHubApiFacade.
        /// </summary>
        protected ILearningHubApiFacade LearningHubApiFacade => this.learningHubApiFacade;

        /// <summary>
        /// Gets the Logger.
        /// </summary>
        protected ILogger Logger => this.logger;
    }
}
