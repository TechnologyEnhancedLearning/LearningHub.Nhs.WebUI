// <copyright file="IUserUserGroupRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The UserUserGroupRepository interface.
    /// </summary>
    public interface IUserUserGroupRepository : IGenericRepository<UserUserGroup>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserUserGroup> GetByIdAsync(int id);

        /// <summary>
        /// to get the user user group details by user id and usergroup id.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="userGroupId">userGroupId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserUserGroup> GetByUserIdandUserGroupIdAsync(int userId, int userGroupId);
    }
}
