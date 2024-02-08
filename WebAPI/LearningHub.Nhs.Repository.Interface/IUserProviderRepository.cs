namespace LearningHub.Nhs.Repository.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Provider;

    /// <summary>
    /// The UserProviderRepository interface.
    /// </summary>
    public interface IUserProviderRepository : IGenericRepository<UserProvider>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserProvider> GetByIdAsync(int id);

        /// <summary>
        /// The get user provider list by user id async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<List<UserProvider>> GetByUserIdAsync(int userId);

        /// <summary>
        /// The update provider list by user id async.
        /// </summary>
        /// <param name="userProviderUpdateModel">The user provider update model.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task UpdateUserProviderAsync(UserProviderUpdateViewModel userProviderUpdateModel);
    }
}
