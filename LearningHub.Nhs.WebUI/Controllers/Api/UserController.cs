// <copyright file="UserController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using elfhHub.Nhs.Models.Common;
    using LearningHub.Nhs.WebUI.Configuration;
    using LearningHub.Nhs.WebUI.Interfaces;
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

        private ILoginWizardService loginWizardService;
        private Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The userService<see cref="IUserService"/>.</param>
        /// <param name="loginWizardService">loginWizardService.</param>
        /// <param name="logger">logger.</param>
        /// <param name="settings">Settings.</param>
        public UserController(IUserService userService, ILoginWizardService loginWizardService, ILogger<UserController> logger, IOptions<Settings> settings)
            : base(logger)
        {
            this.userService = userService;
            this.loginWizardService = loginWizardService;
            this.settings = settings.Value;
        }

        /// <summary>
        /// The Current.
        /// </summary>
        /// <returns>The current user.</returns>
        [HttpGet("Current")]
        public async Task<IActionResult> Current()
        {
            return this.Ok(await this.userService.GetCurrentUserAsync());
        }

        /// <summary>
        /// The UserProfile.
        /// </summary>
        /// <returns>The current user.</returns>
        [HttpGet("CurrentProfile")]
        public async Task<IActionResult> CurrentProfile()
        {
            return this.Ok(await this.userService.GetCurrentUserProfileAsync());
        }

        /// <summary>
        /// Get current user's basic details.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetCurrentUserBasicDetails")]
        public async Task<ActionResult> GetCurrentUserBasicDetails()
        {
            var user = await this.userService.GetCurrentUserBasicDetailsAsync();
            return this.Ok(user);
        }

        /// <summary>
        /// Get current user's access type.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetUserAccessType")]
        public async Task<ActionResult> GetUserAccessType()
        {
            var isGeneralUser = this.User.IsInRole("BasicUser");
            return this.Ok(isGeneralUser);
        }

        /// <summary>
        /// to get user role.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("CheckUserRole")]
        public async Task<ActionResult> CheckUserRole()
        {
            var isSystemAdmin = this.User.IsInRole("Administrator");
            return this.Ok(isSystemAdmin);
        }

        /// <summary>
        /// The GetCurrentUserPersonalDetails.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpGet]
        [Route("GetCurrentUserPersonalDetails")]
        public async Task<ActionResult> GetCurrentUserPersonalDetails()
        {
            var userPersonalDetails = await this.userService.GetCurrentUserPersonalDetailsAsync();
            return this.Ok(userPersonalDetails);
        }

        /// <summary>
        /// GetSecurityQuestions.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetSecurityQuestions")]
        public async Task<IActionResult> GetSecurityQuestions()
        {
            var result = await this.loginWizardService.GetSecurityQuestionsModel(this.CurrentUserId);
            if (result != null && result.UserSecurityQuestions != null)
            {
                foreach (var userSecurityQuestion in result.UserSecurityQuestions)
                {
                    if (!string.IsNullOrEmpty(userSecurityQuestion.SecurityQuestionAnswerHash))
                    {
                        userSecurityQuestion.SecurityQuestionAnswerHash = "********";
                    }
                }
            }

            return this.Ok(result);
        }

        /// <summary>
        /// UpdateSecurityQuestions.
        /// </summary>
        /// <param name="userSecurityQuestions">userSecurityQuestions.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateSecurityQuestions")]
        public async Task<IActionResult> UpdateSecurityQuestions([FromBody] List<UserSecurityQuestionViewModel> userSecurityQuestions)
        {
            await this.userService.UpdateUserSecurityQuestions(userSecurityQuestions);
            return this.Ok(true);
        }

        /// <summary>
        /// The UpdatePersonalDetails.
        /// </summary>
        /// <param name="personalDetailsViewModel">Personal details.</param>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        [Route("UpdatePersonalDetails")]
        public async Task<ActionResult> UpdatePersonalDetails([FromBody] PersonalDetailsViewModel personalDetailsViewModel)
        {
            await this.userService.UpdatePersonalDetails(personalDetailsViewModel);
            return this.Ok(true);
        }

        /*
        /// <summary>
        /// The change password.
        /// </summary>
        /// <param name="changePasswordViewModel">The model<see cref="ChangePasswordViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel changePasswordViewModel)
        {
            await this.userService.UpdatePassword(changePasswordViewModel.NewPassword);
            return this.Ok(true);
        }
        */

        /// <summary>
        /// The get active content.
        /// </summary>
        /// <returns>The content.</returns>
        [HttpGet("GetActiveContent")]
        public async Task<IActionResult> GetActiveContent()
        {
            return this.Ok(await this.userService.GetActiveContentAsync());
        }

        /// <summary>
        /// The KeepUserSessionAlive.
        /// </summary>
        /// <returns>The <see cref="Task{ActionResult}"/>.</returns>
        [HttpPost]
        [Route("KeepUserSessionAlive")]
        public ActionResult KeepUserSessionAlive()
        {
            return this.Ok(true);
        }

        /// <summary>
        /// Gets KeepUserSessionAliveInterval.
        /// </summary>
        /// <returns>The content.</returns>
        [HttpGet("GetkeepUserSessionAliveInterval")]
        public IActionResult GetkeepUserSessionAliveInterval()
        {
            return this.Ok(Convert.ToInt32(this.settings.KeepUserSessionAliveIntervalMins) * 60000);
        }
    }
}
