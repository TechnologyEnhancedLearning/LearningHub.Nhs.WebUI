namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The assessment resource version map.
    /// </summary>
    public class AssessmentResourceVersionMap : BaseEntityMap<AssessmentResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<AssessmentResourceVersion> modelBuilder)
        {
            modelBuilder.ToTable("AssessmentResourceVersion", "resources");

            modelBuilder.HasKey(e => e.Id)
                .HasName("PK_Resources_AssessmentResourceVersion");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.AssessmentResourceVersion)
                .HasForeignKey<AssessmentResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentResourceVersion_ResourceVersion");

            modelBuilder.HasOne(d => d.AssessmentContent)
                .WithOne()
                .HasForeignKey<AssessmentResourceVersion>(d => d.AssessmentContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentResourceVersion_AssessmentContentId");

            modelBuilder.HasOne(d => d.EndGuidance)
                .WithOne()
                .HasForeignKey<AssessmentResourceVersion>(d => d.EndGuidanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentResourceVersion_EndGuidanceId");
        }
    }
}
