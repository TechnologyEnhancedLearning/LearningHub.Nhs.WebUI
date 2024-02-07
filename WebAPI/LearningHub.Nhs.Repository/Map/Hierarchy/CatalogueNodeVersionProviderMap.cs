// <copyright file="CatalogueNodeVersionProviderMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The CatalogueNodeVersionProviderMap.
    /// </summary>
    public class CatalogueNodeVersionProviderMap : BaseEntityMap<CatalogueNodeVersionProvider>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder.</param>
        protected override void InternalMap(EntityTypeBuilder<CatalogueNodeVersionProvider> modelBuilder)
        {
            modelBuilder.ToTable("CatalogueNodeVersionProvider", "hierarchy");
            modelBuilder.HasKey(e => e.Id);
            modelBuilder.Property(e => e.Id)
                    .HasColumnName("Id");
            modelBuilder.Property(e => e.CatalogueNodeVersionId)
            .HasColumnName("CatalogueNodeVersionId").IsRequired();
            modelBuilder.Property(e => e.ProviderId).HasColumnName("ProviderId");
            modelBuilder.Property(e => e.RemovalDate).HasColumnName("RemovalDate").IsRequired(false);
        }
    }
}
