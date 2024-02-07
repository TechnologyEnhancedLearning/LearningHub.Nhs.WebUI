// <copyright file="MediaBlockMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The media block map.
    /// </summary>
    public class MediaBlockMap : BaseEntityMap<MediaBlock>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<MediaBlock> modelBuilder)
        {
            modelBuilder.ToTable("MediaBlock", "resources");

            modelBuilder.Property(mb => mb.BlockId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(mb => mb.BlockId);

            modelBuilder.HasOne(mb => mb.Block)
                .WithOne(b => b.MediaBlock)
                .HasForeignKey<MediaBlock>(mb => mb.BlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MediaBlock_BlockId");

            modelBuilder.HasOne(mb => mb.Attachment)
                .WithMany(a => a.MediaBlocks)
                .HasForeignKey(mb => mb.AttachmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MediaBlock_AttachmentId");

            modelBuilder.HasOne(mb => mb.Image)
                .WithMany(a => a.MediaBlocks)
                .HasForeignKey(mb => mb.ImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MediaBlock_ImageId");

            modelBuilder.HasOne(mb => mb.Video)
                .WithMany(a => a.MediaBlocks)
                .HasForeignKey(mb => mb.VideoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MediaBlock_VideoId");
        }
    }
}