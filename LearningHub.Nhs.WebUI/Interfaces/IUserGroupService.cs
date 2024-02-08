namespace LearningHub.Nhs.WebUI.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.User;

    /// <summary>
    /// Defines the <see cref="IUserGroupService" />.
    /// </summary>
    public interface IUserGroupService
    {
        /// <summary>
        /// The GetRoleUserGroupDetailAsync.
        /// </summary>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailAsync();

        /// <summary>
        /// The GetRoleUserGroupDetailForUserAsync.
        /// </summary>
        /// <param name="userId">The userId.</param>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<List<RoleUserGroupViewModel>> GetRoleUserGroupDetailForUserAsync(int userId);

        /// <summary>
        /// Check if user has given permission.
        /// </summary>
        /// <param name="permissionCode">To check against permission code.</param>
        /// <returns>Success or not.</returns>
        Task<bool> UserHasPermissionAsync(string permissionCode);
    }
}
