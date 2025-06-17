namespace LearningHub.Nhs.Repository.Interface
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// The UserPasswordResetRequestsRepository interface.
    /// </summary>
    public interface IUserPasswordResetRequestsRepository
    {
        /// <summary>
        /// To check user can request a password reset.
        /// </summary>
        /// <param name="emailAddress">
        /// The lookup.
        /// </param>
        /// <param name="passwordRequestLimitingPeriod">The passwordRequestLimitingPeriod.</param>
        /// <param name="passwordRequestLimit">ThepasswordRequestLimit.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> CanRequestPasswordResetAsync(string emailAddress, int passwordRequestLimitingPeriod, int passwordRequestLimit);

        /// <summary>
        /// CreatePasswordRequests.
        /// </summary>
        /// <param name="emailAddress">The emailAddress.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<bool> CreatePasswordRequests(string emailAddress);
    }
}