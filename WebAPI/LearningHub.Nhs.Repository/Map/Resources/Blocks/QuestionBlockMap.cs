// <copyright file="QuestionBlockMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The question block map.
    /// </summary>
    public class QuestionBlockMap : BaseEntityMap<QuestionBlock>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<QuestionBlock> modelBuilder)
        {
            modelBuilder.ToTable("QuestionBlock", "resources");

            modelBuilder.Property(e => e.BlockId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.BlockId);

            modelBuilder.HasOne(d => d.Block)
                .WithOne(p => p.QuestionBlock)
                .HasForeignKey<QuestionBlock>(d => d.BlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuestionBlock_BlockId");

            modelBuilder.HasMany(d => d.Answers)
                .WithOne(a => a.QuestionBlock)
                .HasForeignKey(d => d.QuestionBlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuestionAnswer_QuestionBlockId");

            modelBuilder.HasOne(d => d.QuestionBlockCollection)
                .WithOne()
                .HasForeignKey<QuestionBlock>(d => d.QuestionBlockCollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuestionBlock_QuestionBlockCollectionId");

            modelBuilder.HasOne(d => d.FeedbackBlockCollection)
                .WithOne()
                .HasForeignKey<QuestionBlock>(d => d.FeedbackBlockCollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuestionBlock_FeedbackBlockCollectionId");
        }
    }
}
