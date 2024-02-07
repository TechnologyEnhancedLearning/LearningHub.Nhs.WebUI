// <copyright file="CatalogueNodeVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The CatalogueNodeVersionMap.
    /// </summary>
    public class CatalogueNodeVersionMap : BaseEntityMap<CatalogueNodeVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder.</param>
        protected override void InternalMap(EntityTypeBuilder<CatalogueNodeVersion> modelBuilder)
        {
            modelBuilder.ToTable("CatalogueNodeVersion", "hierarchy");

            modelBuilder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Property(x => x.Url)
                .IsRequired()
                .HasMaxLength(1000);

            modelBuilder.Property(x => x.BadgeUrl)
                .IsRequired(false)
                .HasMaxLength(128);

            modelBuilder.Property(x => x.BannerUrl)
                .IsRequired(false)
                .HasMaxLength(128);

            modelBuilder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1800);

            modelBuilder.HasMany(x => x.Keywords)
                .WithOne()
                .HasForeignKey(x => x.CatalogueNodeVersionId);

            modelBuilder.Property(x => x.OwnerName)
                .HasMaxLength(250);

            modelBuilder.Property(x => x.OwnerEmailAddress)
                .HasMaxLength(250);

            modelBuilder.Property(x => x.Notes);

            modelBuilder.Property(x => x.Order)
                .IsRequired();

            modelBuilder.HasOne(d => d.NodeVersion)
                .WithOne(p => p.CatalogueNodeVersion)
                .HasForeignKey<CatalogueNodeVersion>(d => d.NodeVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueNodeVersion_NodeVersion");
        }
    }
}
