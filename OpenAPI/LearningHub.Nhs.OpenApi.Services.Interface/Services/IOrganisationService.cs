using LearningHub.Nhs.Models.Common;
using LearningHub.Nhs.Models.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// Provides organisation and organisation membership operations.
    /// </summary>
    public interface IOrganisationService
    {
        /// <summary>
        /// Gets an organisation by id.
        /// </summary>
        /// <param name="id">The organisation id.</param>
        /// <returns>The organisation.</returns>
        Task<OrganisationViewModel?> GetByIdAsync(int id);

        /// <summary>
        /// Searches organisations.
        /// </summary>
        /// <param name="request">Search and paging parameters.</param>
        /// <returns>Paged organisations.</returns>
        Task<PagedResultSet<OrganisationViewModel>> SearchAsync(OrganisationSearchRequest request);

        /// <summary>
        /// Validates an ODS organisation code.
        /// </summary>
        /// <param name="odsCode">The ODS code.</param>
        /// <returns>The ODS validation result.</returns>
        Task<OdsOrganisationValidationViewModel> ValidateOdsCodeAsync(string odsCode);

        /// <summary>
        /// Soft deletes an organisation.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>True when the organisation was removed.</returns>
        Task<bool> SoftDeleteAsync(int organisationId, int currentUserId);

        /// <summary>
        /// Gets an organisation hierarchy.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <returns>The hierarchy.</returns>
        Task<OrganisationHierarchyViewModel?> GetHierarchyAsync(int organisationId);

        /// <summary>
        /// Gets organisation memberships for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="includeEnded">Whether ended memberships should be included.</param>
        /// <returns>User organisation memberships.</returns>
        Task<IReadOnlyList<UserOrganisationViewModel>?> GetUserOrganisationsAsync(int userId, bool includeEnded = false);

        /// <summary>
        /// Adds a user organisation membership.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <param name="request">The membership request.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>The created membership.</returns>
        Task<UserOrganisationViewModel> AddMembershipAsync(int organisationId, AddOrganisationMembershipRequest request, int currentUserId);

        /// <summary>
        /// Updates an organisation membership.
        /// </summary>
        /// <param name="membershipId">The membership id.</param>
        /// <param name="request">The membership update.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>The updated membership.</returns>
        Task<UserOrganisationViewModel?> UpdateMembershipAsync(int membershipId, UpdateOrganisationMembershipRequest request, int currentUserId);

        /// <summary>
        /// Ends an organisation membership.
        /// </summary>
        /// <param name="membershipId">The membership id.</param>
        /// <param name="endDate">Optional membership end date.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>True when the membership was ended.</returns>
        Task<bool> EndMembershipAsync(int membershipId, DateTime? endDate, int currentUserId);

        /// <summary>
        /// Gets available job role types.
        /// </summary>
        /// <returns>Job role types.</returns>
        Task<IReadOnlyList<JobRoleTypeViewModel>> GetJobRoleTypesAsync();
    }
}

