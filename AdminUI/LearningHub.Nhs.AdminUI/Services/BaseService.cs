// <copyright file="BaseService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Services
{
    using LearningHub.Nhs.AdminUI.Interfaces;

    /// <summary>
    /// Defines the <see cref="BaseService" />.
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// Defines the learningHubHttpClient.
        /// </summary>
        private ILearningHubHttpClient learningHubHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        protected BaseService(ILearningHubHttpClient learningHubHttpClient)
        {
            this.learningHubHttpClient = learningHubHttpClient;
        }

        /// <summary>
        /// Gets the LearningHubHttpClient.
        /// </summary>
        protected ILearningHubHttpClient LearningHubHttpClient => this.learningHubHttpClient;
    }
}
