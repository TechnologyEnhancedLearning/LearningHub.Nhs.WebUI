// <copyright file="CaseResourceVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The case resource version map.
    /// </summary>
    public class CaseResourceVersionMap : BaseEntityMap<CaseResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<CaseResourceVersion> modelBuilder)
        {
            modelBuilder.ToTable("CaseResourceVersion", "resources");

            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_CaseResourceVersion");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithOne(p => p.CaseResourceVersion)
                        .HasForeignKey<CaseResourceVersion>(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CaseResourceVersion_ResourceVersion");

            modelBuilder.HasOne(d => d.BlockCollection)
                        .WithOne(p => p.CaseResourceVersion)
                        .HasForeignKey<CaseResourceVersion>(d => d.BlockCollectionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_CaseResourceVersion_BlockCollectionId");
        }
    }
}
