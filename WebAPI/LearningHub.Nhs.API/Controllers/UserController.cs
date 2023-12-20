// <copyright file="UserController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Api.Helpers;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The log controller.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ApiControllerBase
    {
        private readonly IUserNotificationService userNotificationService;
        private readonly ISecurityService securityService;

        /// <summary>
        /// The user profile service.
        /// </summary>
        private readonly IUserProfileService userProfileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The lh user service.
        /// </param>
        /// <param name="userProfileService">
        /// The user profile service.
        /// </param>
        /// <param name="userNotificationService">
        /// The user notification service.
        /// </param>
        /// <param name="securityService">The security service.</param>
        /// <param name="logger">The logger.</param>
        public UserController(
            IUserService userService,
            IUserProfileService userProfileService,
            IUserNotificationService userNotificationService,
            ISecurityService securityService,
            ILogger<UserController> logger)
            : base(userService, logger)
        {
            this.userProfileService = userProfileService;
            this.userNotificationService = userNotificationService;
            this.securityService = securityService;
        }

        /// <summary>
        /// The GetActiveContent.
        /// </summary>
        /// <returns>The active content.</returns>
        [HttpGet("GetActiveContent")]
        public async Task<IActionResult> GetActiveContent()
        {
            return this.Ok(await this.UserService.GetActiveContentAsync(this.CurrentUserId));
        }

        /// <summary>
        /// Add active content for the logged in user.
        /// </summary>
        /// <param name="activeContentViewModel">The activeContentViewModel<see cref="ActiveContentViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("AddActiveContent")]
        public async Task<IActionResult> AddActiveContent(ActiveContentViewModel activeContentViewModel)
        {
            var vr = await this.UserService.AddActiveContent(activeContentViewModel, this.CurrentUserId);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Release active content for the logged in user.
        /// </summary>
        /// <param name="activeContentReleaseViewModel">The activeContentReleaseViewModel<see cref="ActiveContentReleaseViewModel"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        [HttpPost]
        [Route("ReleaseActiveContent")]
        public async Task<IActionResult> ReleaseActiveContent(ActiveContentReleaseViewModel activeContentReleaseViewModel)
        {
            var vr = await this.UserService.ReleaseActiveContent(activeContentReleaseViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Get specific User Profile by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserProfile/{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfileAsync(int id)
        {
            return this.Ok(await this.userProfileService.GetByIdAsync(id));
        }

        /// <summary>
        /// Create specific User Profile.
        /// </summary>
        /// <param name="userProfile">The userProfile.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("CreateUserProfile")]
        public async Task<ActionResult<UserProfile>> CreateUserProfileAsync(UserProfile userProfile)
        {
            var vr = await this.userProfileService.CreateUserProfileAsync(this.CurrentUserId, userProfile);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Update specific User Profile.
        /// </summary>
        /// <param name="userProfile">The userProfile.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPut]
        [Route("UpdateUserProfile")]
        public async Task<ActionResult<UserProfile>> UpdateUserProfileAsync(UserProfile userProfile)
        {
            var vr = await this.userProfileService.UpdateUserProfileAsync(this.CurrentUserId, userProfile);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Get specific User Profile by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetByUserId/{id}")]
        public async Task<ActionResult<User>> GetByUserIdAsync(int id)
        {
            return this.Ok(await this.UserService.GetByIdAsync(id));
        }

        /// <summary>
        /// Get current User Profile.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetCurrentUserProfile")]
        public async Task<ActionResult<UserProfile>> GetCurrentUserProfileAsync()
        {
            return this.Ok(await this.userProfileService.GetByIdAsync(this.CurrentUserId));
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="userCreateViewModel">The userCreateViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUserAsync(UserCreateViewModel userCreateViewModel)
        {
            var vr = await this.UserService.CreateUserAsync(this.CurrentUserId, userCreateViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="userUpdateViewModel">The userCreateViewModel.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUserAsync(UserUpdateViewModel userUpdateViewModel)
        {
            var vr = await this.UserService.UpdateUserAsync(this.CurrentUserId, userUpdateViewModel);

            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }

        /// <summary>
        /// GetLastIssuedEmailChangeValidationToken.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("GetLastIssuedEmailChangeValidationToken")]
        public async Task<IActionResult> GetLastIssuedEmailChangeValidationToken()
        {
            var result = await this.securityService.GetLastIssuedEmailChangeValidationToken(this.CurrentUserId);
            return this.Ok(result);
        }

        /// <summary>
        /// Generate email change token.
        /// </summary>
        /// <param name="emailAddress">emailAddress.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("GenerateEmailChangeValidationTokenAndSendEmail/{emailAddress}/{isUserRoleUpgrade}")]
        public async Task<IActionResult> GenerateEmailChangeValidationTokenAndSendEmail(string emailAddress, bool isUserRoleUpgrade)
        {
            await this.securityService.GenerateEmailChangeValidationTokenAndSendEmail(this.CurrentUserId, emailAddress, isUserRoleUpgrade);
            return this.Ok();
        }

        /// <summary>
        /// Regenerate email change token.
        /// </summary>
        /// <param name="newPrimaryEmail">emailAddress.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("ReGenerateEmailChangeValidationToken/{newPrimaryEmail}/{isUserRoleUpgrade}")]
        public async Task<IActionResult> ReGenerateEmailChangeValidationToken(string newPrimaryEmail, bool isUserRoleUpgrade)
        {
            var result = await this.securityService.ReGenerateEmailChangeValidationToken(this.CurrentUserId, newPrimaryEmail, isUserRoleUpgrade);
            return this.Ok(result);
        }

        /// <summary>
        /// Regenerate email change token.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("CancelEmailChangeValidationToken")]
        public async Task<IActionResult> CancelEmailChangeValidationToken()
        {
            await this.securityService.CancelEmailChangeValidationToken(this.CurrentUserId);
            return this.Ok(true);
        }

        /// <summary>
        /// Validate email change token.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="locToken">
        /// The loc Token.
        /// </param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [Route("ValidateEmailChangeToken/{token}/{loctoken}/{isUserRoleUpgrade}")]
        public async Task<IActionResult> ValidateEmailChangeTokenAsync(string token, string locToken, bool isUserRoleUpgrade)
        {
            var result = await this.securityService.ValidateEmailChangeTokenAsync(token.DecodeParameter(), locToken.DecodeParameter(), isUserRoleUpgrade);
            return this.Ok(result);
        }

        /////// <summary>
        /////// Get a filtered page of User records.
        /////// </summary>
        /////// <param name="page">
        /////// The page.
        /////// </param>
        /////// <param name="pageSize">
        /////// The page size.
        /////// </param>
        /////// <param name="sortColumn">
        /////// The sort column.
        /////// </param>
        /////// <param name="sortDirection">
        /////// The sort direction.
        /////// </param>
        /////// <param name="filter">
        /////// The filter.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////[HttpGet]
        ////[Route("GetUserAdminBasicFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{*filter}")]
        ////public async Task<IActionResult> GetUserAdminBasicFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string filter)
        ////{
        ////    filter = HttpUtility.UrlDecode(filter);
        ////    PagedResultSet<elfhHub.Nhs.Models.Common.UserAdminBasicViewModel> pagedResultSet = await this.ElfhUserService.GetUserAdminBasicPageAsync(page, pageSize, sortColumn, sortDirection, filter);
        ////    return this.Ok(pagedResultSet);
        ////}

        /// <summary>
        /// Get a filtered page of LH User records.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The preset filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetLHUserAdminBasicFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        public async Task<IActionResult> GetLHUserAdminBasicFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        {
            presetFilter = HttpUtility.UrlDecode(presetFilter);
            filter = HttpUtility.UrlDecode(filter);
            PagedResultSet<LearningHub.Nhs.Models.User.UserAdminBasicViewModel> pagedResultSet = await this.UserService.GetUserAdminBasicPageAsync(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
            return this.Ok(pagedResultSet);
        }

        /////// <summary>
        /////// Returns the tentants for a given user id.
        /////// </summary>
        /////// <param name="userId">
        /////// The id.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////[HttpGet]
        ////[Route("GetTenantDescriptionByUserId/{userId}")]
        ////public async Task<IActionResult> GetTenantDescriptionByUserId(int userId)
        ////{
        ////    var tenant = await this.ElfhUserService.GetTenantDescriptionByUserId(userId);

        ////    return this.Ok(tenant);
        ////}

        /////// <summary>
        /////// Update an existing User.
        /////// </summary>
        /////// <param name="user">
        /////// The user.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////[HttpPut("{user}")]
        ////public async Task<IActionResult> UpdateUser([FromBody] UserAdminDetailViewModel user)
        ////{
        ////    var (result, permissionNotification) = await this.ElfhUserService.UpdateUserAsync(user, this.CurrentUserId);
        ////    if (result.IsValid)
        ////    {
        ////        if (permissionNotification)
        ////        {
        ////            // Code ready for next iteration Notification work
        ////            // var notificationId = await this.notificationService.CreatePermisssionNotificationAsync(this.CurrentUserId, user.ReadOnlyUser);

        ////            // await this.userNotificationService.CreateAsync(
        ////            //    this.CurrentUserId, new UserNotification { UserId = user.Id, NotificationId = notificationId });
        ////        }

        ////        return this.Ok(new ApiResponse(true, result));
        ////    }
        ////    else
        ////    {
        ////        return this.BadRequest(new ApiResponse(false, result));
        ////    }
        ////}

        /////// <summary>
        /////// Send Admin Password Reset Email to the user.
        /////// </summary>
        /////// <param name="userId">
        /////// The user id.
        /////// </param>
        /////// <returns>
        /////// The <see cref="IActionResult"/>.
        /////// </returns>
        ////[HttpPost]
        ////[Route("SendAdminPasswordResetEmail")]
        ////public async Task<IActionResult> SendAdminPasswordResetEmail([FromBody] int userId)
        ////{
        ////    var vr = await this.ElfhUserService.SendAdminPasswordResetEmail(userId);

        ////    if (vr.IsValid)
        ////    {
        ////        return this.Ok(new ApiResponse(true, vr));
        ////    }
        ////    else
        ////    {
        ////        return this.BadRequest(new ApiResponse(false, vr));
        ////    }
        ////}

        /////// <summary>
        /////// Sends an email to the user.
        /////// </summary>
        /////// <param name="vm">
        /////// The view model.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////[HttpPost]
        ////[Route("SendEmailToUser")]
        ////public async Task<IActionResult> SendEmailToUser(UserContactViewModel vm)
        ////{
        ////    await this.ElfhUserService.SendEmailToUserAsync(vm);

        ////    // TODO: update the contacthistory table
        ////    return this.Ok();
        ////}

        /////// <summary>
        /////// The has multiple users for email.
        /////// </summary>
        /////// <param name="emailAddress">
        /////// The email address.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////[HttpGet]
        ////[Route("HasMultipleUsersForEmail/{emailAddress}")]
        ////[AllowAnonymous]
        ////public async Task<IActionResult> HasMultipleUsersForEmail(string emailAddress)
        ////{
        ////    var result = await this.ElfhUserService.HasMultipleUsersForEmailAsync(emailAddress);
        ////    return this.Ok(result);
        ////}

        /////// <summary>
        /////// The forgot password.
        /////// </summary>
        /////// <param name="model">
        /////// The model.
        /////// </param>
        /////// <returns>
        /////// The <see cref="Task"/>.
        /////// </returns>
        ////[HttpPost]
        ////[AllowAnonymous]
        ////[Route("ForgotPassword")]
        ////public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        ////{
        ////    await this.ElfhUserService.SendForgotPasswordEmail(model.EmailAddress);
        ////    return this.Ok(new ApiResponse(true));
        ////}

        /////// <summary>
        /////// Link Employment Record to User.
        /////// </summary>
        /////// <returns>
        /////// Nothing.
        /////// </returns>
        ////[HttpGet]
        ////[Route("LinkEmploymentRecordToUser")]
        ////public async Task LinkEmploymentRecordToUser()
        ////{
        ////    await this.ElfhUserService.LinkEmploymentRecordToUser(this.CurrentUserId);
        ////}
    }
}