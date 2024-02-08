namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version flag map.
    /// </summary>
    public class ResourceVersionFlagMap : BaseEntityMap<ResourceVersionFlag>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionFlag> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionFlag", "resources");

            modelBuilder.Property(e => e.Details).HasMaxLength(1024);

            modelBuilder.Property(e => e.Resolution).HasMaxLength(1024);

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithMany(p => p.ResourceVersionFlag)
                .HasForeignKey(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersionFlag_ResourceVersion");
        }
    }
}
