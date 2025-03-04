namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using LearningHub.Nhs.OpenApi.Services.Interface.HttpClients;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The abstract base service.
    /// </summary>
    /// <typeparam name="T">
    /// The type of each service.
    /// </typeparam>
    public abstract class BaseService<T>
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The Find Wise HTTP Client.
        /// </summary>
        private IFindwiseClient findwiseClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// The  base service.
        /// </summary>
        /// <param name="findwiseClient">The Find Wise http client.</param>
        /// <param name="logger">The logger.</param>
        protected BaseService(IFindwiseClient findwiseClient, ILogger<T> logger)
        {
            this.findwiseClient = findwiseClient;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Gets the Find Wise HTTP Client.
        /// </summary>
        protected IFindwiseClient FindwiseClient
        {
            get { return this.findwiseClient; }
        }
    }
}
