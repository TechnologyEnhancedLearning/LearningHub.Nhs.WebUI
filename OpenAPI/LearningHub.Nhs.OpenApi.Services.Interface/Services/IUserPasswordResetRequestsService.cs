namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Theuser password resets interface.
    /// </summary>
    public interface IUserPasswordResetRequestsService
    {
        /// <summary>
        /// The check user can rtequest password reset async.
        /// </summary>
        /// <param name="emailAddress">The user name.</param>
        /// <param name="passwordRequestLimitingPeriod">The passwordRequestLimitingPeriod.</param>
        /// <param name="passwordRequestLimit">ThepasswordRequestLimit.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> CanRequestPasswordReset(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit);

        /// <summary>
        /// CreatePasswordRequests.
        /// </summary>
        /// <param name="emailAddress">The emailAddress.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> CreateUserPasswordRequest(string emailAddress);
    }
}
