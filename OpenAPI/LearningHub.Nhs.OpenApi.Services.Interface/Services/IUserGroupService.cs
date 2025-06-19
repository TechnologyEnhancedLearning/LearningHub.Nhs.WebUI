namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// The UserGroupService interface.
    /// </summary>
    public interface IUserGroupService
    {
        /// <summary>
        /// The GetRoleUserGroupDetailAsync.
        /// </summary>
        /// <returns>The <see cref="Task{List}"/>.</returns>
        Task<bool> UserHasCatalogueContributionPermission(int userId);
    }
}
