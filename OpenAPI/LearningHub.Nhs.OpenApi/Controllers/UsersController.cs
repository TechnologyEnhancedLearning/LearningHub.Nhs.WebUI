namespace LearningHub.Nhs.OpenApi.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.ProfessionalBody;
    using LearningHub.Nhs.Models.User;
    using LearningHub.NHS.OpenAPI.Controllers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Provides operations for Learning Hub users.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("users")]
    public class UsersController : OpenApiControllerBase
    {
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">userService.</param>
        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Searches users.
        /// </summary>
        /// <param name="request">Search and paging parameters.</param>
        /// <returns>A paged collection of users.</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResultSet<UserViewModel>>> SearchAsync([FromQuery] UserSearchRequest request)
        {
            var result = await this.userService.SearchAsync(request);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(
            typeof(UserViewModel),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> GetCurrentUserAsync()
        {
            var userId = this.CurrentUserId.GetValueOrDefault();

            var user = await this.userService.GetByIdAsync(userId);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }

        /// <summary>
        /// Gets a user by id.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(UserViewModel),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> GetByIdAsync(int id)
        {
            var user = await this.userService.GetByIdAsync(id);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserViewModel>> CreateAsync([FromBody] CreateUserRequest request)
        {
            var user = await this.userService.CreateAsync(request, this.CurrentUserId.GetValueOrDefault());

            return this.CreatedAtAction(nameof(this.GetByIdAsync), new { id = user.Id }, user);
        }

        /// <summary>
        /// Partially updates a user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> PatchAsync( int id, [FromBody] PatchUserRequest request)
        {
            var user = await this.userService.PatchAsync(id, request,  this.CurrentUserId.GetValueOrDefault());

            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }

        /// <summary>
        /// Partially updates the currently authenticated user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPatch("me")]
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserViewModel>> PatchCurrentUserAsync([FromBody] PatchUserRequest request)
        {
            var userId = this.CurrentUserId.GetValueOrDefault();

            var user = await this.userService.PatchAsync(userId,request,userId);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }

        /// <summary>
        /// Soft deletes a user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await this.userService.SoftDeleteAsync( id, this.CurrentUserId.GetValueOrDefault());

            if (!deleted)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        /// <summary>
        /// Restores a previously soft-deleted user.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost("{id:int}/restorations")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            var restored = await this.userService.RestoreAsync( id, this.CurrentUserId.GetValueOrDefault());

            if (!restored)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }

        /// <summary>
        /// Checks whether an email address is available for use.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AllowAnonymous]
        [HttpGet("email-availability")]
        [ProducesResponseType(typeof(EmailAvailabilityViewModel),StatusCodes.Status200OK)]
        public async Task<ActionResult<EmailAvailabilityViewModel>> GetEmailAvailabilityAsync([FromQuery] string email, [FromQuery] int? excludeUserId = null)
        {
            var available =
                await this.userService.IsEmailAvailableAsync(
                    email,
                    excludeUserId);

            return this.Ok(new EmailAvailabilityViewModel
            {
                Email = email,
                Available = available,
            });
        }

        /// <summary>
        /// Gets the available professional bodies.
        /// </summary>
        [HttpGet("professional-bodies")]
        [ProducesResponseType(typeof(IReadOnlyList<ProfessionalBodyViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProfessionalBodyViewModel>>> GetProfessionalBodiesAsync()
        {
            var professionalBodies = await this.userService.GetProfessionalBodiesAsync();

            return this.Ok(professionalBodies);
        }

        /// <summary>
        /// Removes a user's professional registration.
        /// </summary>
        [HttpDelete("{id:int}/professional-registration")]
        [ProducesResponseType(typeof(UserViewModel),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserViewModel>> ClearProfessionalRegistrationAsync(int id)
        {
            var user = await this.userService.ClearProfessionalRegistrationAsync(id, this.CurrentUserId.GetValueOrDefault());

            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user);
        }
    }
}