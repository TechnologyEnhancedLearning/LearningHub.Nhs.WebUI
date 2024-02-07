// <copyright file="VideoAssetMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Content
{
    using LearningHub.Nhs.Models.Entities.Content;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="VideoAssetMap" />.
    /// </summary>
    public class VideoAssetMap : BaseEntityMap<VideoAsset>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="entity">The entity<see cref="EntityTypeBuilder{VideoAsset}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<VideoAsset> entity)
        {
            entity.ToTable("VideoAsset", "content");

            entity.HasOne(d => d.ClosedCaptionsFile)
                .WithMany(p => p.VideoAssettClosedCaptionsFiles)
                .HasForeignKey(d => d.ClosedCaptionsFileId)
                .HasConstraintName("FK_VideoAssett_File");

            entity.HasOne(d => d.ThumbnailImageFile)
                .WithMany(p => p.VideoAssettThumbnailImageFiles)
                .HasForeignKey(d => d.ThumbnailImageFileId)
                .HasConstraintName("FK_VideoAssett_ThumbnailFile");

            entity.HasOne(d => d.TranscriptFile)
                .WithMany(p => p.VideoAssettTranscriptFiles)
                .HasForeignKey(d => d.TranscriptFileId)
                .HasConstraintName("FK_VideoAssett_TransScriptFile");

            entity.HasOne(d => d.VideoFile)
                .WithMany(p => p.VideoAssettVideoFiles)
                .HasForeignKey(d => d.VideoFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VideoAssett_VideoFile");
        }
    }
}
