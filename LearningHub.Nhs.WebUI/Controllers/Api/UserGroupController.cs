// <copyright file="UserGroupController.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Controllers.Api
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The UserGroupController class.
    /// </summary>
    [Authorize]
    [Route("api")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupController"/> class.
        /// </summary>
        /// <param name="userGroupService">The UserGroupService.</param>
        public UserGroupController(IUserGroupService userGroupService)
        {
            this.userGroupService = userGroupService;
        }

        /// <summary>
        /// Gets the role user group detail for the current user.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("UserGroup/GetRoleUserGroupDetail")]
        public async Task<IActionResult> GetRoleUserGroupDetail()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return this.Ok(new List<RoleUserGroupViewModel>());
            }

            return this.Ok(await this.userGroupService.GetRoleUserGroupDetailAsync());
        }

        /// <summary>
        /// Gets the role user group detail for the supplied user id.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet]
        [Route("UserGroup/GetRoleUserGroupDetail/{userId}")]
        public async Task<IActionResult> GetRoleUserGroupDetailForUserId(int userId)
        {
            return this.Ok(await this.userGroupService.GetRoleUserGroupDetailForUserAsync(userId));
        }
    }
}
