// <copyright file="UserGroupService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.AdminUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.AdminUI.Interfaces;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.Paging;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="UserGroupService" />.
    /// </summary>
    public class UserGroupService : BaseService, IUserGroupService
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly ICacheService cacheService;
        private readonly IRoleService roleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupService"/> class.
        /// </summary>
        /// <param name="learningHubHttpClient">The learningHubHttpClient<see cref="ILearningHubHttpClient"/>.</param>
        /// <param name="contextAccessor">The http context accessor.</param>
        /// <param name="cacheService">The cacheService.</param>
        /// <param name="roleService">The roleService.</param>
        public UserGroupService(
            ILearningHubHttpClient learningHubHttpClient,
            ICacheService cacheService,
            IRoleService roleService,
            IHttpContextAccessor contextAccessor)
        : base(learningHubHttpClient)
        {
            this.contextAccessor = contextAccessor;
            this.cacheService = cacheService;
            this.roleService = roleService;
        }

        /// <inheritdoc />
        public async Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailAsync()
        {
            var cacheKey = $"{this.contextAccessor.HttpContext.User.Identity.GetCurrentUserId()}:AllRolesWithPermissions";
            return await this.cacheService.GetOrFetchAsync(cacheKey, () => this.FetchRoleUserGroupDetailAsync());
        }

        /// <summary>
        /// The GetUserGroupAdminBasicPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserAdminBasicViewModel}"/>.</returns>
        public async Task<PagedResultSet<UserGroupAdminBasicViewModel>> GetUserGroupAdminBasicPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<UserGroupAdminBasicViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
            var presetFilter = JsonConvert.SerializeObject(pagingRequestModel.PresetFilter);

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetUserGroupAdminBasicFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{Uri.EscapeUriString(presetFilter)}"
                + $"/{Uri.EscapeUriString(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<UserGroupAdminBasicViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserGroupAdminDetailbyIdAsync.
        /// </summary>
        /// <param name="id">The id.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{UserGroupAdminDetailViewModel}"/>.</returns>
        public async Task<UserGroupAdminDetailViewModel> GetUserGroupAdminDetailbyIdAsync(int id)
        {
            UserGroupAdminDetailViewModel viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetUserGroupAdminDetailById/{id}";
            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<UserGroupAdminDetailViewModel>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetUserGroupAdminRoleDetailByIdAsync.
        /// </summary>
        /// <param name="id">The id.<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        public async Task<List<RoleUserGroupViewModel>> GetUserGroupAdminRoleDetailByIdAsync(int id)
        {
            List<RoleUserGroupViewModel> viewmodel = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetUserGroupAdminRoleDetailById/{id}";
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

        /// <summary>
        /// The CreateUserGroup.
        /// </summary>
        /// <param name="userGroup">The user<see cref="UserGroupAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> CreateUserGroup(UserGroupAdminDetailViewModel userGroup)
        {
            var json = JsonConvert.SerializeObject(userGroup);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/CreateUserGroup";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The UpdateUserGroup.
        /// </summary>
        /// <param name="userGroup">The user<see cref="UserGroupAdminDetailViewModel"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateUserGroup(UserGroupAdminDetailViewModel userGroup)
        {
            var json = JsonConvert.SerializeObject(userGroup);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/UpdateUserGroup";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("save failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The DeleteUserGroup.
        /// </summary>
        /// <param name="userGroupId">The userGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteUserGroup(int userGroupId)
        {
            var userGroup = new UserGroupAdminBasicViewModel() { Id = userGroupId };
            var json = JsonConvert.SerializeObject(userGroup);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/DeleteUserGroup";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The AddUserGroupsToUser.
        /// </summary>
        /// <param name="userGroupId">The userId<see cref="int"/>.</param>
        /// <param name="userIdList">The userIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AddUsersToUserGroup(int userGroupId, string userIdList)
        {
            var userUserGroups = new List<UserUserGroupViewModel>();
            foreach (var userId in userIdList.Split(","))
            {
                userUserGroups.Add(new UserUserGroupViewModel() { UserGroupId = userGroupId, UserId = int.Parse(userId) });
            }

            var json = JsonConvert.SerializeObject(userUserGroups);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/AddUserUserGroups";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("add failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The AddUserGroupsToCatalogue.
        /// </summary>
        /// <param name="catalogueNodeId">The catalogueNodeId<see cref="int"/>.</param>
        /// <param name="roleId">The id<see cref="int"/>.</param>
        /// <param name="userGroupIdList">The userGroupIdList<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{IActionResult}"/>.</returns>
        public async Task<LearningHubValidationResult> AddUserGroupsToCatalogue(int catalogueNodeId, int roleId, string userGroupIdList)
        {
            var roleUserGroups = new List<RoleUserGroupUpdateViewModel>();
            foreach (var userGroupId in userGroupIdList.Split(","))
            {
                roleUserGroups.Add(new RoleUserGroupUpdateViewModel() { UserGroupId = int.Parse(userGroupId), RoleId = roleId, ScopeType = Nhs.Models.Enums.ScopeTypeEnum.Catalogue, CatalogueNodeId = catalogueNodeId });
            }

            var json = JsonConvert.SerializeObject(roleUserGroups);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/AddRoleUserGroups";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("add failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The DeleteUserUserGroup.
        /// </summary>
        /// <param name="userUserGroupId">The userUserGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteUserUserGroup(int userUserGroupId)
        {
            var userGroup = new UserUserGroupViewModel() { Id = userUserGroupId };
            var json = JsonConvert.SerializeObject(userGroup);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/DeleteUserUserGroup";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The DeleteRoleUserGroup.
        /// </summary>
        /// <param name="roleUserGroupId">The roleUserGroupId<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{LearningHubValidationResult}"/>.</returns>
        public async Task<LearningHubValidationResult> DeleteRoleUserGroup(int roleUserGroupId)
        {
            var roleUserGroupUpdate = new RoleUserGroupUpdateViewModel() { Id = roleUserGroupId };
            var json = JsonConvert.SerializeObject(roleUserGroupUpdate);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/DeleteRoleUserGroup";
            var response = await client.PostAsync(request, stringContent).ConfigureAwait(false);

            ApiResponse apiResponse = null;
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

                if (!apiResponse.Success)
                {
                    throw new Exception("delete failed!");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);
                if (apiResponse.ValidationResult == null)
                {
                    apiResponse.ValidationResult = new LearningHubValidationResult(false, "Error encountered performing requested operation.");
                }
            }

            return apiResponse.ValidationResult;
        }

        /// <summary>
        /// The GetUserUserGroupPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{UserUserGroupViewModel}"/>.</returns>
        public async Task<PagedResultSet<UserUserGroupViewModel>> GetUserUserGroupPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<UserUserGroupViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
            var presetFilter = JsonConvert.SerializeObject(pagingRequestModel.PresetFilter);

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetUserUserGroupAdminFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{Uri.EscapeUriString(presetFilter)}"
                + $"/{Uri.EscapeUriString(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<UserUserGroupViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
        }

        /// <summary>
        /// The GetRoleUserGroupPageAsync.
        /// </summary>
        /// <param name="pagingRequestModel">The pagingRequestModel<see cref="PagingRequestModel"/>.</param>
        /// <returns>The <see cref="PagedResultSet{RoleUserGroupViewModel}"/>.</returns>
        public async Task<PagedResultSet<RoleUserGroupViewModel>> GetRoleUserGroupPageAsync(PagingRequestModel pagingRequestModel)
        {
            PagedResultSet<RoleUserGroupViewModel> viewmodel = null;

            if (string.IsNullOrEmpty(pagingRequestModel.SortColumn))
            {
                pagingRequestModel.SortColumn = " ";
            }

            if (string.IsNullOrEmpty(pagingRequestModel.SortDirection))
            {
                pagingRequestModel.SortDirection = " ";
            }

            var filter = JsonConvert.SerializeObject(pagingRequestModel.Filter);
            var presetFilter = JsonConvert.SerializeObject(pagingRequestModel.PresetFilter);

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var request = $"UserGroup/GetRoleUserGroupAdminFilteredPage"
                + $"/{pagingRequestModel.Page}"
                + $"/{pagingRequestModel.PageSize}"
                + $"/{pagingRequestModel.SortColumn}"
                + $"/{pagingRequestModel.SortDirection}"
                + $"/{Uri.EscapeUriString(presetFilter)}"
                + $"/{Uri.EscapeUriString(filter)}";

            var response = await client.GetAsync(request).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                viewmodel = JsonConvert.DeserializeObject<PagedResultSet<RoleUserGroupViewModel>>(result);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return viewmodel;
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
    }
}
