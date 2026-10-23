using LearningHub.Nhs.Models.Entities;
using LearningHub.Nhs.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.Role
{
    /// <summary>
    /// Represents the scope associated with a user role assignment.
    /// Encapsulates the scope identifiers used when assigning or
    /// updating a user's role within the Learning Hub.
    /// </summary>
    /// <param name="OrganisationId">
    /// The organisation identifier associated with the role assignment.
    /// </param>
    /// <param name="CatalogueId">
    /// The catalogue node version identifier associated with the role assignment.
    /// </param>
    /// <param name="CategoryId">
    /// The catalogue node version category identifier associated with the role assignment.
    /// </param>
    /// <param name="SelfAssessmentId">
    /// The self-assessment identifier associated with the role assignment.
    /// Self-assessment scope linking will be implemented separately.
    /// </param>
    public readonly record struct RoleScope(
        int? OrganisationId,
        int? CatalogueId,
        int? CategoryId,
        int? SelfAssessmentId)
    {
        public static RoleScope From(UserRoleCreateViewModel request)
        {
            return new(request.ScopeOrganisationId, request.ScopeCatalogueId,
            request.ScopeCategoryId, request.ScopeSelfAssessmentId);
        }

        public static RoleScope From(UserRoleScopeUpdateViewModel request)
        {
            return new(request.ScopeOrganisationId, request.ScopeCatalogueId,
                request.ScopeCategoryId, request.ScopeSelfAssessmentId);
        }

        public static RoleScope From(UserRole assignment)
        {
            return new(assignment.ScopeOrganisationId, assignment.ScopeCatalogueId,
                assignment.ScopeCategoryId, assignment.ScopeSelfAssessmentId);
        }
    }
}
