namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;

    /// <summary>
    /// The SecurityService interface.
    /// </summary>
    public interface ISecurityService
    {

        /// <summary>
        /// The validate token async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="loctoken">
        /// The loctoken.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<PasswordValidationTokenResult> ValidateTokenAsync(string token, string loctoken);

        /// <summary>
        /// The set initial password async.
        /// </summary>
        /// <param name="passwordCreateModel">
        /// The password create model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> SetInitialPasswordAsync(PasswordCreateModel passwordCreateModel);

        /// <summary>
        /// The email change validate token async.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="loctoken">
        /// The loctoken.
        /// </param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<EmailChangeValidationTokenResult> ValidateEmailChangeTokenAsync(string token, string loctoken, bool isUserRoleUpgrade);

        /// <summary>
        /// GenerateEmailChangeValidationTokenAndSendEmail.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <param name="email">email.</param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task GenerateEmailChangeValidationTokenAndSendEmail(int userId, string email, bool isUserRoleUpgrade);

        /// <summary>
        /// GetLastIssuedEmailChangeValidationToken.
        /// </summary>
        /// <param name="userId">userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<EmailChangeValidationTokenViewModel> GetLastIssuedEmailChangeValidationToken(int userId);

        /// <summary>
        /// ReGenerateEmailChangeValidationToken.
        /// </summary>
        /// <param name="userId">
        /// The userId.
        /// </param>
        /// <param name="newPrimaryEmail">
        /// The newPrimaryEmail.
        /// </param>
        /// <param name="isUserRoleUpgrade">isUserRoleUpgrade.</param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<EmailChangeValidationTokenViewModel> ReGenerateEmailChangeValidationToken(int userId, string newPrimaryEmail, bool isUserRoleUpgrade);

        /// <summary>
        /// ReGenerateEmailChangeValidationToken.
        /// </summary>
        /// <param name="userId">
        /// The userId.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task CancelEmailChangeValidationToken(int userId);
    }
}
