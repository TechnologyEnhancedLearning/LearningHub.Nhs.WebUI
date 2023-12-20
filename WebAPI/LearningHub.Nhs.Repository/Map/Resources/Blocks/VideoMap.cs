// <copyright file="VideoMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The video map.
    /// </summary>
    public class VideoMap : BaseEntityMap<Video>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Video> modelBuilder)
        {
            modelBuilder.ToTable("Video", "resources");

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.Videos)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Video_FileId");
        }
    }
}