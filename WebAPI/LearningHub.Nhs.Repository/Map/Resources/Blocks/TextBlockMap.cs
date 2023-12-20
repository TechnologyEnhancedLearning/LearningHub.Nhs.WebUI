// <copyright file="TextBlockMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The text block map.
    /// </summary>
    public class TextBlockMap : BaseEntityMap<TextBlock>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<TextBlock> modelBuilder)
        {
            modelBuilder.ToTable("TextBlock", "resources");

            modelBuilder.Property(e => e.BlockId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.BlockId);

            modelBuilder.HasOne(d => d.Block)
                .WithOne(p => p.TextBlock)
                .HasForeignKey<TextBlock>(d => d.BlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TextBlock_BlockId");
        }
    }
}
