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
    public class OrganisationTypeMap : AuditableEntityMap<OrganisationType>
    {
        protected override void InternalMap(
            EntityTypeBuilder<OrganisationType> modelBuilder)
        {
            modelBuilder.ToTable("OrganisationType", "hub");

            modelBuilder.Property(e => e.OrganisationTypeName)
                .IsRequired()
                .HasColumnName("OrganisationType")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.Description)
                .IsRequired()
                .HasColumnName("Description")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.EligibilityLevelId)
                .HasColumnName("EligibilityLevelId");
        }
    }
}
