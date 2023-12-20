// <copyright file="IRoleRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The RoleRepository interface.
    /// </summary>
    public interface IRoleRepository : IGenericRepository<Role>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Role> GetByIdAsync(int id);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="includePermissions">The include permissions.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<Role> GetByIdAsync(int id, bool includePermissions);

        /// <summary>
        /// The get available for user group async.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<Role>> GetAvailableForUserGroupAsync(int userGroupId);

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="role">The role.</param>
        /// <param name="updatePermissions">The update permissions.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateAsync(int userId, Role role, bool updatePermissions);

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="roleId">The role id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task DeleteAsync(int userId, int roleId);

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="pagesReturned">The pages returned.</param>
        /// <returns>The Role list.</returns>
        List<Role> Search(string searchText, int page, int pageSize, out int pagesReturned);

        /// <summary>
        /// Get Roles.
        /// </summary>
        /// <returns>List of role including permissions.</returns>
        Task<IEnumerable<Role>> GetAllRoles();
    }
}
