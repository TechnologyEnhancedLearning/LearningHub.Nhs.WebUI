// <copyright file="FileChunkDetailMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
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
        /// <param name="modelBuilder">The model builder.</param>
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
