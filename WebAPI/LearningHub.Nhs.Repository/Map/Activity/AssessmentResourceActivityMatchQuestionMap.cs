// <copyright file="AssessmentResourceActivityMatchQuestionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource activity match question map.
    /// </summary>
    public class AssessmentResourceActivityMatchQuestionMap : BaseEntityMap<AssessmentResourceActivityMatchQuestion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<AssessmentResourceActivityMatchQuestion> modelBuilder)
        {
            modelBuilder.ToTable("AssessmentResourceActivityMatchQuestion", "activity");

            modelBuilder.HasOne(e => e.AssessmentResourceActivity)
                .WithMany(e => e.MatchQuestions)
                .HasForeignKey(d => d.AssessmentResourceActivityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityMatchQuestion_AssessmentResourceActivityId");

            modelBuilder.HasOne(e => e.FirstMatchAnswer)
                .WithOne()
                .HasForeignKey<AssessmentResourceActivityMatchQuestion>(d => d.FirstMatchAnswerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityMatchQuestion_FirstMatchAnswerId");

            modelBuilder.HasOne(e => e.SecondMatchAnswer)
                .WithOne()
                .HasForeignKey<AssessmentResourceActivityMatchQuestion>(d => d.SecondMatchAnswerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityMatchQuestion_SecondMatchAnswerId");
        }
    }
}
