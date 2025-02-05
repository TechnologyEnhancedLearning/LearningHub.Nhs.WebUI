namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;

    /// <summary>
    /// The EmailChangeValidationTokenRepository interface.
    /// </summary>
    public interface IEmailChangeValidationTokenRepository
    {
        /// <summary>
        /// The get by token.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<EmailChangeValidationToken> GetByToken(string lookup);

        /// <summary>
        /// The GetLastIssuedEmailChangeValidationToken.
        /// </summary>
        /// <param name="userId">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<EmailChangeValidationToken> GetLastIssuedEmailChangeValidationToken(int userId);

        /// <summary>
        /// The expire email change validation token.
        /// </summary>
        /// <param name="lookup">
        /// The lookup.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task ExpireEmailChangeValidationToken(string lookup);

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="emailChangeValidationToken">
        /// The email change validation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<int> CreateAsync(int userId, EmailChangeValidationToken emailChangeValidationToken);

        /// <summary>
        /// The create async.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="emailChangeValidationToken">
        /// The email change validation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task UpdateAsync(int userId, EmailChangeValidationToken emailChangeValidationToken);
    }
}