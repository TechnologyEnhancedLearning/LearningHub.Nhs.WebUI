namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The UserGroupAttributeRepository interface.
    /// </summary>
    public interface IUserGroupAttributeRepository : IGenericRepository<UserGroupAttribute>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserGroupAttribute> GetByIdAsync(int id);

        /// <summary>
        /// The get by user group id async.
        /// </summary>
        /// <param name="userGroupId">The user group id.</param>
        /// <param name="attributeId">The attribute id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserGroupAttribute> GetByUserGroupIdAttributeId(int userGroupId, int attributeId);
    }
}
