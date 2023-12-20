// <copyright file="IRoleService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.User;

    /// <summary>
    /// Role service interface.
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Get Roles.
        /// </summary>
        /// <returns>List of role including permissions.</returns>
        Task<IEnumerable<RolePermissionsViewModel>> GetRolesAsync();
    }
}