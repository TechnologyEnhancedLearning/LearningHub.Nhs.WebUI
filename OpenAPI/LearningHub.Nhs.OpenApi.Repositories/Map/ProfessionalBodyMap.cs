using LearningHub.Nhs.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    public class ProfessionalBodyMap : AuditableEntityMap<ProfessionalBody>
    {
        protected override void InternalMap(
            EntityTypeBuilder<ProfessionalBody> modelBuilder)
        {
            modelBuilder.ToTable("ProfessionalBody", "hub");

            modelBuilder.Property(e => e.ProfessionalBodyName)
                .IsRequired()
                .HasColumnName("ProfessionalBody")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.OrderByNumber)
                .HasColumnName("OrderByNumber");

            modelBuilder.Property(e => e.PlaceholderText)
                .IsRequired()
                .HasColumnName("PlaceholderText")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.HelpText)
                .IsRequired()
                .HasColumnName("HelpText")
                .HasMaxLength(250);

            modelBuilder.Property(e => e.RegexPattern)
                .HasColumnName("RegexPattern")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.RegisterUrl)
                .HasColumnName("RegisterUrl")
                .HasMaxLength(250);

            modelBuilder.Property(e => e.IsActive)
                .HasColumnName("IsActive");
        }
    }
}
