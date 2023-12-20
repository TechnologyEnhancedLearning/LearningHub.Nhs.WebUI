// <copyright file="AssessmentResourceActivityInteractionMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource activity map.
    /// </summary>
    public class AssessmentResourceActivityInteractionMap : BaseEntityMap<AssessmentResourceActivityInteraction>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<AssessmentResourceActivityInteraction> modelBuilder)
        {
            modelBuilder.ToTable("AssessmentResourceActivityInteraction", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");
            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(e => e.QuestionBlock)
                .WithMany()
                .HasForeignKey(d => d.QuestionBlockId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityInteraction_QuestionBlockId");

            modelBuilder.HasOne(e => e.AssessmentResourceActivity)
                .WithMany(e => e.AssessmentResourceActivityInteractions)
                .HasForeignKey(d => d.AssessmentResourceActivityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AssessmentResourceActivityInteraction_AssessmentResourceActivity");
        }
    }
}
