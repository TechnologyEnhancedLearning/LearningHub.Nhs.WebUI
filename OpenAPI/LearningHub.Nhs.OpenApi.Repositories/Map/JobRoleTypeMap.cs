using LearningHub.Nhs.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    public class JobRoleTypeMap : AuditableEntityMap<JobRoleType>
    {
        protected override void InternalMap(
            EntityTypeBuilder<JobRoleType> modelBuilder)
        {
            modelBuilder.ToTable("JobRoleType", "hub");

            modelBuilder.Property(e => e.JobRoleTypeName)
                .IsRequired()
                .HasColumnName("JobRoleType")
                .HasMaxLength(50);
        }
    }
}
