// <copyright file="VideoResourceVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The video resource version map.
    /// </summary>
    public class VideoResourceVersionMap : BaseEntityMap<VideoResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<VideoResourceVersion> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_VideoResourceVersion");

            modelBuilder.ToTable("VideoResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.VideoResourceVersion)
                .HasForeignKey(d => d.VideoFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoResource_File");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.VideoResourceVersion)
                .HasForeignKey<VideoResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoResourceVersion_ResourceVersion");

            modelBuilder.HasOne(d => d.TranscriptFile)
                .WithOne()
                .HasForeignKey<VideoResourceVersion>(d => d.TranscriptFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoResource_TranscriptFile");

            modelBuilder.HasOne(d => d.ClosedCaptionsFile)
                .WithOne()
                .HasForeignKey<VideoResourceVersion>(d => d.ClosedCaptionsFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoResource_ClosedCaptionsFile");

            // modelBuilder.HasOne(d => d.ResourceAzureMediaAsset)
            //   .WithOne()
            //   .HasForeignKey<VideoResourceVersion>(d => d.ResourceAzureMediaAssetId)
            //   .OnDelete(DeleteBehavior.ClientSetNull)
            //   .HasConstraintName("FK_VideoResource_ResourceAzureMediaAsset");
            modelBuilder.HasOne(d => d.ResourceAzureMediaAsset)
                   .WithMany(p => p.VideoResourceVersion)
                   .HasForeignKey(d => d.ResourceAzureMediaAssetId)
                   .HasConstraintName("FK_VideoResourceVersion_ResourceAzureMediaAsset");
        }
    }
}
