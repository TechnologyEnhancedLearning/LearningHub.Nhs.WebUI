namespace LearningHub.Nhs.Services
{
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
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// The  base service.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BaseService(ILogger<T> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger
        {
            get { return this.logger; }
        }
    }
}
