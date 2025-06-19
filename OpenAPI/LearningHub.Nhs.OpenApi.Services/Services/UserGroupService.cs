namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Caching;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Models.Extensions;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The user group service.
    /// </summary>
    public class UserGroupService : IUserGroupService
    {

        private readonly IRoleUserGroupRepository roleUserGroupRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserGroupService"/> class.
        /// </summary>
        /// <param name="roleUserGroupRepository">roleUserGroupRepository.</param>
        public UserGroupService(IRoleUserGroupRepository roleUserGroupRepository)
        {
            this.roleUserGroupRepository = roleUserGroupRepository;
        }


        /// <inheritdoc />
        public async Task<bool> UserHasCatalogueContributionPermission(int userId)
        {
            var userRoleGroups = await this.roleUserGroupRepository.GetRoleUserGroupViewModelsByUserId(userId);
            if (userRoleGroups != null && userRoleGroups.Any(r => r.RoleEnum == RoleEnum.LocalAdmin || r.RoleEnum == RoleEnum.Editor))
            {
                return true;
            }

            return false;
        }

    }
}
