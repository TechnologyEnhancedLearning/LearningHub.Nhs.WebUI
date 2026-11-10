using LearningHub.Nhs.Models.Entities.External;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    /// <summary>
    /// The External System User Repository interface.
    /// </summary>
    public interface IExternalSystemUserRepository : IGenericRepository<ExternalSystemUser>
    {
        /// <summary>
        /// Get external system user entity by id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="externalSystemId">The external system id.</param>
        /// <returns>The <see cref="ExternalSystemUser"/>.</returns>
        Task<ExternalSystemUser> GetByIdAsync(int userId, int externalSystemId);

        /// <summary>
        /// Create External system user.
        /// </summary>
        /// <param name="userExternalSystem">The userExternalSystem.</param>
        /// <returns>The <see cref="ExternalSystemUser"/>.</returns>
        Task CreateExternalSystemUserAsync(ExternalSystemUser userExternalSystem);
    }
}
