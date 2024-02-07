// <copyright file="MigrationSourceMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Migrations
{
    using System;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The migration source map.
    /// </summary>
    public class MigrationSourceMap : BaseEntityMap<MigrationSource>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MigrationSource> modelBuilder)
        {
            modelBuilder.ToTable("MigrationSource", "migrations");

            modelBuilder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(256);
            modelBuilder.Property(e => e.HostName)
               .HasMaxLength(256);
            modelBuilder.Property(e => e.ResourcePath)
              .HasMaxLength(256);
            modelBuilder.Property(e => e.ResourceIdentifierPosition)
                .IsRequired();
            modelBuilder.Property(e => e.ResourceRegEx)
               .HasMaxLength(256);
        }
    }
}
