namespace LearningHub.Nhs.Api.Controllers
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.UserGroup;
    using LearningHub.NHS.OpenAPI.Controllers;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// User Group operations.
    /// </summary>
    [Authorize]
    [Route("usergroups")]
    [ApiController]
    public class UserGroupController : OpenApiControllerBase
    {
        /// <summary>
        /// The user group service.
        /// </summary>
        private readonly IUserGroupService userGroupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupController"/> class.
        /// </summary>
        /// <param name="userGroupService">The user group service.</param>

        public UserGroupController(IUserGroupService userGroupService)
        {
            this.userGroupService = userGroupService;
        }

        /// <summary>
        /// Gets all active user groups.
        /// </summary>
        /// <returns>A collection of user groups.</returns>
        /// <response code="200">
        /// The user groups were retrieved successfully.
        /// </response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await userGroupService.GetAllAsync();

            return Ok(result);
        }

        /// <summary>
        /// Gets a user group by identifier.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <returns>The requested user group.</returns>
        /// <response code="200">
        /// The user group was retrieved successfully.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(
            int id)
        {
            var result = await userGroupService.GetByIdAsync(id);

            return result == null
                ? NotFound()
                : Ok(result);
        }

        /// <summary>
        /// Creates a new user group.
        /// </summary>
        /// <param name="request">
        /// The user group details.
        /// </param>
        /// <returns>The newly created user group.</returns>
        /// <response code="201">
        /// The user group was created successfully.
        /// </response>
        /// <response code="400">
        /// The request failed validation.
        /// </response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(
            [FromBody] CreateUserGroupRequest request)
        {
            var created = await userGroupService.CreateAsync(
                request,
                CurrentUserId.GetValueOrDefault());

            return CreatedAtAction(
                nameof(GetByIdAsync),
                new { id = created.Id },
                created);
        }

        /// <summary>
        /// Partially updates a user group.
        /// </summary>
        /// <remarks>
        /// Only non-null properties in the request are updated.
        /// Properties with a null value are left unchanged.
        /// </remarks>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="request">
        /// The properties to update.
        /// </param>
        /// <returns>The updated user group.</returns>
        /// <response code="200">
        /// The user group was updated successfully.
        /// </response>
        /// <response code="400">
        /// The request failed validation.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchAsync(
            int id,
            [FromBody] UpdateUserGroupRequest request)
        {
            var updated = await userGroupService.PatchAsync(
                id,
                request,
                CurrentUserId.GetValueOrDefault());

            return updated == null
                ? NotFound()
                : Ok(updated);
        }

        /// <summary>
        /// Soft-deletes a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <response code="204">
        /// The user group was deleted successfully.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAsync(
            int id)
        {
            var deleted = await userGroupService.SoftDeleteAsync(
                id,
                CurrentUserId.GetValueOrDefault());

            return deleted
                ? NoContent()
                : NotFound();
        }

        /// <summary>
        /// Gets the users belonging to a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <response code="200">
        /// The users were retrieved successfully.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpGet("{id:int}/users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUsersAsync(
            int id)
        {
            var result = await userGroupService.GetUsersAsync(id);

            return result == null
                ? NotFound()
                : Ok(result);
        }

        /// <summary>
        /// Adds users to a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="request">
        /// The users to add.
        /// </param>
        /// <response code="201">
        /// The users were added successfully.
        /// </response>
        /// <response code="400">
        /// The request failed validation or contains
        /// duplicate memberships.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpPost("{id:int}/users")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddUsersAsync(
            int id,
            [FromBody] AddUserGroupUsersRequest request)
        {
            var result = await userGroupService.AddUsersAsync(
                id,
                request,
                CurrentUserId.GetValueOrDefault());

            if (result == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetUsersAsync),
                new { id },
                result);
        }

        /// <summary>
        /// Removes a user from a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="userId">
        /// The user identifier.
        /// </param>
        /// <response code="204">
        /// The user was removed successfully.
        /// </response>
        /// <response code="404">
        /// The user group membership was not found.
        /// </response>
        [HttpDelete("{id:int}/users/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveUserAsync(
            int id,
            int userId)
        {
            var removed = await userGroupService.RemoveUserAsync(
                id,
                userId,
                CurrentUserId.GetValueOrDefault());

            return removed
                ? NoContent()
                : NotFound();
        }

        /// <summary>
        /// Gets the reporters assigned to a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <response code="200">
        /// The reporters were retrieved successfully.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpGet("{id:int}/reporters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReportersAsync(
            int id)
        {
            var result = await userGroupService.GetReportersAsync(id);

            return result == null
                ? NotFound()
                : Ok(result);
        }

        /// <summary>
        /// Adds reporters to a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="request">
        /// The reporters to add.
        /// </param>
        /// <response code="201">
        /// The reporters were added successfully.
        /// </response>
        /// <response code="400">
        /// The request failed validation or contains
        /// duplicate reporter assignments.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpPost("{id:int}/reporters")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddReportersAsync(
            int id,
            [FromBody] AddUserGroupReportersRequest request)
        {
            var result = await userGroupService.AddReportersAsync(
                id,
                request,
                CurrentUserId.GetValueOrDefault());

            if (result == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetReportersAsync),
                new { id },
                result);
        }

        /// <summary>
        /// Removes a reporter from a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="userId">
        /// The reporter's user identifier.
        /// </param>
        /// <response code="204">
        /// The reporter was removed successfully.
        /// </response>
        /// <response code="404">
        /// The reporter assignment was not found.
        /// </response>
        [HttpDelete("{id:int}/reporters/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveReporterAsync(
            int id,
            int userId)
        {
            var removed = await userGroupService.RemoveReporterAsync(
                id,
                userId,
                CurrentUserId.GetValueOrDefault());

            return removed
                ? NoContent()
                : NotFound();
        }

        /// <summary>
        /// Gets catalogue links for a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <response code="200">
        /// The catalogue links were retrieved successfully.
        /// </response>
        /// <response code="404">
        /// The user group was not found.
        /// </response>
        [HttpGet("{id:int}/catalogues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCataloguesAsync(
            int id)
        {
            var result = await userGroupService.GetCataloguesAsync(id);

            return result == null
                ? NotFound()
                : Ok(result);
        }

        /// <summary>
        /// Links a catalogue and role to a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="request">
        /// The catalogue node and role to link.
        /// </param>
        /// <response code="201">
        /// The catalogue link was created successfully.
        /// </response>
        /// <response code="400">
        /// The request failed validation or the link
        /// already exists.
        /// </response>
        /// <response code="404">
        /// The user group or referenced resource
        /// was not found.
        /// </response>
        [HttpPost("{id:int}/catalogues")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LinkCatalogueAsync(
            int id,
            [FromBody] LinkUserGroupCatalogueRequest request)
        {
            var result = await userGroupService.LinkCatalogueAsync(
                id,
                request,
                CurrentUserId.GetValueOrDefault());

            if (result == null)
            {
                return NotFound();
            }

            return CreatedAtAction(
                nameof(GetCataloguesAsync),
                new { id },
                result);
        }

        /// <summary>
        /// Removes a catalogue and role link from a user group.
        /// </summary>
        /// <param name="id">
        /// The user group identifier.
        /// </param>
        /// <param name="catalogueNodeId">
        /// The catalogue node identifier.
        /// </param>
        /// <param name="roleId">
        /// The role identifier.
        /// </param>
        /// <response code="204">
        /// The catalogue link was removed successfully.
        /// </response>
        /// <response code="404">
        /// The catalogue link was not found.
        /// </response>
        [HttpDelete(
            "{id:int}/catalogues/{catalogueNodeId:int}/roles/{roleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlinkCatalogueAsync(
            int id,
            int catalogueNodeId,
            int roleId)
        {
            var removed = await userGroupService.UnlinkCatalogueAsync(
                id,
                catalogueNodeId,
                roleId,
                CurrentUserId.GetValueOrDefault());

            return removed
                ? NoContent()
                : NotFound();
        }

        /////////////////////////////////////////////////////////

        ///// <summary>
        ///// Returns the UserGroupAdminDetail model for a particular user group id.
        ///// </summary>
        ///// <param name="id">The id.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetUserGroupAdminDetailById/{id}")]
        //public async Task<IActionResult> GetUserGroupAdminDetailById(int id)
        //{
        //    var userGroup = await this.userGroupService.GetUserGroupAdminDetailByIdAsync(id);

        //    return this.Ok(userGroup);
        //}

        ///// <summary>
        ///// Returns the UserGroupAdminDetail model for a particular user group id.
        ///// </summary>
        ///// <param name="id">The id.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetUserGroupAdminRoleDetailById/{id}")]
        //public async Task<IActionResult> GetUserGroupAdminRoleDetailById(int id)
        //{
        //    var retVal = await this.userGroupService.GetUserGroupRoleDetailByUserGroupId(id);

        //    return this.Ok(retVal);
        //}

        ///// <summary>
        ///// Returns the user group - role detail for a particular user  id.
        ///// </summary>
        ///// <param name="userId">The id.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetUserGroupRoleDetailByUserId/{userId}")]
        //public async Task<IActionResult> GetRoleUserGroupDetailByUserId(int userId)
        //{
        //    var retVal = await this.userGroupService.GetRoleUserGroupDetailByUserId(userId);

        //    return this.Ok(retVal);
        //}

        ///// <summary>
        ///// Returns the user group - role detail for a current user  id.
        ///// </summary>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetUserGroupRoleDetail")]
        //public async Task<IActionResult> GetRoleUserGroupDetail()
        //{
        //    var retVal = await this.userGroupService.GetRoleUserGroupDetailByUserId(this.CurrentUserId.GetValueOrDefault());

        //    return this.Ok(retVal);
        //}

        ///// <summary>
        ///// Create a User Group.
        ///// </summary>
        ///// <param name="userGroup">The user group.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("CreateUserGroup")]
        //public async Task<IActionResult> CreateUserGroupAsync([FromBody]UserGroupAdminDetailViewModel userGroup)
        //{
        //    var vr = await this.userGroupService.CreateUserGroupAsync(userGroup, this.CurrentUserId.GetValueOrDefault());
        //    if (vr.IsValid)
        //    {
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    else
        //    {
        //        return this.BadRequest(new ApiResponse(false, vr));
        //    }
        //}

        ///// <summary>
        ///// Update an existing User Group.
        ///// </summary>
        ///// <param name="userGroup">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("UpdateUserGroup")]
        //public async Task<IActionResult> UpdateUserGroup(UserGroupAdminDetailViewModel userGroup)
        //{
        //    var vr = await this.userGroupService.UpdateUserGroupAsync(userGroup, this.CurrentUserId.GetValueOrDefault());
        //    if (vr.IsValid)
        //    {
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    else
        //    {
        //        return this.BadRequest(new ApiResponse(false, vr));
        //    }
        //}

        ///// <summary>
        ///// Delete an existing User Group.
        ///// </summary>
        ///// <param name="userGroup">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("DeleteUserGroup")]
        //public async Task<IActionResult> DeleteUserGroup(UserGroupAdminBasicViewModel userGroup)
        //{
        //    var vr = await this.userGroupService.DeleteAsync(this.CurrentUserId.GetValueOrDefault(), userGroup.Id);
        //    if (vr.IsValid)
        //    {
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    else
        //    {
        //        return this.BadRequest(new ApiResponse(false, vr));
        //    }
        //}

        ///// <summary>
        ///// Add Users to User Group.
        ///// </summary>
        ///// <param name="userUserGroups">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("AddUserUserGroups")]
        //public async Task<IActionResult> AddUserUserGroups(List<UserUserGroupViewModel> userUserGroups)
        //{
        //    try
        //    {
        //        var vr = await this.userGroupService.AddUserUserGroups(userUserGroups, this.CurrentUserId.GetValueOrDefault());
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
        //    }
        //}

        ///// <summary>
        ///// Add Role - User Group - Scope association.
        ///// </summary>
        ///// <param name="roleUserGroups">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("AddRoleUserGroups")]
        //public async Task<IActionResult> AddRoleUserGroups(List<RoleUserGroupUpdateViewModel> roleUserGroups)
        //{
        //    try
        //    {
        //        var vr = await this.userGroupService.AddRoleUserGroups(roleUserGroups, this.CurrentUserId.GetValueOrDefault());
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
        //    }
        //}

        ///// <summary>
        ///// Removes User from a User Group.
        ///// </summary>
        ///// <param name="userUserGroupViewModel">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("DeleteUserUserGroup")]
        //public async Task<IActionResult> DeleteUserUserGroupAsync(UserUserGroupViewModel userUserGroupViewModel)
        //{
        //    try
        //    {
        //        var vr = await this.userGroupService.DeleteUserUserGroupAsync(userUserGroupViewModel, this.CurrentUserId.GetValueOrDefault());
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
        //    }
        //}

        ///// <summary>
        ///// Add User Group - Attribute.
        ///// </summary>
        ///// <param name="userGroupAttribute">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("AddUserGroupAttribute")]
        //public async Task<IActionResult> AddUserGroupAttributeAsync(UserGroupAttributeViewModel userGroupAttribute)
        //{
        //    try
        //    {
        //        var vr = await this.userGroupService.AddUserGroupAttribute(userGroupAttribute, this.CurrentUserId.GetValueOrDefault());
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
        //    }
        //}

        ///// <summary>
        ///// Removes a User Group Attribute.
        ///// </summary>
        ///// <param name="userGroupAttribute">The user.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("DeleteUserGroupAttribute")]
        //public async Task<IActionResult> DeleteUserGroupAttributeAsync(UserGroupAttributeViewModel userGroupAttribute)
        //{
        //    try
        //    {
        //        var vr = await this.userGroupService.DeleteUserGroupAttributeAsync(userGroupAttribute, this.CurrentUserId.GetValueOrDefault());
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
        //    }
        //}

        ///// <summary>
        ///// Removes a Role - User Group.
        ///// </summary>
        ///// <param name="roleUserGroupUpdateViewModel">The roleUserGroupUpdateViewModel.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpPost]
        //[Route("DeleteRoleUserGroup")]
        //public async Task<IActionResult> DeleteRoleUserGroupAsync(RoleUserGroupUpdateViewModel roleUserGroupUpdateViewModel)
        //{
        //    try
        //    {
        //        var vr = await this.userGroupService.DeleteRoleUserGroupAsync(roleUserGroupUpdateViewModel, this.CurrentUserId.GetValueOrDefault());
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    catch (Exception ex)
        //    {
        //        return this.Ok(new ApiResponse(false, new LearningHubValidationResult(false, ex.Message)));
        //    }
        //}

        ///// <summary>
        ///// Get a filtered page of User records.
        ///// </summary>
        ///// <param name="page">The page.</param>
        ///// <param name="pageSize">The page size.</param>
        ///// <param name="sortColumn">The sort column.</param>
        ///// <param name="sortDirection">The sort direction.</param>
        ///// <param name="presetFilter">The preset filter.</param>
        ///// <param name="filter">The filter.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetUserGroupAdminBasicFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        //public async Task<IActionResult> GetUserGroupAdminBasicFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        //{
        //    PagedResultSet<UserGroupAdminBasicViewModel> pagedResultSet = await this.userGroupService.GetUserGroupAdminBasicPageAsync(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
        //    return this.Ok(pagedResultSet);
        //}

        ///// <summary>
        ///// Get a filtered page of User records.
        ///// </summary>
        ///// <param name="page">The page.</param>
        ///// <param name="pageSize">The page size.</param>
        ///// <param name="sortColumn">The sort column.</param>
        ///// <param name="sortDirection">The sort direction.</param>
        ///// <param name="presetFilter">The presetFilter.</param>
        ///// <param name="filter">The filter.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetUserUserGroupAdminFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        //public async Task<IActionResult> GetUserUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        //{
        //    PagedResultSet<UserUserGroupViewModel> pagedResultSet = await this.userGroupService.GetUserUserGroupAdminFilteredPage(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
        //    return this.Ok(pagedResultSet);
        //}

        ///// <summary>
        ///// Get a filtered page of role user group records.
        ///// </summary>
        ///// <param name="page">The page.</param>
        ///// <param name="pageSize">The page size.</param>
        ///// <param name="sortColumn">The sort column.</param>
        ///// <param name="sortDirection">The sort direction.</param>
        ///// <param name="presetFilter">The presetFilter.</param>
        ///// <param name="filter">The filter.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet]
        //[Route("GetRoleUserGroupAdminFilteredPage/{page}/{pageSize}/{sortColumn}/{sortDirection}/{presetFilter}/{filter}")]
        //public async Task<IActionResult> GetRoleUserGroupAdminFilteredPage(int page, int pageSize, string sortColumn, string sortDirection, string presetFilter, string filter)
        //{
        //    PagedResultSet<RoleUserGroupViewModel> pagedResultSet = await this.userGroupService.GetRoleUserGroupAdminFilteredPage(page, pageSize, sortColumn, sortDirection, presetFilter, filter);
        //    return this.Ok(pagedResultSet);
        //}

        ///// <summary>
        ///// Get specific UserGroup by Id.
        ///// </summary>
        ///// <param name="id">The id.</param>
        ///// <param name="includeRoles">The include roles.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        //[HttpGet("{id}/{includeRoles}")]
        //public async Task<ActionResult<UserGroup>> GetAsync(int id, bool includeRoles)
        //{
        //    return this.Ok(await this.userGroupService.GetByIdAsync(id, includeRoles));
        //}

        ///// <summary>
        ///// Create a new UserGroup.
        ///// </summary>
        ///// <param name="userGroup">The user group.</param>
        ///// <returns>The <see cref="Task"/>.</returns>
        ////// todo[Authorize(Roles = "System Administrator")]
        //[HttpPost]
        //public async Task<IActionResult> CreateAsync([FromBody] UserGroup userGroup)
        //{
        //    var vr = await this.userGroupService.CreateAsync(this.CurrentUserId.GetValueOrDefault(), userGroup);

        //    if (vr.IsValid)
        //    {
        //        return this.Ok(new ApiResponse(true, vr));
        //    }
        //    else
        //    {
        //        return this.BadRequest(new ApiResponse(false, vr));
        //    }
        //}
    }
}
