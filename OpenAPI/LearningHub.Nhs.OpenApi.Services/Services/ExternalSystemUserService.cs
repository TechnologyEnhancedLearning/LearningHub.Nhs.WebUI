
namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.External;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.UserApi.Services.Interface;

    /// <summary>
    /// The external system user service.
    /// </summary>
    public class ExternalSystemUserService : IExternalSystemUserService
    {
        private readonly IExternalSystemUserRepository extSystemUserRepo;
        private readonly IUserExternalSystemRepository userExternalSystemRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalSystemUserService"/> class.
        /// </summary>
        /// <param name="extSystemUserRepo">The external system user repository.</param>
        /// <param name="userExternalSystemRepo">The ELFH external system user repository.</param>
        public ExternalSystemUserService(IExternalSystemUserRepository extSystemUserRepo,
            IUserExternalSystemRepository userExternalSystemRepo)
        {
            this.extSystemUserRepo = extSystemUserRepo;
            this.userExternalSystemRepo = userExternalSystemRepo;
        }

        /// <inheritdoc/>
        public async Task<ExternalSystemUser> GetByIdAsync(int userId, int externalSystemId)
        {
            return await this.extSystemUserRepo.GetByIdAsync(userId, externalSystemId);
        }

        /// <inheritdoc/>
        public async Task<UserExternalSystem> GetElfhExternalUserByUserIdAndClientCode(int userId, string clientCode)
        {
            return await this.userExternalSystemRepo.GetUserExternalSystemByUserIdandCodeAsync(userId, clientCode);
        }

        /// <inheritdoc/>
        public async Task<UserExternalSystem> GetLhExternalUserByUserIdAndClientCode(int userId, string clientCode)
        {
            return await this.userExternalSystemRepo.GetUserExternalSystemByUserIdandCodeAsync(userId, clientCode);
        }
    }
}
