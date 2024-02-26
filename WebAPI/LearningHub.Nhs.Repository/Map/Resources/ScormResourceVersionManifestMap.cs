namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scorm resource version map.
    /// </summary>
    public class ScormResourceVersionManifestMap : BaseEntityMap<ScormResourceVersionManifest>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ScormResourceVersionManifest> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_ScormResourceVersionManifest");

            modelBuilder.ToTable("ScormResourceVersionManifest", "resources");

            modelBuilder.Property(e => e.ScormResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ScormResourceVersionId);

            modelBuilder.HasOne(d => d.ScormResourceVersion)
                .WithOne(p => p.ScormResourceVersionManifest)
                .HasForeignKey<ScormResourceVersionManifest>(d => d.ScormResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ScormResourceVersionManifest_ScormResourceVersion");

            modelBuilder.Property(e => e.Author).HasMaxLength(512);

            modelBuilder.Property(e => e.CatalogEntry).HasMaxLength(128);

            modelBuilder.Property(e => e.Copyright).HasMaxLength(30);

            modelBuilder.Property(e => e.Duration).HasMaxLength(128);

            modelBuilder.Property(e => e.ItemIdentifier).HasMaxLength(128);

            modelBuilder.Property(e => e.Keywords).HasMaxLength(1000);

            modelBuilder.Property(e => e.ManifestUrl)
                .HasColumnName("ManifestURL")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.MasteryScore).HasColumnType("decimal(16, 2)");

            modelBuilder.Property(e => e.QuicklinkId).HasMaxLength(30);

            modelBuilder.Property(e => e.ResourceIdentifier).HasMaxLength(128);

            modelBuilder.Property(e => e.TemplateVersion).HasMaxLength(50);

            modelBuilder.Property(e => e.Title).HasMaxLength(255);
        }
    }
}
