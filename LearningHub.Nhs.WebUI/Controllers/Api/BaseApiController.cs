// <copyright file="BaseApiController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using LearningHub.Nhs.Models.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Base API Controller.
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BaseApiController(ILogger logger)
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

        /// <summary>
        /// Gets the current user id.
        /// </summary>
        protected int CurrentUserId => this.User.Identity.GetCurrentUserId();
    }
}
