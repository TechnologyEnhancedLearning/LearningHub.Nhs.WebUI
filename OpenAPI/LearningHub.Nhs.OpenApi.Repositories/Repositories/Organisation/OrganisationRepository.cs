namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Organisation
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Organisation;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Provides data access operations for organisations and their memberships,
    /// implementing <see cref="IOrganisationRepository"/> using Entity Framework Core.
    /// </summary>
    public class OrganisationRepository : GenericRepository<Organisation>, IOrganisationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="tzOffsetManager">The timezone offset manager.</param>
        public OrganisationRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// Gets an organisation by its identifier.
        /// </summary>
        /// <param name="id">The organisation identifier.</param>
        /// <returns>
        /// The organisation if found and not removed; otherwise, null.
        /// </returns>
        public async Task<Organisation?> GetByIdAsync(int id)
        {
            return await this.DbContext.Organisation
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.RemoveDate == null);
        }

        /// <summary>
        /// Gets an organisation for update operations.
        /// </summary>
        /// <param name="id">The organisation identifier.</param>
        /// <returns>
        /// The organisation if found and not removed; otherwise, null.
        /// </returns>
        public async Task<Organisation?> GetForUpdateAsync(int id)
        {
            return await this.DbContext.Organisation
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.RemoveDate == null);
        }

        /// <summary>
        /// Determines whether an organisation exists.
        /// </summary>
        /// <param name="id">The organisation identifier.</param>
        /// <returns>
        /// True if the organisation exists and is not removed; otherwise, false.
        /// </returns>
        public async Task<bool> OrganisationExistsAsync(int id)
        {
            return await this.DbContext.Organisation
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == id &&
                         x.RemoveDate == null);
        }

        /// <summary>
        /// Determines whether the organisation has active child organisations.
        /// </summary>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <returns>
        /// True if active children exist; otherwise, false.
        /// </returns>
        public async Task<bool> HasActiveChildrenAsync(int organisationId)
        {
            return await this.DbContext.Organisation
                .AsNoTracking()
                .AnyAsync(
                    x => x.ParentId == organisationId &&
                         x.RemoveDate == null);
        }

        /// <summary>
        /// Determines whether the organisation has active user memberships.
        /// </summary>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <returns>
        /// True if active memberships exist; otherwise, false.
        /// </returns>
        public async Task<bool> HasActiveMembershipsAsync(int organisationId)
        {
            return await this.DbContext.UserOrganisation
                .AsNoTracking()
                .AnyAsync(
                    x => x.OrganisationId == organisationId &&
                         x.EndDate == null &&
                         x.RemoveDate == null);
        }

        /// <summary>
        /// Updates an organisation record.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the update.</param>
        /// <param name="organisation">The organisation to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateOrganisationAsync(int currentUserId, Organisation organisation)
        {
            organisation.AmendDate = DateTime.UtcNow;
            organisation.AmendUserId = currentUserId;

            await this.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the full organisation hierarchy (ancestors and descendants)
        /// for the specified organisation.
        /// </summary>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <returns>A read-only list of hierarchy rows.</returns>
        public async Task<IReadOnlyList<OrganisationHierarchyRow>> GetHierarchyAsync(int organisationId)
        {
            var query = this.DbContext.Database.SqlQuery<OrganisationHierarchyRow>(
                $"""
                WITH Ancestors AS
                (
                    SELECT
                        Id,
                        OrganisationName,
                        ODSCode,
                        PostCode,
                        OrganisationTypeId,
                        ParentId,
                        RegionId,
                        0 AS RelativeDepth
                    FROM Organisation
                    WHERE Id = {organisationId}
                      AND RemoveDate IS NULL

                    UNION ALL

                    SELECT
                        parent.Id,
                        parent.OrganisationName,
                        parent.ODSCode,
                        parent.PostCode,
                        parent.OrganisationTypeId,
                        parent.ParentId,
                        parent.RegionId,
                        ancestor.RelativeDepth - 1
                    FROM Organisation parent
                    INNER JOIN Ancestors ancestor
                        ON ancestor.ParentId = parent.Id
                    WHERE parent.RemoveDate IS NULL
                ),
                Descendants AS
                (
                    SELECT
                        Id,
                        OrganisationName,
                        ODSCode,
                        PostCode,
                        OrganisationTypeId,
                        ParentId,
                        RegionId,
                        0 AS RelativeDepth
                    FROM Organisation
                    WHERE Id = {organisationId}
                      AND RemoveDate IS NULL

                    UNION ALL

                    SELECT
                        child.Id,
                        child.OrganisationName,
                        child.ODSCode,
                        child.PostCode,
                        child.OrganisationTypeId,
                        child.ParentId,
                        child.RegionId,
                        descendant.RelativeDepth + 1
                    FROM Organisation child
                    INNER JOIN Descendants descendant
                        ON child.ParentId = descendant.Id
                    WHERE child.RemoveDate IS NULL
                )

                SELECT * FROM Ancestors

                UNION ALL

                SELECT * FROM Descendants
                WHERE RelativeDepth > 0

                OPTION (MAXRECURSION 100)
                """);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Gets the user’s organisation memberships.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="includeEnded">Whether to include ended memberships.</param>
        /// <returns>An <see cref="IQueryable{UserOrganisation}"/> of memberships.</returns>
        public IQueryable<UserOrganisation> GetUserOrganisations(int userId, bool includeEnded = false)
        {
            var query = this.DbContext.UserOrganisation
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.RemoveDate == null);

            if (!includeEnded)
            {
                query = query.Where(x => x.EndDate == null);
            }

            return query;
        }

        /// <summary>
        /// Gets a membership by its identifier.
        /// </summary>
        /// <param name="membershipId">The membership identifier.</param>
        /// <returns>The membership if found; otherwise, null.</returns>
        public async Task<UserOrganisation?> GetMembershipByIdAsync(int membershipId)
        {
            return await this.DbContext.UserOrganisation
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Include(x => x.JobRoleType)
                .FirstOrDefaultAsync(x =>
                    x.Id == membershipId &&
                    x.RemoveDate == null);
        }

        /// <summary>
        /// Gets a membership for update operations.
        /// </summary>
        /// <param name="membershipId">The membership identifier.</param>
        /// <returns>The membership if found; otherwise, null.</returns>
        public async Task<UserOrganisation?> GetMembershipForUpdateAsync(int membershipId)
        {
            return await this.DbContext.UserOrganisation
                .FirstOrDefaultAsync(x =>
                    x.Id == membershipId &&
                    x.RemoveDate == null);
        }

        /// <summary>
        /// Determines whether the user has an active membership in the organisation.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="organisationId">The organisation identifier.</param>
        /// <param name="excludeMembershipId">An optional membership identifier to exclude.</param>
        /// <returns>True if an active membership exists; otherwise, false.</returns>
        public async Task<bool> HasActiveMembershipAsync(int userId, int organisationId, int? excludeMembershipId = null)
        {
            var query = this.DbContext.UserOrganisation
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.OrganisationId == organisationId &&
                    x.EndDate == null &&
                    x.RemoveDate == null);

            if (excludeMembershipId.HasValue)
            {
                query = query.Where(x => x.Id != excludeMembershipId.Value);
            }

            return await query.AnyAsync();
        }

        /// <summary>
        /// Determines whether the user exists.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        public async Task<bool> UserExistsAsync(int userId)
        {
            return await this.DbContext.User
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == userId &&
                    !x.Deleted);
        }

        /// <summary>
        /// Determines whether the job role type exists.
        /// </summary>
        /// <param name="jobRoleTypeId">The job role type identifier.</param>
        /// <returns>True if the job role type exists; otherwise, false.</returns>
        public async Task<bool> JobRoleTypeExistsAsync(int jobRoleTypeId)
        {
            return await this.DbContext.JobRoleType
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == jobRoleTypeId &&
                    x.RemoveDate == null);
        }

        /// <summary>
        /// Creates a new membership for the user.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the operation.</param>
        /// <param name="membership">The membership to create.</param>
        /// <returns>The identifier of the newly created membership.</returns>
        public async Task<int> CreateMembershipAsync(int currentUserId, UserOrganisation membership)
        {
            membership.CreateDate = DateTime.UtcNow;
            membership.CreateUserId = currentUserId;

            this.DbContext.UserOrganisation.Add(membership);
            await this.DbContext.SaveChangesAsync();

            return membership.Id;
        }

        /// <summary>
        /// Updates an existing membership.
        /// </summary>
        /// <param name="currentUserId">The identifier of the user performing the update.</param>
        /// <param name="membership">The membership to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateMembershipAsync(int currentUserId, UserOrganisation membership)
        {
            membership.AmendDate = DateTime.UtcNow;
            membership.AmendUserId = currentUserId;

            await this.DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all job role types.
        /// </summary>
        /// <returns>A read-only list of job role types.</returns>
        public async Task<IReadOnlyList<JobRoleType>> GetJobRoleTypesAsync()
        {
            return await this.DbContext.JobRoleType
                .AsNoTracking()
                .Where(x => x.RemoveDate == null)
                .OrderBy(x => x.JobRoleTypeName)
                .ToListAsync();
        }
    }
}