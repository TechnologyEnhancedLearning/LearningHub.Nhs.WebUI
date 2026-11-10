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
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// The role service
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;

        /// <summary>
        /// The role service contructor
        /// </summary>
        /// <param name="roleRepository"></param>
        /// <param name="userRoleRepository"></param>
        public RoleService(IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
        {
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
        }




   

        /// <inheritdoc/>
        public async Task<IReadOnlyList<RoleViewModel>> GetAllAsync(CancellationToken ct = default) =>
            (await roleRepository.GetAllAsync(ct)).Select(Map).ToArray();

        /// <inheritdoc/>
        public async Task<RoleViewModel> GetByIdAsync(int roleId, CancellationToken ct = default) =>
            Map(await roleRepository.GetByIdAsync(roleId, ct)
                ?? throw new HttpResponseException("Role was not found.", HttpStatusCode.NotFound));

        /// <inheritdoc/>
        public async Task<RoleViewModel> CreateAsync(int actorId, RoleCreateViewModel request, CancellationToken ct = default)
        {
            if (request is null) throw new HttpResponseException("Request body is required.", HttpStatusCode.BadRequest);
            var code = NormaliseCode(request.Code);
            var type = RoleScopeRules.Parse(request.ScopeType);
            var entity = new Role
            {
                Code = code,
                Name = ValidateName(request.Name),
                ScopeType = type.ToString(),
                Description = ValidateDescription(request.Description),
                CreateDate = DateTimeOffset.UtcNow,
                CreateUserId = actorId
            };
            return Map(await roleRepository.CreateUniqueAsync(entity, ct));
        }

        /// <inheritdoc/>
        public async Task<RoleViewModel> UpdateAsync(int actorId, int roleId, RoleUpdateViewModel request, CancellationToken ct = default)
        {
            if (request is null) throw new HttpResponseException("Request body is required.", HttpStatusCode.BadRequest);
            var name = ValidateName(request.Name);
            var type = RoleScopeRules.Parse(request.ScopeType);
            var description = ValidateDescription(request.Description);
            return await roleRepository.InSerializableTransactionAsync(async () =>
            {
                var entity = await roleRepository.GetTrackedByIdAsync(roleId, ct)
                    ?? throw new HttpResponseException("Role was not found.", HttpStatusCode.NotFound);

                // Changing the role type must not invalidate any existing grants.
                foreach (var assignment in await userRoleRepository.GetActiveByRoleIdAsync(roleId, ct))
                {
                    try { RoleScopeRules.Validate(type.ToString(), RoleScope.From(assignment)); }
                    catch (HttpResponseException ex)
                    {
                        throw new HttpResponseException($"ScopeType change would invalidate user-role assignment {assignment.Id}: {ex.Message}", HttpStatusCode.Conflict);
                    }
                }

                entity.Name = name;
                entity.ScopeType = type.ToString();
                entity.Description = description;
                entity.AmendDate = DateTimeOffset.UtcNow;
                entity.AmendUserId = actorId;
                await roleRepository.SaveChangesAsync(ct);
                return Map(entity);
            }, ct);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int actorId, int roleId, CancellationToken ct = default)
        {
            await roleRepository.InSerializableTransactionAsync(async () =>
            {
                var entity = await roleRepository.GetTrackedByIdAsync(roleId, ct)
                    ?? throw new HttpResponseException("Role was not found.", HttpStatusCode.NotFound);
                if (await userRoleRepository.HasActiveAssignmentsAsync(roleId, ct))
                    throw new HttpResponseException("Remove active user-role assignments before deleting this role.", HttpStatusCode.Conflict);

                entity.RemoveDate = DateTimeOffset.UtcNow;
                entity.RemoveUserId = actorId;
                await roleRepository.SaveChangesAsync(ct);
                return true;
            }, ct);
        }

        private static RoleViewModel Map(Role role) => new RoleViewModel
        {
            Id = role.Id,
            Code = role.Code,
            Name = role.Name,
            ScopeType = role.ScopeType,
            Description = role.Description,
            CreateDate = role.CreateDate,
            AmendDate = role.AmendDate
        };

        private static string NormaliseCode(string? value)
        {
            var code = value?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(code) || code.Length > 50)
                throw new HttpResponseException("Code is required and must be at most 50 characters.", HttpStatusCode.BadRequest);
            if (!code.All(c => char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c) || c == '_' || c == '-'))
                throw new HttpResponseException("Code may contain only a-z, 0-9, underscore and hyphen.", HttpStatusCode.BadRequest);
            return code;
        }

        private static string ValidateName(string? value)
        {
            var name = value?.Trim();
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new HttpResponseException("Name is required and must be at most 100 characters.", HttpStatusCode.BadRequest);
            return name;
        }

        private static string? ValidateDescription(string? value)
        {
            var description = value?.Trim();
            if (description?.Length > 500)
                throw new HttpResponseException("Description must be at most 500 characters.", HttpStatusCode.BadRequest);
            return string.IsNullOrWhiteSpace(description) ? null : description;
        }
    }
}
