// <copyright file="UserGroupController.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// User Group operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ApiControllerBase
    {
        /// <summary>
        /// The user group service.
        /// </summary>
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="userGroupService">The user group service.</param>
        /// <param name="logger">The logger.</param>
        public UserGroupController(
            IUserService userService,
            IUserGroupService userGroupService,
            ILogger<UserGroupController> logger)
            : base(userService, logger)
        {
            this.userGroupService = userGroupService;
        }

        /// <summary>
        /// Returns the UserGroupAdminDetail model for a particular user group id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserGroupAdminDetailById/{id}")]
        public async Task<IActionResult> GetUserGroupAdminDetailById(int id)
        {
            var userGroup = await this.userGroupService.GetUserGroupAdminDetailByIdAsync(id);

            return this.Ok(userGroup);
        }

        /// <summary>
        /// Returns the UserGroupAdminDetail model for a particular user group id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserGroupAdminRoleDetailById/{id}")]
        public IActionResult GetUserGroupAdminRoleDetailById(int id)
        {
            var retVal = this.userGroupService.GetUserGroupRoleDetailByUserGroupId(id);

            return this.Ok(retVal);
        }

        /// <summary>
        /// Returns the user group - role detail for a particular user  id.
        /// </summary>
        /// <param name="userId">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserGroupRoleDetailByUserId/{userId}")]
        public IActionResult GetRoleUserGroupDetailByUserId(int userId)
        {
            var retVal = this.userGroupService.GetRoleUserGroupDetailByUserId(userId);

            return this.Ok(retVal);
        }

        /// <summary>
        /// Returns the user group - role detail for a current user  id.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserGroupRoleDetail")]
        public IActionResult GetRoleUserGroupDetail()
        {
            var retVal = this.userGroupService.GetRoleUserGroupDetailByUserId(this.CurrentUserId);

            return this.Ok(retVal);
        }

        /// <summary>
        /// Create a User Group.
        /// </summary>
        /// <param name="userGroup">The user group.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("CreateUserGroup")]
        public async Task<IActionResult> CreateUserGroupAsync(UserGroupAdminDetailViewModel userGroup)
        {
            var vr = await this.userGroupService.CreateUserGroupAsync(userGroup, this.CurrentUserId);
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
        /// Update an existing User Group.
        /// </summary>
        /// <param name="userGroup">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("UpdateUserGroup")]
        public async Task<IActionResult> UpdateUserGroup(UserGroupAdminDetailViewModel userGroup)
        {
            var vr = await this.userGroupService.UpdateUserGroupAsync(userGroup, this.CurrentUserId);
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
        /// Delete an existing User Group.
        /// </summary>
        /// <param name="userGroup">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("DeleteUserGroup")]
        public async Task<IActionResult> DeleteUserGroup(UserGroupAdminBasicViewModel userGroup)
        {
            var vr = await this.userGroupService.DeleteAsync(this.CurrentUserId, userGroup.Id);
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
        /// Add Users to User Group.
        /// </summary>
        /// <param name="userUserGroups">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("AddUserUserGroups")]
        public async Task<IActionResult> AddUserUserGroups(List<UserUserGroupViewModel> userUserGroups)
        {
            try
            {
                var vr = await this.userGroupService.AddUserUserGroups(userUserGroups, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Add Role - User Group - Scope association.
        /// </summary>
        /// <param name="roleUserGroups">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("AddRoleUserGroups")]
        public async Task<IActionResult> AddRoleUserGroups(List<RoleUserGroupUpdateViewModel> roleUserGroups)
        {
            try
            {
                var vr = await this.userGroupService.AddRoleUserGroups(roleUserGroups, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Removes User from a User Group.
        /// </summary>
        /// <param name="userUserGroupViewModel">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("DeleteUserUserGroup")]
        public async Task<IActionResult> DeleteUserUserGroupAsync(UserUserGroupViewModel userUserGroupViewModel)
        {
            try
            {
                var vr = await this.userGroupService.DeleteUserUserGroupAsync(userUserGroupViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Add User Group - Attribute.
        /// </summary>
        /// <param name="userGroupAttribute">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("AddUserGroupAttribute")]
        public async Task<IActionResult> AddUserGroupAttributeAsync(UserGroupAttributeViewModel userGroupAttribute)
        {
            try
            {
                var vr = await this.userGroupService.AddUserGroupAttribute(userGroupAttribute, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Removes a User Group Attribute.
        /// </summary>
        /// <param name="userGroupAttribute">The user.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("DeleteUserGroupAttribute")]
        public async Task<IActionResult> DeleteUserGroupAttributeAsync(UserGroupAttributeViewModel userGroupAttribute)
        {
            try
            {
                var vr = await this.userGroupService.DeleteUserGroupAttributeAsync(userGroupAttribute, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Removes a Role - User Group.
        /// </summary>
        /// <param name="roleUserGroupUpdateViewModel">The roleUserGroupUpdateViewModel.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [Route("DeleteRoleUserGroup")]
        public async Task<IActionResult> DeleteRoleUserGroupAsync(RoleUserGroupUpdateViewModel roleUserGroupUpdateViewModel)
        {
            try
            {
                var vr = await this.userGroupService.DeleteRoleUserGroupAsync(roleUserGroupUpdateViewModel, this.CurrentUserId);
                return this.Ok(new ApiResponse(true, vr));
            }
            catch (Exception ex)
            {
                return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
            }
        }

        /// <summary>
        /// Get a filtered page of User records.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The preset filter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserGroupAdminBasicFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        public async Task<IActionResult> GetUserGroupAdminBasicFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        {
            PagedResultSet<UserGroupAdminBasicViewModel> pagedResultSet = await this.userGroupService.GetUserGroupAdminBasicPageAsync(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
            return this.Ok(pagedResultSet);
        }

        /// <summary>
        /// Get a filtered page of User records.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The presetFilter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetUserUserGroupAdminFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        public async Task<IActionResult> GetUserUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        {
            PagedResultSet<UserUserGroupViewModel> pagedResultSet = await this.userGroupService.GetUserUserGroupAdminFilteredPage(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
            return this.Ok(pagedResultSet);
        }

        /// <summary>
        /// Get a filtered page of role user group records.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="presetFilter">The presetFilter.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet]
        [Route("GetRoleUserGroupAdminFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        public async Task<IActionResult> GetRoleUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        {
            PagedResultSet<RoleUserGroupViewModel> pagedResultSet = await this.userGroupService.GetRoleUserGroupAdminFilteredPage(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
            return this.Ok(pagedResultSet);
        }

        /// <summary>
        /// Get specific UserGroup by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpGet("{id}/{includeRoles}")]
        public async Task<ActionResult<UserGroup>> GetAsync(int id, bool includeRoles)
        {
            return this.Ok(await this.userGroupService.GetByIdAsync(id, includeRoles));
        }

        /// <summary>
        /// Create a new UserGroup.
        /// </summary>
        /// <param name="userGroup">The user group.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        //// todo[Authorize(Roles = "System Administrator")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] UserGroup userGroup)
        {
            var vr = await this.userGroupService.CreateAsync(this.CurrentUserId, userGroup);

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
        /// Delete specific UserGroup by Id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var vr = await this.userGroupService.DeleteAsync(this.CurrentUserId, id);
            if (vr.IsValid)
            {
                return this.Ok(new ApiResponse(true, vr));
            }
            else
            {
                return this.BadRequest(new ApiResponse(false, vr));
            }
        }
    }
}
