// <copyright file="FileChunkDetailMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The file map.
    /// </summary>
    public class FileChunkDetailMap : BaseEntityMap<FileChunkDetail>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<FileChunkDetail> modelBuilder)
        {
            modelBuilder.ToTable("FileChunkDetail", "resources");

            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.Property(e => e.ChunkCount)
                .IsRequired();

            modelBuilder.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(1024);

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithMany(p => p.FileChunkDetail)
                .HasForeignKey(d => d.ResourceVersionId)
                .HasConstraintName("FK_FileChunkDetail_ResourceVersion");
        }
    }
}
