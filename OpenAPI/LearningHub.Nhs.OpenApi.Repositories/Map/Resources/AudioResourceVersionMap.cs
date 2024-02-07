// <copyright file="AudioResourceVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The audio resource version map.
    /// </summary>
    public class AudioResourceVersionMap : BaseEntityMap<AudioResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<AudioResourceVersion> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_AudioResourceVersion");

            modelBuilder.ToTable("AudioResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.Property(e => e.DurationInMilliseconds);

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.AudioResourceVersion)
                .HasForeignKey(d => d.AudioFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AudioResource_File");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.AudioResourceVersion)
                .HasForeignKey<AudioResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AudioResourceVersion_ResourceVersion");

            modelBuilder.HasOne(d => d.TranscriptFile)
                .WithOne()
                .HasForeignKey<AudioResourceVersion>(d => d.TranscriptFileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AudioResource_TranscriptFile");

            modelBuilder.HasOne(d => d.ResourceAzureMediaAsset)
                   .WithMany(p => p.AudioResourceVersion)
                   .HasForeignKey(d => d.ResourceAzureMediaAssetId)
                   .HasConstraintName("FK_AudioResourceVersion_ResourceAzureMediaAsset");
        }
    }
}
