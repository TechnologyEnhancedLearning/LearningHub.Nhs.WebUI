namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scorm resource version map.
    /// </summary>
    public class ResourceVersionValidationResultMap : BaseEntityMap<ResourceVersionValidationResult>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionValidationResult> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_ResourceVersionValidationResult");

            modelBuilder.ToTable("ResourceVersionValidationResult", "resources");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithMany()
                .HasForeignKey(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersionValidationResult_ResourceVersion");

            modelBuilder.Property(e => e.Details)
                .IsRequired()
                .HasMaxLength(1024);
        }
    }
}
