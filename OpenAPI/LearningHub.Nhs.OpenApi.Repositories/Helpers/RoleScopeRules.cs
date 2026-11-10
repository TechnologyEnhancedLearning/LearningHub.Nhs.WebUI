using LearningHub.Nhs.Models.Entities.Hierarchy;
using LearningHub.Nhs.Models.Role;
using LearningHub.Nhs.OpenApi.Models.Exceptions;
using LearningHub.Nhs.OpenApi.Models.ServiceModels.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Helpers
{
    /// <summary>
    /// The Role Scope rules
    /// </summary>
    public static class RoleScopeRules
    {

        public static RoleScopeType Parse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                !Enum.TryParse<RoleScopeType>(value.Trim(), ignoreCase: true, out var type) ||
                !Enum.IsDefined(type))
            {
                throw new HttpResponseException("ScopeType must be System, Organisation, Group, Catalogue, Category, Course, SelfAssessment or Multi.", HttpStatusCode.BadRequest);
            }

            return type;
        }

        public static void Validate(string? configuredScopeType, RoleScope scope)
        {
            var type = Parse(configuredScopeType);
            if (scope.OrganisationId is <= 0 || scope.CatalogueId is <= 0 ||
                scope.CategoryId is <= 0 || scope.SelfAssessmentId is <= 0)
            {
                throw new HttpResponseException("Scope identifiers must be positive integers.", HttpStatusCode.BadRequest);
            }

            // Future link: this column is intentionally not writable in this iteration.
            if (scope.SelfAssessmentId.HasValue)
            {
                throw new HttpResponseException("Self-assessment scope linking is not implemented yet.", HttpStatusCode.BadRequest);
            }

            var selectedCount = (scope.OrganisationId.HasValue ? 1 : 0)
                              + (scope.CatalogueId.HasValue ? 1 : 0)
                              + (scope.CategoryId.HasValue ? 1 : 0);

            if (selectedCount > 1)
            {
                throw new HttpResponseException("Specify at most one scope ID per assignment. Assign the role again for other scopes.", HttpStatusCode.BadRequest);
            }

            var valid = type switch
            {
                RoleScopeType.System => selectedCount == 0,
                RoleScopeType.Organisation => scope.OrganisationId.HasValue,
                RoleScopeType.Catalogue => scope.CatalogueId.HasValue,
                RoleScopeType.Category => scope.CategoryId.HasValue,
                RoleScopeType.Multi => true, // includes no ID = system-wide assignment
                RoleScopeType.Group or RoleScopeType.Course or RoleScopeType.SelfAssessment =>
                throw new HttpResponseException($"{type} scopes cannot be assigned with the current API.", HttpStatusCode.BadRequest),
                _ => false
            };

            if (!valid)
            {
                throw new HttpResponseException($"Assignment scope does not match the role's ScopeType '{type}'.", HttpStatusCode.BadRequest);
            }
        }
    }
}
