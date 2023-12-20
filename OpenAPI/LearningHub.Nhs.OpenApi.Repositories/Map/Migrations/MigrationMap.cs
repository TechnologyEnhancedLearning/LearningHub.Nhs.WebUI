// <copyright file="MigrationMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Migrations
{
    using System;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The migration map.
    /// </summary>
    public class MigrationMap : BaseEntityMap<Migration>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Migration> modelBuilder)
        {
            modelBuilder.ToTable("Migration", "migrations");

            modelBuilder.Property(e => e.MigrationSourceId)
                .IsRequired()
                .HasMaxLength(1024);

            modelBuilder.Property(e => e.MigrationSourceId)
                .IsRequired();

            modelBuilder.Property(e => e.AzureMigrationContainerName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Property(e => e.MetadataFileName)
                .HasMaxLength(1024);

            modelBuilder.Property(e => e.MetadataFilePath)
                .HasMaxLength(1024);

            modelBuilder.Property(e => e.MigrationStatusEnum).HasColumnName("MigrationStatusId")
               .HasConversion<int>();
        }
    }
}
