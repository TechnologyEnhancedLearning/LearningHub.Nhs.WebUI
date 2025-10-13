namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.User;
    using LearningHub.NHS.OpenAPI.Helpers;
    using LearningHub.Nhs.OpenApi.Models.Configuration;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The log controller.
    /// </summary>
    [Route("User")]
    [ApiController]
    [Authorize]
    public class UserController : OpenApiControllerBase
    {
        private readonly ISecurityService securityService;

        /// <summary>
        /// The user profile service.
        /// </summary>
        private readonly IUserProfileService userProfileService;
        private readonly IUserService userService;
        private readonly ICacheService cacheService;
        private readonly LearningHubConfig learningHubConfig;
        private readonly IUserNotificationService userNotificationService;
        private readonly IUserPasswordResetRequestsService userPasswordResetRequestsService;
        private readonly INavigationPermissionService permissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The lh user service.
        /// </param>
        /// <param name="userProfileService">
        /// The user profile service.
        /// </param>
        /// <param name="securityService">
        /// The securityService service.
        /// </param>
        /// <param name="userNotificationService">The userNotificationService.</param>
        /// <param name="permissionService">The permissionService.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="learningHubConfig">The learningHubConfig.</param>
        public UserController(
            IUserService userService,
            IUserProfileService userProfileService,
            ISecurityService securityService,
            IUserNotificationService userNotificationService,
            IUserPasswordResetRequestsService userPasswordResetRequestsService,
            INavigationPermissionService permissionService,
            ICacheService cacheService,
            IOptions<LearningHubConfig> learningHubConfig)
        {
            this.userProfileService = userProfileService;
            this.securityService = securityService;
            this.userService = userService;
            this.userNotificationService = userNotificationService;
            this.userPasswordResetRequestsService = userPasswordResetRequestsService;
            this.permissionService = permissionService;
            this.cacheService = cacheService;
            this.learningHubConfig = learningHubConfig.Value;
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
            var vr = await userProfileService.CreateUserProfileAsync(this.CurrentUserId.GetValueOrDefault(), userProfile);

            if (vr.IsValid)
            {
                return Ok(new ApiResponse(true, vr));
            }
            else
            {
                return BadRequest(new ApiResponse(false, vr));
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
            var vr = await userProfileService.UpdateUserProfileAsync(this.CurrentUserId.GetValueOrDefault(), userProfile);

            if (vr.IsValid)
            {
                return Ok(new ApiResponse(true, vr));
            }
            else
            {
                return BadRequest(new ApiResponse(false, vr));
            }
        }

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
            PagedResultSet<LearningHub.Nhs.Models.User.UserAdminBasicViewModel> pagedResultSet = await this.userService.GetUserAdminBasicPageAsync(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
            return this.Ok(pagedResultSet);
        }

        /// <summary>
        /// Get specific User Profile by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetByUserId/{id}")]
        public async Task<ActionResult<UserLHBasicViewModel>> GetByUserIdAsync(int id)
        {
            return this.Ok(await this.userService.GetByIdAsync(id));
        }

        /// <summary>
        /// Get current User Profile.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetCurrentUserProfile")]
        public async Task<ActionResult<UserProfile>> GetCurrentUserProfileAsync()
        {
            return this.Ok(await userProfileService.GetByIdAsync(this.CurrentUserId.GetValueOrDefault()));
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
            var vr = await this.userService.CreateUserAsync(this.CurrentUserId.GetValueOrDefault(), userCreateViewModel);

            if (vr.IsValid)
            {
                return Ok(new ApiResponse(true, vr));
            }
            else
            {
                return BadRequest(new ApiResponse(false, vr));
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
            var vr = await this.userService.UpdateUserAsync(this.CurrentUserId.GetValueOrDefault(), userUpdateViewModel);

            if (vr.IsValid)
            {
                return Ok(new ApiResponse(true, vr));
            }
            else
            {
                return BadRequest(new ApiResponse(false, vr));
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
            var result = await securityService.GetLastIssuedEmailChangeValidationToken(this.CurrentUserId.GetValueOrDefault());
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
            await securityService.GenerateEmailChangeValidationTokenAndSendEmail(this.CurrentUserId.GetValueOrDefault(), emailAddress, isUserRoleUpgrade);
            return Ok();
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
            var result = await securityService.ReGenerateEmailChangeValidationToken(this.CurrentUserId.GetValueOrDefault(), newPrimaryEmail, isUserRoleUpgrade);
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
            await securityService.CancelEmailChangeValidationToken(this.CurrentUserId.GetValueOrDefault());
            return Ok(true);
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
            var result = await securityService.ValidateEmailChangeTokenAsync(token.DecodeParameter(), locToken.DecodeParameter(), isUserRoleUpgrade);
            return this.Ok(result);
        }

        /// <summary>
        /// Check user can request password reset.
        /// </summary>
        /// <param name="emailAddress">emailAddress.</param>
        /// <param name="passwordRequestLimitingPeriod">The passwordRequestLimitingPeriod.</param>
        /// <param name="passwordRequestLimit">ThepasswordRequestLimit.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("CanRequestPasswordReset/{emailAddress}/{passwordRequestLimitingPeriod}/{passwordRequestLimit}")]
        public async Task<bool> CanRequestPasswordReset(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit)
        {
            var result = await this.userPasswordResetRequestsService.CanRequestPasswordReset(emailAddress, passwordRequestLimitingPeriod, passwordRequestLimit);
            if (result)
            {
                await this.userPasswordResetRequestsService.CreateUserPasswordRequest(emailAddress);
            }

            return result;
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
        /// The GetActiveContent.
        /// </summary>
        /// <returns>The active content.</returns>
        [HttpGet("GetActiveContent")]
        public async Task<IActionResult> GetActiveContent()
        {
            return this.Ok(await this.userService.GetActiveContentAsync(this.CurrentUserId.GetValueOrDefault()));
        }

        /// <summary>
        /// GetLHUserNavigation.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("GetLHUserNavigation")]
        public async Task<List<Dictionary<string, object>>> GetLHUserNavigation()
        {
            NavigationModel model;

            if (!this.User.Identity.IsAuthenticated)
            {
                model = this.permissionService.NotAuthenticated();
            }
            else
            {
                var userId = this.CurrentUserId.GetValueOrDefault();

                var (cacheExists, _) = await this.cacheService.TryGetAsync<string>($"{userId}:LoginWizard");

                model = await this.permissionService.GetNavigationModelAsync(this.User, !cacheExists, string.Empty);

                model.NotificationCount = await this.userNotificationService.GetUserUnreadNotificationCountAsync(userId);
            }

            return this.MenuItems(model);
        }

        private List<Dictionary<string, object>> MenuItems(NavigationModel model)
        {
            var menu = new List<Dictionary<string, object>>
            {
               new Dictionary<string, object>
                {
                    { "title", "Home" },
                    { "url", this.learningHubConfig.BaseUrl },
                    { "visible", model.ShowMyLearning },
                },
               new Dictionary<string, object>
                {
                    { "title", "My learning activity" },
                    { "url", this.learningHubConfig.MyLearningUrl },
                    { "visible", model.ShowMyLearning },
                },
               new Dictionary<string, object>
                {
                    { "title", "My contributions" },
                    { "url", this.learningHubConfig.MyContributionsUrl },
                    { "visible", model.ShowMyContributions },
                },
               new Dictionary<string, object>
                {
                    { "title", "Browse catalogues" },
                    { "url", this.learningHubConfig.BrowseCataloguesUrl },
                    { "visible", model.ShowBrowseCatalogues },
                },
               new Dictionary<string, object>
                {
                    { "title", "Reports" },
                    { "url", this.learningHubConfig.ReportUrl },
                    { "visible", model.ShowReports },
                },
               new Dictionary<string, object>
                {
                    { "title", "Admin" },
                    { "url", this.learningHubConfig.AdminUrl },
                    { "visible", model.ShowAdmin },
                },
               new Dictionary<string, object>
                {
                    { "title", "Sign Out" },
                    { "url", this.learningHubConfig.SignOutUrl },
                    { "visible", model.ShowSignOut },
                },
            };
            return menu;
        }
    }
}