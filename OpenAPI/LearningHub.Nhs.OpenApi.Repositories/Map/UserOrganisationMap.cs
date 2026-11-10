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
    public class UserOrganisationMap : AuditableEntityMap<UserOrganisation>
    {
        protected override void InternalMap(
            EntityTypeBuilder<UserOrganisation> modelBuilder)
        {
            modelBuilder.ToTable("UserOrganisation", "hub");

            modelBuilder.Property(e => e.UserId)
                .HasColumnName("UserId");

            modelBuilder.Property(e => e.OrganisationId)
                .HasColumnName("OrganisationId");

            modelBuilder.Property(e => e.JobRoleTypeId)
                .HasColumnName("JobRoleTypeId");

            modelBuilder.Property(e => e.JobRole)
                .HasColumnName("JobRole")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.StartDate)
                .HasColumnName("StartDate");

            modelBuilder.Property(e => e.EndDate)
                .HasColumnName("EndDate");
        }
    }
}
