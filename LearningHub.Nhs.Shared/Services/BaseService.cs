namespace LearningHub.Nhs.Shared.Services
{
    using LearningHub.Nhs.Shared.Interfaces.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Tha base service.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public abstract class BaseService<T>
    {
        private readonly ILogger logger;
        private readonly ILearningHubHttpClient learningHubHttpClient;
        private readonly IOpenApiHttpClient openApiHttpClient;

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
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="openApiHttpClient">The openApiHttpClient<see cref="IOpenApiHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{T}"/>.</param>
        protected BaseService(ILearningHubHttpClient learningHubHttpClient, IOpenApiHttpClient openApiHttpClient, ILogger<T> logger)
        {
            this.learningHubHttpClient = learningHubHttpClient;
            this.openApiHttpClient = openApiHttpClient;
            this.logger = logger;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// </summary>
        /// <param name="openApiHttpClient">The openApiHttpClient<see cref="IOpenApiHttpClient"/>.</param>
        /// <param name="logger">The logger<see cref="ILogger{T}"/>.</param>
        protected BaseService(IOpenApiHttpClient openApiHttpClient, ILogger<T> logger)
        {
            this.openApiHttpClient = openApiHttpClient;
            this.logger = logger;
        }


        /// <summary>
        /// Gets the LearningHubHttpClient.
        /// </summary>
        protected ILearningHubHttpClient LearningHubHttpClient => this.learningHubHttpClient;

        /// <summary>
        /// Gets the OpenApiHttpClient.
        /// </summary>
        protected IOpenApiHttpClient OpenApiHttpClient => this.openApiHttpClient;

        /// <summary>
        /// Gets the Logger.
        /// </summary>
        protected ILogger Logger => this.logger;
    }
}
