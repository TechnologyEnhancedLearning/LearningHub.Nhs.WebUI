namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.Map;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The html resource version map.
    /// </summary>
    public class HtmlResourceVersionMap : BaseEntityMap<HtmlResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<HtmlResourceVersion> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_HtmlResourceVersion");

            modelBuilder.ToTable("HtmlResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.HtmlResourceVersion)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HtmlResource_File");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.HtmlResourceVersion)
                .HasForeignKey<HtmlResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HtmlResourceVersion_ResourceVersion");
        }
    }
}
