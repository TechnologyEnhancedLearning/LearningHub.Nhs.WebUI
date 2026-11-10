namespace LearningHub.Nhs.OpenApi.Services.Services
{

    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Role;
    using LearningHub.Nhs.OpenApi.Models.Exceptions;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Role;
    using LearningHub.Nhs.OpenApi.Repositories.Helpers;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The user role service.
    /// </summary>
    public class UserRoleService : IUserRoleService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;

        /// <summary>
        /// The user role service constructor
        /// </summary>
        /// <param name="roleRepository"></param>
        /// <param name="userRoleRepository"></param>
        public UserRoleService(IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
        {
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
        }


   

        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserRoleViewModel>> GetForUserAsync(int userId, CancellationToken ct = default)
        {
            await EnsureUserExistsAsync(userId, ct);
            return await userRoleRepository.GetByUserIdAsync(userId, ct);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<UserRoleViewModel>> GetForRoleAsync(int roleId, CancellationToken ct = default)
        {
            await EnsureRoleExistsAsync(roleId, ct);
            return await userRoleRepository.GetByRoleIdAsync(roleId, ct);
        }

        /// <inheritdoc/>
        public async Task<UserRoleViewModel> GetAsync(int userId, int assignmentId, CancellationToken ct = default)
        {
            ValidateId(userId, nameof(userId));
            ValidateId(assignmentId, nameof(assignmentId));
            return await userRoleRepository.GetByIdAsync(userId, assignmentId, ct)
                ?? throw new HttpResponseException("User role assignment was not found.", HttpStatusCode.NotFound);
        }

        /// <inheritdoc/>
        public async Task<UserRoleViewModel> AssignAsync(int actorId, int userId, UserRoleCreateViewModel request, CancellationToken ct = default)
        {
            if (request is null) throw new HttpResponseException("Request body is required.", HttpStatusCode.BadRequest);
            await EnsureUserExistsAsync(userId, ct);
            var role = await EnsureRoleExistsAsync(request.RoleId, ct);
            var scope = RoleScope.From(request);
            await ValidateTargetAsync(role.ScopeType, scope, ct);

            var entity = new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                ScopeOrganisationId = scope.OrganisationId,
                ScopeCatalogueId = scope.CatalogueId,
                ScopeCategoryId = scope.CategoryId,
                ScopeSelfAssessmentId = scope.SelfAssessmentId,
                CreateDate = DateTimeOffset.UtcNow,
                CreateUserId = actorId
            };

            await userRoleRepository.CreateUniqueAsync(entity, ct);
            return await GetAsync(userId, entity.Id, ct);
        }

        /// <inheritdoc/>
        public async Task<UserRoleViewModel> UpdateScopeAsync(
            int actorId, int userId, int assignmentId,
            UserRoleScopeUpdateViewModel request, CancellationToken ct = default)
        {

            if (request is null) throw new HttpResponseException("Request body is required.", HttpStatusCode.BadRequest);
            var existing = await GetAsync(userId, assignmentId, ct);
            var role = await EnsureRoleExistsAsync(existing.RoleId, ct);
            var scope = RoleScope.From(request);
            await ValidateTargetAsync(role.ScopeType, scope, ct);

            await userRoleRepository.UpdateScopeUniqueAsync(userId, assignmentId, scope, actorId, ct);
            return await GetAsync(userId, assignmentId, ct);
        }


        /// <inheritdoc/>
        public async Task UnassignAsync(int actorId, int userId, int assignmentId, CancellationToken ct = default)
        {
            ValidateId(userId, nameof(userId));
            ValidateId(assignmentId, nameof(assignmentId));
            if (!await userRoleRepository.SoftDeleteAsync(userId, assignmentId, actorId, ct))
                throw new HttpResponseException("User role assignment was not found.", HttpStatusCode.NotFound);
        }

        private static void ValidateId(int id, string name)
        {
            if (id <= 0) throw new HttpResponseException($"{name} must be a positive integer.", HttpStatusCode.BadRequest);
        }

        private async Task EnsureUserExistsAsync(int userId, CancellationToken ct)
        {
            ValidateId(userId, nameof(userId));
            if (!await roleRepository.UserExistsAsync(userId, ct))
                throw new HttpResponseException("User was not found.", HttpStatusCode.NotFound);
        }

        private async Task<Role> EnsureRoleExistsAsync(int roleId, CancellationToken ct)
        {
            ValidateId(roleId, nameof(roleId));
            return await roleRepository.GetByIdAsync(roleId, ct)
                ?? throw new HttpResponseException("Role was not found.", HttpStatusCode.NotFound);
        }

        private async Task ValidateTargetAsync(string? configuredScopeType, RoleScope scope, CancellationToken ct)
        {
            RoleScopeRules.Validate(configuredScopeType, scope);
            if (!await roleRepository.ScopeTargetExistsAsync(scope, ct))
                throw new HttpResponseException("The specified scope target does not exist.", HttpStatusCode.BadRequest);

        }
    }
}
