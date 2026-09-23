using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Organisation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
{
    /// <summary>
    /// Defines data access operations for organisations and their memberships.
    /// </summary>
    public interface IOrganisationRepository : IGenericRepository<Organisation>
    {
        /// <summary>
        /// Gets an organisation by its identifier.
        /// </summary>
        /// <param name="id">The organisation identifier.</param>
        /// <returns>The organisation if found; otherwise, null.</returns>
        Task<Organisation?> GetByIdAsync(int id);

        /// <summary>
        /// Gets an organisation for update operations.
        /// </summary>
        /// <param name="id">The organisation identifier.</param>
        /// <returns>The organisation if found; otherwise, null.</returns>
        Task<Organisation?> GetForUpdateAsync(int id);

        /// <summary>
        /// Determines whether an organisation exists.
        /// </summary>
        /// <param name="id">The organisation identifier.</param>
        /// <returns>True if the organisation exists; otherwise, false.</returns>
        Task<bool> OrganisationExistsAsync(int id);

        /// <summary>
        /// Determines whether the organisation has active child organisations.
        /// </summary>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <returns>True if active children exist; otherwise, false.</returns>
        Task<bool> HasActiveChildrenAsync(int organisationId);

        /// <summary>
        /// Determines whether the organisation has active user memberships.
        /// </summary>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <returns>True if active memberships exist; otherwise, false.</returns>
        Task<bool> HasActiveMembershipsAsync(int organisationId);

        /// <summary>
        /// Updates an organisation record.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the update.</param>
        /// <param name="organisation">The organisation to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateOrganisationAsync(int currentUserId, Organisation organisation);

        /// <summary>
        /// Gets the hierarchy for the specified organisation.
        /// </summary>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <returns>A read-only list of hierarchy rows.</returns>
        Task<IReadOnlyList<OrganisationHierarchyRow>> GetHierarchyAsync(int organisationId);

        /// <summary>
        /// Gets the user’s organisation memberships.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="includeEnded">Whether to include ended memberships.</param>
        /// <returns>An <see cref="IQueryable{T}"/> of user organisation memberships.</returns>
        IQueryable<UserOrganisation> GetUserOrganisations(int userId, bool includeEnded = false);

        /// <summary>
        /// Gets a membership by its identifier.
        /// </summary>
        /// <param name="membershipId">The membership identifier.</param>
        /// <returns>The membership if found; otherwise, null.</returns>
        Task<UserOrganisation?> GetMembershipByIdAsync(int membershipId);

        /// <summary>
        /// Gets a membership for update operations.
        /// </summary>
        /// <param name="membershipId">The membership identifier.</param>
        /// <returns>The membership if found; otherwise, null.</returns>
        Task<UserOrganisation?> GetMembershipForUpdateAsync(int membershipId);

        /// <summary>
        /// Determines whether the user has an active membership in the organisation.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <param name="excludeMembershipId">An optional membership identifier to exclude.</param>
        /// <returns>True if an active membership exists; otherwise, false.</returns>
        Task<bool> HasActiveMembershipAsync(int userId, int organisationId, int? excludeMembershipId = null);

        /// <summary>
        /// Determines whether the user exists.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        Task<bool> UserExistsAsync(int userId);

        /// <summary>
        /// Determines whether the job role type exists.
        /// </summary>
        /// <param name="jobRoleTypeId">The job role type identifier.</param>
        /// <returns>True if the job role type exists; otherwise, false.</returns>
        Task<bool> JobRoleTypeExistsAsync(int jobRoleTypeId);

        /// <summary>
        /// Creates a new membership for the user.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the operation.</param>
        /// <param name="membership">The membership to create.</param>
        /// <returns>The identifier of the newly created membership.</returns>
        Task<int> CreateMembershipAsync(int currentUserId, UserOrganisation membership);

        /// <summary>
        /// Updates an existing membership.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the update.</param>
        /// <param name="membership">The membership to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateMembershipAsync(int currentUserId, UserOrganisation membership);

        /// <summary>
        /// Gets all job role types.
        /// </summary>
        /// <returns>A read-only list of job role types.</returns>
        Task<IReadOnlyList<JobRoleType>> GetJobRoleTypesAsync();
    }

}
