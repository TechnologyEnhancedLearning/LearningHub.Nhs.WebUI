// <copyright file="ApiControllerBase.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The api controller base.
    /// </summary>
    public abstract class ApiControllerBase : ControllerBase
    {
        private const int PortalAdminId = 4;

        /// <summary>
        /// The current user.
        /// </summary>
        private UserLHBasicViewModel currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiControllerBase"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="logger">logger.</param>
        public ApiControllerBase(IUserService userService, ILogger logger)
        {
            this.UserService = userService;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the current user id.
        /// </summary>
        public int CurrentUserId
        {
            get
            {
                var currentUserId = this.User.Identity.GetCurrentUserId();
                return currentUserId == 0 ? PortalAdminId : currentUserId;
            }
        }

        /// <summary>
        /// Gets the elfh user service.
        /// </summary>
        protected IUserService UserService { get; }

        /// <summary>
        /// The get current user async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        internal async Task<UserLHBasicViewModel> GetCurrentUserAsync()
        {
            if (this.currentUser == null)
            {
                this.currentUser = await this.UserService.GetByIdAsync(this.CurrentUserId);
            }

            return this.currentUser;
        }
    }
}