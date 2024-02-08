namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Common;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.User;
    using LearningHub.Nhs.Models.Validation;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// The role service.
    /// </summary>
    public class RoleService : IRoleService
    {
        /// <summary>
        /// The role repository.
        /// </summary>
        private readonly IRoleRepository roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        /// <inheritdoc />
        public async Task<Role> GetByIdAsync(int id, bool includePermissions)
        {
            return await this.roleRepository.GetByIdAsync(id, includePermissions);
        }

        /// <inheritdoc />
        public async Task<List<Role>> GetAvailableForUserGroupAsync(int userGroupId)
        {
            return await this.roleRepository.GetAvailableForUserGroupAsync(userGroupId);
        }

        /// <inheritdoc />
        public async Task<LearningHubValidationResult> CreateAsync(int userId, Role role)
        {
            var roleValidator = new RoleValidator();
            var vr = await roleValidator.ValidateAsync(role);

            if (vr.IsValid)
            {
                await this.roleRepository.CreateAsync(userId, role);
            }

            return new LearningHubValidationResult(vr);
        }

        /// <inheritdoc />
        public async Task<LearningHubValidationResult> UpdateAsync(int userId, Role role, bool updatePermissions)
        {
            var roleValidator = new RoleValidator();
            var vr = await roleValidator.ValidateAsync(role);

            if (vr.IsValid)
            {
                await this.roleRepository.UpdateAsync(userId, role, updatePermissions);
            }

            return new LearningHubValidationResult(vr);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int userId, int roleId)
        {
            await this.roleRepository.DeleteAsync(userId, roleId);
        }

        /// <inheritdoc />
        public SearchResult Search(string searchText, int page, int pageSize)
        {
            int pagesReturned;
            var results = this.roleRepository.Search(searchText, page, pageSize, out pagesReturned);
            return new SearchResult()
            {
                Page = page,
                PageSize = pageSize,
                PagesReturned = pagesReturned,
                Results = results.ConvertAll(x => (EntityBase)x),
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RolePermissionsViewModel>> GetAllRoles()
        {
            var roles = await this.roleRepository.GetAllRoles();

            return roles.Select(r => new RolePermissionsViewModel
            {
                RoleId = r.Id,
                Permissions = r.PermissionRole.Select(p => p.Permission.Code),
            });
        }
    }
}