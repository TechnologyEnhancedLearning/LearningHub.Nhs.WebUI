namespace LearningHub.NHS.OpenAPI.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;
    using LearningHub.NHS.OpenAPI.Helpers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The lh user service.
        /// </param>
        /// <param name="userProfileService">
        /// The user profile service.
        /// </param>

        /// <param name="securityService">The security service.</param>
        /// <param name="logger">The logger.</param>
        public UserController(
            IUserService userService,
            IUserProfileService userProfileService,
            ISecurityService securityService,
            ILogger<UserController> logger)
        {
            this.userProfileService = userProfileService;
            this.securityService = securityService;
            this.userService = userService;
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
        /// Get specific User Profile by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetByUserId/{id}")]
        public async Task<ActionResult<User>> GetByUserIdAsync(int id)
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

    }
}