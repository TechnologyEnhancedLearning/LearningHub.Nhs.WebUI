namespace LearningHub.Nhs.AdminUI.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using elfhHub.Nhs.Models.Enums;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The UserController class.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseApiController
    {
        /// <summary>
        /// The elfh user service..
        /// </summary>
        private IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="loginWizardService">loginWizardService.</param>
        /// <param name="logger">logger.</param>
        /// <param name="settings">Settings.</param>
        public UserController(IUserService userService, ILogger<UserController> logger)
            : base(logger)
        {
            this.userService = userService;
        }

        /// <summary>
        /// The SessionTimeout.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost("browser-close")]
        public IActionResult BrowserClose()
        {
            // Add browser close to the UserHistory
            UserHistoryViewModel userHistory = new UserHistoryViewModel()
            {
                UserId = this.CurrentUserId,
                UserHistoryTypeId = (int)UserHistoryType.Logout,
                Detail = @"User browser closed",
            };

            this.userService.StoreUserHistory(userHistory);

            return this.Ok(true);
        }
    }
}
