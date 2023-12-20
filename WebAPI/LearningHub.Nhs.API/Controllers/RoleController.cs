// <copyright file="RoleController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

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
    /// Role operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ApiControllerBase
    {
        /// <summary>
        /// The role service.
        /// </summary>
        private readonly IRoleService roleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user service.
        /// </param>
        /// <param name="roleService">
        /// The role service.
        /// </param>
        /// <param name="logger">The logger.</param>
        public RoleController(
            IUserService userService,
            IRoleService roleService,
            ILogger<RoleController> logger)
            : base(userService, logger)
        {
            this.roleService = roleService;
        }

        /// <summary>
        /// Get Roles.
        /// </summary>
        /// <returns>List of role including permissions.</returns>
        [HttpGet("all")]
        public async Task<ActionResult> GetAllRoles()
        {
            return this.Ok(await this.roleService.GetAllRoles());
        }

        /// <summary>
        /// Get specific Role by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includePermissions">The include permissions.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{id}/{includePermissions}")]
        public async Task<ActionResult> GetAsync(int id, bool includePermissions)
        {
            return this.Ok(await this.roleService.GetByIdAsync(id, includePermissions));
        }

        /// <summary>
        /// Get Roles available for User Group.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [AllowAnonymous] // todo
        [Route("getAvailableForUserGroup/{userGroupId}")]
        public async Task<ActionResult<Role>> GetAvailableForUserGroupAsync(int userGroupId)
        {
            return this.Ok(await this.roleService.GetAvailableForUserGroupAsync(userGroupId));
        }

        /// <summary>
        /// Search roles.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        [Route("search/{searchText}/{page}/{pageSize}")]
        public ActionResult<SearchResult> Search(string searchText, int page, int pageSize)
        {
            return this.Ok(this.roleService.Search(searchText, page, pageSize));
        }

        /// <summary>
        /// The create role async.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] Role role)
        {
            var vr = await this.roleService.CreateAsync(this.CurrentUserId, role);
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
        /// Update an existing Role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="includePermissions">The include permissions.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpPut("{includePermissions}")]
        public async Task<IActionResult> PutAsync([FromBody] Role role, bool includePermissions)
        {
            var vr = await this.roleService.UpdateAsync(this.CurrentUserId, role, includePermissions);
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
        /// Delete specific Role by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [Authorize(Roles = "System Administrator")]
        [HttpDelete("{id}")]
        public async Task DeleteAsync(int id)
        {
            await this.roleService.DeleteAsync(this.CurrentUserId, id);
        }
    }
}
