namespace LearningHub.Nhs.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using Microsoft.AspNetCore.Mvc;

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
