﻿namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource map.
    /// </summary>
    public class ResourceMap : BaseEntityMap<Resource>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Resource> modelBuilder)
        {
            modelBuilder.ToTable("Resource", "resources");

            modelBuilder.Property(e => e.ResourceTypeEnum).HasColumnName("ResourceTypeId")
               .HasConversion<int>();

            modelBuilder.HasOne(d => d.CurrentResourceVersion)
                .WithOne(p => p.ResourceWhereCurrent)
                .HasForeignKey<Resource>(b => b.CurrentResourceVersionId)
                .HasConstraintName("FK_Resource_CurrentResourceVersion");

            modelBuilder.HasOne(d => d.DuplicatedFromResourceVersion)
                .WithMany()
                .HasForeignKey(x => x.DuplicatedFromResourceVersionId);
        }
    }
}
