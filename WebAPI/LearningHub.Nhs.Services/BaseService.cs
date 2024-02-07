// <copyright file="BaseService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using LearningHub.Nhs.Services.Interface;
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
        private IFindWiseHttpClient findWiseHttpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseService{T}"/> class.
        /// The  base service.
        /// </summary>
        /// <param name="findWiseHttpClient">The Find Wise http client.</param>
        /// <param name="logger">The logger.</param>
        protected BaseService(IFindWiseHttpClient findWiseHttpClient, ILogger<T> logger)
        {
            this.findWiseHttpClient = findWiseHttpClient;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger
        {
            get { return this.logger; }
        }

        /// <summary>
        /// Gets the Find Wise HTTP Client.
        /// </summary>
        protected IFindWiseHttpClient FindWiseHttpClient
        {
            get { return this.findWiseHttpClient; }
        }
    }
}
