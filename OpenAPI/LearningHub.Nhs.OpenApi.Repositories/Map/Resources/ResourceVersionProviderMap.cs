namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version provider map.
    /// </summary>
    public class ResourceVersionProviderMap : BaseEntityMap<ResourceVersionProvider>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionProvider> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionProvider", "resources");
            modelBuilder.Property(e => e.ProviderId).HasColumnName("ProviderId");
            modelBuilder.Property(e => e.ResourceVersionId).HasColumnName("ResourceVersionId");

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithMany(p => p.ResourceVersionProvider)
                        .HasForeignKey(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_resourceVersionProvider_resourceVersion");

            modelBuilder.HasOne(d => d.Provider)
               .WithMany(p => p.ResourceVersionProvider)
               .HasForeignKey(d => d.ProviderId)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_resourceVersionProvider_provider");
        }
    }
}
