// <copyright file="BlockMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The block map.
    /// </summary>
    public class BlockMap : BaseEntityMap<Block>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Block> modelBuilder)
        {
            modelBuilder.ToTable("Block", "resources");

            modelBuilder.HasOne(d => d.BlockCollection)
                .WithMany(p => p.Blocks)
                .HasForeignKey(d => d.BlockCollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Block_BlockCollectionId");
        }
    }
}
