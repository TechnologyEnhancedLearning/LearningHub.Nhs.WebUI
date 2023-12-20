// <copyright file="IPermissionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The PermissionRepository interface.
    /// </summary>
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Permission> GetByIdAsync(int id);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includeRoles">The include roles.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Permission> GetByIdAsync(int id, bool includeRoles);

        /// <summary>
        /// The get by name async.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Permission> GetByNameAsync(string name);

        /// <summary>
        /// The get available for role async.
        /// </summary>
        /// <param name="roleId">The role id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<Permission>> GetAvailableForRoleAsync(int roleId);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="permissionId">The permission id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int userId, int permissionId);

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="pagesReturned">The pages returned.</param>
        /// <returns>The Permission list.</returns>
        List<Permission> Search(string searchText, int page, int pageSize, out int pagesReturned);
    }
}
