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
        /// Defines the openApiHttpClient.
        /// </summary>
        private IOpenApiHttpClient openApiHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        protected BaseService(ILearningHubHttpClient learningHubHttpClient)
        {
            this.learningHubHttpClient = learningHubHttpClient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="openApiHttpClient">The openApiHttpClient<see cref="IOpenApiHttpClient"/>.</param>
        protected BaseService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient)
        {
            this.learningHubHttpClient = learningHubHttpClient;
            this.openApiHttpClient = openApiHttpClient;
        }

        /// <summary>
        /// Gets the LearningHubHttpClient.
        /// </summary>
        protected ILearningHubHttpClient LearningHubHttpClient => this.learningHubHttpClient;

        /// <summary>
        /// Gets the OpenApiHttpClient.
        /// </summary>
        protected IOpenApiHttpClient OpenApiHttpClient => this.openApiHttpClient;
    }
}
