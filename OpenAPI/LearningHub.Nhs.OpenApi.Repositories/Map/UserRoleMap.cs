using LearningHub.Nhs.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    public class UserRoleMap : AuditableEntityMap<UserRole>
    {
        protected override void InternalMap(
            EntityTypeBuilder<UserRole> modelBuilder)
        {
            modelBuilder.ToTable("UserRole", "hub");

            modelBuilder.Property(e => e.UserId)
                .HasColumnName("UserId");

            modelBuilder.Property(e => e.RoleId)
                .HasColumnName("RoleId");

            modelBuilder.Property(e => e.ScopeOrganisationId)
                .HasColumnName("ScopeOrganisationId");

            modelBuilder.Property(e => e.ScopeCatalogueId)
                .HasColumnName("ScopeCatalogueId");

            modelBuilder.Property(e => e.ScopeCategoryId)
                .HasColumnName("ScopeCategoryId");

            modelBuilder.Property(e => e.ScopeSelfAssessmentId)
                .HasColumnName("ScopeSelfAssessmentId");
        }
    }
}
