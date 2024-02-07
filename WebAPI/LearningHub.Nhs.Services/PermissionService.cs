// <copyright file="PermissionService.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The permission service.
    /// </summary>
    public class PermissionService : IPermissionService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger<PermissionService> logger;

        /// <summary>
        /// The permission repository.
        /// </summary>
        private IPermissionRepository permissionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionService"/> class.
        /// </summary>
        /// <param name="permissionRepository">The permission repository.</param>
        /// <param name="logger">The logger.</param>
        public PermissionService(
            IPermissionRepository permissionRepository,
            ILogger<PermissionService> logger)
        {
            this.permissionRepository = permissionRepository;
            this.logger = logger;
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Permission> GetByIdAsync(int id, bool includeRoles)
        {
            return await this.permissionRepository.GetByIdAsync(id, includeRoles);
        }

        /// <summary>
        /// The get available for role async.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<Permission>> GetAvailableForRoleAsync(int roleId)
        {
            return await this.permissionRepository.GetAvailableForRoleAsync(roleId);
        }

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="permission">The permission.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> CreateAsync(int userId, Permission permission)
        {
            var vr = await this.ValidateAsync(permission);

            if (vr.IsValid)
            {
                await this.permissionRepository.CreateAsync(userId, permission);
            }

            return vr;
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="permission">The permission.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> UpdateAsync(int userId, Permission permission)
        {
            var vr = await this.ValidateAsync(permission);

            if (vr.IsValid)
            {
                await this.permissionRepository.UpdateAsync(userId, permission);
            }

            return vr;
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="permissionId">The permission id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int userId, int permissionId)
        {
            await this.permissionRepository.DeleteAsync(userId, permissionId);
        }

        /// <summary>
        /// The validate async.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<LearningHubValidationResult> ValidateAsync(Permission permission)
        {
            var permissionValidator = new PermissionValidator();
            var clientValidationResult = await permissionValidator.ValidateAsync(permission);

            var retVal = new LearningHubValidationResult(clientValidationResult);

            if (retVal.IsValid)
            {
                var ug = await this.permissionRepository.GetByNameAsync(permission.Name);
                if (ug != null)
                {
                    if (permission.IsNew()
                        || (!permission.IsNew() && permission.Id != ug.Id))
                    {
                        var detail = string.Format("Name '{0}' is already in use", permission.Name);
                        retVal.Add(new LearningHubValidationResult(false, detail));
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="SearchResult"/>.</returns>
        public SearchResult Search(string searchText, int page, int pageSize)
        {
            int pagesReturned;
            var results = this.permissionRepository.Search(searchText, page, pageSize, out pagesReturned);
            return new SearchResult()
            {
                Page = page,
                PageSize = pageSize,
                PagesReturned = pagesReturned,
                Results = results.ConvertAll(x => (EntityBase)x),
            };
        }
    }
}
