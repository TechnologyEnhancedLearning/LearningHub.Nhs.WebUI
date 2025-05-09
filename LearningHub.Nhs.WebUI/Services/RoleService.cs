﻿namespace LearningHub.Nhs.WebUI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.WebUI.Interfaces;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The role service.
    /// </summary>
    public class RoleService : BaseService<RoleService>, IRoleService
    {
        private readonly ICacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="cacheService">The cache service.</param>
        /// <param name="learningHubHttpClient">Learning hub http client.</param>
        /// <param name="logger">Logger.</param>
        public RoleService(ICacheService cacheService, ILearningHubHttpClient learningHubHttpClient, ILogger<RoleService> logger)
        : base(learningHubHttpClient, logger)
        {
            this.cacheService = cacheService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RolePermissionsViewModel>> GetRolesAsync()
        {
            return await this.cacheService.GetOrFetchAsync("AllRolesWithPermissions", () => this.GetAllRoles());
        }

        private async Task<IEnumerable<RolePermissionsViewModel>> GetAllRoles()
        {
            IEnumerable<RolePermissionsViewModel> vm = null;

            var client = await this.LearningHubHttpClient.GetClientAsync();

            var response = await client.GetAsync("role/all").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                vm = JsonSerializer.Deserialize<IEnumerable<RolePermissionsViewModel>>(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
                        || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new Exception("AccessDenied");
            }

            return vm;
        }
    }
}