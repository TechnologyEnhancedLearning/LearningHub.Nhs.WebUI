// <copyright file="WholeSlideImageBlockMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The whole slide image block map.
    /// </summary>
    public class WholeSlideImageBlockMap : BaseEntityMap<WholeSlideImageBlock>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<WholeSlideImageBlock> modelBuilder)
        {
            modelBuilder.ToTable("WholeSlideImageBlock", "resources");

            modelBuilder.Property(e => e.BlockId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.BlockId);

            modelBuilder.HasOne(d => d.Block)
                .WithOne(p => p.WholeSlideImageBlock)
                .HasForeignKey<WholeSlideImageBlock>(d => d.BlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WholeSlideImageBlock_BlockId");
        }
    }
}
