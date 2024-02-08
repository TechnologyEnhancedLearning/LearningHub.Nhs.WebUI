namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The UserProfileRepository interface.
    /// </summary>
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserProfile> GetByIdAsync(int id);

        /// <summary>
        /// The GetByEmailAddressAsync.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns>The userProfile.</returns>
        Task<UserProfile> GetByEmailAddressAsync(string emailAddress);

        /// <summary>
        /// The GetByUsernameAsync.
        /// </summary>
        /// <param name="userName">The userName.</param>
        /// <returns>The userProfile.</returns>
        Task<UserProfile> GetByUsernameAsync(string userName);
    }
}
