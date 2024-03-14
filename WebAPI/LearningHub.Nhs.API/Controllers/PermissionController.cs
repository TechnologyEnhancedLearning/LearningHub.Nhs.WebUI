namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Permission operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ApiControllerBase
    {
        /// <summary>
        /// The permission service.
        /// </summary>
        private readonly IPermissionService permissionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="permissionService">The permission service.</param>
        /// <param name="logger">The logger.</param>
        public PermissionController(
            IUserService userService,
            IPermissionService permissionService,
            ILogger<PermissionController> logger)
            : base(userService, logger)
        {
            this.permissionService = permissionService;
        }

        /// <summary>
        /// Get specific Permission by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{id}/{includeRoles}")]
        public async Task<ActionResult<Permission>> GetAsync(int id, bool includeRoles)
        {
            return this.Ok(await this.permissionService.GetByIdAsync(id, includeRoles));
        }

        /// <summary>
        /// Get Permissions available for Role.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("getAvailableForRole/{roleId}")]
        public async Task<ActionResult<Permission>> GetAvailableForRoleAsync(int roleId)
        {
            return this.Ok(await this.permissionService.GetAvailableForRoleAsync(roleId));
        }

        /// <summary>
        /// Search Permissions.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [Route("search/{searchText}/{page}/{pageSize}")]
        public ActionResult<SearchResult> Search(string searchText, int page, int pageSize)
        {
            return this.Ok(this.permissionService.Search(searchText, page, pageSize));
        }

        /// <summary>
        /// Create a new Permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Permission permission)
        {
            var vr = await this.permissionService.CreateAsync(this.CurrentUserId, permission);
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
        /// Update an existing Permission.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] Permission permission)
        {
            var vr = await this.permissionService.UpdateAsync(this.CurrentUserId, permission);
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
        /// Delete specific Permission by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpDelete("{id}")]
        public async Task DeleteAsync(int id)
        {
            await this.permissionService.DeleteAsync(this.CurrentUserId, id);
        }
    }
}
