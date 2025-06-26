namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// The UserGroup service.
    /// </summary>
    public class UserGroupService : BaseService<UserGroupService>, IUserGroupService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ICacheService cacheService;
        private readonly IRoleService roleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learning hub http client.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="contextAccessor">The http context accessor.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="roleService">roleService.</param>
        public UserGroupService(
            ILearningHubHttpClient learningHubHttpClient,
            ILogger<UserGroupService> logger,
            IHttpContextAccessor contextAccessor,
            ICacheService cacheService,
            IRoleService roleService)
        : base(learningHubHttpClient, logger)
        {
            this.contextAccessor = contextAccessor;
            this.cacheService = cacheService;
            this.roleService = roleService;
        }

        /// <inheritdoc />
        public async Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailAsync()
        {
            var cacheKey = $"{this.contextAccessor.HttpContext.User.Identity.GetCurrentUserId()}:AllRolesWithPermissions";
            ////return await this.cacheService.GetOrFetchAsync(cacheKey, () => this.FetchRoleUserGroupDetailAsync());
            return await this.FetchRoleUserGroupDetailAsync();
        }

        /// <inheritdoc />
        public async Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailForUserAsync(int userId)
        {
            var cacheKey = $"{userId}:AllRolesWithPermissions";
            return await this.cacheService.GetOrFetchAsync(cacheKey, () => this.FetchRoleUserGroupDetailForUserAsync(userId));
        }

        /// <inheritdoc />
        public async Task<bool> UserHasCatalogueContributionPermission()
        {
            var userRoleGroups = await this.GetRoleUserGroupDetailAsync();
            if (userRoleGroups != null && userRoleGroups.Any(r => r.RoleEnum == RoleEnum.Editor))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<bool> UserHasPermissionAsync(string permissionCode)
        {
            var userGroupsTask = this.GetRoleUserGroupDetailAsync();
            var rolesTask = this.roleService.GetRolesAsync();

            await Task.WhenAll(userGroupsTask, rolesTask);

            var userRoles = userGroupsTask.Result.Select(t => t.RoleId).Distinct();

            return rolesTask.Result
                        .Where(r => userRoles.Contains(r.RoleId))
                        .SelectMany(r => r.Permissions)
                        .Any(p => p == permissionCode);
        }

        private async Task<List<RoleUserGroupViewModel>> FetchRoleUserGroupDetailAsync()
        {
            List<RoleUserGroupViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetUserGroupRoleDetail";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<RoleUserGroupViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        private async Task<List<RoleUserGroupViewModel>> FetchRoleUserGroupDetailForUserAsync(int userId)
        {
            List<RoleUserGroupViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetUserGroupRoleDetailByUserId/{userId}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<List<RoleUserGroupViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }
    }
}
