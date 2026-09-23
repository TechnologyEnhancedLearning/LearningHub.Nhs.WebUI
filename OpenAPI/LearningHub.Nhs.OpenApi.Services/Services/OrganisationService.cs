using AutoMapper;
using LearningHub.Nhs.Models.Common;
using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Organisation;
using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
using LearningHub.Nhs.OpenApi.Services.Interface.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace LearningHub.Nhs.OpenApi.Services.Services
{
    /// <summary>
    /// Provides organisation and organisation membership operations.
    /// </summary>
    public class OrganisationService : IOrganisationService
    {
        private readonly IOrganisationRepository organisationRepository;
        private readonly IOdsOrganisationClient odsOrganisationClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationService"/> class.
        /// </summary>
        /// <param name="organisationRepository">The organisation repository.</param>
        /// <param name="odsOrganisationClient">The ODS organisation client.</param>
        /// <param name="mapper">The mapper.</param>
        public OrganisationService(
            IOrganisationRepository organisationRepository,
            IOdsOrganisationClient odsOrganisationClient,
            IMapper mapper)
        {
            this.organisationRepository = organisationRepository;
            this.odsOrganisationClient = odsOrganisationClient;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets an organisation by id.
        /// </summary>
        /// <param name="id">The organisation id.</param>
        /// <returns>The organisation view model, or null if not found.</returns>
        public async Task<OrganisationViewModel?> GetByIdAsync(int id)
        {
            var organisation = await this.organisationRepository.GetByIdAsync(id);
            return organisation == null ? null : this.mapper.Map<OrganisationViewModel>(organisation);
        }

        /// <summary>
        /// Searches organisations.
        /// </summary>
        /// <param name="request">Search and paging parameters.</param>
        /// <returns>Paged organisations.</returns>
        public async Task<PagedResultSet<OrganisationViewModel>> SearchAsync(OrganisationSearchRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, 100);

            var query = this.organisationRepository
                .GetAll()
                .AsNoTracking()
                .Where(x => x.RemoveDate == null);

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                var search = request.Query.Trim();
                query = query.Where(x =>
                    x.OrganisationName.Contains(search) ||
                    x.ODSCode.Contains(search) ||
                    (x.PostCode != null && x.PostCode.Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(request.OdsCode))
            {
                var odsCode = request.OdsCode.Trim().ToUpperInvariant();
                query = query.Where(x => x.ODSCode == odsCode);
            }

            if (!string.IsNullOrWhiteSpace(request.PostCode))
            {
                var postCode = request.PostCode.Trim();
                query = query.Where(x => x.PostCode != null && x.PostCode.Contains(postCode));
            }

            if (request.OrganisationTypeId.HasValue)
            {
                query = query.Where(x => x.OrganisationTypeId == request.OrganisationTypeId.Value);
            }

            if (request.ParentId.HasValue)
            {
                query = query.Where(x => x.ParentId == request.ParentId.Value);
            }

            if (request.RegionId.HasValue)
            {
                query = query.Where(x => x.RegionId == request.RegionId.Value);
            }

            var totalItemCount = await query.CountAsync();

            query = ApplyOrganisationOrdering(query, request.Sort, request.Direction);

            var items = await this.mapper
                .ProjectTo<OrganisationViewModel>(query.Skip((page - 1) * pageSize).Take(pageSize))
                .ToListAsync();

            return new PagedResultSet<OrganisationViewModel>
            {
                Items = items,
                TotalItemCount = totalItemCount,
            };
        }

        /// <summary>
        /// Validates an ODS organisation code.
        /// </summary>
        /// <param name="odsCode">The ODS code.</param>
        /// <returns>The validation result.</returns>
        public async Task<OdsOrganisationValidationViewModel> ValidateOdsCodeAsync(string odsCode)
        {
            if (string.IsNullOrWhiteSpace(odsCode))
            {
                return new OdsOrganisationValidationViewModel
                {
                    OdsCode = odsCode ?? string.Empty,
                    Valid = false,
                };
            }

            var normalisedCode = odsCode.Trim().ToUpperInvariant();
            var odsOrganisation = await this.odsOrganisationClient.GetByOdsCodeAsync(normalisedCode);

            if (odsOrganisation == null)
            {
                return new OdsOrganisationValidationViewModel
                {
                    OdsCode = normalisedCode,
                    Valid = false,
                };
            }

            return new OdsOrganisationValidationViewModel
            {
                OdsCode = odsOrganisation.OdsCode,
                OrganisationName = odsOrganisation.OrganisationName,
                PostCode = odsOrganisation.PostCode,
                Active = odsOrganisation.Active,
                Valid = true,
            };
        }

        /// <summary>
        /// Soft deletes an organisation.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>True when the organisation was removed.</returns>
        public async Task<bool> SoftDeleteAsync(int organisationId, int currentUserId)
        {
            var organisation = await this.organisationRepository.GetForUpdateAsync(organisationId);
            if (organisation == null)
            {
                return false;
            }

            if (await this.organisationRepository.HasActiveMembershipsAsync(organisationId))
            {
                throw new ValidationException("The organisation cannot be removed because it has active user memberships.");
            }

            if (await this.organisationRepository.HasActiveChildrenAsync(organisationId))
            {
                throw new ValidationException("The organisation cannot be removed because it has active child organisations.");
            }

            organisation.RemoveDate = DateTime.UtcNow;
            organisation.RemoveUserId = currentUserId;

            await this.organisationRepository.UpdateAsync(currentUserId, organisation);
            return true;
        }

        /// <summary>
        /// Gets an organisation hierarchy.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <returns>The hierarchy view model, or null if not found.</returns>
        public async Task<OrganisationHierarchyViewModel?> GetHierarchyAsync(int organisationId)
        {
            var rows = await this.organisationRepository.GetHierarchyAsync(organisationId);

            var selected = rows.FirstOrDefault(x => x.Id == organisationId && x.RelativeDepth == 0);
            if (selected == null)
            {
                return null;
            }

            var ancestors = rows
                .Where(x => x.RelativeDepth < 0)
                .OrderBy(x => x.RelativeDepth)
                .Select(MapHierarchyRow)
                .ToList();

            var descendants = rows.Where(x => x.RelativeDepth > 0).ToList();
            var childrenByParent = descendants.ToLookup(x => x.ParentId);

            var children = childrenByParent[selected.Id]
                .Select(child => this.BuildHierarchyNode(child, childrenByParent, new HashSet<int>()))
                .ToList();

            return new OrganisationHierarchyViewModel
            {
                Organisation = MapHierarchyRow(selected),
                Ancestors = ancestors,
                Children = children,
            };
        }

        /// <summary>
        /// Gets organisation memberships for a user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="includeEnded">Whether ended memberships should be included.</param>
        /// <returns>User organisation memberships, or null if user does not exist.</returns>
        public async Task<IReadOnlyList<UserOrganisationViewModel>?> GetUserOrganisationsAsync(int userId, bool includeEnded = false)
        {
            var userExists = await this.organisationRepository.UserExistsAsync(userId);
            if (!userExists)
            {
                return null;
            }

            var query = this.organisationRepository.GetUserOrganisations(userId, includeEnded);

            return await this.mapper
                .ProjectTo<UserOrganisationViewModel>(query)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a user organisation membership.
        /// </summary>
        /// <param name="organisationId">The organisation id.</param>
        /// <param name="request">The membership request.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>The created membership.</returns>
        public async Task<UserOrganisationViewModel> AddMembershipAsync(
            int organisationId,
            AddOrganisationMembershipRequest request,
            int currentUserId)
        {
            ArgumentNullException.ThrowIfNull(request);

            var startDate = request.StartDate ?? DateTime.UtcNow;

            await this.ValidateMembershipAsync(
                request.UserId,
                organisationId,
                request.JobRoleTypeId,
                request.JobRole,
                startDate);

            var duplicateExists = await this.organisationRepository.HasActiveMembershipAsync(
                request.UserId,
                organisationId);

            if (duplicateExists)
            {
                throw new ValidationException("The user already has an active membership for this organisation.");
            }

            var membership = new UserOrganisation
            {
                UserId = request.UserId,
                OrganisationId = organisationId,
                JobRoleTypeId = request.JobRoleTypeId,
                JobRole = NormaliseJobRole(request.JobRole),
                StartDate = startDate,
            };

            var membershipId = await this.organisationRepository.CreateMembershipAsync(currentUserId, membership);
            var created = await this.organisationRepository.GetMembershipByIdAsync(membershipId);

            if (created == null)
            {
                throw new InvalidOperationException("The membership was created but could not be retrieved.");
            }

            return this.mapper.Map<UserOrganisationViewModel>(created);
        }

        /// <summary>
        /// Updates an organisation membership.
        /// </summary>
        /// <param name="membershipId">The membership id.</param>
        /// <param name="request">The membership update request.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>The updated membership, or null if not found.</returns>
        public async Task<UserOrganisationViewModel?> UpdateMembershipAsync(
            int membershipId,
            UpdateOrganisationMembershipRequest request,
            int currentUserId)
        {
            ArgumentNullException.ThrowIfNull(request);

            var membership = await this.organisationRepository.GetMembershipForUpdateAsync(membershipId);
            if (membership == null)
            {
                return null;
            }

            if (membership.EndDate.HasValue)
            {
                throw new ValidationException("An ended organisation membership cannot be updated.");
            }

            await this.ValidateMembershipAsync(
                membership.UserId,
                membership.OrganisationId,
                request.JobRoleTypeId,
                request.JobRole,
                request.StartDate);

            membership.JobRoleTypeId = request.JobRoleTypeId;
            membership.JobRole = NormaliseJobRole(request.JobRole);
            membership.StartDate = request.StartDate;

            await this.organisationRepository.UpdateMembershipAsync(currentUserId, membership);

            var updated = await this.organisationRepository.GetMembershipByIdAsync(membershipId);
            if (updated == null)
            {
                throw new InvalidOperationException("The membership was updated but could not be retrieved.");
            }

            return this.mapper.Map<UserOrganisationViewModel>(updated);
        }

        /// <summary>
        /// Ends an organisation membership.
        /// </summary>
        /// <param name="membershipId">The membership id.</param>
        /// <param name="endDate">Optional membership end date.</param>
        /// <param name="currentUserId">The user performing the operation.</param>
        /// <returns>True when the membership was ended.</returns>
        public async Task<bool> EndMembershipAsync(int membershipId, DateTime? endDate, int currentUserId)
        {
            var membership = await this.organisationRepository.GetMembershipForUpdateAsync(membershipId);
            if (membership == null)
            {
                return false;
            }

            if (membership.EndDate.HasValue)
            {
                return true;
            }

            var effectiveEndDate = endDate ?? DateTime.UtcNow;

            if (effectiveEndDate < membership.StartDate)
            {
                throw new ValidationException("Membership end date cannot be before the membership start date.");
            }

            if (effectiveEndDate > DateTime.UtcNow)
            {
                throw new ValidationException("Membership end date cannot be in the future.");
            }

            membership.EndDate = effectiveEndDate;

            await this.organisationRepository.UpdateMembershipAsync(currentUserId, membership);
            return true;
        }

        /// <summary>
        /// Gets available job role types.
        /// </summary>
        /// <returns>Job role types.</returns>
        public async Task<IReadOnlyList<JobRoleTypeViewModel>> GetJobRoleTypesAsync()
        {
            var jobRoleTypes = await this.organisationRepository.GetJobRoleTypesAsync();

            return jobRoleTypes
                .Select(x => new JobRoleTypeViewModel
                {
                    Id = x.Id,
                    Name = x.JobRoleTypeName,
                })
                .ToList();
        }

        private async Task ValidateMembershipAsync(
            int userId,
            int organisationId,
            int jobRoleTypeId,
            string? jobRole,
            DateTime startDate)
        {
            var userExists = await this.organisationRepository.UserExistsAsync(userId);
            if (!userExists)
            {
                throw new ValidationException("The user does not exist.");
            }

            var organisationExists = await this.organisationRepository.OrganisationExistsAsync(organisationId);
            if (!organisationExists)
            {
                throw new ValidationException("The organisation does not exist.");
            }

            var jobRoleTypeExists = await this.organisationRepository.JobRoleTypeExistsAsync(jobRoleTypeId);
            if (!jobRoleTypeExists)
            {
                throw new ValidationException("The selected job role type does not exist.");
            }

            if (!string.IsNullOrWhiteSpace(jobRole) && jobRole.Trim().Length > 100)
            {
                throw new ValidationException("Job role cannot exceed 100 characters.");
            }

            if (startDate > DateTime.UtcNow)
            {
                throw new ValidationException("Membership start date cannot be in the future.");
            }
        }

        private static IQueryable<Organisation> ApplyOrganisationOrdering(
            IQueryable<Organisation> query,
            string? sort,
            string? direction)
        {
            var descending =
                string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(direction, "d", StringComparison.OrdinalIgnoreCase);

            return sort?.Trim().ToLowerInvariant() switch
            {
                "name" or "organisationname" =>
                    descending ? query.OrderByDescending(x => x.OrganisationName)
                               : query.OrderBy(x => x.OrganisationName),

                "odscode" =>
                    descending ? query.OrderByDescending(x => x.ODSCode)
                               : query.OrderBy(x => x.ODSCode),

                "postcode" =>
                    descending ? query.OrderByDescending(x => x.PostCode)
                               : query.OrderBy(x => x.PostCode),

                "organisationtypeid" =>
                    descending ? query.OrderByDescending(x => x.OrganisationTypeId)
                               : query.OrderBy(x => x.OrganisationTypeId),

                "id" =>
                    descending ? query.OrderByDescending(x => x.Id)
                               : query.OrderBy(x => x.Id),

                _ => query.OrderBy(x => x.OrganisationName),
            };
        }

        private OrganisationHierarchyNodeViewModel BuildHierarchyNode(
            OrganisationHierarchyRow row,
            ILookup<int?, OrganisationHierarchyRow> childrenByParent,
            HashSet<int> path)
        {
            if (!path.Add(row.Id))
            {
                return new OrganisationHierarchyNodeViewModel
                {
                    Organisation = MapHierarchyRow(row),
                };
            }

            var node = new OrganisationHierarchyNodeViewModel
            {
                Organisation = MapHierarchyRow(row),
            };

            foreach (var child in childrenByParent[row.Id])
            {
                node.Children.Add(this.BuildHierarchyNode(child, childrenByParent, path));
            }

            path.Remove(row.Id);
            return node;
        }

        private static OrganisationViewModel MapHierarchyRow(OrganisationHierarchyRow row)
        {
            return new OrganisationViewModel
            {
                Id = row.Id,
                OrganisationName = row.OrganisationName,
                OdsCode = row.ODSCode,
                PostCode = row.PostCode,
                OrganisationTypeId = row.OrganisationTypeId,
                ParentId = row.ParentId,
                RegionId = row.RegionId,
            };
        }

        private static string? NormaliseJobRole(string? jobRole)
        {
            return string.IsNullOrWhiteSpace(jobRole) ? null : jobRole.Trim();
        }
    }
}

