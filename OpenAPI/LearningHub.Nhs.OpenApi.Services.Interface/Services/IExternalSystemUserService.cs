namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.External;

    /// <summary>
    /// The External system user service interface.
    /// </summary>
    public interface IExternalSystemUserService
    {
        /// <summary>
        /// Get external system user entity by id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="externalSystemId">The external system id.</param>
        /// <returns>The <see cref="ExternalSystemUser"/>.</returns>
        Task<ExternalSystemUser> GetByIdAsync(int userId, int externalSystemId);

        /// <summary>
        /// Get ELFH external system user entity by userId and code.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="clientCode">The client code.</param>
        /// <returns>The <see cref="UserExternalSystem"/>.</returns>
        Task<UserExternalSystem> GetElfhExternalUserByUserIdAndClientCode(int userId, string clientCode);

        /// <summary>
        /// Get ELFH external system user entity by userId and code.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="clientCode">The client code.</param>
        /// <returns>The <see cref="UserExternalSystem"/>.</returns>
        Task<UserExternalSystem> GetLhExternalUserByUserIdAndClientCode(int userId, string clientCode);
    }
}
