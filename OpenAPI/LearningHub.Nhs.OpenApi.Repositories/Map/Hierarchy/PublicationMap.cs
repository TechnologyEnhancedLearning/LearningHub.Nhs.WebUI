namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using System;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The publication map.
    /// </summary>
    public class PublicationMap : BaseEntityMap<Publication>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Publication> modelBuilder)
        {
            modelBuilder.ToTable("Publication", "hierarchy");

            modelBuilder.Property(e => e.Notes)
                .IsRequired()
                .HasMaxLength(4000);

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.Publication)
                .HasForeignKey<ResourceVersion>(d => d.PublicationId)
                .HasConstraintName("FK_Publication_ResourceVersion");
        }
    }
}
