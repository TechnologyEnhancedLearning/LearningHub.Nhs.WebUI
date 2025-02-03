namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Validation;

    /// <summary>
    /// The UserProfileService interface.
    /// </summary>
    public interface IUserProfileService
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<UserProfile> GetByIdAsync(int id);

        /// <summary>
        /// Create a new userProfil async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userProfile">The user profile.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> CreateUserProfileAsync(int userId, UserProfile userProfile);

        /// <summary>
        /// Update the lh user async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="userProfile">The userProfile.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<LearningHubValidationResult> UpdateUserProfileAsync(int userId, UserProfile userProfile);
    }
}
