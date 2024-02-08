namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scorm resource version map.
    /// </summary>
    public class ResourceVersionValidationRuleResultMap : BaseEntityMap<ResourceVersionValidationRuleResult>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionValidationRuleResult> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_ResourceVersionValidationRuleResult");

            modelBuilder.ToTable("ResourceVersionValidationRuleResult", "resources");

            modelBuilder.HasOne(d => d.ResourceVersionValidationResult)
                .WithMany(d => d.ResourceVersionValidationRuleResults)
                .HasForeignKey(d => d.ResourceVersionValidationResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersionValidationRuleResult_ResourceVersionValidationResult");

            modelBuilder.Property(e => e.ResourceTypeValidationRuleEnum).HasColumnName("ResourceTypeValidationRuleId")
                .HasConversion<int>();

            modelBuilder.Property(e => e.Details)
                .IsRequired()
                .HasMaxLength(1024);
        }
    }
}
