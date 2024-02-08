namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The embedded resource version map.
    /// </summary>
    public class EmbeddedResourceVersionMap : BaseEntityMap<EmbeddedResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<EmbeddedResourceVersion> modelBuilder)
        {
            modelBuilder.ToTable("EmbeddedResourceVersion", "resources");

            modelBuilder.Property(e => e.EmbedCode)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithOne(p => p.EmbeddedResourceVersion)
                        .HasForeignKey<EmbeddedResourceVersion>(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_EmbeddedResourceVersion_ResourceVersion");
        }
    }
}
