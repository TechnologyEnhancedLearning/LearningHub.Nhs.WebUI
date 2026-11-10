using LearningHub.Nhs.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    public class OrganisationMap : AuditableEntityMap<Organisation>
    {
        protected override void InternalMap(
            EntityTypeBuilder<Organisation> modelBuilder)
        {
            modelBuilder.ToTable("Organisation", "hub");

            modelBuilder.Property(e => e.OrganisationName)
                .IsRequired()
                .HasColumnName("OrganisationName")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.ODSCode)
                .HasColumnName("ODSCode")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.PostCode)
                .HasColumnName("PostCode")
                .HasMaxLength(20);

            modelBuilder.Property(e => e.OrganisationTypeId)
                .HasColumnName("OrganisationTypeId");

            modelBuilder.Property(e => e.ParentId)
                .HasColumnName("ParentId");

            modelBuilder.Property(e => e.RegionId)
                .HasColumnName("RegionId");
        }
    }
}
