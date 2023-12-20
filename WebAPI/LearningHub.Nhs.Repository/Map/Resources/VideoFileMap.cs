// <copyright file="VideoFileMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The video file map.
    /// </summary>
    public class VideoFileMap : BaseEntityMap<VideoFile>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<VideoFile> modelBuilder)
        {
            modelBuilder.ToTable("VideoFile", "resources");

            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.Status)
                .IsRequired();

            modelBuilder.HasOne(d => d.File)
                .WithOne(p => p.VideoFile)
                .HasForeignKey<VideoFile>(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoFile_FileId");
        }
    }
}