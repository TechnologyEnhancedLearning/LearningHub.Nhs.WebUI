// <copyright file="AssessmentResourceActivityInteractionAnswerMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The assessment resource activity interaction answer map.
    /// </summary>
    public class AssessmentResourceActivityInteractionAnswerMap : BaseEntityMap<AssessmentResourceActivityInteractionAnswer>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<AssessmentResourceActivityInteractionAnswer> modelBuilder)
        {
            modelBuilder.ToTable("AssessmentResourceActivityInteractionAnswer", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(e => e.QuestionAnswer)
                .WithMany()
                .HasForeignKey(d => d.QuestionAnswerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityInteractionAnswer_QuestionAnswerId");

            modelBuilder.HasOne(e => e.AssessmentResourceActivityInteraction)
                .WithMany(d => d.Answers)
                .HasForeignKey(d => d.AssessmentResourceActivityInteractionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityInteractionAnswer_AssessmentResourceActivityInteraction");
        }
    }
}
