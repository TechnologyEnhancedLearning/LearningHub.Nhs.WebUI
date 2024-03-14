namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The image resource version map.
    /// </summary>
    public class ImageResourceVersionMap : BaseEntityMap<ImageResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ImageResourceVersion> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_ImageResourceVersion");

            modelBuilder.ToTable("ImageResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.Property(e => e.AltTag)
                .HasMaxLength(125);

            modelBuilder.Property(e => e.Description);

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.ImageResourceVersion)
                .HasForeignKey(d => d.ImageFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImageResource_File");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.ImageResourceVersion)
                .HasForeignKey<ImageResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImageResourceVersion_ResourceVersion");
        }
    }
}
