// <copyright file="WholeSlideImageBlockItemMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The whole slide image block item map.
    /// </summary>
    public class WholeSlideImageBlockItemMap : BaseEntityMap<WholeSlideImageBlockItem>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<WholeSlideImageBlockItem> modelBuilder)
        {
            modelBuilder.ToTable("WholeSlideImageBlockItem", "resources");

            modelBuilder.HasOne(d => d.WholeSlideImageBlock)
                .WithMany(p => p.WholeSlideImageBlockItems)
                .HasForeignKey(d => d.WholeSlideImageBlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WholeSlideImageBlockItem_WholeSlideImageBlockId");

            modelBuilder.HasOne(d => d.WholeSlideImage)
                .WithMany(p => p.WholeSlideImageBlockItems)
                .HasForeignKey(d => d.WholeSlideImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WholeSlideImageBlockItem_WholeSlideImageId");
        }
    }
}